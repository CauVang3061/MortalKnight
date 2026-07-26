using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Kéo Panel Pause (StopLayer) vào đây")]
    [SerializeField] private GameObject pausePanel;

    [Header("Scene Configuration")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip clickSound;

    [Header("Music Settings (Optional)")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private Image musicToggleButtonImage;
    [SerializeField] private Sprite playMusicSprite; // continueMusic.png
    [SerializeField] private Sprite stopMusicSprite; // stopMusic.png

    private bool isPaused = false;
    private bool isMusicPlaying = true;

    private void Start()
    {
        // Ẩn bảng Pause khi bắt đầu game
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Đảm bảo thời gian chạy bình thường
        Time.timeScale = 1f;

        // Tự tìm AudioSource nếu chưa gán
        if (uiAudioSource == null)
        {
            uiAudioSource = GetComponent<AudioSource>();
        }

        // Đồng bộ trạng thái ảnh của nút Âm nhạc ban đầu
        UpdateMusicButtonUI();
    }

    private void Update()
    {
        // Nhấn ESC (hoặc P) để bật/tắt nhanh Pause Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Phát tiếng Click nút
    public void PlayClickSound()
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    // 1. Tạm dừng Game
    public void PauseGame()
    {
        PlayClickSound();
        isPaused = true;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Time.timeScale = 0f; // Ngừng mọi hoạt động trong game (vật lý, chuyển động...)
    }

    // 2. Tiếp tục chơi Game
    public void ResumeGame()
    {
        PlayClickSound();
        isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f; // Khôi phục thời gian bình thường
    }

    // 3. Chơi lại màn hiện tại
    public void RestartGame()
    {
        PlayClickSound();
        Time.timeScale = 1f; // Trả lại thời gian bình thường trước khi load
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 4. Quay về Menu chính
    public void LoadMainMenu()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 5. Bật/Tắt Nhạc nền
    public void ToggleMusic()
    {
        PlayClickSound();
        if (backgroundMusicSource != null)
        {
            isMusicPlaying = !isMusicPlaying;
            backgroundMusicSource.mute = !isMusicPlaying;
            UpdateMusicButtonUI();
        }
    }

    private void UpdateMusicButtonUI()
    {
        if (musicToggleButtonImage != null)
        {
            musicToggleButtonImage.sprite = isMusicPlaying ? playMusicSprite : stopMusicSprite;
        }
    }
}
