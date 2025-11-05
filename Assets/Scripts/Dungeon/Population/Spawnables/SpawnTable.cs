using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 生成表 ScriptableObject（增强版）
/// 使用权重算法从物品列表中随机选择，支持房间深度、稀有度、防重复等高级功能
/// </summary>
[CreateAssetMenu(fileName = "NewSpawnTable", menuName = "Dungeon/Spawn Table", order = 1)]
public class SpawnTable : ScriptableObject
{
    [Header("生成物品列表")]
    [Tooltip("所有可生成的物品及其配置")]
    public SpawnableItem[] items;
    
    [Header("调试信息")]
    [Tooltip("在控制台显示权重计算详情")]
    public bool showDebugInfo = false;
    
    // 运行时缓存：存储当前房间已生成的物品（防重复）
    private HashSet<GameObject> spawnedInCurrentRoom = new HashSet<GameObject>();
    
    /// <summary>
    /// 重置房间缓存（每次开始填充新房间时调用）
    /// </summary>
    public void ResetRoomCache()
    {
        spawnedInCurrentRoom.Clear();
    }
    
    /// <summary>
    /// 按权重随机获取一个预制体
    /// </summary>
    /// <param name="roomDepth">当前房间的深度（stepToStart）</param>
    /// <returns>随机选择的预制体，如果没有合适的物品则返回 null</returns>
    public GameObject GetRandomPrefab(int roomDepth = 0)
    {
        if (items == null || items.Length == 0)
        {
            if (showDebugInfo) Debug.LogWarning($"SpawnTable [{name}] 没有配置任何物品！");
            return null;
        }
        
        // 构建权重列表
        List<SpawnableItem> validItems = new List<SpawnableItem>();
        List<float> weights = new List<float>();
        float totalWeight = 0f;
        
        foreach (var item in items)
        {
            if (item.prefab == null) continue;
            
            // 检查防重复设置
            if (item.preventDuplicatesInRoom && spawnedInCurrentRoom.Contains(item.prefab))
            {
                if (showDebugInfo) 
                    Debug.Log($"跳过 [{item.prefab.name}] - 已在当前房间生成过");
                continue;
            }
            
            // 计算调整后的权重
            float adjustedWeight = item.GetAdjustedWeight(roomDepth);
            
            if (adjustedWeight > 0f)
            {
                validItems.Add(item);
                weights.Add(adjustedWeight);
                totalWeight += adjustedWeight;
                
                if (showDebugInfo)
                    Debug.Log($"物品 [{item.prefab.name}] 权重={adjustedWeight:F2} (基础={item.baseWeight}, 稀有度={item.rarity}, 深度={roomDepth})");
            }
        }
        
        // 没有合适的物品
        if (validItems.Count == 0 || totalWeight <= 0f)
        {
            if (showDebugInfo) 
                Debug.LogWarning($"SpawnTable [{name}] 在深度 {roomDepth} 没有可用物品！");
            return null;
        }
        
        // 加权随机选择
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        for (int i = 0; i < validItems.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
            {
                SpawnableItem selectedItem = validItems[i];
                
                // 如果设置了防重复，将其加入缓存
                if (selectedItem.preventDuplicatesInRoom)
                {
                    spawnedInCurrentRoom.Add(selectedItem.prefab);
                }
                
                if (showDebugInfo)
                    Debug.Log($"✔ 选中物品: [{selectedItem.prefab.name}] (概率: {(weights[i]/totalWeight*100f):F1}%)");
                
                return selectedItem.prefab;
            }
        }
        
        // 理论上不应该到达这里，但作为安全措施返回第一个
        return validItems[0].prefab;
    }
    
    /// <summary>
    /// 获取多个不重复的预制体
    /// </summary>
    /// <param name="count">需要的数量</param>
    /// <param name="roomDepth">房间深度</param>
    /// <returns>预制体列表</returns>
    public List<GameObject> GetRandomPrefabs(int count, int roomDepth = 0)
    {
        List<GameObject> results = new List<GameObject>();
        
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = GetRandomPrefab(roomDepth);
            if (prefab != null)
            {
                results.Add(prefab);
            }
        }
        
        return results;
    }
}
