// Cách setup trong Unity:

// 1. Tạo Canvas (GameObject → UI → Canvas)
// 2. Trong Canvas, tạo Slider (GameObject → UI → Slider) - đặt tên HealthBar, kéo nó vào góc màn hình góc trên bên trái
// 3. Trong Slider, xóa/ẩn phần Handle Slide Area vì thanh máu không cần kéo tay - chỉ cần phần Fill Area hiển thị
// 4. Đổi màu phần Fill sang đỏ/xanh lá tùy ý
// 5. Tạo GameObject rỗng tên StatusBarManager, gắn script StatusBar.cs, kéo Slider vào ô Health Slider
// (ô Player Health có thể để trống, script tự tìm theo Tag "Player")
// 6. Nhấn Play, để quái đánh trúng Player - thanh máu sẽ tự giảm theo

using UnityEngine;
using UnityEngine.UI;
// Cập nhật thanh máu (Slider) trên UI dựa theo event từ PlayerHealth.
public class StatusBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;
    private void Awake()
    {
        // Nếu quên kéo tham chiếu trong Inspector, tự tìm Player trong scene theo Tag.
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }
    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            // Cập nhật ngay khi bật lên, không cần chờ lần TakeDamage đầu tiên.
            HandleHealthChanged(playerHealth.CurrentHP, GetMaxHP());
        }
    }
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }
    private void HandleHealthChanged(float current, float max)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }
    private float GetMaxHP()
    {
        return healthSlider != null && healthSlider.maxValue > 0f ? healthSlider.maxValue : 10f;
    }
}
