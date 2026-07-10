using UnityEngine;
/// <summary>
/// Dữ liệu của 1 khẩu súng. Mỗi khẩu súng là 1 asset ScriptableObject với stat khác nhau.
/// Muốn thêm súng mới, chỉ cần chuột phải -> Create -> Weapon -> Gun Data, không cần viết thêm code.
/// </summary>
[CreateAssetMenu(fileName = "NewGunData", menuName = "Weapon/Gun Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Old Pistol";
    public Sprite weaponSprite;
    public float attack = 3f;
    public float fireCooldown = 0.3f;
    [Tooltip("Tầm bắn tối đa để tự động khóa mục tiêu (nếu súng gắn cho quái/turret dùng auto-aim)")]
    public float attackRange = 300f;
    [Tooltip("Độ chính xác: số càng lớn thì đạn càng lệch nhiều so với hướng ngắm (đơn vị: độ)")]
    public float spreadAngle = 5f;
    [Tooltip("Số viên đạn bắn ra mỗi lần nhấn — ví dụ Shotgun sẽ để 5-8")]
    public int bulletsPerShot = 1;
    public Bullet bulletPrefab;
    public float bulletSpeed = 15f;
}
