using UnityEngine;
public class PickupWeapon : Interactable
{
    [SerializeField] private WeaponData gunData;
    [SerializeField] private MeleeWeaponData meleeData;
    protected override void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        if (!player.TryGetComponent<PlayerWeaponEquipper>(out var equipper)) return;
        if (gunData != null)
        {
            equipper.EquipGun(gunData);
        }
        else if (meleeData != null)
        {
            equipper.EquipMelee(meleeData);
        }
        Destroy(gameObject);
    }
}
