# 地牢填充系统 - 使用指南

## 📚 系统概述

此地牢填充系统基于**策略模式 + ScriptableObject**，实现了高度解耦、易维护的程序化内容生成。

## 🎯 满足的需求

- ✅ **(1) 墙壁装饰**: 随机在上方墙壁生成火把或旗帜
- ✅ **(2) 地板贴花**: 在地板上随机生成新的纹路，视觉覆盖原先的层
- ✅ **(3) 地板道具**: 随机在地板生成火炬、盒子、宝箱
- ✅ **(4) Boss出口**: 在最后的Boss关卡生成梯子
- ✅ **(5) 敌人生成**: 在生成的房间生成怪物

## 📁 项目结构

```
Assets/
├── Scripts/
│   ├── Dungeon/
│   │   ├── Generation/          # 地牢形状生成
│   │   │   ├── RoomGenerator.cs
│   │   │   └── Room.cs (已扩展)
│   │   │
│   │   └── Population/          # 地牢内容填充
│   │       ├── DungeonPopulationManager.cs  # 核心管理器
│   │       ├── BossRoomController.cs        # Boss战斗控制
│   │       │
│   │       ├── Spawnables/
│   │       │   ├── SpawnableItem.cs  # 物品配置
│   │       │   └── SpawnTable.cs     # 生成表（权重系统）
│   │       │
│   │       └── Rules/
│   │           ├── RoomPopulationRule.cs        # 规则基类
│   │           ├── Rule_SpawnWallDecorations.cs # 墙壁装饰
│   │           ├── Rule_SpawnFloorProps.cs      # 地板道具
│   │           ├── Rule_SpawnEnemies.cs         # 敌人生成
│   │           └── Rule_SpawnFloorDecals.cs     # 地板贴花
│   │
│   ├── Core/
│   │   └── StateMachine/
│   ├── Character/
│   └── Gameplay/
│
├── Editor/
│   └── RoomEditor.cs  # Scene视图可视化工具
│
└── ScriptableObjects/
    └── Dungeon/
        ├── SpawnTables/        # 创建生成表资产
        └── PopulationRules/    # 创建规则资产
```

## 🚀 快速开始（5步配置）

### 第1步：准备预制体

创建以下预制体文件夹：
- `Assets/Prefabs/Props/` - 放置盒子、宝箱、火炬等
- `Assets/Prefabs/Decorations/` - 放置火把、旗帜等
- `Assets/Prefabs/Decals/` - 放置地板纹路贴花
- `Assets/Prefabs/Enemies/` - 放置敌人预制体
- `Assets/Prefabs/Exits/` - 放置梯子/出口预制体

### 第2步：创建生成表（SpawnTable）

右键菜单 `Create > Dungeon > Spawn Table`

**示例1: 墙壁装饰生成表**
```
名称: T1_Wall_Decorations
配置:
  - 火把 Prefab, 权重=60, 稀有度=Common
  - 旗帜 Prefab, 权重=40, 稀有度=Common
```

**示例2: 地板道具生成表**
```
名称: T1_Floor_Props
配置:
  - 盒子 Prefab, 权重=50, 稀有度=Common
  - 宝箱 Prefab, 权重=10, 稀有度=Rare, 最小深度=3
  - 火炬 Prefab, 权重=30, 稀有度=Common
```

**示例3: 敌人生成表**
```
名称: T1_Enemies
配置:
  - Slime Prefab, 权重=60, 稀有度=Common
  - Skeleton Prefab, 权重=30, 稀有度=Uncommon, 最小深度=2
  - RangedEnemy Prefab, 权重=10, 稀有度=Rare, 最小深度=5
```

### 第3步：创建规则（Population Rules）

#### 规则1: 墙壁火把（满足需求1）
```
右键菜单: Create > Dungeon > Population Rule > Wall Decorations
名称: Rule_TopWall_Torches

配置:
  - Target Walls: ✓ Up (勾选上墙)
  - Spawn Table: 拖入 T1_Wall_Decorations
  - Min Amount: 1
  - Max Amount: 3
  - Apply To Normal Rooms: ✓
```

#### 规则2: 地板宝箱（满足需求3）
```
右键菜单: Create > Dungeon > Population Rule > Floor Props
名称: Rule_Floor_Treasures

配置:
  - Spawn Table: 拖入 T1_Floor_Props
  - Min Amount: 1
  - Max Amount: 2
  - Collision Check Radius: 0.5
  - Apply To Normal Rooms: ✓
```

#### 规则3: 敌人生成（满足需求5）
```
右键菜单: Create > Dungeon > Population Rule > Spawn Enemies
名称: Rule_Normal_Enemies

配置:
  - Enemy Table: 拖入 T1_Enemies
  - Min Amount: 1
  - Max Amount: 3
  - Scale With Depth: ✓
  - Depth Per Extra Enemy: 5
  - Apply To Normal Rooms: ✓
```

#### 规则4: 地板贴花（满足需求2）
```
右键菜单: Create > Dungeon > Population Rule > Floor Decals
名称: Rule_Floor_Patterns

配置:
  - Decal Table: 拖入贴花生成表
  - Min Amount: 2
  - Max Amount: 6
  - Sorting Layer Name: "Floor"
  - Sorting Order: 1
  - Random Rotation: ✓
```

### 第4步：场景配置

1. **创建 DungeonPopulationManager 对象**
   - 在场景中创建空物体，命名为 `DungeonPopulationManager`
   - 添加 `DungeonPopulationManager` 组件
   
2. **配置规则列表**
   ```
   Start Room Rules: (通常留空，不刷怪)
   
   Normal Room Rules:
     - Rule_TopWall_Torches
     - Rule_Floor_Treasures
     - Rule_Normal_Enemies
     - Rule_Floor_Patterns
   
   Boss Room Rules:
     - (可以配置更强的敌人或不配置)
   ```

3. **配置Boss出口**
   ```
   Boss Exit Prefab: 拖入梯子预制体
   Exit Spawn Offset: (0, 0, 0)
   ```

4. **连接到 RoomGenerator**
   - 选中场景中的 `RoomGenerator` 对象
   - 在 Inspector 找到 `Population Manager` 字段
   - 拖入刚创建的 `DungeonPopulationManager` 对象

### 第5步：配置 Room 预制体

打开 `Assets/Prefabs/BasicRoom.prefab`：

1. **添加生成区域配置**
   ```
   Floor Area Size: (14, 8)  - 地板可生成区域
   Wall Deco Area Size: (16, 1.5)
   Wall Deco Y Offset: 4.5
   Obstacle Layer: 选择障碍物图层（Room层）
   ```

2. **设置 Sorting Layer**
   - 打开 `Project Settings > Tags and Layers > Sorting Layers`
   - 添加层级（从底到顶）:
     ```
     Default
     Floor        (基础地板)
     FloorDecals  (地板贴花)
     Props        (道具)
     Walls        (墙壁)
     ```

## 🎨 可视化调试工具

选中场景中任意 Room 对象，在 Scene 视图中会自动显示：
- 🟩 **绿色区域**: 地板生成区域
- 🟧 **橙色区域**: 墙壁装饰区域
- 🔵 **蓝色文本**: 房间深度和门数量

## 🔧 高级配置

### 权重系统详解

**基础权重计算**:
```
最终权重 = 基础权重 × 稀有度倍率 × 深度曲线系数
```

**稀有度倍率**:
- Common (普通): x1.0
- Uncommon (罕见): x0.6
- Rare (稀有): x0.3
- Epic (史诗): x0.1
- Legendary (传说): x0.03

**深度曲线**:
使用 AnimationCurve 可以定制物品在不同房间深度的出现概率：
```csharp
// 示例：宝箱在深房间更常见
depthWeightModifier:
  - (0, 0.5)  // 起始房间权重减半
  - (5, 1.0)  // 中期房间正常
  - (10, 2.0) // 深处房间权重翻倍
```

### 碰撞检测配置

在 Room 组件中：
```
Obstacle Layer: 设置为包含以下内容的图层
  - Room (房间本身)
  - Wall (墙壁)
  - Props (已生成的道具)
```

### Boss战斗流程

**方案1: 使用事件系统（推荐）**
```csharp
// 在 CharacterBase.cs 中添加死亡事件
public event System.Action OnDeath;

// 死亡时调用
OnDeath?.Invoke();

// BossRoomController 会自动订阅此事件
```

**方案2: 使用轮询检测（当前实现）**
```csharp
// BossRoomController 会每0.5秒检测Boss状态
// 当 bossEnemy == null 或 bossEnemy.isDead 时触发胜利
```

## 📝 扩展指南

### 添加新的道具类型

1. 创建预制体并放入对应文件夹
2. 添加到对应的 SpawnTable 中
3. 设置权重和稀有度
4. 无需修改任何代码！

### 创建自定义规则

```csharp
[CreateAssetMenu(fileName = "Rule_Custom", menuName = "Dungeon/Population Rule/Custom")]
public class Rule_Custom : RoomPopulationRule
{
    protected override void Apply(Room room, bool isStartRoom, bool isBossRoom)
    {
        // 你的自定义生成逻辑
        Vector3 pos = room.GetValidFloorPosition();
        if (pos != Vector3.zero)
        {
            // 生成物体...
        }
    }
}
```

### 根据房间类型差异化配置

```csharp
// 在规则的 Inspector 中：
Apply To Start Room: false   // 不应用于起始房间
Apply To Boss Room: false    // 不应用于Boss房间
Apply To Normal Rooms: true  // 只应用于普通房间
```

## ⚠️ 注意事项

1. **Layer 配置**: 确保 `obstacleLayer` 包含所有障碍物
2. **Sorting Layer**: 贴花必须设置正确的 Sorting Layer 才能覆盖地板
3. **预制体检查**: 所有预制体必须包含必要的组件（如 SpriteRenderer）
4. **碰撞半径**: 根据道具大小调整 `collisionCheckRadius`
5. **Boss组件**: Boss房间必须有 `BossRoomController` 组件（会自动添加）

## 🐛 常见问题

**Q: 道具生成位置重叠？**
A: 增大 `collisionCheckRadius` 或检查 `obstacleLayer` 是否正确配置

**Q: 墙壁装饰没有生成？**
A: 检查房间该方向是否真的有墙（即该方向没有相邻房间）

**Q: 权重不生效？**
A: 确保 SpawnTable 的 `showDebugInfo = true`，查看控制台日志

**Q: Boss梯子无法激活？**
A: 检查 Boss 预制体是否继承自 `CharacterBase`，且实现了死亡检测

**Q: Scene视图看不到可视化区域？**
A: 确保选中了 Room 对象，并检查 Editor 脚本是否正确放置

## 🎮 测试流程

1. 运行游戏
2. 观察控制台日志（如果启用 `showDebugLogs`）
3. 检查各个房间的生成内容
4. 进入Boss房间，击败Boss
5. 观察梯子是否正确激活

## 📊 性能优化建议

- 使用对象池（Object Pool）管理频繁生成的道具
- 限制每个房间的生成数量上限
- 降低碰撞检测的重试次数
- 使用 LOD（Level of Detail）系统管理远处房间

---

**系统完成日期**: 2025年11月5日  
**作者**: AI Assistant  
**版本**: 1.0
