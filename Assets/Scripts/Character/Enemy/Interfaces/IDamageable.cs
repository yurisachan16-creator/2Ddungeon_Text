using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 受到伤害的接口
/// </summary>
public interface IDamageable
{
    // 受到伤害
    void TakeDamage(float damageAmount);

    // 死亡
    void Die();

    float MaxHealth { get; set; }

    float CurrentHealth { get; set; }
}
