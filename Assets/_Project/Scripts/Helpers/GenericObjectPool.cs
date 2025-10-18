using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helpers
{
    /// <summary>
    /// Generic pool for any UnityEngine.Object.
    /// Automatically handles GameObject activation/deactivation if applicable.
    /// Supports multiple prefab types and random retrieval.
    /// </summary>
    public class ObjectPool<T> where T : Object
    {
        private readonly Transform parent;
        private readonly List<T> prefabs;
        public List<T> Pool { get; private set; } = new(); // single list for pool

        /// <summary>
        /// Constructor for single prefab
        /// </summary>
        public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
        {
            this.prefabs = new List<T> { prefab };
            this.parent = parent;
            Prewarm(initialSize);
        }

        /// <summary>
        /// Constructor for multiple prefabs
        /// </summary>
        public ObjectPool(IEnumerable<T> prefabs, int initialSize = 10, Transform parent = null)
        {
            this.prefabs = new List<T>(prefabs);
            this.parent = parent;
            Prewarm(initialSize);
        }

        /// <summary>
        /// Pre-instantiate objects from all prefabs
        /// </summary>
        private void Prewarm(int countPerPrefab) {
            foreach (var prefab in prefabs)
            {
                for (int i = 0; i < countPerPrefab; i++)
                {
                    T obj = Object.Instantiate(prefab);
                    SetActiveIfGameObject(obj, false);
                    Pool.Add(obj);
                }
            }
        }


        /// <summary>
        /// Get an object from the pool (last-in, first-out)
        /// </summary>
        public T Get()
        {
            if (Pool.Count == 0)
                return Object.Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Count)]);

            int lastIndex = Pool.Count - 1;
            T obj = Pool[lastIndex];
            Pool.RemoveAt(lastIndex);

            SetActiveIfGameObject(obj, true);
            return obj;
        }

        /// <summary>
        /// Get a random object from the pool
        /// </summary>
        public T GetRandom()
        {
            if (Pool.Count == 0)
                return Object.Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Count)]);

            int index = UnityEngine.Random.Range(0, Pool.Count);
            T obj = Pool[index];
            Pool.RemoveAt(index);

            SetActiveIfGameObject(obj, true);
            return obj;
        }

        /// <summary>
        /// Get an object from the pool by condition (predicate)
        /// Example: pool.Get(x => ((BaseAd)(object)x).AdType == AdType.Static)
        /// </summary>
        public T Get(Func<T, bool> predicate)
        {
            for (int i = 0; i < Pool.Count; i++)
            {
                if (predicate(Pool[i]))
                {
                    T obj = Pool[i];
                    Pool.RemoveAt(i);
                    SetActiveIfGameObject(obj, true);
                    return obj;
                }
            }

            // If no object matches in pool, instantiate a prefab that satisfies the condition
            foreach (var prefab in prefabs)
            {
                if (predicate(prefab))
                {
                    T obj = Object.Instantiate(prefab);
                    SetActiveIfGameObject(obj, true);
                    return obj;
                }
            }

            // If still nothing matches, fallback to first prefab
            T fallback = Object.Instantiate(prefabs[0]);
            SetActiveIfGameObject(fallback, true);
            return fallback;
        }


        /// <summary>
        /// Release an object back to the pool
        /// </summary>
        public void Release(T obj) {
            SetActiveIfGameObject(obj, false);
            if (!Pool.Contains(obj))
                Pool.Add(obj);
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
            if (obj is GameObject go)
            {
                go.SetActive(active);
                if (!active && parent != null) go.transform.SetParent(parent);
            }
            else if (obj is Component c)
            {
                c.gameObject.SetActive(active);
                if (!active && parent != null) c.transform.SetParent(parent);
            }
        }
    }
}
