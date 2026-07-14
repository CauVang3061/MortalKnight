using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Màn hình chính — tương đương StartScene bản gốc:
/// nút Start (vào game), nút End (thoát), nút bật/tắt nhạc nền, phát âm thanh click khi bấm bất kỳ nút nào
/// Đặt script này trong 1 Scene riêng (ví dụ "MainMenu") — Start sẽ load Scene gameplay bằng tên
/// Phải thêm cả 2 Scene (MainMenu + Gameplay) vào File --> Build Settings --> Scenes In Build
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonClickClip;
    private void Awake()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
    /// Gọi từ nút Start
    public void OnStartClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene(gameplaySceneName);
    }
    /// Gọi từ nút End - thoát game
    public void OnQuitClicked()
    {
        PlayClickSound();
        Application.Quit();
#if UNITY_EDITOR
        // Application.Quit() không có tác dụng trong Editor, dùng lệnh dưới để test nút End khi nhấn Play trong Unity
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    /// Gọi từ nút bật/tắt nhạc
    public void OnToggleMusicClicked()
    {
        PlayClickSound();
        if (musicSource == null) return;
        if (musicSource.isPlaying) musicSource.Pause();
        else musicSource.UnPause();
    }
    private void PlayClickSound()
    {
        if (sfxSource != null && buttonClickClip != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }
}
