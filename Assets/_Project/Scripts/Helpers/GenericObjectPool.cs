using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Generic pool for any UnityEngine.Object.
    /// Automatically handles GameObject activation/deactivation if applicable.
    /// </summary>
    public class ObjectPool<T> where T : Object {
        private readonly T prefab;
        private readonly Transform parent;
        public Stack<T> Pool { get; private set; } = new Stack<T>();

        public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            Prewarm(initialSize);
        }

        /// <summary>
        /// Pre-instantiate objects
        /// </summary>
        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = Object.Instantiate(prefab);
                SetActiveIfGameObject(obj, false);
                Pool.Push(obj);
            }
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Get()
        {
            T obj = Pool.Count > 0 ? Pool.Pop() : Object.Instantiate(prefab);
            SetActiveIfGameObject(obj, true);
            return obj;
        }

        /// <summary>
        /// Release an object back to the pool
        /// </summary>
        public void Release(T obj)
        {
            SetActiveIfGameObject(obj, false);
            Pool.Push(obj);
        }

        /// <summary>
        /// Current number of objects available in the pool
        /// </summary>
        public int Count => Pool.Count;

        /// <summary>
        /// Activates/deactivates the associated GameObject if T is GameObject or Component
        /// </summary>
        private void SetActiveIfGameObject(T obj, bool active)
        {
            GameObject go = null;

            switch (obj)
            {
                case GameObject g:
                    go = g;
                    break;
                case Component c:
                    go = c.gameObject;
                    break;
            }

            if (go != null)
            {
                go.SetActive(active);

                // Re-parent when releasing
                if (!active && parent != null)
                    go.transform.SetParent(parent);
            }
        }
    }
}
