using UnityEngine;
/// <summary>
/// Sinh quái 1 lần duy nhất khi Player đi vào vùng trigger 
/// Gắn script này vào 1 GameObject có Collider2D (Is Trigger = true, ví dụ CircleCollider2D) đặt tại vị trí muốn đặt bẫy quái trong phòng.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private Monster[] monsterPrefabs;
    [SerializeField] private int minSpawnCount = 3;
    [SerializeField] private int maxSpawnCount = 4;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private int maxPlacementAttempts = 20;
    private bool hasSpawned = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;
        hasSpawned = true;
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
        SpawnMonsters(spawnCount);
    }
    private void SpawnMonsters(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (TryFindValidSpawnPoint(out Vector2 point))
            {
                Monster prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
                Instantiate(prefab, point, Quaternion.identity);
            }
        }
    }
    private bool TryFindValidSpawnPoint(out Vector2 result)
    {
        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            Vector2 candidate = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            if (Physics2D.OverlapCircle(candidate, 0.3f, obstacleLayer) == null)
            {
                result = candidate;
                return true;
            }
        }
        result = transform.position;
        return false;
    }
}
