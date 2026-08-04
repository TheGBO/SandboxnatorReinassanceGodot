using System;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace NullCyan.Util.GodotHelpers;

/// <summary>
/// This class is intended to replace the old legacy msgpack approach <see cref="IO.MPacker"/>
/// </summary>
public static class DictPack
{
    /// <summary>
    /// Converts complex data types into those godot.collections dictionaries
    /// </summary>
    public static Dictionary Serialize<T>(T data) where T : class
    {
        var dict = new Dictionary();
        if (data == null) return dict;

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            object val = prop.GetValue(data);
            if (val == null) continue;

            dict[prop.Name] = ObjectToVariant(val);
        }

        return dict;
    }

    private static Variant ObjectToVariant(object val)
    {
        return val switch
        {
            string s => Variant.From(s),
            Color c => Variant.From(c),
            int i => Variant.From(i),
            float f => Variant.From(f),
            double d => Variant.From(d),
            bool b => Variant.From(b),
            Vector2 v2 => Variant.From(v2),
            Vector3 v3 => Variant.From(v3),
            // Other godot data types should be added here if DTOs require more types
            _ => default
        };
    }

    /// <summary>
    /// Converts a dictionary into a complex data type essentially reconstructing it.
    /// </summary>
    public static T Deserialize<T>(Dictionary dict) where T : class, new()
    {
        var instance = new T();
        if (dict == null || dict.Count == 0) return instance;

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (dict.TryGetValue(prop.Name, out var variant))
            {
                object rawVal = variant.Obj;
                if (rawVal == null) continue;

                Type propType = prop.PropertyType;
                if (propType.IsEnum)
                {
                    prop.SetValue(instance, Enum.ToObject(propType, rawVal));
                }
                else if (!propType.IsAssignableFrom(rawVal.GetType()))
                {
                    prop.SetValue(instance, Convert.ChangeType(rawVal, propType));
                }
                else
                {
                    prop.SetValue(instance, rawVal);
                }
            }
        }

        return instance;
    }
}