// Cách setup:
// Gắn Knight.cs vào GameObject Player (cùng chỗ các script khác).
// Nhấn Space để dùng skill (lao 3 unit về hướng đang di chuyển, bất tử 0.2s, tốn 50 MP, hồi chiêu 3s).

using System.Collections;
using UnityEngine;
// Skill của Knight: lao tới theo hướng đang di chuyển, kèm bất tử tạm thời trong lúc lao
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInputReader))]
public class Knight : MonoBehaviour
{
    [SerializeField] private KeyCode activateKey = KeyCode.Space;
    [SerializeField] private float mpCost = 50f;
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float cooldown = 3f;
    public float CooldownRemaining { get; private set; }
    public bool IsOnCooldown => CooldownRemaining > 0f;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerInputReader inputReader;
    private Vector2 lastFacingDirection = Vector2.right;
    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        inputReader = GetComponent<PlayerInputReader>();
    }
    private void Update()
    {
        if (CooldownRemaining > 0f)
        {
            CooldownRemaining -= Time.deltaTime;
        }
        // Ghi nhớ hướng di chuyển gần nhất để dash theo hướng đó ngay cả khi
        // người chơi bấm skill lúc đang đứng yên (không có input di chuyển).
        if (inputReader.MoveDirection.sqrMagnitude > 0.0001f)
        {
            lastFacingDirection = inputReader.MoveDirection;
        }
        if (Input.GetKeyDown(activateKey))
        {
            TryActivate();
        }
    }
    private void TryActivate()
    {
        if (IsOnCooldown) return;
        if (!playerHealth.TrySpendMP(mpCost)) return;
        CooldownRemaining = cooldown;
        StartCoroutine(DashRoutine());
    }
    private IEnumerator DashRoutine()
    {
        playerHealth.GrantInvincibility(dashDuration);
        playerMovement.IsKnockedBack = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(lastFacingDirection.normalized * dashDistance);
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / dashDuration);
            yield return null;
        }
        transform.position = endPos;
        playerMovement.IsKnockedBack = false;
    }
}
