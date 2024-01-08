// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data;
using System.Data.Common;
using System.Globalization;
using ExampleFramework.TestAdapter;
using ExampleFramework.TestAdapter.PlatformServices;
using ExampleFramework.TestAdapter.PlatformServices.Interfaces;
using ExampleFramework.Tooling;

namespace ExampleFramework.TestAdapater.PlatformServices.Services;

/// <summary>
/// Internal implementation of TestContext exposed to the user.
/// The virtual string properties of the TestContext are retrieved from the property dictionary
/// like GetProperty&lt;string&gt;("TestName") or GetProperty&lt;string&gt;("FullyQualifiedTestClassName").
/// </summary>
public class TestContextImplementation : TestContext, ITestContext
{
#if !WINDOWS_UWP
    /// <summary>
    /// List of result files associated with the test.
    /// </summary>
    private readonly IList<string> _testResultFiles;
#endif

    /// <summary>
    /// Writer on which the messages given by the user should be written.
    /// </summary>
    private readonly ThreadSafeStringWriter _threadSafeStringWriter;

    /// <summary>
    /// Test Method.
    /// </summary>
    private readonly UIExample _example;

    /// <summary>
    /// Specifies whether the writer is disposed or not.
    /// </summary>
    private bool _stringWriterDisposed = false;

    /// <summary>
    /// Unit test outcome.
    /// </summary>
    private FrameworkUnitTestOutcome _outcome;

#if NETFRAMEWORK
    /// <summary>
    /// DB connection for test context.
    /// </summary>
    private DbConnection? _dbConnection;

    /// <summary>
    /// Data row for TestContext.
    /// </summary>
    private DataRow? _dataRow;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="TestContextImplementation"/> class.
    /// </summary>
    /// <param name="example">The test example.</param>
    /// <param name="stringWriter">The writer where diagnostic messages are written to.</param>
    /// <param name="properties">Properties/configuration passed in.</param>
    public TestContextImplementation(UIExample example, ThreadSafeStringWriter stringWriter, IDictionary<string, object?> properties)
    {
        _example = example;

        // Cannot get this type in constructor directly, because all signatures for all platforms need to be the same.
        _threadSafeStringWriter = stringWriter;
        // TODO: See if/how these properties are used and update as appropriate
        this.Properties = new Dictionary<string, object?>(properties)
        {
            [FullyQualifiedTestClassNameLabel] = _example.FullName,
            [ManagedTypeLabel] = _example.UIComponentType.FullName,
            [ManagedMethodLabel] = "METHOD",
            [TestNameLabel] = _example.Title,
        };

#if !WINDOWS_UWP
        _testResultFiles = new List<string>();
#endif
    }

    #region TestContext impl

    /// <inheritdoc/>
    public override FrameworkUnitTestOutcome CurrentTestOutcome => _outcome;

#if NETFRAMEWORK
    /// <inheritdoc/>
    public override DbConnection? DataConnection => _dbConnection;

    /// <inheritdoc/>
    public override DataRow? DataRow => _dataRow;
#endif

    /// <inheritdoc/>
    public override IDictionary<string, object?> Properties { get; }

#if !WINDOWS_UWP && !WIN_UI
    /// <inheritdoc/>
    public override string? TestRunDirectory => base.TestRunDirectory;

    /// <inheritdoc/>
    public override string? DeploymentDirectory => base.DeploymentDirectory;

    /// <inheritdoc/>
    public override string? ResultsDirectory => base.ResultsDirectory;

    /// <inheritdoc/>
    public override string? TestRunResultsDirectory => base.TestRunResultsDirectory;

    /// <inheritdoc/>
    public override string? TestResultsDirectory => base.TestResultsDirectory;

    /// <inheritdoc/>
    public override string FullyQualifiedTestClassName => base.FullyQualifiedTestClassName!;

#if NETFRAMEWORK
    /// <inheritdoc/>
    public override string ManagedType => base.ManagedType!;

    /// <inheritdoc/>
    public override string ManagedMethod => base.ManagedMethod!;
#endif

    /// <inheritdoc/>
    public override string TestName => base.TestName!;
#endif

    public TestContext Context => this;

    /// <inheritdoc/>
    public override void AddResultFile(string fileName)
    {
#if !WINDOWS_UWP && !WIN_UI
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("The parameter should not be null or empty", nameof(fileName));
        }

        _testResultFiles.Add(Path.GetFullPath(fileName));
#endif
    }

    /// <summary>
    /// When overridden in a derived class, used to write trace messages while the
    ///     test is running.
    /// </summary>
    /// <param name="message">The formatted string that contains the trace message.</param>
    public override void Write(string? message)
    {
        if (_stringWriterDisposed)
        {
            return;
        }

        try
        {
            var msg = message?.Replace("\0", "\\0");
            _threadSafeStringWriter.Write(msg);
        }
        catch (ObjectDisposedException)
        {
            _stringWriterDisposed = true;
        }
    }

    /// <summary>
    /// When overridden in a derived class, used to write trace messages while the
    ///     test is running.
    /// </summary>
    /// <param name="format">The string that contains the trace message.</param>
    /// <param name="args">Arguments to add to the trace message.</param>
    public override void Write(string format, params object?[] args)
    {
        if (_stringWriterDisposed)
        {
            return;
        }

        try
        {
            string message = string.Format(CultureInfo.CurrentCulture, format.Replace("\0", "\\0"), args);
            _threadSafeStringWriter.Write(message);
        }
        catch (ObjectDisposedException)
        {
            _stringWriterDisposed = true;
        }
    }

    /// <summary>
    /// When overridden in a derived class, used to write trace messages while the
    ///     test is running.
    /// </summary>
    /// <param name="message">The formatted string that contains the trace message.</param>
    public override void WriteLine(string? message)
    {
        if (_stringWriterDisposed)
        {
            return;
        }

        try
        {
            var msg = message?.Replace("\0", "\\0");
            _threadSafeStringWriter.WriteLine(msg);
        }
        catch (ObjectDisposedException)
        {
            _stringWriterDisposed = true;
        }
    }

    /// <summary>
    /// When overridden in a derived class, used to write trace messages while the
    ///     test is running.
    /// </summary>
    /// <param name="format">The string that contains the trace message.</param>
    /// <param name="args">Arguments to add to the trace message.</param>
    public override void WriteLine(string format, params object?[] args)
    {
        if (_stringWriterDisposed)
        {
            return;
        }

        try
        {
            string message = string.Format(CultureInfo.CurrentCulture, format.Replace("\0", "\\0"), args);
            _threadSafeStringWriter.WriteLine(message);
        }
        catch (ObjectDisposedException)
        {
            _stringWriterDisposed = true;
        }
    }

    /// <summary>
    /// Set the unit-test outcome.
    /// </summary>
    /// <param name="outcome">The test outcome.</param>
    public void SetOutcome(FrameworkUnitTestOutcome outcome)
    {
        _outcome = outcome;
    }

    /// <summary>
    /// Set data row for particular run of TestMethod.
    /// </summary>
    /// <param name="dataRow">data row.</param>
    public void SetDataRow(object? dataRow)
    {
#if NETFRAMEWORK
        _dataRow = dataRow as DataRow;
#endif
    }

    /// <summary>
    /// Set connection for TestContext.
    /// </summary>
    /// <param name="dbConnection">db Connection.</param>
    public void SetDataConnection(object? dbConnection)
    {
#if NETFRAMEWORK
        _dbConnection = dbConnection as DbConnection;
#endif
    }

    /// <summary>
    /// Returns whether property with parameter name is present or not.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="propertyValue">The property value.</param>
    /// <returns>True if found.</returns>
    public bool TryGetPropertyValue(string propertyName, out object? propertyValue)
    {
        if (Properties == null)
        {
            propertyValue = null;
            return false;
        }

        return Properties.TryGetValue(propertyName, out propertyValue);
    }

    /// <summary>
    /// Adds the parameter name/value pair to property bag.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="propertyValue">The property value.</param>
    public void AddProperty(string propertyName, string propertyValue)
    {
        Properties.Add(propertyName, propertyValue);
    }

    /// <summary>
    /// Result files attached.
    /// </summary>
    /// <returns>Results files generated in run.</returns>
    public IList<string>? GetResultFiles()
    {
#if !WINDOWS_UWP && !WIN_UI
        if (!_testResultFiles.Any())
        {
            return null;
        }

        List<string> results = _testResultFiles.ToList();

        // clear the result files to handle data driven tests
        _testResultFiles.Clear();

        return results;
#else
        // Returns null as this feature is not supported in ASP .net and UWP
        return null;
#endif
    }

    /// <summary>
    /// Gets messages from the testContext writeLines.
    /// </summary>
    /// <returns>The test context messages added so far.</returns>
    public string? GetDiagnosticMessages()
    {
        return _threadSafeStringWriter.ToString();
    }

    /// <summary>
    /// Clears the previous testContext writeline messages.
    /// </summary>
    public void ClearDiagnosticMessages()
    {
        _threadSafeStringWriter.ToStringAndClear();
    }

    #endregion
}