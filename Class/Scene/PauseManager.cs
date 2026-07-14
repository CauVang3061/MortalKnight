using UnityEngine;
/// <summary>
/// Quản lý trạng thái tạm dừng game — tương đương PauseLayer bản gốc (được gọi khi
/// bấm nút pause trong HelloWorldScene.cpp: Director::getInstance()->pushScene(PauseLayer...)).
/// Dùng Time.timeScale = 0 để dừng toàn bộ Update/FixedUpdate dựa trên deltaTime
/// kiểm tra thủ công ở từng script như PlayerMovement/Bullet/MonsterWander
/// </summary>
public class PauseManager : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private GameObject pausePanel; // UI Panel hiện lên khi pause, kéo vào Inspector
    public static bool IsPaused { get; private set; }
    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }
    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }
    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    private void OnDestroy()
    {
        // Đảm bảo game không bị "kẹt" ở trạng thái timeScale = 0 nếu scene bị đổi trong lúc đang pause.
        Time.timeScale = 1f;
    }
}
