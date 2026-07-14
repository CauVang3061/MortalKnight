// Setup:
// 1. Tạo 2 UI Panel (giống Pause): LosePanel (chữ "Game Over" + nút "Restart" gọi LoseScene.Restart())
// và WinPanel (chữ "Winning!" + nút tương tự gọi WinScene.Restart()), cả 2 để ẩn sẵn
// 2. Gắn LoseScene.cs vào 1 GameObject quản lý, kéo GameOverPanel vào
// 3. Gắn WinScene.cs vào 1 GameObject khác, kéo WinPanel vào
// 4. Tạo cổng dịch chuyển: GameObject có Collider2D (Is Trigger) + SpriteRenderer, gắn TransferPortal.cs,
// tick IsFinalPortal (nếu đây là màn cuối) và kéo WinScene vào. Đứng gần, bấm E.
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Hiện màn hình thắng — tương đương WinScene bản gốc (gọi từ menuWinCallBack(),
/// kích hoạt khi hoàn thành màn cuối qua TransferPortal).
/// </summary>
public class WinScene : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    private void Awake()
    {
        if (winPanel != null) winPanel.SetActive(false);
    }
    /// <summary>Gọi từ TransferPortal khi Player hoàn thành màn chơi.</summary>
    public void Show()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    /// <summary>Gọi từ nút "Chơi lại" trên UI.</summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
