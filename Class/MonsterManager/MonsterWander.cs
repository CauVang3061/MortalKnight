using UnityEngine;
[RequireComponent(typeof(Monster))]
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterWander : MonoBehaviour
{
    private Monster monster;
    private Rigidbody2D rb;
    private StatusEffectReceiver statusEffects;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 currentDirection;
    private float stateTimer;
    private bool isWalking;
    private void Awake()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody2D>();
        statusEffects = GetComponent<StatusEffectReceiver>();
        EnterPauseState();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        EnterPauseState();
    }
    private void FixedUpdate()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (isWalking)
        {
            float speedMultiplier = statusEffects != null ? statusEffects.CurrentSpeedMultiplier : 1f;
            // BƯỚC 2: Sửa công thức di chuyển (Nhân thêm biến speedMultiplier vào)
            rb.MovePosition(rb.position + currentDirection * monster.Data.moveSpeed * speedMultiplier * Time.fixedDeltaTime);

            // BƯỚC 3: Gọi hàm lật mặt quái vật mỗi khi nó di chuyển
            UpdateFacingDirection(currentDirection);
        }
        if (stateTimer <= 0f)
        {
            if (isWalking) EnterPauseState();
            else EnterWalkState();
        }
    }
    private void EnterWalkState()
    {
        isWalking = true;
        // Chọn hướng ngẫu nhiên liên tục (0-360°)
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        currentDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        stateTimer = Random.Range(0.3f, monster.Data.maxWanderDuration);
    }
    private void EnterPauseState()
    {
        isWalking = false;
        currentDirection = Vector2.zero;
        stateTimer = monster.Data.pauseDuration;
    }
    private void UpdateFacingDirection(Vector2 direction)
    {
        if (spriteRenderer == null) return;

        if (direction.x < 0f)
        {
            spriteRenderer.flipX = true;  // Đi sang trái -> Quay mặt trái
        }
        else if (direction.x > 0f)
        {
            spriteRenderer.flipX = false; // Đi sang phải -> Quay mặt phải
        }
    }
}
