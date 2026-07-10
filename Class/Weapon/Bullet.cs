using UnityEngine;
public enum WeaponSide { Player, Enemy }
/// <summary>
/// Di chuyển theo 1 hướng cố định (được truyền vào lúc Fire), gây sát thương khi
/// chạm đối tượng thuộc phe đối địch, tự hủy khi chạm tường.
/// Phe bắn ra viên đạn — dùng để biết đạn không được gây sát thương ngược lại phe mình.
/// Nhận thẳng aimDirection lúc Fire().
/// - Dùng Time.deltaTime để di chuyển.
/// - Va chạm dùng Collider2D + OnTriggerEnter2D có sẵn của Unity.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private Vector2 direction;
    private WeaponSide side;
    [Tooltip("Layer được coi là 'tường' — đạn chạm vào sẽ tự hủy")]
    [SerializeField] private LayerMask wallLayer;
    /// <summary>
    /// Gọi ngay sau khi Instantiate để thiết lập viên đạn.
    /// aimDirection phải được normalized từ nơi gọi (WeaponController).
    /// </summary>
    public void Fire(Vector2 aimDirection, float bulletDamage, float bulletSpeed, WeaponSide shooterSide)
    {
        direction = aimDirection;
        damage = bulletDamage;
        speed = bulletSpeed;
        side = shooterSide;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private void Update()
    {
        // Nhân với Time.deltaTime khi tính calPosition()
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chạm tường (layer riêng, set trong Inspector) -> hủy đạn ngay.
        if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }
        // Chỉ gây sát thương nếu đối tượng va chạm không cùng phe với người bắn.
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            bool isEnemyTarget = side == WeaponSide.Player && other.CompareTag("Enemy");
            bool isPlayerTarget = side == WeaponSide.Enemy && other.CompareTag("Player");
            if (isEnemyTarget || isPlayerTarget)
            {
                damageable.TakeDamage(damage, transform.position);
                Destroy(gameObject);
            }
        }
    }
}
