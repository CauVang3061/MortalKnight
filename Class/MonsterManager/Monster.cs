using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Monster : MonoBehaviour, IDamageable
{
    [SerializeField] private MonsterData data;
    [Tooltip("Thời gian chờ giữa 2 lần gây sát thương tiếp xúc, tránh trừ máu player mỗi frame khi đứng đè lên nhau")]
    [SerializeField] private float contactDamageCooldown = 1f;
    public float CurrentHP { get; private set; }
    public MonsterData Data => data;
    private float lastContactDamageTime = -999f;
    private void Awake()
    {
        CurrentHP = data.maxHP;
    }
    public void TakeDamage(float amount, Vector2 sourcePosition)
    {
        // Monster không có knockback (chỉ Character/Player mới có) nên không có sourcePosition
        // vẫn phải nhận đủ tham số để khớp interface.
        CurrentHP -= amount;
        if (CurrentHP <= 0f)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        HandleContactDamage(other.gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleContactDamage(collision.gameObject);
    }

    private void HandleContactDamage(GameObject target)
    {
        if (!target.CompareTag("Player")) return;
        if (Time.time - lastContactDamageTime < contactDamageCooldown) return;
        if (target.TryGetComponent<IDamageable>(out var damageable)){
                damageable.TakeDamage(data.contactDamage,transform.position);
                lastContactDamageTime = Time.time;
        }
    }
}
