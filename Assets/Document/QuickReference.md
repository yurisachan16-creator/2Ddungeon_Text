# 地牢填充系统 - 快速参考卡

## 📋 核心类速查

| 类名 | 路径 | 职责 |
|------|------|------|
| `DungeonPopulationManager` | `Dungeon/Population/` | 协调所有规则，管理填充流程 |
| `SpawnTable` | `Population/Spawnables/` | 权重随机生成表 |
| `RoomPopulationRule` | `Population/Rules/` | 规则抽象基类 |
| `BossRoomController` | `Population/` | Boss战斗和出口激活 |
| `Room` (扩展) | `Dungeon/Generation/` | 提供生成位置辅助方法 |

## 🎮 快速配置流程

```
1. 创建预制体 → 2. 创建SpawnTable → 3. 创建Rule → 4. 配置Manager → 5. 运行测试
```

## 📊 权重计算公式

```csharp
最终权重 = baseWeight × rarityMultiplier × depthCurve(roomDepth)

稀有度倍率:
  Common:    1.0
  Uncommon:  0.6
  Rare:      0.3
  Epic:      0.1
  Legendary: 0.03
```

## 🔧 Room 辅助方法

```csharp
// 获取随机地板位置（无碰撞检测）
Vector3 pos = room.GetRandomFloorPosition();

// 获取有效地板位置（带碰撞检测）
Vector3 validPos = room.GetValidFloorPosition(radius: 0.5f);

// 获取墙壁装饰位置
Vector3? wallPos = room.GetRandomWallPosition(WallDirection.Up);
if (wallPos.HasValue) { /* 生成装饰 */ }

// 检查某方向是否有墙
bool hasWall = room.HasWall(WallDirection.Left);
```

## 📝 Rule 创建模板

```csharp
[CreateAssetMenu(fileName = "Rule_YourRule", 
                 menuName = "Dungeon/Population Rule/Your Rule")]
public class Rule_YourRule : RoomPopulationRule
{
    [Header("配置")]
    public SpawnTable spawnTable;
    public int minAmount = 1;
    public int maxAmount = 3;
    
    protected override void Apply(Room room, bool isStartRoom, bool isBossRoom)
    {
        spawnTable.ResetRoomCache(); // 重要！
        
        int count = Random.Range(minAmount, maxAmount + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = room.GetValidFloorPosition();
            if (pos == Vector3.zero) continue; // 失败检查
            
            GameObject prefab = spawnTable.GetRandomPrefab(room.stepToStart);
            if (prefab != null)
            {
                Instantiate(prefab, pos, Quaternion.identity, room.transform);
            }
        }
    }
}
```

## 🎨 Sorting Layer 推荐设置

```
0. Default
1. Floor          ← 基础地板
2. FloorDecals    ← 地板贴花（覆盖地板）
3. Props          ← 道具、敌人
4. Walls          ← 墙壁
5. Effects        ← 特效
```

## ⚙️ Inspector 常用配置

### SpawnTable
```
Items:
  [0] Prefab: Torch,    Weight: 60,  Rarity: Common
  [1] Prefab: Banner,   Weight: 40,  Rarity: Uncommon
Show Debug Info: ✓ (调试时开启)
```

### Rule_SpawnWallDecorations
```
Target Walls: ✓ Up  ✓ Down  ☐ Left  ☐ Right
Spawn Table: T1_Wall_Decorations
Min/Max Amount: 1-3
Min Spacing: 2.0
Apply To Normal Rooms: ✓
```

### DungeonPopulationManager
```
Normal Room Rules:
  - Rule_TopWall_Torches
  - Rule_Floor_Treasures
  - Rule_Normal_Enemies
  
Boss Exit Prefab: Ladder_Prefab
Show Debug Logs: ✓
```

## 🐛 调试检查清单

- [ ] SpawnTable 配置了预制体？
- [ ] SpawnTable 的 items 数组不为空？
- [ ] Rule 的 `isEnabled = true`？
- [ ] Rule 的房间类型过滤正确？
- [ ] Room 的 `obstacleLayer` 包含了所有障碍物？
- [ ] 预制体有必要的组件（SpriteRenderer 等）？
- [ ] Sorting Layer 名称正确且存在？
- [ ] Boss房间有 BossRoomController 组件？

## 📐 推荐配置值

| 参数 | 推荐值 | 说明 |
|------|--------|------|
| Floor Area Size | (14, 8) | 地板生成区域 |
| Wall Deco Y Offset | 4.5 | 上下墙装饰偏移 |
| Collision Radius | 0.5 | 道具碰撞检测半径 |
| Min Enemy Spacing | 2.0 | 敌人最小间距 |
| Max Retry Attempts | 10 | 生成失败重试次数 |

## 🚀 性能优化提示

```csharp
// ❌ 不好：每次都新建列表
for (int i = 0; i < 100; i++) {
    List<GameObject> temp = new List<GameObject>();
}

// ✅ 好：复用列表
List<GameObject> reusableList = new List<GameObject>();
for (int i = 0; i < 100; i++) {
    reusableList.Clear();
    // 使用 reusableList...
}
```

## 📞 常用调试命令

```csharp
// 在 SpawnTable 中启用详细日志
spawnTable.showDebugInfo = true;

// 在 DungeonPopulationManager 中启用日志
populationManager.showDebugLogs = true;

// 手动触发重新生成
if (Input.GetKeyDown(KeyCode.R)) {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
```

## 🎯 五大需求对应

| 需求 | 对应规则 | 配置要点 |
|------|----------|----------|
| (1) 墙壁火把/旗帜 | `Rule_SpawnWallDecorations` | Target Walls = Up |
| (2) 地板纹路覆盖 | `Rule_SpawnFloorDecals` | Sorting Layer 配置 |
| (3) 地板宝箱/盒子 | `Rule_SpawnFloorProps` | Collision Check |
| (4) Boss梯子 | `DungeonPopulationManager` | bossExitPrefab |
| (5) 敌人生成 | `Rule_SpawnEnemies` | Scale With Depth |

---
**版本**: 1.0 | **日期**: 2025-11-05
