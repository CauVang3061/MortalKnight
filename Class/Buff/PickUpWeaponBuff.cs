using UnityEngine;
/// <summary>
/// Vật phẩm buff vũ khí nằm trên sàn - áp dụng vào bất kỳ vũ khí nào Player
/// đang cầm lúc đó (súng hoặc cận chiến, dựa theo PlayerWeaponEquipper.CurrentType).
/// </summary>
public class PickupWeaponBuff : Interactable
{
    [SerializeField] private WeaponBuffData buffData;
    protected override void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        if (!player.TryGetComponent<PlayerWeaponEquipper>(out var equipper)) return;
        MonoBehaviour activeController = equipper.CurrentType == PlayerWeaponEquipper.EquippedType.Gun
            ? (MonoBehaviour)equipper.GunController
            : (MonoBehaviour)equipper.MeleeController;
        if (activeController != null && activeController.TryGetComponent<WeaponBuffController>(out var buffController))
        {
            buffController.AddBuff(buffData);
        }
        Destroy(gameObject);
    }
}
