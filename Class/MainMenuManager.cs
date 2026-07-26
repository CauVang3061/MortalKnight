using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("Tên chính xác của Scene màn chơi mà người chơi sẽ bắt đầu.")]
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip startGameSound;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        Debug.Log($"[MainMenu] Manager đã sẵn sàng. Tên Scene cần tải: {gameplaySceneName}");
    }

    public void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void StartGame()
    {
        Debug.Log("[MainMenu] Đã nhấn nút START!");

        // Kiểm tra xem Scene có tồn tại trong danh sách build không
        if (Application.CanStreamedLevelBeLoaded(gameplaySceneName))
        {
            // Cách xử lý tối ưu:
            // 1. Phát tiếng click chuột ngắn (clickSound - button.mp3) để phản hồi người chơi ngay lập tức
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }

            // 2. Chuyển cảnh gần như ngay lập tức (chỉ trì hoãn 0.15 giây để tiếng click phát ra tự nhiên)
            Debug.Log("[MainMenu] Chuyển scene sau 0.15 giây...");
            Invoke(nameof(LoadGameplayScene), 0.15f);
        }
        else
        {
            Debug.LogError($"[ERROR] Không thể chuyển cảnh! Scene '{gameplaySceneName}' KHÔNG tồn tại hoặc CHƯA ĐƯỢC THÊM vào File -> Build Profiles!");
        }
    }

    private void LoadGameplayScene()
    {
        Debug.Log($"[MainMenu] Bắt đầu tải Scene: {gameplaySceneName}");
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] Đã nhấn nút EXIT!");
        PlayClickSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}