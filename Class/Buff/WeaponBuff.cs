using UnityEngine;
[CreateAssetMenu(fileName = "NewWeaponBuff", menuName = "Weapon/Weapon Buff Data")]
public class WeaponBuff : ScriptableObject
{
    public string buffName = "Strengthen";
    [Header("Cộng thêm vào chỉ số gốc (nếu chỉ số là âm, tức bị giảm)")]
    public float attackBonus = 0f;
    public float cooldownReduction = 0f; // trừ vào fireCooldown --> bắn nhanh hơn
    public float rangeBonus = 0f;
    public float spreadReduction = 0f;   // trừ vào spreadAngle --> bắn chính xác hơn
    public bool isPermanent = false;
    [Tooltip("Chỉ dùng nếu IsPermanent = false")]
    public float duration = 5f;
}
