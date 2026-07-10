// Cách setup:
// tạo MeleeWeaponData asset, gắn MeleeWeaponController.cs vào 1 GameObject con của Player, gán targetLayer = layer "Enemy"
using UnityEngine;
/// <summary>
/// Dữ liệu vũ khí cận chiến (Fish, Wand...) — cùng tinh thần data-driven với WeaponData: add vào trong Unity, không cần thêm code
/// </summary>
[CreateAssetMenu(fileName = "NewMeleeData", menuName = "Weapon/Melee Data")]
public class MeleeWeaponData : ScriptableObject
{
    public string weaponName = "Fish";
    public Sprite weaponSprite;
    public float attack = 3f;
    public float attackCooldown = 0.5f;
    [Tooltip("Bán kính gây sát thương — mọi kẻ địch trong bán kính này đều trúng đòn, không cần đúng hướng")]
    public float attackRange = 1.2f;
    public float swingDuration = 0.2f;
}
