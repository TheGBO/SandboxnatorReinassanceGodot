using System;
using System.Collections.Generic;
namespace NullCyan.Util;

public class Registry<T>
{
    private readonly Dictionary<string, T> _entries = [];

    /// <summary>
    /// Registers an entry into the registry.
    /// </summary>
    public void Register(string id, T value)
    {
        if (_entries.ContainsKey(id))
            throw new Exception($"[Registry<{typeof(T).Name}>] Duplicate ID '{id}'");

        _entries[id] = value;
    }

    /// <summary>
    /// Gets an entry by ID.
    /// </summary>
    public T Get(string id)
    {
        if (!_entries.TryGetValue(id, out var value))
            throw new Exception($"[Registry<{typeof(T).Name}>] Entry '{id}' not found.");

        return value;
    }

    /// <summary>
    /// Checks if an entry exists.
    /// </summary>
    public bool Contains(string id) => _entries.ContainsKey(id);

    /// <summary>
    /// Returns all registered IDs.
    /// </summary>
    public IEnumerable<string> GetAllIds() => _entries.Keys;

    /// <summary>
    /// Returns all registered objects.
    /// </summary>
    public IEnumerable<T> GetAllValues() => _entries.Values;
}
