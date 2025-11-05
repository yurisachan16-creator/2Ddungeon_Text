using UnityEngine;

/// <summary>
/// 可生成的物品配置（增强版）
/// 支持稀有度分级、房间深度权重调整、防止重复等高级功能
/// </summary>
[System.Serializable]
public class SpawnableItem
{
    [Header("基础配置")]
    [Tooltip("要生成的预制体")]
    public GameObject prefab;
    
    [Tooltip("基础权重 (1-100)")]
    [Range(1f, 100f)]
    public float baseWeight = 10f;
    
    [Header("高级配置")]
    [Tooltip("稀有度标签：Common/Uncommon/Rare/Epic/Legendary")]
    public RarityType rarity = RarityType.Common;
    
    [Tooltip("是否防止在同一房间内重复生成")]
    public bool preventDuplicatesInRoom = false;
    
    [Tooltip("根据房间深度（stepToStart）调整权重的曲线")]
    [SerializeField] 
    private AnimationCurve depthWeightModifier = AnimationCurve.Constant(0, 10, 1f);
    
    [Header("生成约束")]
    [Tooltip("最小房间深度要求（0表示无限制）")]
    [Range(0, 50)]
    public int minRoomDepth = 0;
    
    [Tooltip("最大房间深度限制（0表示无限制）")]
    [Range(0, 50)]
    public int maxRoomDepth = 0;
    
    /// <summary>
    /// 获取考虑房间深度后的实际权重
    /// </summary>
    public float GetAdjustedWeight(int roomDepth)
    {
        // 检查房间深度约束
        if (minRoomDepth > 0 && roomDepth < minRoomDepth) return 0f;
        if (maxRoomDepth > 0 && roomDepth > maxRoomDepth) return 0f;
        
        // 应用深度权重曲线
        float depthMultiplier = depthWeightModifier.Evaluate(roomDepth);
        return baseWeight * depthMultiplier * GetRarityMultiplier();
    }
    
    /// <summary>
    /// 获取稀有度权重倍率
    /// </summary>
    private float GetRarityMultiplier()
    {
        switch (rarity)
        {
            case RarityType.Common: return 1.0f;
            case RarityType.Uncommon: return 0.6f;
            case RarityType.Rare: return 0.3f;
            case RarityType.Epic: return 0.1f;
            case RarityType.Legendary: return 0.03f;
            default: return 1.0f;
        }
    }
}

/// <summary>
/// 稀有度类型枚举
/// </summary>
public enum RarityType
{
    Common,      // 普通 - 权重 x1.0
    Uncommon,    // 罕见 - 权重 x0.6
    Rare,        // 稀有 - 权重 x0.3
    Epic,        // 史诗 - 权重 x0.1
    Legendary    // 传说 - 权重 x0.03
}
