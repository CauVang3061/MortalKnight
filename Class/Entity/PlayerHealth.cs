// Setup: gắn PlayerHealth.cs vào cùng GameObject Player (cùng chỗ với PlayerMovement, PlayerInputReader).
// Nhấn Play, để quái chạm vào Player hoặc để đạn địch (nếu có) bắn trúng - máu sẽ trừ.
// gán Obstacle Layer = layer "Wall"
// Chỉnh Knockback Distance/Knockback Duration cho phù hợp tỉ lệ world unit (đặt giá trị ban đầu là 1.5/10)

using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float maxArmor = 5f;
    [Header("MP (dùng cho skill)")]
    [SerializeField] private float maxMP = 200f;
    [SerializeField] private float mpRegenPerSecond = 10f;
    [Header("Nhấp nháy khi trúng đòn")]
    [Tooltip("Thời gian nhấp nháy (giây) sau khi trúng đòn")]
    [SerializeField] private float invincibilityDuration = 2f;
    [Header("Hồi giáp theo thời gian")]
    [Tooltip("Thời gian (giây) không trúng đòn trước khi bắt đầu hồi giáp")]
    [SerializeField] private float timeBeforeArmorRegen = 5f;
    [Tooltip("Thời gian (giây) để hồi 1 điểm giáp")]
    [SerializeField] private float armorRegenInterval = 2f;
    [Header("Knockback")]
    [Tooltip("Khoảng cách bị đẩy lùi khi trúng đòn")]
    [SerializeField] private float knockbackDistance = 1.5f;
    [Tooltip("Thời gian thực hiện knockback")]
    [SerializeField] private float knockbackDuration = 0.1f;
    [SerializeField] private LayerMask obstacleLayer;
    public float CurrentHP { get; private set; }
    public float CurrentArmor { get; private set; }
    public float CurrentMP { get; private set; }
    public bool IsInvincible { get; private set; }
    private float invincibilityTimer;
    private float timeSinceLastHit;
    private float armorRegenTimer;
    private PlayerMovement playerMovement;
    // Event cho UI (StatusBar) lắng nghe
    public event Action<float, float> OnHealthChanged; // (currentHP, maxHP)
    public event Action<float, float> OnArmorChanged;   // (currentArmor, maxArmor)
    public event Action<float, float> OnMPChanged;      // (currentMP, maxMP)
    public event Action OnDied;

    private void Awake()
    {
        CurrentHP = maxHP;
        CurrentArmor = maxArmor;
        CurrentMP = maxMP;
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (IsInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f) IsInvincible = false;
        }
        UpdateArmorRegen();
        UpdateMPRegen();
    }

    private void UpdateMPRegen()
    {
        if (CurrentMP >= maxMP) return;
        CurrentMP = Mathf.Min(CurrentMP + mpRegenPerSecond * Time.deltaTime, maxMP);
        OnMPChanged?.Invoke(CurrentMP, maxMP);
    }

    /// <summary>
    /// Gọi từ Knight (hoặc skill khác sau này) để tiêu MP. Trả về false nếu không đủ MP,
    /// skill nên kiểm tra giá trị trả về trước khi thực hiện hành động.
    /// </summary>
    public bool TrySpendMP(float amount)
    {
        if (CurrentMP < amount) return false;
        CurrentMP -= amount;
        OnMPChanged?.Invoke(CurrentMP, maxMP);
        return true;
    }
    /// <summary>
    /// Cho phép skill/hiệu ứng bên ngoài cấp bất tử tạm thời — dùng chung state với bất tử sau khi trúng đòn
    /// </summary>
    public void GrantInvincibility(float duration)
    {
        IsInvincible = true;
        invincibilityTimer = Mathf.Max(invincibilityTimer, duration);
    }
    /// <summary>Hồi máu — dùng cho bình đỏ (PickBottle).</summary>
    public void Heal(float amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }
    /// <summary>Hồi MP — dùng cho bình xanh (PickBottle).</summary>
    public void RestoreMP(float amount)
    {
        CurrentMP = Mathf.Min(CurrentMP + amount, maxMP);
        OnMPChanged?.Invoke(CurrentMP, maxMP);
    }
    private void UpdateArmorRegen()
    {
        if (CurrentArmor >= maxArmor) return;
        timeSinceLastHit += Time.deltaTime;
        if (timeSinceLastHit < timeBeforeArmorRegen) return;
        armorRegenTimer += Time.deltaTime;
        if (armorRegenTimer >= armorRegenInterval)
        {
            armorRegenTimer = 0f;
            CurrentArmor = Mathf.Min(CurrentArmor + 1f, maxArmor);
            OnArmorChanged?.Invoke(CurrentArmor, maxArmor);
        }
    }
    public void TakeDamage(float amount, Vector2 sourcePosition)
    {
        if (IsInvincible) return;
        timeSinceLastHit = 0f;
        armorRegenTimer = 0f;
        if (CurrentArmor >= amount)
        {
            CurrentArmor -= amount;
        }
        else
        {
            float overflow = amount - CurrentArmor;
            CurrentArmor = 0f;
            CurrentHP = Mathf.Max(CurrentHP - overflow, 0f);
            OnHealthChanged?.Invoke(CurrentHP, maxHP);
        }
        OnArmorChanged?.Invoke(CurrentArmor, maxArmor);
        if (CurrentHP <= 0f)
        {
            Die();
            return;
        }
        IsInvincible = true;
        invincibilityTimer = invincibilityDuration;
        StartCoroutine(KnockbackRoutine(sourcePosition));
    }
    private IEnumerator KnockbackRoutine(Vector2 sourcePosition)
    {
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;
        float distance = knockbackDistance;
        while (distance > 0.1f && Physics2D.OverlapCircle((Vector2)transform.position + direction * distance, 0.2f, obstacleLayer))
        {
            distance -= 0.1f;
        }
        playerMovement.IsKnockedBack = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(direction * distance);
        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / knockbackDuration);
            yield return null;
        }
        transform.position = endPos;
        playerMovement.IsKnockedBack = false;
    }
    private void Die()
    {
        IsInvincible = true;
        if (TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.color = new Color(0.6f, 0.6f, 0.6f);
        }
        OnDied?.Invoke();
    }
}
