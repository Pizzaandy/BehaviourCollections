using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class FastTypeDictionary<TValue>
{
    private static int typeIndex;
    private static readonly HashSet<Type> initializedTypes = new();

    private readonly object _lockObject = new();

    private TValue[] _values = new TValue[100];

    public void Add(Type key, TValue value)
    {
        if (!initializedTypes.Add(key)) { return; }

        typeof(FastTypeDictionary<TValue>)
            .GetMethod("Add")
            .MakeGenericMethod(key)
            .Invoke(this, new object[] { value });
    }

    public void Add<TKey>(TValue value)
    {
        var type = typeof(TKey);
        if (!initializedTypes.Add(type)) { return; }

        lock (_lockObject)
        {
            var id = TypeKey<TKey>.Id;
            if (id >= _values.Length)
            {
                Array.Resize(ref _values, id * 2);
            }
            _values[id] = value;
        }

        Debug.Log($"Added key {type.Name}");
    }

    public TValue Get<TKey>()
    {
        var id = TypeKey<TKey>.Id;
        return id >= _values.Length ? default : _values[id];
    }

    public bool Contains<TKey>()
    {
        var id = TypeKey<TKey>.Id;
        return id < _values.Length && _values[id] is not null;
    }

    public void Remove<TKey>()
    {
        var id = TypeKey<TKey>.Id;
        if (id >= _values.Length) { return; }
        _values[id] = default;
    }

    private static class TypeKey<TKey>
    {
        internal static readonly int Id = Interlocked.Increment(ref typeIndex);
    }
}
