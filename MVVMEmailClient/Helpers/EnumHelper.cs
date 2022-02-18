using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperTest.Helpers;

/// <summary>
/// Helper class providing Enum related functionalities.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Retrieves the provided <see cref="Enum"/>'s values and its associated descriptions.
    /// </summary>
    /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
    /// <returns>KeyValuePairs holding the <see cref="Enum"/>'s values and corresponding descriptions.</returns>
    public static IEnumerable<KeyValuePair<T, string>> GetEnumValuesAndDescriptions<T>() where T : struct, Enum
    {
        return Enums.GetValues(typeof(T)).Select(x => new KeyValuePair<T, string>((T)x, ((T)x).AsString(EnumFormat.Description)));
    }
}

