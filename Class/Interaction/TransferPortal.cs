using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Cổng dịch chuyển — tương đương TransferPortal bản gốc (checkPortalState() +
/// menuReGenerateCallBack(), tăng dần _mapOrder qua từng màn).
/// </summary>
public class TransferPortal : Interactable
{
    [SerializeField] private bool isFinalPortal = true;
    [SerializeField] private string nextSceneName;
    [SerializeField] private WinUI winUI;

    protected override void Interact()
    {
        if (isFinalPortal)
        {
            if (winUI == null) winUI = FindFirstObjectByType<WinUI>();
            winUI?.Show();
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
