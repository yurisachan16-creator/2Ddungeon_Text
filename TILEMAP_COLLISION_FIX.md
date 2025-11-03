# Tilemap 门口碰撞问题 - 解决方案指南

## 问题描述
Tilemap Collider 2D 在门的位置阻挡了玩家（Dynamic Rigidbody2D），导致无法通过第二个房间的门。

## 🎯 解决方案汇总

### 方案 1：物理层碰撞矩阵设置（最简单，推荐）

#### 步骤：
1. 在 Unity 中，选择 `Edit > Project Settings > Physics 2D`
2. 向下滚动到 `Layer Collision Matrix`
3. 设置图层碰撞关系：
   - **取消勾选** `Player 层` 与 `Wall 层` 的交叉格
   - 这样玩家就不会与墙壁的 Tilemap Collider 发生物理碰撞

#### 优点：
- ✅ 最简单，不需要写代码
- ✅ 不需要修改预制体或 Tilemap
- ✅ 立即生效

#### 缺点：
- ⚠️ 玩家将完全穿过所有墙壁（需要其他方式限制玩家范围）

---

### 方案 2：使用 Composite Collider 2D（专业做法）

#### 步骤：
1. 在墙体预制体上添加 `Composite Collider 2D` 组件
2. 设置 `Tilemap Collider 2D` 的 `Used By Composite` 为 true
3. 在生成墙体后，使用 `DoorColliderRemover.cs` 移除门位置的 Tiles

#### 优点：
- ✅ 专业的 Tilemap 使用方式
- ✅ 可以精确控制碰撞形状
- ✅ 优化性能（合并多个碰撞器）

#### 使用方法：
```csharp
// 1. 将 DoorColliderRemover.cs 附加到房间预制体上
// 2. 在 Inspector 中配置门的偏移值
// 3. 运行游戏，脚本会自动移除门位置的 Tiles
```

---

### 方案 3：禁用门位置的 Tilemap Collider（手动）

#### 步骤：
1. 不使用 Tilemap Collider 2D 的自动生成
2. 手动添加 Box Collider 2D 或 Polygon Collider 2D
3. 在门的位置手动"挖空"碰撞器

#### 优点：
- ✅ 完全控制碰撞形状
- ✅ 不需要运行时计算

#### 缺点：
- ⚠️ 需要手动为每个墙体预制体配置
- ⚠️ 维护成本高

---

### 方案 4：使用 Kinematic Rigidbody2D（临时方案）

#### 步骤：
将玩家的 Rigidbody2D 类型改为 `Kinematic`

#### 优点：
- ✅ 立即解决问题
- ✅ 不需要修改场景或预制体

#### 缺点：
- ⚠️ 失去物理交互能力（击退、推动等）
- ⚠️ 不是根本解决方案

---

## 🔧 推荐实施顺序

1. **立即解决**：方案 1（物理层碰撞矩阵）或方案 4（Kinematic）
2. **长期优化**：方案 2（Composite Collider + DoorColliderRemover）
3. **如需完全控制**：方案 3（手动配置碰撞器）

---

## 📝 注意事项

### 如果使用方案 1（物理层碰撞）：
- 需要其他机制防止玩家走出房间边界
- 可以使用触发器（Trigger Collider）检测边界
- 或者通过代码限制玩家移动范围

### 如果使用方案 2（DoorColliderRemover）：
- 需要调整 `doorLeftOffset`、`doorRightOffset` 等参数
- 偏移值应该与墙体预制体的实际门位置匹配
- 可能需要根据 Tilemap 的 Cell Size 调整

### 如果墙体是运行时生成的：
- 确保 `DoorColliderRemover` 在墙体生成后执行
- 使用 `StartCoroutine` 和 `WaitForEndOfFrame` 延迟执行

---

## 🐛 调试技巧

1. **查看 Tilemap 结构**：
   ```
   - 在 Hierarchy 中展开墙体对象
   - 检查是否有 Tilemap 组件
   - 查看 Tilemap Collider 2D 的配置
   ```

2. **可视化碰撞器**：
   ```
   - 在 Scene 视图中点击右上角的 Gizmos
   - 确保勾选了 "Collider" 选项
   - 绿色线框表示碰撞器范围
   ```

3. **测试碰撞**：
   ```csharp
   // 在 PlayerController 中添加调试日志
   void OnCollisionEnter2D(Collision2D collision)
   {
       Debug.Log($"碰撞到: {collision.gameObject.name}");
   }
   ```

---

## 相关文件
- `Assets/Scripts/DoorColliderRemover.cs` - 自动移除门位置 Tiles 的脚本
- `Assets/Scripts/Room.cs` - 房间管理脚本
- `ProjectSettings/Physics2DSettings.asset` - 物理层配置
