// Cách làm cụ thể trong Unity:
// 1. Tạo GameObject mới cho Charger (sprite, Rigidbody2D, Collider2D)
// 2. Add Component --> gõ tìm Monster --> add
// 3. Add Component --> gõ tìm StatusEffectReceiver --> add
// 4. Add Component --> gõ tìm ChargerAndMummy --> add
// 5. Chỉnh số liệu trong Inspector
// 6. Kéo thành prefab
// Đối với Mummy, làm tương tự (chỉ khác bước 5: chỉnh số liệu lại trong Inspector sao cho phù hợp với Mummy)
using UnityEngine;
public enum ChargerState { Charging, Wandering }
/// <summary>
/// AI của Charger và Mummy — lao thẳng vào player
/// khi đâm vào tường thì tạm chuyển sang lang thang ngẫu nhiên
/// Vector2.normalized. Pha lang thang dùng thời gian thực (giây) qua Time.fixedDeltaTime.
/// </summary>
[RequireComponent(typeof(Monster))]
[RequireComponent(typeof(Rigidbody2D))]
public class ChargerAndMummy : MonoBehaviour
{
    [SerializeField] private float chargeSpeed = 5f;
    [SerializeField] private int maxChargeSteps = 100;
    [SerializeField] private float stopDistance = 0.2f;
    [Header("Wander — lang thang khi bị chặn bởi tường")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float wanderStepDuration = 1.5f;
    [SerializeField] private int maxTwists = 3;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleCheckDistance = 0.3f;

    private Monster monster;
    private Rigidbody2D rb;
    private StatusEffectReceiver statusEffects;
    private Transform playerTarget;

    private ChargerState state = ChargerState.Charging;
    private int chargeStepsRemaining;
    private int twistsRemaining;
    private Vector2 wanderDirection;
    private float wanderTimer;

    private void Awake()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody2D>();
        statusEffects = GetComponent<StatusEffectReceiver>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
        chargeStepsRemaining = maxChargeSteps;
        twistsRemaining = maxTwists;
    }

    private void FixedUpdate()
    {
        if (playerTarget == null) return;
        float speedMultiplier = statusEffects != null ? statusEffects.CurrentSpeedMultiplier : 1f;
        if (speedMultiplier <= 0f) return; // đang bị đóng băng — đứng im hoàn toàn
        if (state == ChargerState.Charging) DoCharge(speedMultiplier);
        else DoWander(speedMultiplier);
    }

    private void DoCharge(float speedMultiplier)
    {
        Vector2 toPlayer = (Vector2)playerTarget.position - rb.position;
        float distance = toPlayer.magnitude;
        if (distance <= stopDistance) return;
        Vector2 direction = toPlayer.normalized;
        if (IsBlocked(direction))
        {
            EnterWanderState();
            return;
        }
        rb.MovePosition(rb.position + direction * chargeSpeed * speedMultiplier * Time.fixedDeltaTime);
        chargeStepsRemaining--;
        if (chargeStepsRemaining <= 0)
        {
            EnterWanderState();
        }
    }

    private void DoWander(float speedMultiplier)
    {
        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f)
        {
            twistsRemaining--;
            if (twistsRemaining <= 0)
            {
                EnterChargeState();
                return;
            }
            PickNewWanderDirection();
        }
        if (!IsBlocked(wanderDirection))
        {
            rb.MovePosition(rb.position + wanderDirection * wanderSpeed * speedMultiplier * Time.fixedDeltaTime);
        }
    }

    private void PickNewWanderDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        wanderTimer = wanderStepDuration;
    }

    private void EnterWanderState()
    {
        state = ChargerState.Wandering;
        twistsRemaining = maxTwists;
        PickNewWanderDirection();
    }

    private void EnterChargeState()
    {
        state = ChargerState.Charging;
        chargeStepsRemaining = maxChargeSteps;
    }

    private bool IsBlocked(Vector2 direction)
    {
        return Physics2D.Raycast(rb.position, direction, obstacleCheckDistance, obstacleLayer);
    }
}
