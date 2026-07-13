using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Cổng dịch chuyển — tương đương TransferPortal bản gốc (checkPortalState() +
/// menuReGenerateCallBack() trong HelloWorldScene.cpp, tăng dần _mapOrder qua từng màn).
/// Kế thừa Interactable nên tự có sẵn cơ chế "đứng gần + bấm E" từ trước.
/// </summary>
public class TransferPortal : Interactable
{
    [SerializeField] private bool isFinalPortal = true;
    [SerializeField] private string nextSceneName;
    [Tooltip("Kéo GameObject có WinUI vào đây nếu isFinalPortal = true")]
    [SerializeField] private WinUI winUI;
    private bool isUsed;
    protected override void Interact()
    {
        if (isUsed) return;
        isUsed = true;
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
