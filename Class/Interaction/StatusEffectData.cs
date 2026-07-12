using UnityEngine;
public enum StatusEffectType { Poison, Burn, Freeze, SpeedUp, SlowDown }
/// <summary>
/// Dữ liệu 1 hiệu ứng trạng thái
/// Data-driven như WeaponData/MonsterData — thêm hiệu ứng mới chỉ cần tạo asset mới.
/// </summary>
[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Status Effect/Status Effect Data")]
public class StatusEffectData : ScriptableObject
{
    public StatusEffectType type;
    public float duration = 3f;
    [Header("Dùng cho Poison/Burn — sát thương theo thời gian")]
    public float tickInterval = 1f;
    public float damagePerTick = 1f;
    [Header("Dùng cho Freeze/SpeedUp/SlowDown — hệ số nhân tốc độ di chuyển")]
    [Tooltip("Freeze nên để 0 (đứng im hoàn toàn), SlowDown < 1, SpeedUp > 1")]
    public float speedMultiplier = 1f;
}
