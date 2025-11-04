# 2D Dungeon Text - Unity 地牢生成器

## 项目简介
基于 Unity 2022.3.62f2c1 开发的 2D 地牢程序化生成原型项目，使用随机游走算法生成房间布局。

## 核心特性

### 地牢生成系统
- 🎲 **随机游走算法**：使用上下左右随机移动生成房间
- 🎨 **颜色标记系统**：自动标记起始房间（绿色）和终点房间（红色）
- 🔄 **热重载测试**：运行时按任意键即可重新生成地牢布局
- 🚫 **重叠检测**：基于 `Physics2D.OverlapCircle` 防止房间重叠

### ⭐ 角色状态机系统（新增）
- 🎮 **通用FSM框架**：适配近战、远程等不同角色类型
- 🗡️ **Knight角色**：完整的玩家控制（移动、双攻击、受伤、死亡）
- 🏹 **远程敌人示例**：AI自动检测和攻击
- 📚 **完整文档**：包含快速指南和详细技术文档
- 🔧 **易于扩展**：3步骤创建新角色类型

## 技术栈
- **引擎版本**：Unity 2022.3.62f2c1
- **核心系统**：
  - Unity 2D Physics (`com.unity.modules.physics2d`)
  - Unity 2D Feature (`com.unity.feature.2d`)
  - TextMeshPro (`com.unity.textmeshpro`)

## 项目结构
```
Assets/
├── Prefabs/
│   └── BasicRoom.prefab          # 房间预制体（需包含 SpriteRenderer）
├── Scenes/
│   ├── SampleScene.unity         # 主场景
│   └── New Scene.unity           # 测试场景
├── Scripts/
│   ├── StateMachine/             # ⭐ 状态机核心框架
│   │   ├── IState.cs            # 状态接口
│   │   └── StateMachine.cs      # 状态机管理类
│   ├── Character/                # ⭐ 角色系统
│   │   ├── CharacterBase.cs     # 角色基类
│   │   ├── Knight/              # Knight角色（近战玩家）
│   │   │   ├── KnightController.cs
│   │   │   ├── KnightIdleState.cs
│   │   │   ├── KnightMoveState.cs
│   │   │   ├── KnightAttack01State.cs
│   │   │   ├── KnightAttack02State.cs
│   │   │   ├── KnightHurtState.cs
│   │   │   └── KnightDeathState.cs
│   │   └── RangedEnemy/         # 远程敌人（AI示例）
│   │       ├── RangedEnemyController.cs
│   │       ├── RangedEnemyIdleState.cs
│   │       ├── RangedEnemyAttackState.cs
│   │       ├── RangedEnemyHurtState.cs
│   │       └── RangedEnemyDeathState.cs
│   ├── RoomGenerator.cs          # 房间生成核心逻辑
│   ├── Room.cs                   # 房间脚本
│   ├── PlayerController.cs       # 原始玩家控制器
│   └── CameraController.cs       # 摄像机控制器
├── Document/                      # ⭐ 项目文档
│   ├── CharacterStateMachine.md  # 状态机完整技术文档
│   ├── 角色状态机快速指南.md      # 快速开始指南
│   └── 文件组织结构.md            # 文件结构说明
└── Tiles/                        # Tilemap 资源（未使用）
    ├── Palettes/                 # 调色板
    └── Tiles/                    # Tile 资源
        ├── Floor/                # 地板 Tiles
        └── Wall/                 # 墙壁 Tiles
```

⭐ **新增**：完整的角色状态机系统！查看 `Assets/Document/` 了解详情。

## 快速开始

### 1. 克隆项目
```bash
git clone https://github.com/yurisachan16-creator/2Ddungeon_Text.git
```

### 2. 使用 Unity 打开
- 打开 Unity Hub
- 点击 "Open" 并选择项目文件夹
- 确保使用 Unity 2022.3.62f2c1 或更高版本

### 3. 运行测试
- 打开 `Assets/Scenes/SampleScene.unity`
- 点击 Play 按钮
- 按任意键重新生成地牢布局

## 配置说明

### RoomGenerator 组件参数
在 Unity Inspector 中配置以下参数：

**房间信息**
- `Room Prefab`：房间预制体（需包含 SpriteRenderer）
- `Room Number`：生成房间数量（建议：10-50）
- `Start Color`：起始房间颜色（默认：绿色）
- `End Color`：终点房间颜色（默认：红色）

**位置控制**
- `Generator Point`：生成起点 Transform
- `X Offset`：横向房间间距（推荐：4-6）
- `Y Offset`：纵向房间间距（推荐：4-6）
- `Room Layer`：必须设置为 "Room" 图层（Layer 6）

### 图层设置
确保在 `Edit > Project Settings > Tags and Layers` 中：
- Layer 6 设置为 "Room"

## 算法说明

### 随机游走算法流程
1. 从起点 `generatorPoint` 开始
2. 随机选择 4 个方向之一（上/下/左/右）
3. 使用 `Physics2D.OverlapCircle` 检测目标位置是否已有房间
4. 如果重叠，重新选择方向（do-while 循环）
5. 如果不重叠，生成新房间并更新生成点位置
6. 重复步骤 2-5 直到达到指定房间数量

### 起点/终点标记逻辑
- **起始房间**：列表中第一个房间（索引 0）
- **终点房间**：通过 `sqrMagnitude` 计算距离原点最远的房间

## 已知限制
⚠️ **算法限制**
- `do-while` 循环无回溯机制，房间过多或间距过小时可能卡住
- 未实现房间间的门/走廊连接系统
- 仅生成房间位置，不处理内部布局

⚠️ **代码架构**
- 无命名空间定义（适合小型原型项目）
- 单一脚本设计，未拆分生成器和房间管理

## 快速开始 - 角色系统

### 使用Knight角色（5分钟）
1. 创建空GameObject，命名为"Knight"，Tag设置为"Player"
2. 添加组件：
   - `KnightController`（脚本）
   - `Rigidbody2D`（Gravity Scale: 0）
   - `Animator`
   - `Sprite Renderer`
   - `Box Collider 2D`
3. 在Knight下创建子对象"AttackPoint"，移至武器位置
4. 配置KnightController参数并运行
5. 控制：WASD移动，左键/右键攻击

**详细教程**：查看 `Assets/Document/角色状态机快速指南.md`

## 扩展建议

### 地牢生成系统
#### 添加回溯机制
```csharp
public void ChangePointPos()
{
    int maxRetries = 100; // 添加重试计数器
    int retries = 0;
    
    do
    {
        // 现有逻辑...
        retries++;
        if (retries > maxRetries)
        {
            Debug.LogWarning("无法找到合适位置，终止生成");
            break;
        }
    } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));
}
```

#### 添加房间连接
考虑使用：
- Tilemap 系统生成走廊
- Line Renderer 绘制路径
- NavMesh 生成导航网格

### 角色系统
- 📖 参考 `Assets/Document/CharacterStateMachine.md` 创建新角色
- 🎨 添加更多状态（跳跃、冲刺、格挡等）
- 🤖 实现更复杂的敌人AI
- 💥 添加技能系统和连招

## AI 开发指南
本项目包含 `.github/copilot-instructions.md`，为 AI 编程助手提供：
- 核心架构说明
- Unity 特定配置要求
- 代码约定和命名规范
- 常见陷阱和调试建议

## 贡献指南
欢迎提交 Issue 和 Pull Request！

建议改进方向：
- 🔧 添加算法回溯机制
- 🚪 实现房间间连接系统
- 🗺️ 使用 Tilemap 生成走廊
- 📊 添加房间类型系统（战斗房/宝藏房/商店）
- 🎮 完善玩家移动和摄像机跟随

## 许可证
MIT License

## 联系方式
- GitHub: [@yurisachan16-creator](https://github.com/yurisachan16-creator)
- 项目地址: https://github.com/yurisachan16-creator/2Ddungeon_Text
