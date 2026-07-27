using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Sinh dungeon ngẫu nhiên bằng cách ghép các phòng theo lưới tọa độ nguyên.
/// 1. "Ô đã đặt chỗ trước" (reservedCells)
/// 2. Bắt buộc khớp cửa với phòng lân cận đã đặt — tương đương exitInfo/isDirAvailable.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

    [Tooltip("Phòng bắt đầu — luôn được đặt tại ô (0,0)")]
    [SerializeField] private RoomTemplate startRoomPrefab;
    [Tooltip("Danh sách các phòng thường, được chọn ngẫu nhiên khi mở rộng dungeon")]
    [SerializeField] private RoomTemplate[] roomPrefabs;
    [Tooltip("Tổng số phòng muốn sinh ra, kể cả phòng bắt đầu")]
    [SerializeField] private int totalRoomCount = 10;
    [Tooltip("Khoảng cách giữa tâm 2 phòng liền kề. X là khoảng cách chiều ngang (Width), Y là khoảng cách chiều dọc (Height). Phải khớp với kích thước phòng thiết kế trong Tilemap")]
    [SerializeField] private Vector2 roomSpacing = new Vector2(20f, 20f);

    public Vector2 RoomSpacing => roomSpacing;

    // Quản lý trạng thái chiến thắng toàn bộ Dungeon
    public int TotalEnemyRooms { get; private set; } = 0;
    public int ClearedEnemyRooms { get; private set; } = 0;

    // Danh sách các phòng sắp xếp tuần tự theo thứ tự đi qua
    public readonly List<RoomController> orderedRooms = new List<RoomController>();
    private int currentRoomIndex = 0;
    private bool waitingForTeleport = false;

    // Lưu phòng đã thực sự được đặt: tọa độ lưới -> RoomTemplate đã instantiate
    private readonly Dictionary<Vector2Int, RoomTemplate> placedRooms = new Dictionary<Vector2Int, RoomTemplate>();
    // Các ô đã được "đặt chỗ trước" (đang chờ trong hàng đợi, chưa thực sự sinh phòng)
    private readonly HashSet<Vector2Int> reservedCells = new HashSet<Vector2Int>();

    private struct FrontierCell
    {
        public Vector2Int cell;
        public DoorDirection requiredDoor;

        public FrontierCell(Vector2Int cell, DoorDirection requiredDoor)
        {
            this.cell = cell;
            this.requiredDoor = requiredDoor;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        // Reset chỉ số khi sinh màn chơi mới
        TotalEnemyRooms = 0;
        ClearedEnemyRooms = 0;

        // Hàng đợi các ô cần xử lý, kèm theo hướng cửa bắt buộc phải có
        // (để nối ngược lại với phòng cha đã sinh ra nó).
        var frontier = new Queue<FrontierCell>();
        // Bước 1: đặt phòng bắt đầu tại (0,0), không yêu cầu cửa bắt buộc nào.
        PlaceRoom(Vector2Int.zero, startRoomPrefab, frontier);
        // Bước 2: mở rộng dần cho đến khi đủ số phòng hoặc hết hàng đợi.
        while (frontier.Count > 0 && placedRooms.Count < totalRoomCount)
        {
            FrontierCell current = frontier.Dequeue();
            Vector2Int cell = current.cell;
            DoorDirection requiredDoor = current.requiredDoor;
            // Ô này có thể đã bị phòng khác chiếm mất trong lúc chờ hàng đợi — bỏ qua.
            if (placedRooms.ContainsKey(cell)) continue;
            RoomTemplate chosenPrefab = PickCompatibleRoom(cell, requiredDoor);
            if (chosenPrefab == null) continue; // không tìm được phòng phù hợp, bỏ qua nhánh này
            PlaceRoom(cell, chosenPrefab, frontier);
        }

        Debug.Log($"[Dungeon] Đã sinh xong {placedRooms.Count} phòng. Số phòng có quái vật: {TotalEnemyRooms}");

        // Sắp xếp các phòng một cách tuần tự (đảm bảo phòng bắt đầu (0,0) luôn đứng đầu ở vị trí số 0)
        orderedRooms.Clear();
        if (placedRooms.TryGetValue(Vector2Int.zero, out RoomTemplate startRoom))
        {
            RoomController startCtrl = startRoom.GetComponent<RoomController>();
            if (startCtrl != null)
            {
                orderedRooms.Add(startCtrl);
            }
        }

        foreach (var kvp in placedRooms)
        {
            if (kvp.Key == Vector2Int.zero) continue;
            RoomController ctrl = kvp.Value.GetComponent<RoomController>();
            if (ctrl != null)
            {
                orderedRooms.Add(ctrl);
            }
        }

        currentRoomIndex = 0;
        waitingForTeleport = false;

        // Trì hoãn một phần mười giây để đảm bảo toàn bộ đối tượng trong Scene được khởi tạo hoàn chỉnh
        Invoke("InitializeFirstRoom", 0.1f);
    }

    /// <summary>
    /// Chọn 1 prefab phòng phù hợp cho ô `cell`, dựa trên các phòng lân cận ĐÃ ĐẶT:
    /// - Hướng nào có phòng lân cận với cửa hướng về phía mình -> BẮT BUỘC phải có cửa đó.
    /// - Hướng nào có phòng lân cận KHÔNG có cửa hướng về phía mình -> BẮT BUỘC không được có cửa đó
    /// (tránh cửa dẫn thẳng vào tường của phòng bên cạnh).
    /// </summary>
    private RoomTemplate PickCompatibleRoom(Vector2Int cell, DoorDirection requiredDoor)
    {
        DoorDirection mustHave = requiredDoor;
        DoorDirection mustNotHave = DoorDirection.None;
        foreach (DoorDirection dir in new[] { DoorDirection.Up, DoorDirection.Down, DoorDirection.Left, DoorDirection.Right })
        {
            Vector2Int neighborCell = cell + RoomTemplate.GetGridOffset(dir);
            if (!placedRooms.TryGetValue(neighborCell, out RoomTemplate neighborRoom)) continue;
            DoorDirection neighborSideTowardsUs = RoomTemplate.GetOpposite(dir);
            bool neighborHasDoorTowardsUs = (neighborRoom.doors & neighborSideTowardsUs) != 0;
            if (neighborHasDoorTowardsUs) mustHave |= dir;
            else mustNotHave |= dir;
        }
        var candidates = roomPrefabs
            .Where(r => (r.doors & mustHave) == mustHave && (r.doors & mustNotHave) == DoorDirection.None)
            .ToList();
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    /// <summary>
    /// Thực sự instantiate phòng vào world, đánh dấu ô là đã đặt,
    /// và thêm các ô lân cận (theo cửa của phòng này) vào hàng đợi để mở rộng tiếp.
    /// </summary>
    private void PlaceRoom(Vector2Int cell, RoomTemplate prefab, Queue<FrontierCell> frontier)
    {
        Vector3 worldPos = new Vector3(cell.x * roomSpacing.x, cell.y * roomSpacing.y, 0f);
        RoomTemplate instance = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        placedRooms[cell] = instance;

        // Tự động thêm RoomController để quản lý việc nhốt người chơi và mở cửa phòng
        RoomController roomCtrl = instance.gameObject.AddComponent<RoomController>();
        MonsterSpawn spawn = instance.GetComponentInChildren<MonsterSpawn>(true);
        if (spawn != null)
        {
            TotalEnemyRooms++;
        }

        reservedCells.Remove(cell);
        foreach (DoorDirection dir in new[] { DoorDirection.Up, DoorDirection.Down, DoorDirection.Left, DoorDirection.Right })
        {
            if ((instance.doors & dir) == 0) continue; // phòng này không có cửa hướng đó
            Vector2Int neighborCell = cell + RoomTemplate.GetGridOffset(dir);
            // Bỏ qua nếu ô đó đã có phòng, hoặc đã được đặt chỗ trước bởi nhánh khác.
            if (placedRooms.ContainsKey(neighborCell) || reservedCells.Contains(neighborCell)) continue;
            reservedCells.Add(neighborCell);
            frontier.Enqueue(new FrontierCell(neighborCell, RoomTemplate.GetOpposite(dir)));
        }
    }

    private void InitializeFirstRoom()
    {
        if (orderedRooms.Count > 0)
        {
            RoomController startRoom = orderedRooms[0];

            // Tìm kiếm người chơi qua tag hoặc component dự phòng
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                PlayerHealth health = FindObjectOfType<PlayerHealth>();
                if (health != null) player = health.gameObject;
            }

            if (player != null)
            {
                Debug.Log($"[Dungeon] Đang khởi tạo phòng bắt đầu: {startRoom.gameObject.name}");

                // Dịch chuyển Player đến tâm của phòng bắt đầu (đặt Z = 0f để đồng bộ physics và rendering)
                player.transform.position = new Vector3(startRoom.transform.position.x, startRoom.transform.position.y, 0f);

                // Dịch chuyển Main Camera đến tâm phòng bắt đầu
                if (Camera.main != null)
                {
                    Camera.main.transform.position = new Vector3(startRoom.transform.position.x, startRoom.transform.position.y, -10f);
                }

                // Log tọa độ cụ thể ra Console để theo dõi dễ dàng
                Debug.Log($"[Dungeon - KHỞI TẠO] Tọa độ XYZ cụ thể của Room 1 ({startRoom.gameObject.name}): {startRoom.transform.position}");
                Debug.Log($"[Dungeon - KHỞI TẠO] Tọa độ XYZ cụ thể của Player: {player.transform.position}");

                // Dịch chuyển các Cinemachine Virtual Camera đến tâm phòng bắt đầu để tránh bị kéo/trượt chậm
                WarpVirtualCameras(startRoom.transform.position);

                // Kích hoạt phòng bắt đầu hoạt động
                startRoom.ActivateRoom();
            }
        }
    }

    private void WarpVirtualCameras(Vector3 targetPosition)
    {
        // Tự động tìm kiếm các thành phần Cinemachine trong Scene để dịch chuyển tức thì theo Player
        var behaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (var b in behaviours)
        {
            if (b != null && b.enabled && (b.GetType().Name.Contains("CinemachineVirtualCamera") || b.GetType().Name.Contains("CinemachineCamera")))
            {
                // Thay đổi vị trí của camera ảo trùng với vị trí phòng mới
                b.transform.position = new Vector3(targetPosition.x, targetPosition.y, b.transform.position.z);

                // Sử dụng Reflection để gọi hàm OnTargetObjectWarped của Cinemachine
                // Hàm này giúp báo cho Cinemachine biết mục tiêu đã dịch chuyển tức thời, nó sẽ snap ngay lập tức (không trượt chậm)
                var warpMethod = b.GetType().GetMethod("OnTargetObjectWarped");
                if (warpMethod != null)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player == null)
                    {
                        PlayerHealth health = FindObjectOfType<PlayerHealth>();
                        if (health != null) player = health.gameObject;
                    }

                    if (player != null)
                    {
                        warpMethod.Invoke(b, new object[] { player.transform, player.transform.position - b.transform.position });
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Lắng nghe nút E khi phòng hiện tại đã dọn sạch và đang chờ chuyển tiếp
        if (waitingForTeleport && Input.GetKeyDown(KeyCode.E))
        {
            TeleportToNextRoom();
        }
    }

    private void TeleportToNextRoom()
    {
        waitingForTeleport = false;

        // Ẩn bảng thông báo chuyển màn
        if (GameplayUIManager.Instance != null)
        {
            GameplayUIManager.Instance.ShowProceedScreen(false);
        }

        currentRoomIndex++;
        if (currentRoomIndex < orderedRooms.Count)
        {
            RoomController nextRoom = orderedRooms[currentRoomIndex];

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                PlayerHealth health = FindObjectOfType<PlayerHealth>();
                if (health != null) player = health.gameObject;
            }

            if (player != null)
            {
                Debug.Log($"[Dungeon] Dịch chuyển người chơi sang phòng {currentRoomIndex + 1}/{orderedRooms.Count}: {nextRoom.gameObject.name}");

                // Dịch chuyển người chơi đến tâm phòng tiếp theo (đặt Z = 0f để đồng bộ physics và rendering)
                player.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, 0f);

                // Dịch chuyển Main Camera đến tâm phòng tiếp theo
                if (Camera.main != null)
                {
                    Camera.main.transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -10f);
                }

                // Log tọa độ cụ thể ra Console để theo dõi dễ dàng
                Debug.Log($"[Dungeon - DỊCH CHUYỂN] Tọa độ XYZ cụ thể của Room {currentRoomIndex + 1} ({nextRoom.gameObject.name}): {nextRoom.transform.position}");
                Debug.Log($"[Dungeon - DỊCH CHUYỂN] Tọa độ XYZ cụ thể của Player: {player.transform.position}");

                // Dịch chuyển các Cinemachine Virtual Camera đến tâm phòng tiếp theo để snap camera lập tức
                WarpVirtualCameras(nextRoom.transform.position);

                // Bắt đầu kích hoạt phòng tiếp theo (spawn quái)
                nextRoom.ActivateRoom();
            }
        }
    }

    /// <summary>
    /// Được gọi bởi RoomController khi phòng hiện tại được dọn sạch kẻ địch.
    /// </summary>
    public void OnRoomCleared(RoomController room)
    {
        int roomIndex = orderedRooms.IndexOf(room);
        if (roomIndex != currentRoomIndex) return;

        ClearedEnemyRooms++;
        Debug.Log($"[Dungeon] Tiến độ phòng: {ClearedEnemyRooms}/{TotalEnemyRooms} (Dọn xong phòng {currentRoomIndex + 1}/{orderedRooms.Count})");

        if (currentRoomIndex >= orderedRooms.Count - 1)
        {
            Debug.Log("[Dungeon] Toàn bộ các phòng có quái vật đã dọn sạch! Chúc mừng chiến thắng!");
            if (GameplayUIManager.Instance != null)
            {
                GameplayUIManager.Instance.ShowVictoryScreen();
            }
        }
        else
        {
            // Hiển thị thông báo bấm E để chuẩn bị dịch chuyển sang phòng mới
            waitingForTeleport = true;
            if (GameplayUIManager.Instance != null)
            {
                GameplayUIManager.Instance.ShowProceedScreen(true);
            }
        }
    }
}
