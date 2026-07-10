// Setup: giống hệt PickupWeapon - GameObject nhỏ có Collider2D (Is Trigger) + SpriteRenderer, gắn PickBottle.cs, chọn Bottle Type (Red/Blue) trong Inspector.
using UnityEngine;
public class PickBottle : Interactable
{
    public enum BottleType { Red, Blue }
    [SerializeField] private BottleType bottleType = BottleType.Red;
    [SerializeField] private float healAmount = 2f;
    [SerializeField] private float mpRestoreAmount = 80f;
    protected override void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        if (!player.TryGetComponent<PlayerHealth>(out var health)) return;
        if (bottleType == BottleType.Red)
        {
            health.Heal(healAmount);
        }
        else
        {
            health.RestoreMP(mpRestoreAmount);
        }
        Destroy(gameObject);
    }
}
