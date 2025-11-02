# Unity 2D 地牢生成器项目 - AI 编程助手指南

## 项目概述
这是一个使用 Unity 2022.3.62f2c1 开发的 2D 地牢程序化生成原型项目，使用随机游走算法生成房间布局。

## 核心架构

### 房间生成系统（`Assets/Scripts/RoomGenerator.cs`）
- **生成算法**：随机游走（Random Walk）算法
  - 从起点开始，随机选择4个方向（上下左右）之一
  - 使用 `Physics2D.OverlapCircle` 和 `roomLayer` 检测重叠，避免房间重复生成在同一位置
  - 持续生成直到达到 `roomNumber` 指定的数量
  
- **关键字段说明**：
  - `generatorPoint.position`: 当前生成位置，在 do-while 循环中不断更新
  - `xOffset` / `yOffset`: 房间间距控制（非房间尺寸）
  - `roomLayer`: 必须设置为 Layer 6 "Room"，用于碰撞检测
  - `generatedRooms`: 存储所有生成的房间引用，用于后续操作

- **颜色标记系统**：
  - 起始房间（索引0）使用 `startColor`
  - 结束房间通过距离原点最远（`sqrMagnitude` 最大）确定，使用 `endColor`

### Unity 特定配置
- **图层设置**：Layer 6 名为 "Room"（见 `ProjectSettings/TagManager.asset`）
- **预制体结构**：`Assets/Prefabs/BasicRoom.prefab` 必须包含 `SpriteRenderer` 组件
- **物理系统**：依赖 Unity 2D Physics（`com.unity.modules.physics2d`）

## 开发工作流

### 快速测试迭代
- 运行时按任意键触发 `SceneManager.LoadScene()` 重新生成地牢（热重载）
- 无需停止 Play Mode 即可查看不同的随机布局

### 调试建议
- 在 Inspector 中调整 `roomNumber` 观察生成密度
- 减小 `xOffset`/`yOffset` 会增加房间重叠检测失败率（无限循环风险）
- 使用 Scene 视图的 2D 模式查看房间布局

## 代码约定

### 命名规范
- **中文注释**：所有注释使用简体中文
- **Inspector 标签**：使用 `[Header("中文标题")]` 分组字段
- **枚举定义**：内嵌在类内部（如 `Direction` 枚举）

### Unity API 使用模式
- **坐标计算**：使用 `Vector3` 加法直接修改位置（`position += new Vector3(...)`）
- **碰撞检测**：使用 `Physics2D.OverlapCircle(position, radius, layerMask)` 判断位置占用
- **颜色操作**：通过 `GetComponent<SpriteRenderer>().color` 直接赋值

## 扩展指南

### 添加新房间类型
1. 确保预制体包含 `SpriteRenderer` 组件（颜色标记依赖）
2. 在 Inspector 中分配到 `roomPrefab` 字段
3. 考虑房间 Collider2D 尺寸与 `OverlapCircle` 半径（0.2f）的匹配

### 修改生成算法
- 当前 `do-while` 循环可能导致死锁（房间过多且偏移过小时）
- 建议添加重试计数器或回溯机制防止无限循环
- 参考 `ChangePointPos()` 方法的控制流结构

## 已知限制
- 无命名空间定义（单脚本项目规模）
- 生成算法无回溯，可能在密集布局时卡住
- 房间间无门/走廊连接逻辑（仅生成位置）
- 未使用 Tilemap 系统（尽管已导入 `com.unity.modules.tilemap`）

## 相关文件
- 场景文件：`Assets/Scenes/SampleScene.unity` 或 `New Scene.unity`
- 预制体：`Assets/Prefabs/BasicRoom.prefab`
- 项目配置：`ProjectSettings/TagManager.asset`（Layer 定义）
