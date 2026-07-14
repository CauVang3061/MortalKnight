using UnityEngine;
/// <summary>
/// Vùng tăng tốc — tương đương AccelerateArea (KNIGHT_SPEED = 6 --> 
/// ACCELERATE_KNIGHT_SPEED = 9 khi đứng trong vùng, có ~1.2s tăng trước khi
/// tốc độ trả về bình thường sau khi rời đi).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AccelerateZone : MonoBehaviour
{
    [Tooltip("Gán 1 StatusEffectData loại SpeedUp - duration nên đặt ~1.2s (ACCELERATE_TIME gốc), speedMultiplier ~1.5")]
    [SerializeField] private StatusEffectData speedBoostEffect;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.TryGetComponent<StatusEffectReceiver>(out var receiver))
        {
            receiver.ApplyEffect(speedBoostEffect);
        }
    }
}
