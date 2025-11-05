using UnityEngine;

/// <summary>
/// 敌人生成规则
/// 满足需求(5)：在生成的房间生成怪物
/// 支持根据房间深度调整敌人数量和类型
/// </summary>
[CreateAssetMenu(fileName = "Rule_Enemies", menuName = "Dungeon/Population Rule/Spawn Enemies", order = 3)]
public class Rule_SpawnEnemies : RoomPopulationRule
{
    [Header("生成配置")]
    [Tooltip("敌人生成表")]
    public SpawnTable enemyTable;
    
    [Tooltip("最少敌人数量")]
    public int minAmount = 1;
    
    [Tooltip("最多敌人数量")]
    public int maxAmount = 3;
    
    [Header("深度影响配置")]
    [Tooltip("是否根据房间深度增加敌人数量")]
    public bool scaleWithDepth = true;
    
    [Tooltip("每增加此深度，额外增加1个敌人")]
    public int depthPerExtraEnemy = 5;
    
    [Header("碰撞检测")]
    [Tooltip("敌人生成碰撞检测半径")]
    public float collisionCheckRadius = 0.8f;
    
    [Header("生成约束")]
    [Tooltip("敌人之间的最小间距")]
    public float minEnemySpacing = 2f;
    
    protected override void Apply(Room room, bool isStartRoom, bool isBossRoom)
    {
        if (enemyTable == null)
        {
            Debug.LogWarning($"Rule [{name}]: EnemyTable 未设置！");
            return;
        }
        
        // 重置生成表的房间缓存
        enemyTable.ResetRoomCache();
        
        // 计算敌人数量
        int baseCount = Random.Range(minAmount, maxAmount + 1);
        int bonusCount = 0;
        
        if (scaleWithDepth && depthPerExtraEnemy > 0)
        {
            bonusCount = room.stepToStart / depthPerExtraEnemy;
        }
        
        int totalCount = baseCount + bonusCount;
        
        // 用于跟踪已生成位置
        System.Collections.Generic.List<Vector3> spawnedPositions = new System.Collections.Generic.List<Vector3>();
        
        for (int i = 0; i < totalCount; i++)
        {
            // 尝试多次找到合适的位置
            bool spawned = false;
            
            for (int retry = 0; retry < 15; retry++)
            {
                Vector3 spawnPos = room.GetValidFloorPosition(collisionCheckRadius);
                
                if (spawnPos == Vector3.zero) continue;
                
                // 检查与其他敌人的间距
                bool tooClose = false;
                foreach (Vector3 existingPos in spawnedPositions)
                {
                    if (Vector3.Distance(spawnPos, existingPos) < minEnemySpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }
                
                if (tooClose) continue;
                
                // 从生成表获取敌人预制体
                GameObject enemyPrefab = enemyTable.GetRandomPrefab(room.stepToStart);
                
                if (enemyPrefab != null)
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, room.transform);
                    enemy.name = $"{enemyPrefab.name}_Enemy_{i}";
                    spawnedPositions.Add(spawnPos);
                    spawned = true;
                    break;
                }
            }
            
            if (!spawned)
            {
                Debug.LogWarning($"Rule [{name}]: 房间 [{room.gameObject.name}] 无法生成第 {i+1} 个敌人（位置冲突或重试耗尽）");
            }
        }
    }
}
