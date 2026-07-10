using UnityEngine;
/// <summary>
/// Điều khiển 1 khẩu súng: xoay theo hướng ngắm và bắn đạn khi có input.
/// WeaponController không tự quyết định ngắm theo chuột hay theo AI,
/// nó chỉ đọc property AimDirection do script khác (PlayerAimController,
/// hoặc AI của quái) gán vào mỗi frame --> Dùng được cho cả đạn quái lẫn đạn player.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private WeaponSide side = WeaponSide.Player;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    // Hướng ngắm hiện tại, được gán từ bên ngoài (PlayerAimController hoặc AI).
    public Vector2 AimDirection { get; set; } = Vector2.right;
    public void Equip(WeaponData newData)
    {
        weaponData = newData;
        cooldownTimer = 0f;
    }
    private float cooldownTimer;
    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (firePoint == null) firePoint = transform;
    }
    private void Update()
    {
        RotateTowardsAim();
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    private void RotateTowardsAim()
    {
        if (AimDirection.sqrMagnitude < 0.0001f) return;
        // Xoay sprite súng theo aimDirection để ngắm đúng hướng.
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        // Lật theo trục Y khi ngắm sang trái, để súng không bị "lộn ngược đầu nòng"
        if (spriteRenderer != null)
        {
            Vector3 scale = transform.localScale;
            scale.y = AimDirection.x < 0f ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            transform.localScale = scale;
        }
    }
    /// <summary>
    /// Gọi hàm này từ input để thử bắn.
    /// Trả về false nếu còn đang trong thời gian hồi chiêu.
    /// </summary>
    public bool TryFire()
    {
        if (cooldownTimer > 0f) return false;
        if (weaponData == null || weaponData.bulletPrefab == null) return false;
        cooldownTimer = weaponData.fireCooldown;
        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            // Mỗi viên đạn lệch ngẫu nhiên trong khoảng [-spreadAngle, +spreadAngle]
            // tương đương công thức deflection dùng weaponPrecision trong bản gốc.
            float spread = Random.Range(-weaponData.spreadAngle, weaponData.spreadAngle);
            Vector2 bulletDirection = Quaternion.Euler(0f, 0f, spread) * AimDirection.normalized;
            Bullet bullet = Instantiate(weaponData.bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.Fire(bulletDirection, weaponData.attack, weaponData.bulletSpeed, side);
        }
        return true;
    }
}
