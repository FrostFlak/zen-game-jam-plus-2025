namespace Helpers {
    [System.Serializable]
    public struct SerializableKeyValue<TKey, TValue> {
        public TKey Key;
        public TValue Value;
    }
}