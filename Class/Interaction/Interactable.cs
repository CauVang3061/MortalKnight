using UnityEngine;

// Base cho mọi vật thể Player có thể tương tác (rương báu, nhặt vũ khí, cổng dịch chuyển...).
[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject promptIcon;

    protected bool IsPlayerNear { get; private set; }
    protected GameObject PlayerObject { get; private set; }

    protected virtual void Awake()
    {
        // Tự động kiểm tra và cảnh báo nếu lập trình viên quên gán Collider là Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"[Interactable] Đối tượng {gameObject.name} có Collider nhưng chưa bật 'Is Trigger'. Đã bật tự động để hỗ trợ tương tác đi xuyên qua!", gameObject);
            col.isTrigger = true;
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj == null) return false;
        return obj.CompareTag("Player") || obj.GetComponent<PlayerHealth>() != null || obj.GetComponent("PlayerMovement") != null;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other.gameObject)) return;
        IsPlayerNear = true;
        PlayerObject = other.gameObject;
        Debug.Log($"[Interactable] Người chơi đã đến gần: {gameObject.name}");
        if (promptIcon != null) promptIcon.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other.gameObject)) return;
        IsPlayerNear = false;
        PlayerObject = null;
        Debug.Log($"[Interactable] Người chơi đã đi xa khỏi: {gameObject.name}");
        if (promptIcon != null) promptIcon.SetActive(false);
    }

    // Hỗ trợ dự phòng trường hợp va chạm vật lý cứng (Solid Collision) để game không bao giờ bị lỗi nhặt
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsPlayer(collision.gameObject)) return;
        IsPlayerNear = true;
        PlayerObject = collision.gameObject;
        Debug.Log($"[Interactable] Người chơi chạm vật lý cứng với: {gameObject.name}");
        if (promptIcon != null) promptIcon.SetActive(true);
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsPlayer(collision.gameObject)) return;
        IsPlayerNear = false;
        PlayerObject = null;
        Debug.Log($"[Interactable] Người chơi dừng chạm vật lý với: {gameObject.name}");
        if (promptIcon != null) promptIcon.SetActive(false);
    }

    protected virtual void Update()
    {
        if (IsPlayerNear && Input.GetKeyDown(interactKey))
        {
            Debug.Log($"[Interactable] Nhấn nút tương tác [ {interactKey} ] với {gameObject.name}");
            Interact();
        }
    }

    protected abstract void Interact();
}
