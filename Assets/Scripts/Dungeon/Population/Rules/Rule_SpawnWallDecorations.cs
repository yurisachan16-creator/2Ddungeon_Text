using UnityEngine;

/// <summary>
/// 墙壁装饰生成规则（增强版）
/// 支持多个方向同时配置，使用位运算标记
/// 满足需求(1)：在上方墙壁生成火把或旗帜
/// </summary>
[CreateAssetMenu(fileName = "Rule_WallDecorations", menuName = "Dungeon/Population Rule/Wall Decorations", order = 1)]
public class Rule_SpawnWallDecorations : RoomPopulationRule
{
    [System.Flags]
    public enum WallFlags
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        All = Up | Down | Left | Right
    }
    
    [Header("墙壁方向配置")]
    [Tooltip("选择在哪些方向的墙壁上生成装饰")]
    public WallFlags targetWalls = WallFlags.Up;
    
    [Header("生成配置")]
    [Tooltip("装饰物生成表（火把、旗帜等）")]
    public SpawnTable spawnTable;
    
    [Tooltip("每面墙最少生成数量")]
    public int minAmount = 0;
    
    [Tooltip("每面墙最多生成数量")]
    public int maxAmount = 2;
    
    [Header("间距控制")]
    [Tooltip("相同墙壁上装饰物之间的最小间距")]
    public float minSpacing = 2f;
    
    protected override void Apply(Room room, bool isStartRoom, bool isBossRoom)
    {
        if (spawnTable == null)
        {
            Debug.LogWarning($"Rule [{name}]: SpawnTable 未设置！");
            return;
        }
        
        // 重置生成表的房间缓存
        spawnTable.ResetRoomCache();
        
        // 处理每个方向的墙壁
        if ((targetWalls & WallFlags.Up) != 0)
            SpawnOnWall(room, WallDirection.Up);
        
        if ((targetWalls & WallFlags.Down) != 0)
            SpawnOnWall(room, WallDirection.Down);
        
        if ((targetWalls & WallFlags.Left) != 0)
            SpawnOnWall(room, WallDirection.Left);
        
        if ((targetWalls & WallFlags.Right) != 0)
            SpawnOnWall(room, WallDirection.Right);
    }
    
    private void SpawnOnWall(Room room, WallDirection direction)
    {
        // 检查该方向是否有墙
        if (!room.HasWall(direction)) return;
        
        int count = Random.Range(minAmount, maxAmount + 1);
        if (count == 0) return;
        
        // 用于跟踪已生成位置（简单间距检查）
        System.Collections.Generic.List<Vector3> spawnedPositions = new System.Collections.Generic.List<Vector3>();
        
        for (int i = 0; i < count; i++)
        {
            Vector3? pos = room.GetRandomWallPosition(direction);
            
            if (!pos.HasValue) continue;
            
            Vector3 spawnPos = pos.Value;
            
            // 检查与已生成位置的间距
            bool tooClose = false;
            foreach (Vector3 existingPos in spawnedPositions)
            {
                if (Vector3.Distance(spawnPos, existingPos) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }
            
            if (tooClose) continue;
            
            GameObject prefab = spawnTable.GetRandomPrefab(room.stepToStart);
            
            if (prefab != null)
            {
                GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity, room.transform);
                spawned.name = $"{prefab.name}_Wall{direction}_{i}";
                spawnedPositions.Add(spawnPos);
            }
        }
    }
}
