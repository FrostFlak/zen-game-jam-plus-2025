using System.Collections.Generic;
using Helpers;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Prefabs", fileName = "PrefabsStorage")]
public class PrefabsStorage : ScriptableObject {

    [field: SerializeField] public List<SerializableKeyValue<AdType, BaseAd>> AdPrefabs { get; private set; }
    [field: SerializeField] public Dzen DzenPrefab { get; set; }
}
