using UnityEngine;
/// <summary>
/// Đọc vị trí chuột trên màn hình, tính hướng ngắm từ vũ khí tới chuột,
/// gán vào WeaponController.AimDirection, và bắn khi giữ chuột trái.
/// </summary>
[RequireComponent(typeof(WeaponController))]
public class PlayerAimController : MonoBehaviour
{
    private WeaponController weaponController;
    private Camera mainCamera;
    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
        mainCamera = Camera.main;
    }
    private void Update()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        Vector2 aimDirection = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
        bool isFiring = Input.GetMouseButton(0);
        // Chỉ điều khiển controller đang thực sự được trang bị — controller còn lại
        // đang bị tắt (SetActive(false)) bởi PlayerWeaponEquipper
        if (equipper.CurrentType == PlayerWeaponEquipper.EquippedType.Gun)
        {
            equipper.GunController.AimDirection = aimDirection;
            if (isFiring) equipper.GunController.TryFire();
        }
        else
        {
            equipper.MeleeController.AimDirection = aimDirection;
            if (isFiring) equipper.MeleeController.TryAttack();
        }
    }
}
