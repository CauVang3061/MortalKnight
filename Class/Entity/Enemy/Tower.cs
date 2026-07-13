// Setup Tower prefab:
// 1. GameObject có SpriteRenderer + Collider2D (Monster.cs yêu cầu)
// 2. Gắn Monster.cs + MonsterData (dùng cho HP/chết) - không cần những cái khác vì Tower đứng yên
// 3. Gắn WeaponController.cs, tạo 1 WeaponData asset riêng cho Tower (damage, cooldown, tầm bắn tùy ý),
// Quan trọng: đổi Side trong Inspector của WeaponController thành Enemy (để đạn gây sát thương cho Player, không phải cho quái khác)
// 4. Gắn Tower.cs, chỉnh Detection Range

using UnityEngine;
/// <summary>
/// AI của Tower — turret đứng yên, tự động ngắm và bắn Player khi trong tầm.
/// Tận dụng lại WeaponController đã xây cho súng của Player — Tower chỉ cần tự tính hướng ngắm về phía Player mỗi frame rồi gọi TryFire()
/// </summary>
[RequireComponent(typeof(WeaponController))]
public class Tower : MonoBehaviour
{
    [SerializeField] private float detectionRange = 6f;
    private WeaponController weapon;
    private Transform playerTarget;
    private void Awake()
    {
        weapon = GetComponent<WeaponController>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }
    private void Update()
    {
        if (playerTarget == null) return;
        Vector2 toPlayer = (Vector2)playerTarget.position - (Vector2)transform.position;
        if (toPlayer.magnitude > detectionRange) return;
        weapon.AimDirection = toPlayer.normalized;
        weapon.TryFire();
    }
}
