using UnityEngine;

/// <summary>
/// 地板道具生成规则
/// 满足需求(3)：随机在地板生成火炬、盒子、宝箱
/// 包含碰撞检测，避免道具重叠
/// </summary>
[CreateAssetMenu(fileName = "Rule_FloorProps", menuName = "Dungeon/Population Rule/Floor Props", order = 2)]
public class Rule_SpawnFloorProps : RoomPopulationRule
{
    [Header("生成配置")]
    [Tooltip("道具生成表（盒子、宝箱、火炬等）")]
    public SpawnTable spawnTable;
    
    [Tooltip("最少生成数量")]
    public int minAmount = 1;
    
    [Tooltip("最多生成数量")]
    public int maxAmount = 3;
    
    [Header("碰撞检测")]
    [Tooltip("道具碰撞检测半径（避免重叠）")]
    public float collisionCheckRadius = 0.5f;
    
    [Tooltip("生成失败时的重试次数")]
    public int maxRetries = 10;
    
    protected override void Apply(Room room, bool isStartRoom, bool isBossRoom)
    {
        if (spawnTable == null)
        {
            Debug.LogWarning($"Rule [{name}]: SpawnTable 未设置！");
            return;
        }
        
        // 重置生成表的房间缓存
        spawnTable.ResetRoomCache();
        
        int count = Random.Range(minAmount, maxAmount + 1);
        int successCount = 0;
        
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = room.GetValidFloorPosition(collisionCheckRadius);
            
            // 检查是否找到有效位置
            if (spawnPos == Vector3.zero)
            {
                Debug.LogWarning($"Rule [{name}]: 房间 [{room.gameObject.name}] 找不到有效地板位置，跳过生成");
                continue;
            }
            
            GameObject prefab = spawnTable.GetRandomPrefab(room.stepToStart);
            
            if (prefab != null)
            {
                GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity, room.transform);
                spawned.name = $"{prefab.name}_Floor_{successCount}";
                successCount++;
            }
        }
    }
}
