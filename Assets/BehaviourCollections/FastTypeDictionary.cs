using System;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// A high-performance dictionary that uses System.Type as keys.
/// </summary>
public class FastTypeDictionary<TValue>
{
    private static int typeIndex;

    private static readonly Dictionary<Type, AddCall> s_typeToAddCall = new();
    private static readonly Dictionary<Type, RemoveCall> s_typeToRemoveCall = new();

    private delegate void AddCall(FastTypeDictionary<TValue> dict, TValue value);
    private delegate void RemoveCall(FastTypeDictionary<TValue> dict);

    private readonly object _lockObject = new();

    private TValue[] _values = new TValue[32];

    /// <summary>
    /// This method uses reflection at runtime! For better performance, use <see cref="FastTypeDictionary{T}.Add{K}(T)"/>
    /// </summary>
    public void AddByTypeParameter(Type type, TValue value)
    {
        if (!s_typeToAddCall.TryGetValue(type, out AddCall addCall))
        {
            RegisterTypeParameter(type);
            addCall = s_typeToAddCall[type];
        }

        addCall.Invoke(this, value);
    }

    public void Add<TKey>(TValue value)
    {
        lock (_lockObject)
        {
            var id = TypeKey<TKey>.Id;
            if (id >= _values.Length)
            {
                Array.Resize(ref _values, id * 2);
            }
            _values[id] = value;
        }
    }

    public void RegisterType<TKey>()
    {
        lock (_lockObject)
        {
            var id = TypeKey<TKey>.Id;
        }
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

    /// <summary>
    /// This method uses reflection at runtime! For better performance, use <see cref="FastTypeDictionary{T}.Remove{K}"/>
    /// </summary>
    public void RemoveByTypeParameter(Type type)
    {
        if (!s_typeToRemoveCall.TryGetValue(type, out RemoveCall removeCall))
        {
            RegisterTypeParameter(type);
            removeCall = s_typeToRemoveCall[type];
        }

        removeCall.Invoke(this);
    }

    public void Remove<TKey>()
    {
        var id = TypeKey<TKey>.Id;
        if (id >= _values.Length) { return; }
        _values[id] = default!;
    }

    /// <summary>
    /// Caches generic add and remove methods created at runtime
    /// </summary>
    public static void RegisterTypeParameter(Type type)
    {
        // open delegates are ~10x faster than MethodInfo.Invoke calls
        // create a delegate for adding/removing each Type and cache it for repeat calls

        if (!s_typeToAddCall.ContainsKey(type))
        {
            var addMethodInfo =
            typeof(FastTypeDictionary<TValue>)
                .GetMethod("Add")
                .MakeGenericMethod(type);

            var addCall = (AddCall)Delegate.CreateDelegate(typeof(AddCall), addMethodInfo);
            s_typeToAddCall.Add(type, addCall);
        }
        
        if (!s_typeToRemoveCall.ContainsKey(type))
        {
            var removeMethodInfo =
                typeof(FastTypeDictionary<TValue>)
                    .GetMethod("Remove")
                    .MakeGenericMethod(type);

            var removeCall = (RemoveCall)Delegate.CreateDelegate(typeof(RemoveCall), removeMethodInfo);
            s_typeToRemoveCall.Add(type, removeCall);
        }
    }

	/// <summary>
    /// "Warm" this dictionary with pre-defined types to prevent expensive method calls during gameplay
    /// </summary>
    public static void WarmTypes(Type[] types)
    {
        foreach (var type in types)
        {
            RegisterTypeParameter(type);
        }
    }

    /// <summary>
    /// An inner class that assigns a unique Id to each Type.
    /// </summary>
    private static class TypeKey<TKey>
    {
        internal static readonly int Id = Interlocked.Increment(ref typeIndex);
    }
}
