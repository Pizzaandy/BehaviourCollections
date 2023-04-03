using UnityEngine;

namespace BehaviourCollections
{
    /// <summary>
    /// Base class for MonoBehaviours that are managed by a behaviour collection
    /// </summary>
    public abstract class ManagedMonoBehaviour<TBehaviour> : BehaviourProvider<TBehaviour>
        where TBehaviour : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public BehaviourProvider<TBehaviour> Collection;

        internal void Initialize()
        {
            if (Collection == null)
            {
                Debug.LogError("ManagedMonoBehaviour has no associated Collection");
            }
            TypeToBehaviour = Collection.TypeToBehaviour;
            TypeToInterface = Collection.TypeToInterface;
        }

        /// <summary>
        /// Removes this <typeparamref name="TBehaviour"/> from its associated collection 
        /// </summary>
        /// <remarks>
        /// It is not recommended to destroy or register components managed by a behaviour collection.
        /// </remarks>
        public void Deregister()
        {
            var type = GetType();
            TypeToBehaviour.RemoveByTypeParameter(type);

            var interfaces = type.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                TypeToInterface.RemoveByTypeParameter(interfaceType);
            }
        }
    }
}