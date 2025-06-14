using System.Collections.Generic;

namespace PreviewFramework;

public class UIComponentBaseTypes
{
    private readonly Dictionary<string, List<string>> _uiComponentBaseTypes = [];

    public void AddBaseType(string platform, string baseType)
    {
        if (!_uiComponentBaseTypes.TryGetValue(platform, out List<string>? baseTypes))
        {
            baseTypes = [];
            _uiComponentBaseTypes.Add(platform, baseTypes);
        }

        if (!baseTypes.Contains(baseType))
        {
            baseTypes.Add(baseType);
        }
    }

    public string? IsUIComponentBaseType(string typeName)
    {
        foreach (KeyValuePair<string, List<string>> keyValuePair in _uiComponentBaseTypes)
        {
            if (keyValuePair.Value.Contains(typeName))
            {
                return keyValuePair.Key;
            }
        }

        return null;
    }
}
