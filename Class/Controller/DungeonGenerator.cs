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
    [Tooltip("Phòng bắt đầu — luôn được đặt tại ô (0,0)")]
    [SerializeField] private RoomTemplate startRoomPrefab;
    [Tooltip("Danh sách các phòng thường, được chọn ngẫu nhiên khi mở rộng dungeon")]
    [SerializeField] private RoomTemplate[] roomPrefabs;
    [Tooltip("Tổng số phòng muốn sinh ra, kể cả phòng bắt đầu")]
    [SerializeField] private int totalRoomCount = 10;
    [Tooltip("Khoảng cách giữa tâm 2 phòng liền kề, tính bằng world unit. Phải khớp với kích thước phòng thiết kế trong Tilemap")]
    [SerializeField] private float roomSpacing = 20f;
    // Lưu phòng đã thực sự được đặt: tọa độ lưới -> RoomTemplate đã instantiate
    private readonly Dictionary<Vector2Int, RoomTemplate> placedRooms = new Dictionary<Vector2Int, RoomTemplate>();
    // Các ô đã được "đặt chỗ trước" (đang chờ trong hàng đợi, chưa thực sự sinh phòng)
    private readonly HashSet<Vector2Int> reservedCells = new HashSet<Vector2Int>();
    private void Start()
    {
        GenerateDungeon();
    }
    private void GenerateDungeon()
    {
        // Hàng đợi các ô cần xử lý, kèm theo hướng cửa bắt buộc phải có
        // (để nối ngược lại với phòng cha đã sinh ra nó).
        var frontier = new Queue<(Vector2Int cell, DoorDirection requiredDoor)>();
        // Bước 1: đặt phòng bắt đầu tại (0,0), không yêu cầu cửa bắt buộc nào.
        PlaceRoom(Vector2Int.zero, startRoomPrefab, frontier);
        // Bước 2: mở rộng dần cho đến khi đủ số phòng hoặc hết hàng đợi.
        while (frontier.Count > 0 && placedRooms.Count < totalRoomCount)
        {
            var (cell, requiredDoor) = frontier.Dequeue();
            // Ô này có thể đã bị phòng khác chiếm mất trong lúc chờ hàng đợi — bỏ qua.
            if (placedRooms.ContainsKey(cell)) continue;
            RoomTemplate chosenPrefab = PickCompatibleRoom(cell, requiredDoor);
            if (chosenPrefab == null) continue; // không tìm được phòng phù hợp, bỏ qua nhánh này
            PlaceRoom(cell, chosenPrefab, frontier);
        }
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
    private void PlaceRoom(Vector2Int cell, RoomTemplate prefab, Queue<(Vector2Int, DoorDirection)> frontier)
    {
        Vector3 worldPos = new Vector3(cell.x * roomSpacing, cell.y * roomSpacing, 0f);
        RoomTemplate instance = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        placedRooms[cell] = instance;
        reservedCells.Remove(cell);
        foreach (DoorDirection dir in new[] { DoorDirection.Up, DoorDirection.Down, DoorDirection.Left, DoorDirection.Right })
        {
            if ((instance.doors & dir) == 0) continue; // phòng này không có cửa hướng đó
            Vector2Int neighborCell = cell + RoomTemplate.GetGridOffset(dir);
            // Bỏ qua nếu ô đó đã có phòng, hoặc đã được đặt chỗ trước bởi nhánh khác.
            if (placedRooms.ContainsKey(neighborCell) || reservedCells.Contains(neighborCell)) continue;
            reservedCells.Add(neighborCell);
            frontier.Enqueue((neighborCell, RoomTemplate.GetOpposite(dir)));
        }
    }
}
