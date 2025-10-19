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

        // Available objects
        public List<T> Pool { get; private set; } = new(); 

        // All objects ever instantiated (including those currently in Pool)
        public List<T> All { get; private set; } = new();

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
        private void Prewarm(int countPerPrefab) 
        {
            foreach (var prefab in prefabs)
            {
                for (int i = 0; i < countPerPrefab; i++)
                {
                    T obj = Object.Instantiate(prefab);
                    SetActiveIfGameObject(obj, false);
                    Pool.Add(obj);
                    All.Add(obj); // <-- track all instantiated objects
                    if (parent != null)
                    {
                        if (obj is GameObject go) go.transform.SetParent(parent);
                        else if (obj is Component c) c.transform.SetParent(parent);
                    }
                }
            }
        }

        public T Get()
        {
            if (Pool.Count == 0)
            {
                T obj = Object.Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Count)]);
                All.Add(obj); // track new instance
                SetActiveIfGameObject(obj, true);
                return obj;
            }

            int lastIndex = Pool.Count - 1;
            T objFromPool = Pool[lastIndex];
            Pool.RemoveAt(lastIndex);
            SetActiveIfGameObject(objFromPool, true);
            return objFromPool;
        }

        public T GetRandom()
        {
            if (Pool.Count == 0)
            {
                T obj = Object.Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Count)]);
                All.Add(obj);
                SetActiveIfGameObject(obj, true);
                return obj;
            }

            int index = UnityEngine.Random.Range(0, Pool.Count);
            T objFromPool = Pool[index];
            Pool.RemoveAt(index);
            SetActiveIfGameObject(objFromPool, true);
            return objFromPool;
        }

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

            foreach (var prefab in prefabs)
            {
                if (predicate(prefab))
                {
                    T obj = Object.Instantiate(prefab);
                    All.Add(obj);
                    SetActiveIfGameObject(obj, true);
                    return obj;
                }
            }

            T fallback = Object.Instantiate(prefabs[0]);
            All.Add(fallback);
            SetActiveIfGameObject(fallback, true);
            return fallback;
        }

        public void Release(T obj) 
        {
            SetActiveIfGameObject(obj, false);
            if (!Pool.Contains(obj))
                Pool.Add(obj);
        }

        private void SetActiveIfGameObject(T obj, bool active) {
            if (obj is GameObject go) {
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
