using UnityEngine;
[RequireComponent(typeof(Monster))]
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterWander : MonoBehaviour
{
    private Monster monster;
    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private float stateTimer;
    private bool isWalking;
    private void Awake()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody2D>();
        EnterPauseState();
    }
    private void FixedUpdate()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (isWalking)
        {
            float speedMultiplier = statusEffects != null ? statusEffects.CurrentSpeedMultiplier : 1f;
            rb.MovePosition(rb.position + currentDirection * monster.Data.moveSpeed * Time.fixedDeltaTime);
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
}
