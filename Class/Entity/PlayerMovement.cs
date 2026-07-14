// Cách gắn vào Unity:
// 1. Tạo một GameObject mới trong Scene, đặt tên Player
// 2. Thêm component Sprite Renderer (Add Component --> Sprite Renderer), kéo sprite nhân vật vào ô Sprite
// 3. Kéo cả 2 file PlayerInputReader.cs và PlayerMovement.cs vào GameObject Player này
// (kéo thả vào Inspector, hoặc Add Component --> gõ tên script)
// 4. Trong Inspector của PlayerMovement, kéo chính GameObject Player (hoặc component Sprite Renderer của nó)
// vào ô Sprite Renderer nếu nó chưa tự nhận
// 5. Nhấn Play, thử W/A/S/D

using UnityEngine;
/// Di chuyển nhân vật dựa trên hướng đọc được từ PlayerInputReader.
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Tốc độ di chuyển, đơn vị: unit Unity / giây")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Sprite của nhân vật, dùng để lật trái/phải theo hướng di chuyển")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private PlayerInputReader inputReader;
    // PlayerHealth bật cờ này lên khi đang chạy hiệu ứng knockback (đẩy lùi khi trúng đòn),
    // để PlayerMovement tạm ngừng đọc input — tránh 2 script cùng ghi đè transform.position
    // trong cùng 1 frame, giống cách bản gốc chặn di chuyển bằng "if (isKnockBack) return;"
    // trong Character::setTagPosition().
    public bool IsKnockedBack { get; set; }
    private void Awake()
    {
        // Lấy tham chiếu tới component PlayerInputReader gắn trên cùng GameObject
        inputReader = GetComponent<PlayerInputReader>();
        // Nếu quên kéo SpriteRenderer vào Inspector, tự tìm trên GameObject này.
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    private void Update()
    {
        if (IsKnockedBack) return; // đang bị đẩy lùi -> PlayerHealth toàn quyền điều khiển transform lúc này
        Vector2 direction = inputReader.MoveDirection;
        // Di chuyển nhân vật theo hướng input, với tốc độ moveSpeed, frame-rate independent
        float speedMultiplier = statusEffects != null ? statusEffects.CurrentSpeedMultiplier : 1f;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        UpdateFacingDirection(direction);
    }
    private void UpdateFacingDirection(Vector2 direction)
    {
        // Chỉ đổi hướng mặt khi có di chuyển theo trục ngang, giữ nguyên hướng cũ
        // khi chỉ đi lên/xuống — giống hệt logic setFaceDirByMoveDir() trong bản gốc.
        if (direction.x < 0f)
        {
            spriteRenderer.flipX = true; // quay trái (tương đương dir == 0 trong bản gốc)
        }
        else if (direction.x > 0f)
        {
            spriteRenderer.flipX = false; // quay phải (tương đương dir == 1 trong bản gốc)
        }
    }
}
