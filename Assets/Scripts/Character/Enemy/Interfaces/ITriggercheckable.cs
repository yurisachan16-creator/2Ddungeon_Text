// ITriggercheckable.cs
using UnityEngine;

public interface ITriggercheckable 
{
    bool IsAggroed { get; set; }    //激怒状态
    bool IsWithinStrikingDistance { get; set; } // 远程攻击
    bool IsWithinMeleeDistance { get; set; }    // 近战攻击
    bool IsWithinEscapeDistance { get; set; }   // 远程逃跑距离

    void SetAggroStatus(bool isAggroed);
    void SetStrikingDistanceBool(bool isWithinStrikingDistance);
    void SetMeleeDistanceBool(bool isWithinMeleeDistance);
    void SetEscapeDistanceBool(bool isWithinEscapeDistance);
}