地牢填充系统 (Dungeon Population System) - 技术文档
📋 文档信息
版本: 1.0

更新日期: 2025年11月5日

核心目的: 对 RoomGenerator (文档1) 生成的地牢进行解耦的内容填充。

编码: UTF-8

🎯 系统概述
本地牢填充系统 (Dungeon Population System) 旨在解决程序化生成的第二阶段：内容填充。

本地牢生成系统 (文档1) 解决了"形状"的生成，本系统则专注于"内容"的填充。它基于策略模式 (Strategy Pattern) 和 ScriptableObject 实现，允许策划（开发者）在 Unity Inspector 中配置复杂的生成规则，而无需修改代码。

核心特性
✅ 高解耦: 地牢生成器 (Generator) 与内容填充器 (Population) 完全分离。

✅ 编辑器驱动: 通过 ScriptableObject 在编辑器中配置生成规则、权重和数量 (满足要求2)。

✅ 易维护与扩展: 添加新道具或新规则 (如陷阱)，只需创建新的 ScriptableObject，无需修改核心代码 (满足要求3)。

✅ 满足特定需求:

(要求1) 随机生成墙壁装饰（火把、旗帜）。

(要求2) 随机生成地板贴花（新纹路）。

(要求3) 随机生成地板道具（宝箱、盒子）。

(要求4) 在Boss房生成特定物体（梯子）。

(要求5) 随机生成怪物。

🏗️ 建议的项目结构 (满足要求1)
为了保持高内聚、低耦合，我们将Dungeon相关脚本分为Generation (形状) 和 Population (内容) 两个子目录。

Assets/
├── 📁 Scripts/
│   ├── 📁 Core/
│   │   └── 📁 StateMachine/         # (已有) 状态机
│   │
│   ├── 📁 Dungeon/                  # 
│   │   ├── 📁 Generation/           # 【形状】专注"形状"
│   │   │   ├── RoomGenerator.cs    # (已有) 地牢生成器
│   │   │   └── Room.cs             # (已有) 房间脚本
│   │   │
│   │   └── 📁 Population/          # 【内容】专注"内容" (新!)
│   │       ├── DungeonPopulationManager.cs   # (新) 填充协调器
│   │       │
│   │       ├── 📁 Spawnables/      # (新) 定义"刷什么"
│   │       │   ├── SpawnableItem.cs
│   │       │   └── SpawnTable.cs   # (新) 按权重随机
│   │       │
│   │       └── 📁 Rules/           # (新) 定义"怎么刷"
│   │           ├── RoomPopulationRule.cs    # (新) 抽象基类
│   │           ├── Rule_SpawnEnemies.cs
│   │           ├── Rule_SpawnFloorObjects.cs
│   │           ├── Rule_SpawnWallDecorations.cs
│   │           └── Rule_SpawnFloorDecals.cs
│   │
│   ├── 📁 Character/
│   │   ├── 📁 Knight/
│   │   └── 📁 RangedEnemy/         # (已有) 远程敌人
│   │
│   └── 📁 Gameplay/
│       └── CameraController.cs     # (已有)
│
└── 📁 ScriptableObjects/           # (新) 策划配置数据
    ├── 📁 Dungeon/
    │   ├── 📁 SpawnTables/
    │   │   ├── T1_Enemies.asset    # (敌人生成表)
    │   │   ├── T1_Floor_Props.asset (地板道具生成表)
    │   │   └── T1_Wall_Decorations.asset (墙壁装饰生成表)
    │   └── 📁 PopulationRules/
    │       ├── Rule_Normal_Spawn_2_Enemies.asset
    │       ├── Rule_Normal_Spawn_Boxes.asset
    │       ├── Rule_Boss_Spawn_Ladder.asset
    │       └── Rule_Spawn_Torches.asset
    └── 📁 Characters/
        └── RangedEnemy.asset
🔧 核心实现步骤
步骤 1：修改 Room.cs (提供“在哪里刷”的信息)
我们需要在 BasicRoom.prefab 上定义可供生成的“锚点”。

打开 Assets/Scripts/Dungeon/Generation/Room.cs (原始文档)，添加新字段：

C#

// 在 Room.cs 中添加
using UnityEngine;
using UnityEngine.UI; // 原始文档中有 Text，可能需要

public class Room : MonoBehaviour
{
    // ... 已有字段 (doorLeft, roomUp, doorNumber, stepToStart 等) ...

    [Header("生成锚点 (Population Anchors)")]
    [Tooltip("地板上所有可生成道具/敌人的点")]
    public Transform[] floorSpawnPoints;

    [Tooltip("上墙（北墙）上可生成火把/旗帜的点")]
    public Transform[] topWallSpawnPoints;

    [Tooltip("下墙（南墙）...")]
    public Transform[] bottomWallSpawnPoints;

    [Tooltip("左墙（西墙）...")]
    public Transform[] leftWallSpawnPoints;

    [Tooltip("右墙（东墙）...")]
    public Transform[] rightWallSpawnPoints;

    [Tooltip("用于生成地板贴花（Decals）的点")]
    public Transform[] decalSpawnPoints;
    
    // ... 已有方法 (SetupDoor, UpdateRoom, OnTriggerStay2D 等) ...

    // 辅助方法：获取一个随机的地板点
    public Vector3 GetRandomFloorPosition()
    {
        if (floorSpawnPoints == null || floorSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Room: " + gameObject.name + " 缺少 floorSpawnPoints，返回房间中心");
            return transform.position; // Fallback
        }
        return floorSpawnPoints[Random.Range(0, floorSpawnPoints.Length)].position;
    }

    // (为其他锚点组添加类似的 GetRandom...Position 方法)
}
操作指南:

编辑 BasicRoom.prefab。

在预制体中，创建空的 GameObject，例如 [Anchors]。

在其下创建子对象，例如 [FloorSpawns], [TopWallSpawns]。

在这些子对象下，放置多个空的 SpawnPoint 对象，调整它们到你希望生成道具的位置（比如贴近上墙、在地板中央等）。

将这些 SpawnPoint 拖拽到 Room.cs 对应的新数组字段中。

步骤 2：创建 SpawnTable.cs (定义“刷什么”)
为了控制权重 (满足要求2)，我们使用 ScriptableObject 来定义“生成表”。

创建 Assets/Scripts/Dungeon/Population/Spawnables/SpawnableItem.cs:

C#

// 1. 定义一个可生成的物品
[System.Serializable]
public class SpawnableItem
{
    public GameObject prefab;
    [Range(1, 100)]
    public float weight = 1.0f; // 权重
}
创建 Assets/Scripts/Dungeon/Population/Spawnables/SpawnTable.cs:

C#

using UnityEngine;

// 2. 创建一个ScriptableObject "生成表"
[CreateAssetMenu(fileName = "NewSpawnTable", menuName = "Dungeon/Spawn Table")]
public class SpawnTable : ScriptableObject
{
    public SpawnableItem[] items;

    // 按权重随机获取一个Prefab
    public GameObject GetRandomPrefab()
    {
        if (items == null || items.Length == 0) return null;

        float totalWeight = 0;
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var item in items)
        {
            currentWeight += item.weight;
            if (randomValue <= currentWeight)
            {
                return item.prefab;
            }
        }
        return null; // 理论上不应到达这里
    }
}
操作指南:

在 Assets/ScriptableObjects/Dungeon/SpawnTables/ 目录下，右键 Create > Dungeon > Spawn Table。

创建 T1_Enemies.asset，在里面放入 RangedEnemy Prefab 和其他敌人。

创建 T1_Wall_Decorations.asset，放入你的“火把”和“旗帜”Prefab，并调整权重。

创建 T1_Floor_Props.asset，放入“盒子”和“宝箱”Prefab。

步骤 3：创建 RoomPopulationRule.cs (定义“怎么刷”)
这是系统的核心（策略模式），定义了所有可能的生成规则。

创建 Assets/Scripts/Dungeon/Population/Rules/RoomPopulationRule.cs:

C#

using UnityEngine;

// 抽象基类
public abstract class RoomPopulationRule : ScriptableObject
{
    [TextArea(3, 5)]
    public string description; // 方便策划备注

    // 每个规则都必须实现Apply方法
    // room: 要填充的房间
    // isBossRoom: 标记这是否为Boss房
    public abstract void Apply(Room room, bool isBossRoom);
}
规则实现 1：地板物体 (怪物/道具) (满足要求3, 5)
这个脚本可以复用。

创建 Assets/Scripts/Dungeon/Population/Rules/Rule_SpawnFloorObjects.cs:

C#

using UnityEngine;

[CreateAssetMenu(fileName = "Rule_SpawnFloor", menuName = "Dungeon/Population Rule/Spawn Floor Objects")]
public class Rule_SpawnFloorObjects : RoomPopulationRule
{
    [Header("规则配置")]
    public SpawnTable spawnTable; // 拖入 T1_Enemies.asset 或 T1_Floor_Props.asset
    public int minAmount = 1;
    public int maxAmount = 3;

    public override void Apply(Room room, bool isBossRoom)
    {
        int count = Random.Range(minAmount, maxAmount + 1);
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = spawnTable.GetRandomPrefab();
            if (prefab != null)
            {
                // 使用 Room 脚本中定义的锚点
                Vector3 position = room.GetRandomFloorPosition();
                Instantiate(prefab, position, Quaternion.identity, room.transform);
            }
        }
    }
}
规则实现 2：墙壁装饰 (满足要求1)
创建 Assets/Scripts/Dungeon/Population/Rules/Rule_SpawnWallDecorations.cs:

C#

using UnityEngine;

[CreateAssetMenu(fileName = "Rule_SpawnWall", menuName = "Dungeon/Population Rule/Spawn Wall Decorations")]
public class Rule_SpawnWallDecorations : RoomPopulationRule
{
    public enum WallDirection { Top, Bottom, Left, Right }
    
    [Header("规则配置")]
    public WallDirection targetWall; // 在Inspector里选择"Top"
    public SpawnTable spawnTable;    // 拖入 T1_Wall_Decorations.asset
    public int minAmount = 0;
    public int maxAmount = 2;
    
    public override void Apply(Room room, bool isBossRoom)
    {
        // 关键检查：只有在那个方向"没有"相邻房间时，才有墙壁
        bool wallExists = false;
        Transform[] spawnPoints = null;

        switch (targetWall)
        {
            case WallDirection.Top:
                if (!room.roomUp) { wallExists = true; spawnPoints = room.topWallSpawnPoints; }
                break;
            case WallDirection.Bottom:
                if (!room.roomDown) { wallExists = true; spawnPoints = room.bottomWallSpawnPoints; }
                break;
            case WallDirection.Left:
                if (!room.roomLeft) { wallExists = true; spawnPoints = room.leftWallSpawnPoints; }
                break;
            case WallDirection.Right:
                if (!room.roomRight) { wallExists = true; spawnPoints = room.rightWallSpawnPoints; }
                break;
        }

        if (wallExists && spawnPoints != null && spawnPoints.Length > 0)
        {
            int count = Random.Range(minAmount, maxAmount + 1);
            for (int i = 0; i < count; i++)
            {
                GameObject prefab = spawnTable.GetRandomPrefab();
                if (prefab != null)
                {
                    Vector3 position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                    Instantiate(prefab, position, Quaternion.identity, room.transform);
                }
            }
        }
    }
}
规则实现 3：地板纹路 (满足要求2)
创建 Assets/Scripts/Dungeon/Population/Rules/Rule_SpawnFloorDecals.cs:

C#

using UnityEngine;

[CreateAssetMenu(fileName = "Rule_SpawnDecal", menuName = "Dungeon/Population Rule/Spawn Floor Decals")]
public class Rule_SpawnFloorDecals : RoomPopulationRule
{
    public SpawnTable decalTable; // 包含各种"纹路"Prefab
    public int minAmount = 1;
    public int maxAmount = 5;
    public string sortingLayerName = "Decals"; // 确保此层在 Floor 之上

    public override void Apply(Room room, bool isBossRoom)
    {
        int count = Random.Range(minAmount, maxAmount + 1);
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = decalTable.GetRandomPrefab();
            if (prefab == null) continue;

            // 使用 Decal 专用锚点
            Vector3 position = room.decalSpawnPoints[Random.Range(0, room.decalSpawnPoints.Length)].position;
            GameObject spawnedDecal = Instantiate(prefab, position, Quaternion.identity, room.transform);

            // 关键：设置Sorting Layer (视觉覆盖)
            SpriteRenderer renderer = spawnedDecal.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = 1; // 确保在基础地板之上
            }
        }
    }
}
(注意: 你需要先在 Project Settings > Tags and Layers > Sorting Layers 中添加 "Decals" 层)

步骤 4：创建 DungeonPopulationManager.cs (“总指挥”)
这是唯一的 MonoBehaviour，负责协调所有规则。

创建 Assets/Scripts/Dungeon/Population/DungeonPopulationManager.cs:

C#

using UnityEngine;
using System.Collections.Generic;

public class DungeonPopulationManager : MonoBehaviour
{
    [Header("常规房间规则")]
    [Tooltip("应用于所有非起始/非Boss房的规则")]
    public List<RoomPopulationRule> normalRoomRules;

    [Header("Boss房间规则")]
    [Tooltip("只应用于Boss房的规则")]
    public List<RoomPopulationRule> bossRoomRules;

    [Header("Boss房特殊生成 (满足要求4)")]
    [Tooltip("在Boss房打完后生成的梯子/出口")]
    public GameObject bossExitPrefab; // 拖入"梯子" Prefab

    
    // 这个方法由外部调用 (比如GameManager或RoomGenerator)
    public void PopulateDungeon(List<Room> allRooms, Room startRoom, Room endRoom)
    {
        if (allRooms == null || allRooms.Count == 0) return;

        foreach (Room room in allRooms)
        {
            // 1. 跳过起始房间 (通常不刷怪)
            if (room == startRoom)
            {
                continue;
            }

            // 2. 判断是Boss房还是一般房间
            bool isBossRoom = (room == endRoom);

            if (isBossRoom)
            {
                // 应用Boss房规则
                foreach (var rule in bossRoomRules)
                {
                    if (rule == null) continue;
                    rule.Apply(room, isBossRoom: true);
                }
                
                // (满足要求4) 生成梯子 (但先设为非激活)
                if (bossExitPrefab != null)
                {
                    Vector3 exitPos = room.GetRandomFloorPosition(); // 或者指定一个中心点
                    GameObject exitLadder = Instantiate(bossExitPrefab, exitPos, Quaternion.identity, room.transform);
                    exitLadder.name = "Boss_Exit_Ladder";
                    exitLadder.SetActive(false); // 等待Boss被击败事件来激活
                }
            }
            else
            {
                // 应用常规房间规则
                foreach (var rule in normalRoomRules)
                {
                    if (rule == null) continue;
                    rule.Apply(room, isBossRoom: false);
                }
            }
        }
    }
}
步骤 5：连接游戏流程 (总装)
最后，我们需要在 RoomGenerator (文档1) 生成地牢之后，调用我们的 DungeonPopulationManager。

打开 Assets/Scripts/Dungeon/Generation/RoomGenerator.cs：

C#

// 在 RoomGenerator.cs 中
using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    // ... 已有字段 (roomPrefab, roomNumber, wallType 等) ...
    
    // (新) 引用填充管理器
    [Header("系统引用")]
    [Tooltip("拖入场景中的DungeonPopulationManager对象")]
    public DungeonPopulationManager populationManager;

    // (已有) 引用所有生成的房间
    // public List<Room> generatedRooms; 
    
    // (已有) 引用结束房间
    // public GameObject endRoom; 

    void Start()
    {
        // ... (省略你已有的 第1阶段：房间生成) ...
        // ... (省略你已有的 第2阶段：房间配置) ...
        // ... (省略你已有的 第3阶段：标记阶段) ...

        // 4. (新) 填充阶段
        if (populationManager != null)
        {
            if (generatedRooms.Count > 0 && endRoom != null)
            {
                // 获取起始和结束房间
                Room startRoom = generatedRooms[0]; // 文档1假定第一个是起始
                Room finalEndRoom = endRoom.GetComponent<Room>(); // 文档1的endRoom是GameObject

                populationManager.PopulateDungeon(generatedRooms, startRoom, finalEndRoom);
            }
            else
            {
                Debug.LogError("房间列表为空或EndRoom未找到，无法填充！");
            }
        }
        else
        {
            Debug.LogWarning("DungeonPopulationManager 未在 RoomGenerator 上设置! 跳过填充阶段。");
        }

        // ... (你已有的热重载逻辑可以保留) ...
    }

    // ... (你已有的 ChangePointPos, SetupRoom, FindEndRoom 等方法) ...
}
🏁 总结：如何使用这个新系统
场景设置:

在场景中创建一个空 GameObject，命名为 DungeonPopulationManager。

把 DungeonPopulationManager.cs 脚本挂载上去。

在 RoomGenerator 组件上，把 DungeonPopulationManager 对象拖到 Population Manager 字段中。

将你的“梯子” Prefab 拖入 DungeonPopulationManager 的 Boss Exit Prefab 字段。

创建数据 (ScriptableObjects):

在 Assets/ScriptableObjects/SpawnTables/ 文件夹下创建你的"生成表" (T1_Enemies, T1_Floor_Props, T1_Wall_Decorations)。

把你的 Prefab（RangedEnemy, Box, Torch, Banner）拖到这些生成表中，并设置权重 (满足要求2)。

创建规则 (ScriptableObjects):

在 Assets/ScriptableObjects/PopulationRules/ 文件夹下创建"规则"实例。

例1 (火把 - 要求1)：右键创建 Dungeon/Population Rule/Spawn Wall Decorations，命名为 Rule_Spawn_Torches。设置 Target Wall = Top，拖入 T1_Wall_Decorations.asset。

例2 (盒子 - 要求3)：右键创建 Dungeon/Population Rule/Spawn Floor Objects，命名为 Rule_Spawn_Boxes。设置 min=0, max=3，拖入 T1_Floor_Props.asset。

例3 (怪物 - 要求5)：右键创建 Dungeon/Population Rule/Spawn Floor Objects，命名为 Rule_Spawn_Enemies。设置 min=1, max=2，拖入 T1_Enemies.asset。

配置管理器:

选中场景中的 DungeonPopulationManager 对象。

把 Rule_Spawn_Boxes 和 Rule_Spawn_Enemies 拖入 Normal Room Rules 列表。

把 Rule_Spawn_Torches 也拖入 Normal Room Rules 列表。

(你可以为 Boss Room Rules 配置一套完全不同的、更难的规则列表)。

现在，当你运行游戏时，RoomGenerator 会先生成地牢，然后 DungeonPopulationManager 会自动根据你在 Inspector 中配置的规则和权重，去填充所有房间。