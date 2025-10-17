using UnityEngine;

public static class ScreenHelper {
    
    public static Vector3 GetRandomScreenPosition(float margin) {
        float randomX = Random.Range(margin, 1f - margin);
        float randomY = Random.Range(margin, 1f - margin);

        Vector3 spawnPos = Game.Instance.Camera.ViewportToWorldPoint(new Vector3(randomX, randomY, 0));
        spawnPos.z = 0;
        return spawnPos;
    }
}
