using UnityEngine;
using UnityEngine.SceneManagement;
public class LoseScene : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private PlayerHealth playerHealth;
    private void Awake()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
