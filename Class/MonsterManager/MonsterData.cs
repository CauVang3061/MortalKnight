// Cách setup và test trong Unity:
// 1. Tạo MonsterData asset: Create → Monster → Monster Data, điền maxHP, moveSpeed, contactDamage
// 2. Tạo Monster prefab: GameObject có SpriteRenderer,
// Rigidbody2D (Body Type = Dynamic, Gravity Scale = 0 vì top-down không cần trọng lực),
// Collider2D (không tick Is Trigger - để va chạm vật lý thật với tường),
// gắn Monster.cs + MonsterWander.cs, kéo MonsterData asset vào. Đặt Tag "Enemy". Kéo thành prefab.
// 3. Cập nhật Player: đảm bảo Player có Tag "Player" và có Collider2D (để Monster và Bullet nhận diện được)
// 4. Tạo vùng bẫy quái: GameObject rỗng đặt trong phòng, thêm CircleCollider2D
// (tick Is Trigger, Radius phù hợp - bản gốc dùng 250px), gắn MonsterSpawn.cs, kéo các Monster prefab vào mảng Monster Prefabs
// 5. Thiết lập Layer "Wall" cho tường, gán vào Obstacle Layer của MonsterSpawn

using UnityEngine;
/// <summary>
/// Dữ liệu của 1 loại quái. Giống WeaponData — mỗi loại quái là 1 asset với stat khác nhau.
/// Logic di chuyển/tấn công (AI) khác nhau bằng cách gắn component AI khác nhau vào từng prefab — MonsterData chỉ chứa số, không chứa hành vi.
/// </summary>
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "Monster/Monster Data")]
public class MonsterData : ScriptableObject
{
    public float maxHP = 10f;
    public float moveSpeed = 3f;
    public float contactDamage = 1f;
    [Tooltip("Thời gian tối đa (giây) cho mỗi lần đi theo 1 hướng trước khi đổi hướng mới")]
    public float maxWanderDuration = 2f;
    [Tooltip("Thời gian đứng yên giữa 2 lần đổi hướng")]
    public float pauseDuration = 1f;
}
