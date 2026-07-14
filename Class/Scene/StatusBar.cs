// Cách setup trong Unity:

// 1. Tạo Canvas (GameObject → UI → Canvas)
// 2. Trong Canvas, tạo Slider (GameObject → UI → Slider) - đặt tên HealthBar, kéo nó vào góc màn hình góc trên bên trái
// 3. Trong Slider, xóa/ẩn phần Handle Slide Area vì thanh máu không cần kéo tay - chỉ cần phần Fill Area hiển thị
// 4. Đổi màu phần Fill sang đỏ/xanh lá tùy ý
// 5. Tạo GameObject rỗng tên StatusBarManager, gắn script StatusBar.cs, kéo Slider vào ô Health Slider
// (ô Player Health có thể để trống, script tự tìm theo Tag "Player")
// 6. Nhấn Play, để quái đánh trúng Player - thanh máu sẽ tự giảm theo
// 7. Thêm 2 Slider nữa trong Canvas (Armor, MP), kéo vào ô Armor Slider/MP Slider của StatusBar (để trống nếu không muốn hiển thị)

using UnityEngine;
using UnityEngine.UI;
// Cập nhật thanh máu/giáp/MP (Slider) trên UI dựa theo event từ PlayerHealth.
public class StatusBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider armorSlider;
    [SerializeField] private Slider mpSlider;
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
        if (playerHealth == null) return;
        playerHealth.OnHealthChanged += HandleHealthChanged;
        playerHealth.OnArmorChanged += HandleArmorChanged;
        playerHealth.OnMPChanged += HandleMPChanged;
        // Cập nhật ngay khi bật lên
        HandleHealthChanged(playerHealth.CurrentHP, GetSliderMax(healthSlider, playerHealth.CurrentHP));
        HandleArmorChanged(playerHealth.CurrentArmor, GetSliderMax(armorSlider, playerHealth.CurrentArmor));
        HandleMPChanged(playerHealth.CurrentMP, GetSliderMax(mpSlider, playerHealth.CurrentMP));
    }
    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.OnHealthChanged -= HandleHealthChanged;
        playerHealth.OnArmorChanged -= HandleArmorChanged;
        playerHealth.OnMPChanged -= HandleMPChanged;
    }
    private void HandleHealthChanged(float current, float max) => UpdateSlider(healthSlider, current, max);
    private void HandleArmorChanged(float current, float max) => UpdateSlider(armorSlider, current, max);
    private void HandleMPChanged(float current, float max) => UpdateSlider(mpSlider, current, max);
    private void UpdateSlider(Slider slider, float current, float max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = current;
    }
    private float GetSliderMax(Slider slider, float fallback)
    {
        return slider != null && slider.maxValue > 0f ? slider.maxValue : fallback;
    }
}
