using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class NormalBox : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 1f;
    [Tooltip("Vật phẩm có thể rơi ra khi vỡ - để trống nếu không muốn rơi gì cả")]
    [SerializeField] private GameObject[] rewardPrefabs;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.5f;
    private float currentHP;
    private void Awake()
    {
        currentHP = maxHP;
    }
    public void TakeDamage(float amount, Vector2 sourcePosition)
    {
        currentHP -= amount;
        if (currentHP <= 0f)
        {
            Break();
        }
    }
    private void Break()
    {
        if (rewardPrefabs != null && rewardPrefabs.Length > 0 && Random.value <= dropChance)
        {
            GameObject reward = rewardPrefabs[Random.Range(0, rewardPrefabs.Length)];
            Instantiate(reward, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
