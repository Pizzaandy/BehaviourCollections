# BehaviourCollections
 A high-performance set of Unity scripts for caching and accessing components.

The idea behind this library was to try making Unity's `Component.TryGetComponent` as fast as possible - no matter the cost.

The result? Calling `ManagedMonoBehaviour.TryGetComponent` from this library is ~4x faster than `Component.TryGetComponent` on the Mono runtime 2x faster on IL2CPP.

Is this worth the added complexity? Absolutely not. But it does feature some interesting C# optimization gimmicks to get there - notably, this one: https://mariusgundersen.net/type-dictionary-trick/
