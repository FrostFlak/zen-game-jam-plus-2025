using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Prefabs", fileName = "PrefabsStorage")]
public class PrefabsStorage : ScriptableObject {

    [field: SerializeField] public Ad AdPrefab { get; private set; }
    [field: SerializeField] public Dzen DzenPrefab { get; set; }
}
