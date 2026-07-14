using UnityEngine;
/// <summary>
/// Chữ/số bay lên rồi mờ dần và biến mất: dùng để hiện số sát thương, số hồi máu, hoặc nhãn vật phẩm khi nhặt.
/// Gắn component này vào 1 prefab có sẵn TextMesh.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class FlowWord : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f;
    [SerializeField] private float lifetime = 1f;
    private TextMesh textMesh;
    private float timer;
    private Color baseColor;
    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }
    /// Gọi ngay sau khi Instantiate để thiết lập nội dung và màu chữ
    public void Show(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        baseColor = color;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        float alpha = Mathf.Clamp01(1f - timer / lifetime);
        Color c = baseColor;
        c.a = alpha;
        textMesh.color = c;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
