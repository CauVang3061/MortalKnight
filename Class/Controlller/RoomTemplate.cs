// Cách setup và test trong Unity (từng bước):
// 1. Chuẩn bị prefab phòng: Tạo vài GameObject phòng, mỗi cái có 1 Tilemap con vẽ sẵn (tường + sàn),
// kích thước giống nhau và trùng khớp roomSpacing (ví dụ phòng 20×20 unit thì roomSpacing = 20)
// 2. Gắn script RoomTemplate vào GameObject gốc của mỗi phòng, tick chọn hướng cửa đúng với thiết kế tilemap
// (ví dụ phòng có lối đi trên + phải thì tick Up và Right)
// 3. Kéo các phòng đó thành Prefab (kéo vào folder Assets)
// 4. Tạo 1 GameObject rỗng tên DungeonManager, gắn script DungeonGenerator
// 5. Trong Inspector: kéo 1 prefab làm Start Room Prefab, kéo các prefab còn lại vào mảng Room Prefabs,
// chỉnh Total Room Count (ví dụ 10) và Room Spacing
// 6. Nhấn Play - dungeon sẽ tự sinh ra từ phòng bắt đầu, mở rộng dần theo cửa

using UnityEngine;
/// <summary>
/// Đại diện cho các hướng cửa của một phòng. Dùng [Flags] để một phòng
/// có thể có nhiều cửa cùng lúc bằng cách kết hợp bit
/// Ví dụ: DoorDirection.Up | DoorDirection.Left = phòng có cửa trên và cửa trái.
/// </summary>
[System.Flags]
public enum DoorDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
}
/// <summary>
/// Gắn script này vào root của mỗi prefab phòng.
/// Khai báo phòng này có cửa ở những hướng nào trong Inspector
/// </summary>
public class RoomTemplate : MonoBehaviour
{
    [Tooltip("Phòng này có cửa ở những hướng nào — chọn nhiều hướng cùng lúc trong Inspector")]
    public DoorDirection doors;
    /// <summary>
    /// Trả về hướng đối diện — dùng để kiểm tra khớp cửa giữa 2 phòng liền kề.
    /// Ví dụ: phòng A có cửa Right, phòng B đứng bên phải A bắt buộc phải có cửa Left.
    /// </summary>
    public static DoorDirection GetOpposite(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.Up: return DoorDirection.Down;
            case DoorDirection.Down: return DoorDirection.Up;
            case DoorDirection.Left: return DoorDirection.Right;
            case DoorDirection.Right: return DoorDirection.Left;
            default: return DoorDirection.None;
        }
    }
    /// <summary>
    /// Chuyển 1 hướng cửa thành độ lệch tọa độ lưới (grid offset).
    /// Ví dụ: Up -> (0, 1) nghĩa là ô phía trên trong lưới.
    /// </summary>
    public static Vector2Int GetGridOffset(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.Up: return new Vector2Int(0, 1);
            case DoorDirection.Down: return new Vector2Int(0, -1);
            case DoorDirection.Left: return new Vector2Int(-1, 0);
            case DoorDirection.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }
}
