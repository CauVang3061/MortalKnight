// Setup: GameObject rương có SpriteRenderer + Collider2D, gắn TreasureBox.cs, kéo sprite rương đóng/mở,
// kéo các prefab phần thưởng (khi có) vào mảng Reward Prefabs. Đứng gần rương, bấm E
using UnityEngine;
/// <summary>
/// Rương báu - kéo-thả 1 mảng prefab phần thưởng trong Inspector — thêm phần thưởng mới
/// </summary>
public class TreasureBox : Interaction
{
    [SerializeField] private SpriteRenderer boxRenderer;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private GameObject[] rewardPrefabs;
    private bool isUsed;
    protected override void Interact()
    {
        if (isUsed) return;
        isUsed = true;
        if (boxRenderer != null && openSprite != null)
        {
            boxRenderer.sprite = openSprite;
        }
        if (rewardPrefabs != null && rewardPrefabs.Length > 0)
        {
            GameObject reward = rewardPrefabs[Random.Range(0, rewardPrefabs.Length)];
            Instantiate(reward, transform.position, Quaternion.identity);
        }
    }
}
