// Cách setup và test trong Unity:

// 1. Tạo Bullet prefab: GameObject có SpriteRenderer (ảnh viên đạn),
// Rigidbody2D (Body Type = Kinematic, vì tự di chuyển bằng code chứ không cần vật lý thật),
// Collider2D (tick Is Trigger), gắn script Bullet.cs. kéo thành prefab.
// 2. Tạo WeaponData asset: Chuột phải trong Project → Create → Weapon → Gun Data.
// Điền attack, fireCooldown, bulletsPerShot, kéo Bullet prefab vào Bullet Prefab.
// 3. Trên GameObject Player (đã có PlayerInputReader + PlayerMovement từ trước): tạo thêm 1 GameObject con tên WeaponPivot,
// gắn SpriteRenderer (ảnh súng) + WeaponController + PlayerAimController.
// Kéo WeaponData asset vào ô Weapon Data của WeaponController.
// 4. Thiết lập Layer/Tag: đặt Tag "Player" cho GameObject Player, "Enemy" cho quái. 
// Tạo 1 Layer tên "Wall" cho tường, gán vào ô Wall Layer của Bullet.
// 5. Nhấn Play, di chuyển chuột để súng xoay theo, giữ chuột trái để bắn.

using UnityEngine;
/// <summary>
/// Bất kỳ thứ gì có thể nhận sát thương (Player, Monster...) thì implement interface này.
/// Bullet chỉ cần biết đối tượng nó va chạm có TakeDamage() hay không
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount, Vector2 sourcePosition);
}
