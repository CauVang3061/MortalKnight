// Trong Inspector của PlayerWeaponEquipper: kéo GunPivot vào ô Gun Controller, MeleePivot vào ô Melee Controller. Chỉ 1 trong 2 hiển thị tại một thời điểm (tự động bật/tắt).
// Tạo vũ khí nhặt được: GameObject nhỏ có SpriteRenderer + Collider2D (Is Trigger), gắn PickupWeapon.cs, gán 1 trong 2 ô Gun Data HOẶC Melee Data (không gán cả 2). Kéo thành prefab, thêm vào mảng Reward Prefabs của TreasureBox để rương báu có thể sinh ra nó ngẫu nhiên.

using UnityEngine;
/// <summary>
/// Quản lý việc Player đang cầm súng hay vũ khí cận chiến — tương đương phần
/// Gắn script vào Player, kéo GameObject chứa WeaponController (súng) và
/// GameObject chứa MeleeWeaponController (cận chiến) vào 2 ô tương ứng.
/// </summary>
public class PlayerWeaponEquipper : MonoBehaviour
{
    public enum EquippedType { Gun, Melee }
    [SerializeField] private WeaponController gunController;
    [SerializeField] private MeleeWeaponController meleeController;
    public EquippedType CurrentType { get; private set; } = EquippedType.Gun;
    public WeaponController GunController => gunController;
    public MeleeWeaponController MeleeController => meleeController;
    private void Awake()
    {
        ShowOnly(EquippedType.Gun);
    }
    public void EquipGun(WeaponData data)
    {
        gunController.Equip(data);
        ShowOnly(EquippedType.Gun);
    }
    public void EquipMelee(MeleeWeaponData data)
    {
        meleeController.Equip(data);
        ShowOnly(EquippedType.Melee);
    }
    private void ShowOnly(EquippedType type)
    {
        CurrentType = type;
        gunController.gameObject.SetActive(type == EquippedType.Gun);
        meleeController.gameObject.SetActive(type == EquippedType.Melee);
    }
}
