using System.Collections;
using UnityEngine;
/// <summary>
/// Điều khiển vũ khí cận chiến: gây sát thương cho MỌI kẻ địch trong bán kính
/// (chỉ kiểm tra khoảng cách, không kiểm tra góc/hướng).
/// </summary>
public class MeleeWeaponController : MonoBehaviour
{
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private WeaponSide side = WeaponSide.Player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask targetLayer;
    public Vector2 AimDirection { get; set; } = Vector2.right;
    public void Equip(MeleeWeaponData newData)
    {
        weaponData = newData;
        cooldownTimer = 0f;
    }
    private float cooldownTimer;
    private bool isSwinging;
    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (!isSwinging) RotateTowardsAim();
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    private void RotateTowardsAim()
    {
        if (AimDirection.sqrMagnitude < 0.0001f) return;
        float angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (spriteRenderer != null)
        {
            Vector3 scale = transform.localScale;
            scale.y = AimDirection.x < 0f ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            transform.localScale = scale;
        }
    }
    /// <summary>
    /// Gọi hàm này từ input để thử tấn công. Trả về false nếu còn hồi chiêu.
    /// </summary>
    public bool TryAttack()
    {
        if (cooldownTimer > 0f) return false;
        if (weaponData == null) return false;
        cooldownTimer = weaponData.attackCooldown;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weaponData.attackRange, targetLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(weaponData.attack, transform.position);
            }
            if (weaponData.appliedEffect != null && hit.TryGetComponent<StatusEffectReceiver>(out var receiver))
            {
                receiver.ApplyEffect(weaponData.appliedEffect);
            }
        }
        StartCoroutine(SwingAnimation());
        return true;
    }
    private IEnumerator SwingAnimation()
    {
        isSwinging = true;
        float elapsed = 0f;
        float baseAngle = transform.eulerAngles.z;
        while (elapsed < weaponData.swingDuration)
        {
            elapsed += Time.deltaTime;
            float rotationProgress = elapsed / weaponData.swingDuration;
            transform.rotation = Quaternion.Euler(0f, 0f, baseAngle + 360f * rotationProgress);
            yield return null;
        }
        isSwinging = false;
    }
}
