using System;
using System.Collections.Generic;

//AI Generated code for prototyping, I don't trust language models so if I modidfy it I will remove this comment
public static class Registry<T>
{
    private static readonly Dictionary<string, T> _entries = new();

    /// <summary>
    /// Registers an entry into the registry.
    /// </summary>
    public static void Register(string id, T value)
    {
        if (_entries.ContainsKey(id))
            throw new Exception($"[Registry<{typeof(T).Name}>] Duplicate ID '{id}'");

        _entries[id] = value;
    }

    /// <summary>
    /// Gets an entry by ID.
    /// </summary>
    public static T Get(string id)
    {
        if (!_entries.TryGetValue(id, out var value))
            throw new Exception($"[Registry<{typeof(T).Name}>] Entry '{id}' not found.");

        return value;
    }

    /// <summary>
    /// Checks if an entry exists.
    /// </summary>
    public static bool Contains(string id) => _entries.ContainsKey(id);

    /// <summary>
    /// Returns all registered IDs.
    /// </summary>
    public static IEnumerable<string> GetAllIds() => _entries.Keys;

    /// <summary>
    /// Returns all registered objects.
    /// </summary>
    public static IEnumerable<T> GetAllValues() => _entries.Values;
}
