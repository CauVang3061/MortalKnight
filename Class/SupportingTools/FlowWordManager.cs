using UnityEngine;
/// <summary>
/// Cho phép mọi script (Bullet, Monster, PickBottle...) gọi FlowWordManager.Spawn(...)
/// để hiện số bay lên mà không cần tự giữ tham chiếu tới prefab.
/// Đặt 1 GameObject duy nhất có script này trong scene (Singleton đơn giản).
/// </summary>
public class FlowWordManager : MonoBehaviour
{
    private static FlowWordManager instance;
    [SerializeField] private FlowWord flowWordPrefab;
    private void Awake()
    {
        instance = this;
    }
    public static void Spawn(string text, Vector3 position, Color color)
    {
        if (instance == null || instance.flowWordPrefab == null) return;
        FlowWord word = Instantiate(instance.flowWordPrefab, position, Quaternion.identity);
        word.Show(text, color);
    }
}
