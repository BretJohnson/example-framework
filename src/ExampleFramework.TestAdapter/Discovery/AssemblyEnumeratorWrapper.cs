// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using ExampleFramework.TestAdapter.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace ExampleFramework.TestAdapter.Discovery;

/// <summary>
/// Enumerates through an assembly to get a set of test methods.
/// </summary>
internal class AssemblyEnumeratorWrapper
{
    /// <summary>
    /// Assembly name for UTF.
    /// </summary>
    private static readonly AssemblyName ExampleFrameworkAssemblyName = typeof(UIExampleAttribute).GetTypeInfo().Assembly.GetName();

    /// <summary>
    /// Gets test elements from an assembly.
    /// </summary>
    /// <param name="assemblyFileName"> The assembly file name. </param>
    /// <param name="runSettings"> The run Settings. </param>
    /// <param name="warnings"> Contains warnings if any, that need to be passed back to the caller. </param>
    /// <returns> A collection of test elements. </returns>
    internal ICollection<TestElement>? GetTests(string? assemblyFileName, IRunSettings? runSettings, out ICollection<string> warnings)
    {
        warnings = new List<string>();

        if (assemblyFileName == null || assemblyFileName.Length == 0)
        {
            return null;
        }

        var fullFilePath = PlatformServiceProvider.Instance.FileOperations.GetFullFilePath(assemblyFileName);

        try
        {
            if (!PlatformServiceProvider.Instance.FileOperations.DoesFileExist(fullFilePath))
            {
                throw new FileNotFoundException(fullFilePath);
            }

            if (!PlatformServiceProvider.Instance.TestSource.IsAssemblyReferenced(ExampleFrameworkAssemblyName, fullFilePath))
            {
                return null;
            }

            // Load the assembly in isolation if required.
            return GetTestsInIsolation(fullFilePath, runSettings, out warnings);
        }
        catch (FileNotFoundException ex)
        {
            var message = string.Format(CultureInfo.CurrentCulture, Resource.TestAssembly_AssemblyDiscoveryFailure, fullFilePath, ex.Message);
            PlatformServiceProvider.Instance.AdapterTraceLogger.LogWarning($"{nameof(AssemblyEnumeratorWrapper)}.{nameof(this.GetTests)}: {Resource.TestAssembly_AssemblyDiscoveryFailure}", fullFilePath, ex);
            warnings.Add(message);

            return null;
        }
        catch (ReflectionTypeLoadException ex)
        {
            var message = string.Format(CultureInfo.CurrentCulture, Resource.TestAssembly_AssemblyDiscoveryFailure, fullFilePath, ex.Message);
            PlatformServiceProvider.Instance.AdapterTraceLogger.LogWarning($"{nameof(AssemblyEnumeratorWrapper)}.{nameof(this.GetTests)}: {Resource.TestAssembly_AssemblyDiscoveryFailure}", fullFilePath, ex);
            PlatformServiceProvider.Instance.AdapterTraceLogger.LogWarning(Resource.ExceptionsThrown);
            warnings.Add(message);

            if (ex.LoaderExceptions != null)
            {
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    PlatformServiceProvider.Instance.AdapterTraceLogger.LogWarning("{0}", loaderEx);
                }
            }

            return null;
        }
        catch (BadImageFormatException)
        {
            // Ignore BadImageFormatException when loading native dll in managed adapter.
            return null;
        }
        catch (Exception ex)
        {
            // Catch all exceptions, if discoverer fails to load the dll then let caller continue with other sources.
            // Discover test doesn't work if there is a managed C++ project in solution
            // Assembly.Load() fails to load the managed cpp executable, with FileLoadException. It can load the dll
            // successfully though. This is known CLR issue.
            PlatformServiceProvider.Instance.AdapterTraceLogger.LogWarning($"{nameof(AssemblyEnumeratorWrapper)}.{nameof(this.GetTests)}: {Resource.TestAssembly_AssemblyDiscoveryFailure}", fullFilePath, ex);
            var message = ex is FileNotFoundException fileNotFoundEx ? fileNotFoundEx.Message : string.Format(CultureInfo.CurrentCulture, Resource.TestAssembly_AssemblyDiscoveryFailure, fullFilePath, ex.Message);

            warnings.Add(message);
            return null;
        }
    }

    private static ICollection<TestElement> GetTestsInIsolation(string fullFilePath, IRunSettings? runSettings, out ICollection<string> warnings)
    {
        using var isolationHost = PlatformServiceProvider.Instance.CreateTestSourceHost(fullFilePath, runSettings, frameworkHandle: null);

        // Create an instance of a type defined in adapter so that adapter gets loaded in the child app domain
        var assemblyEnumerator = (AssemblyEnumerator)isolationHost.CreateInstanceForType(typeof(AssemblyEnumerator), new object[] { TestSettings.CurrentSettings })!;

        // This might not be supported if an older version of Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices
        // assembly is already loaded into the App Domain.
        try
        {
            assemblyEnumerator.RunSettingsXml = runSettings?.SettingsXml;
        }
        catch
        {
            PlatformServiceProvider.Instance.AdapterTraceLogger.LogWarning(Resource.OlderTFMVersionFound);
        }

        return assemblyEnumerator.EnumerateAssembly(fullFilePath, out warnings);
    }
}