using UnityEngine;

[RequireComponent(typeof(Monster))]
public class EnemyShootAI : MonoBehaviour
{
    [Header("Weapon Config")]
    [Tooltip("Kéo component WeaponController của khẩu súng con vào đây")]
    [SerializeField] private WeaponController weaponController;

    [Tooltip("Khoảng cách tối đa quái phát hiện người chơi để bắt đầu bắn")]
    [SerializeField] private float detectionRange = 10f;

    private Transform playerTransform;
    private Monster monster;

    private void Awake()
    {
        monster = GetComponent<Monster>();

        // Tự động tìm WeaponController ở các Object con nếu quên chưa gán trong Inspector
        if (weaponController == null)
        {
            weaponController = GetComponentInChildren<WeaponController>();
        }
    }

    private void Start()
    {
        // Tìm người chơi thông qua Tag "Player" có sẵn trong Scene
        FindPlayer();
        if (weaponController != null) {
            weaponController.Side = WeaponSide.Enemy;
            if (weaponController.WeaponData == null)
            {
                Debug.LogWarning($"[EnemyShootAI] Quái { gameObject.name} có súng nhưng chưa gán WeaponData trong Inspector!", gameObject);
            }
            else if (weaponController.WeaponData == null)
            {
                Debug.LogWarning($"[EnemyShootAI] Dữ liệu {weaponController.WeaponData.name} trên quái {gameObject.name} chưa được gán Bullet Prefab!", gameObject);
            }
        }
        else
        {
            Debug.LogError($"[EnemyShootAI] Quái { gameObject.name} chưa được gắn WeaponController ở bản thân hoặc các Object con!", gameObject);
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        // Tính khoảng cách đến người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 1. Tính toán hướng ngắm từ Quái sang Người chơi
            Vector2 aimDirection = (playerTransform.position - transform.position).normalized;

            if (weaponController != null)
            {
                // 2. Truyền hướng ngắm sang khẩu súng để súng tự xoay theo hướng Player
                weaponController.AimDirection = aimDirection;

                // 3. Thực hiện bắn đạn (Hàm TryFire tự động kiểm tra cooldown hồi chiêu súng)
                weaponController.TryFire();
            }
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // Dự phòng tìm kiếm người chơi thông qua Class PlayerHealth nếu quên gắn Tag "Player"
            PlayerHealth health = FindObjectOfType<PlayerHealth>();
            if (health != null)
            {
                player = health.gameObject;
            }
        }

        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    // Vẽ vòng tròn tầm bắn trong Scene Editor để dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}