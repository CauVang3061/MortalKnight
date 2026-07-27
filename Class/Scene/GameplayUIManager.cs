using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Instance { get; private set; }

    [Header("UI Panels")]
    [Tooltip("Kéo Panel Pause (StopLayer) vào đây")]
    [SerializeField] private GameObject pausePanel;
    [Tooltip("Kéo Panel Thua cuộc (GameOverPanel) vào đây")]
    [SerializeField] private GameObject gameOverPanel;
    [Tooltip("Kéo Panel Chiến thắng (VictoryPanel) vào đây")]
    [SerializeField] private GameObject victoryPanel;
    [Tooltip("Kéo Panel Chuyển phòng (ProceedPanel) vào đây - Bỏ trống nếu muốn tự tạo chữ động")]
    [SerializeField] private GameObject proceedPanel;
    [Tooltip("Kéo tham chiếu PlayerHealth của Player vào đây, hoặc tự tìm theo Tag")]
    [SerializeField] private PlayerHealth playerHealth;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ẩn các bảng khi bắt đầu game
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
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

        // Tìm kiếm và đăng ký sự kiện chết của Player
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

        if (playerHealth != null)
        {
            playerHealth.OnDied += HandlePlayerDied;
        }
    }

    private void Update()
    {
        // Nếu màn hình Game Over hoặc Victory đang hiện, không cho phép mở Pause Menu nữa
        if (gameOverPanel != null && gameOverPanel.activeSelf) return;
        if (victoryPanel != null && victoryPanel.activeSelf) return;

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

    private void OnDestroy()
    {
        // Hủy đăng ký event để tránh rò rỉ bộ nhớ
        if (playerHealth != null)
        {
            playerHealth.OnDied -= HandlePlayerDied;
        }
    }

    private void HandlePlayerDied()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Dừng thời gian game khi thua cuộc
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Dừng thời gian game khi thắng cuộc
    }

    private GameObject autoProceedPanel;

    public void ShowProceedScreen(bool show)
    {
        if (proceedPanel != null)
        {
            proceedPanel.SetActive(show);
        }
        else
        {
            // Tự động tạo chữ thông báo màu xanh lục nếu người chơi chưa kéo thả Proceed Panel trong Inspector
            if (autoProceedPanel == null && show)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    autoProceedPanel = new GameObject("AutoProceedPanel");
                    autoProceedPanel.transform.SetParent(canvas.transform, false);

                    // Thêm Text component
                    var text = autoProceedPanel.AddComponent<UnityEngine.UI.Text>();
                    text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    text.fontSize = 28;
                    text.color = Color.green;
                    text.text = "ROOM CLEARED!\nPress [E] to Proceed to Next Room";
                    text.alignment = TextAnchor.MiddleCenter;

                    // Thêm Outline viền đen để chữ nổi bật trên mọi nền game
                    var outline = autoProceedPanel.AddComponent<UnityEngine.UI.Outline>();
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(2f, -2f);

                    // Định vị căn giữa nửa trên màn hình
                    var rect = autoProceedPanel.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.sizeDelta = new Vector2(600, 120);
                    rect.anchoredPosition = new Vector2(0, 100);
                }
            }

            if (autoProceedPanel != null)
            {
                autoProceedPanel.SetActive(show);
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
