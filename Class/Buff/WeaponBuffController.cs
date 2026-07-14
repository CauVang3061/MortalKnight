using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Quản lý danh sách buff đang áp dụng lên 1 vũ khí (gắn cùng GameObject với
/// WeaponController hoặc MeleeWeaponController). Mỗi lần cần dùng (base value + tổng các buff đang active) sẽ tính lại từ đầu
/// </summary>
public class WeaponBuffController : MonoBehaviour
{
    private class ActiveBuff
    {
        public WeaponBuff data;
        public float remainingTime;
    }
    private readonly List<ActiveBuff> activeBuffs = new List<ActiveBuff>();
    public void AddBuff(WeaponBuff data)
    {
        if (data == null) return;
        activeBuffs.Add(new ActiveBuff { data = data, remainingTime = data.duration });
    }
    private void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (activeBuffs[i].data.isPermanent) continue;
            activeBuffs[i].remainingTime -= Time.deltaTime;
            if (activeBuffs[i].remainingTime <= 0f)
            {
                activeBuffs.RemoveAt(i);
            }
        }
    }
    public float GetAttackBonus() => Sum(b => b.attackBonus);
    public float GetCooldownReduction() => Sum(b => b.cooldownReduction);
    public float GetRangeBonus() => Sum(b => b.rangeBonus);
    public float GetSpreadReduction() => Sum(b => b.spreadReduction);
    private float Sum(Func<WeaponBuffData, float> selector)
    {
        float total = 0f;
        foreach (ActiveBuff buff in activeBuffs) total += selector(buff.data);
        return total;
    }
}
