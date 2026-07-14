// Setup: gắn StatusEffectReceiver.cs vào cả Player và mọi Monster prefab (PlayerMovement/MonsterWander tự tìm qua GetComponent).
// Tạo các asset StatusEffectData (Create → Status Effect → Status Effect Data) cho từng loại:
// Poison (damage theo tick), Burn (tương tự Poison, khác tên/hiệu ứng hình ảnh sau này), Freeze (speedMultiplier = 0),
// SlowDown (speedMultiplier < 1), SpeedUp (speedMultiplier > 1).

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Quản lý danh sách hiệu ứng trạng thái đang áp dụng lên đối tượng này (Player hoặc Monster).
/// Gắn component này vào GameObject nào có IDamageable là dùng được ngay — không cần biết đó là Player hay Monster
/// PlayerMovement/MonsterWanderAI đọc CurrentSpeedMultiplier để biết có đang bị chậm/đóng băng không.
/// </summary>
public class StatusEffectReceiver : MonoBehaviour
{
    private class ActiveEffect
    {
        public StatusEffectData data;
        public float remainingTime;
        public float tickTimer;
    }
    private readonly List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    private IDamageable damageable;
    public float CurrentSpeedMultiplier { get; private set; } = 1f;
    private void Awake()
    {
        damageable = GetComponent<IDamageable>();
    }
    /// <summary>
    /// Áp dụng 1 hiệu ứng mới. Nếu đối tượng đang có hiệu ứng CÙNG LOẠI, làm mới thời gian
    /// thay vì cộng dồn — tránh 1 loại hiệu ứng chồng chất vô hạn khi trúng liên tục.
    /// </summary>
    public void ApplyEffect(StatusEffectData data)
    {
        if (data == null) return;
        ActiveEffect existing = activeEffects.Find(e => e.data.type == data.type);
        if (existing != null)
        {
            existing.remainingTime = data.duration;
            existing.tickTimer = 0f;
        }
        else
        {
            activeEffects.Add(new ActiveEffect { data = data, remainingTime = data.duration });
        }
        RecalculateSpeedMultiplier();
    }
    private void Update()
    {
        bool anyExpired = false;
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffect effect = activeEffects[i];
            effect.remainingTime -= Time.deltaTime;
            if (effect.data.type == StatusEffectType.Poison || effect.data.type == StatusEffectType.Burn)
            {
                effect.tickTimer += Time.deltaTime;
                if (effect.tickTimer >= effect.data.tickInterval)
                {
                    effect.tickTimer = 0f;
                    damageable?.TakeDamage(effect.data.damagePerTick, transform.position);
                }
            }
            if (effect.remainingTime <= 0f)
            {
                activeEffects.RemoveAt(i);
                anyExpired = true;
            }
        }
        if (anyExpired) RecalculateSpeedMultiplier();
    }
    private void RecalculateSpeedMultiplier()
    {
        float multiplier = 1f;
        foreach (ActiveEffect effect in activeEffects)
        {
            if (effect.data.type == StatusEffectType.Freeze)
            {
                multiplier = 0f;
            }
            else if (effect.data.type == StatusEffectType.SlowDown || effect.data.type == StatusEffectType.SpeedUp)
            {
                multiplier *= effect.data.speedMultiplier;
            }
        }
        CurrentSpeedMultiplier = multiplier;
    }
}
