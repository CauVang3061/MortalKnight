using UnityEngine;
/// <summary>
/// Đọc trạng thái bàn phím mỗi frame và tính ra hướng di chuyển.
/// class này chỉ đọc input, không tự di chuyển nhân vật.
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    // Hướng di chuyển hiện tại, đã được chuẩn hóa (độ dài vector luôn <= 1).
    public Vector2 MoveDirection { get; private set; }
    private void Update()
    {
        // Đọc từng phím riêng biệt
        float horizontal = 0f;
        float vertical = 0f;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        Vector2 rawDirection = new Vector2(horizontal, vertical);
        // normalized khi đi chéo — đảm bảo tốc độ đi chéo không nhanh hơn đi thẳng.
        // Kiểm tra != Vector2.zero để tránh lỗi khi đứng yên (không có input).
        MoveDirection = rawDirection != Vector2.zero
            ? rawDirection.normalized
            : Vector2.zero;
    }
}
