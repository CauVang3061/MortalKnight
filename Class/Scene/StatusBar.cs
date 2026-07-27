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
    [Header("Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider armorSlider;
    [SerializeField] private Slider mpSlider;
    private void Awake()
    {
        FindPlayerIfNeeded();
    }
    private void OnEnable()
    {
        FindPlayerIfNeeded();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            playerHealth.OnArmorChanged += HandleArmorChanged;
            playerHealth.OnMPChanged += HandleMPChanged;

            // Cập nhật ngay khi bật lên, đề phòng trường hợp UI bị tắt đi bật lại trong lúc chơi
            InitializeUI();
        }
    }
    private void Start()
    {
        // Start luôn chạy sau tất cả Awake(), đảm bảo PlayerHealth đã khởi tạo xong giá trị
        FindPlayerIfNeeded();
        InitializeUI();
    }
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
            playerHealth.OnArmorChanged -= HandleArmorChanged;
            playerHealth.OnMPChanged -= HandleMPChanged;
        }
    }
    private void FindPlayerIfNeeded()
    {
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
            else
            {
                playerHealth = FindObjectOfType<PlayerHealth>();
            }
        }
    }
    private void InitializeUI()
    {
        if (playerHealth != null)
        {
            HandleHealthChanged(playerHealth.CurrentHP, playerHealth.MaxHP);
            HandleArmorChanged(playerHealth.CurrentArmor, playerHealth.MaxArmor);
            HandleMPChanged(playerHealth.CurrentMP, playerHealth.MaxMP);
        }
    }
    private void HandleHealthChanged(float current, float max)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    private void HandleArmorChanged(float current, float max)
    {
        if (armorSlider == null) return;
        armorSlider.maxValue = max;
        armorSlider.value = current;
    }

    private void HandleMPChanged(float current, float max)
    {
        if (mpSlider == null) return;
        mpSlider.maxValue = max;
        mpSlider.value = current;
    }
}
