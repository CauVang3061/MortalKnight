using UnityEngine;
// Base cho mọi vật thể Player có thể tương tác (rương báu, nhặt vũ khí, cổng dịch chuyển...).
[RequireComponent(typeof(Collider2D))]
public abstract class Interaction : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject promptIcon;
    protected bool IsPlayerNear { get; private set; }
    protected GameObject PlayerObject { get; private set; }
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsPlayerNear = true;
        if (promptIcon != null) promptIcon.SetActive(true);
    }
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsPlayerNear = false;
        if (promptIcon != null) promptIcon.SetActive(false);
    }
    protected virtual void Update()
    {
        if (IsPlayerNear && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }
    protected abstract void Interact();
}
