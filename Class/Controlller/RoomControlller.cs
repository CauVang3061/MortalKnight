using UnityEngine;

public class RoomController : MonoBehaviour
{
    private enum RoomState { Inactive, Active, Cleared }
    private RoomState currentState = RoomState.Inactive;

    private MonsterSpawn monsterSpawn;

    private void Start()
    {
        // Tìm Spawner quái vật trong phòng (tìm cả các đối tượng đang Inactive trong Prefab)
        monsterSpawn = GetComponentInChildren<MonsterSpawn>(true);

        // Nếu phòng không có quái vật (như phòng bắt đầu) -> Đánh dấu dọn sạch ban đầu
        if (monsterSpawn == null)
        {
            currentState = RoomState.Cleared;
        }
    }

    private void Update()
    {
        if (currentState != RoomState.Active) return;

        // Nếu phòng không có quái vật -> Tự động qua màn
        if (monsterSpawn == null)
        {
            ClearRoom();
            return;
        }

        // Kiểm tra xem quái vật đã sinh ra chưa, và nếu rồi thì đã chết hết chưa
        if (monsterSpawn.HasSpawned)
        {
            bool allDead = true;
            foreach (var monster in monsterSpawn.SpawnedMonsters)
            {
                if (monster != null)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                ClearRoom();
            }
        }
    }

    /// <summary>
    /// Kích hoạt phòng này hoạt động khi người chơi được dịch chuyển đến.
    /// </summary>
    public void ActivateRoom()
    {
        currentState = RoomState.Active;

        if (monsterSpawn != null)
        {
            monsterSpawn.TriggerSpawn();
        }
        else
        {
            // Nếu phòng không có quái vật (như phòng bắt đầu) -> Tự động dọn sạch để chuyển tiếp ngay
            ClearRoom();
        }
    }

    private void ClearRoom()
    {
        currentState = RoomState.Cleared;

        // Báo cáo thành tích dọn sạch phòng về hệ thống trung tâm DungeonGenerator
        if (DungeonGenerator.Instance != null)
        {
            DungeonGenerator.Instance.OnRoomCleared(this);
        }
    }

    // Hàm phụ trợ xác minh thực thể Player
    private bool IsPlayer(GameObject obj)
    {
        if (obj == null) return false;
        return obj.CompareTag("Player") || obj.GetComponent<PlayerHealth>() != null || obj.GetComponent("PlayerMovement") != null;
    }
}
