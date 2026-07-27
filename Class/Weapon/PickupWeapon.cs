using UnityEngine;
public class PickupWeapon : Interactable
{
    [SerializeField] private WeaponData gunData;
    [SerializeField] private MeleeWeaponData meleeData;
    protected override void Interact()
    {
        // Sử dụng PlayerObject từ lớp cha Interactable, hoặc tự tìm kiếm dự phòng
        GameObject player = PlayerObject;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player == null)
        {
            PlayerHealth health = FindObjectOfType<PlayerHealth>();
            if (health != null) player = health.gameObject;
        }

        if (player == null)
        {
            Debug.LogError("[PickupWeapon] Không tìm thấy Player để trang bị vũ khí!");
            return;
        }

        Debug.Log($"[PickupWeapon] Đang chuẩn bị trang bị vũ khí cho: {player.name}");

        if (!player.TryGetComponent<PlayerWeaponEquipper>(out var equipper))
        {
            Debug.LogError($"[PickupWeapon] LỖI: GameObject '{player.name}' KHÔNG có Component 'PlayerWeaponEquipper'! Hãy gắn thêm script 'PlayerWeaponEquipper.cs' vào đối tượng nhân vật chính (Player) trong Unity Inspector.");
            return;
        }

        if (gunData != null)
        {
            Debug.Log($"[PickupWeapon] Đang trang bị Súng thành công: {gunData.name}");
            equipper.EquipGun(gunData);
        }
        else if (meleeData != null)
        {
            Debug.Log($"[PickupWeapon] Đang trang bị Vũ khí cận chiến thành công: {meleeData.name}");
            equipper.EquipMelee(meleeData);
        }
        else
        {
            Debug.LogWarning("[PickupWeapon] CẢNH BÁO: Vật phẩm nhặt súng này chưa được gán dữ liệu GunData hay MeleeData trong Unity Inspector!");
        }

        Destroy(gameObject);
    }
}
