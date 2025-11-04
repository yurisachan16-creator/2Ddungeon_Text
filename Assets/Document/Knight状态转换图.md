# Knight角色状态转换图

## 完整状态流程图

```
                    游戏开始
                       ↓
                  ┌─────────┐
            ┌────→│  Idle   │←────┐
            │     │ 空闲状态 │     │
            │     └─────────┘     │
            │          ↓           │
            │     检测输入...      │
            │          ↓           │
            │     ┌─────────┐     │
            │  ┌─→│  Move   │─┐   │
            │  │  │ 移动状态 │ │   │
            │  │  └─────────┘ │   │
            │  │       ↓       │   │
            │  │   停止移动    │   │
            │  │       ↓       │   │
            │  └───────┼───────┘   │
            │          │           │
            │    ┌─────┴─────┐     │
            │    ↓           ↓     │
            │ 左键攻击    右键攻击  │
            │    ↓           ↓     │
       结束 │ ┌─────────┐ ┌─────────┐ 结束
       ────┼→│Attack01 │ │Attack02 │←┼────
            │ │ 攻击1   │ │ 攻击2   │ │
            │ └─────────┘ └─────────┘ │
            │          │           │   │
            │          └─────┬─────┘   │
            │                ↓         │
            └────────────────┼─────────┘
                             │
                        受到伤害
                             ↓
                        ┌─────────┐
                        │  Hurt   │
                        │ 受伤状态 │
                        └─────────┘
                             │
                    ┌────────┴────────┐
                    ↓                 ↓
              生命值>0           生命值=0
                    ↓                 ↓
              返回Idle          ┌─────────┐
                                │  Death  │←─ 直接死亡
                                │ 死亡状态 │
                                └─────────┘
                                     ↓
                                 游戏结束
```

---

## 状态优先级

```
死亡检测 (Death)           [最高优先级]
    ↓
受伤检测 (Hurt)            [高优先级]
    ↓
攻击输入 (Attack01/02)     [中优先级]
    ↓
移动输入 (Move)            [低优先级]
    ↓
默认状态 (Idle)            [最低优先级]
```

---

## 输入映射表

| 输入 | 当前状态 | 目标状态 | 条件 |
|------|----------|----------|------|
| 任意 | 任意 | Death | currentHealth <= 0 |
| 任意 | 非Death | Hurt | isHurt == true |
| 左键 | Idle/Move | Attack01 | 鼠标左键按下 |
| 右键 | Idle/Move | Attack02 | 鼠标右键按下 |
| WASD | Idle | Move | 有移动输入 |
| 无输入 | Move | Idle | 无移动输入 |
| - | Attack* | Idle | 攻击动画结束 |
| - | Hurt | Idle | 受伤动画结束 |

---

## 各状态详细说明

### 1️⃣ IdleState（空闲状态）

**特征**：
- 🎯 默认状态
- 👀 等待玩家输入
- 🔄 可转换到所有其他状态

**进入条件**：
- 游戏开始（初始状态）
- 移动停止
- 攻击动画结束
- 受伤恢复

**退出条件**：
- 检测到移动输入 → Move
- 检测到攻击输入 → Attack01/Attack02
- 受到伤害 → Hurt
- 生命值归零 → Death

**代码逻辑**：
```csharp
OnEnter()  → 设置Speed=0
OnUpdate() → 检测输入和状态标志
           → 判断转换条件
OnExit()   → 无特殊操作
```

---

### 2️⃣ MoveState（移动状态）

**特征**：
- 🏃 处理角色移动
- 🔄 根据方向翻转角色
- 🎬 播放移动动画

**进入条件**：
- 从Idle检测到移动输入

**退出条件**：
- 移动输入停止 → Idle
- 攻击输入 → Attack01/Attack02
- 受到伤害 → Hurt
- 生命值归零 → Death

**移动逻辑**：
```csharp
OnUpdate()      → 获取输入，更新动画
OnFixedUpdate() → Rigidbody2D.MovePosition()
OnExit()        → 清空movement向量
```

**角色翻转**：
```
movement.x > 0 → localScale.x = 1  (面向右)
movement.x < 0 → localScale.x = -1 (面向左)
```

---

### 3️⃣ Attack01State（攻击1状态）

**特征**：
- ⚔️ 鼠标左键攻击
- ⏱️ 持续时间：0.5秒
- 🔒 攻击期间锁定移动

**进入条件**：
- 从Idle或Move检测到左键按下

**退出条件**：
- 攻击时间结束 → Idle
- 死亡（优先级更高）→ Death

**攻击判定**：
```csharp
OnEnter() → 播放Attack01动画
          → 设置isAttacking=true
OnUpdate() → 更新计时器
           → 时间到切换到Idle
OnExit()  → 重置isAttacking=false
```

**伤害判定**：
- 由`PerformAttack()`方法执行
- 建议在动画关键帧调用（Animation Event）
- 使用`Physics2D.OverlapCircleAll`检测敌人

---

### 4️⃣ Attack02State（攻击2状态）

**特征**：
- ⚔️ 鼠标右键攻击
- ⏱️ 持续时间：0.6秒（比Attack01稍长）
- 🔒 攻击期间锁定移动

**与Attack01的区别**：
- 触发方式：右键 vs 左键
- 持续时间：0.6秒 vs 0.5秒
- 动画Trigger：Attack02 vs Attack01
- 其他逻辑相同

**可扩展**：
- 可设置不同伤害值
- 可添加特殊效果
- 可实现连招系统

---

### 5️⃣ HurtState（受伤状态）

**特征**：
- 💔 播放受伤动画
- 🛡️ 提供无敌帧（0.4秒）
- ⚠️ 高优先级状态

**进入条件**：
- 调用`TakeDamage()`后isHurt设为true
- 生命值 > 0

**退出条件**：
- 受伤时间结束 → Idle
- 生命值归零 → Death（优先）

**无敌帧机制**：
```csharp
OnEnter() → 播放Hurt动画
          → 重置计时器
OnUpdate() → timer += Time.deltaTime
           → timer >= 0.4f 时退出
OnExit()  → isHurt = false
```

**可扩展**：
- 添加击退效果
- 闪烁渲染（无敌效果）
- 播放受伤音效

---

### 6️⃣ DeathState（死亡状态）

**特征**：
- 💀 终态，不可逆
- 🚫 禁用所有控制
- ⏱️ 播放死亡动画（1.5秒）

**进入条件**：
- 任何状态检测到currentHealth <= 0

**退出条件**：
- 无（除非复活）

**死亡处理**：
```csharp
OnEnter() → 播放Death动画
          → Rigidbody2D.simulated = false
          → 停止移动
OnUpdate() → 等待动画结束
           → 可销毁对象或触发复活
OnExit()  → 重新启用物理模拟（复活时）
```

---

## 状态转换条件代码示例

### 标准检查顺序
```csharp
public void OnUpdate()
{
    // 1. 死亡检测（最高优先级）
    if (character.IsDead)
    {
        character.StateMachine.ChangeState(new KnightDeathState(character));
        return; // 立即返回
    }
    
    // 2. 受伤检测（高优先级）
    if (character.IsHurt)
    {
        character.StateMachine.ChangeState(new KnightHurtState(character));
        return;
    }
    
    // 3. 攻击检测（中优先级）
    if (Input.GetMouseButtonDown(0))
    {
        character.StateMachine.ChangeState(new KnightAttack01State(character));
        return;
    }
    
    if (Input.GetMouseButtonDown(1))
    {
        character.StateMachine.ChangeState(new KnightAttack02State(character));
        return;
    }
    
    // 4. 移动检测（低优先级）
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");
    
    if (horizontal != 0 || vertical != 0)
    {
        character.StateMachine.ChangeState(new KnightMoveState(character));
        return;
    }
    
    // 5. 默认：保持当前状态
}
```

---

## 动画参数配置

### Animator Controller参数

| 参数名 | 类型 | 值范围 | 用途 |
|--------|------|--------|------|
| Speed | Float | 0-1 | 0=Idle, >0=Move |
| IsAttacking | Bool | true/false | 攻击状态标志 |
| Attack01 | Trigger | - | 触发攻击1动画 |
| Attack02 | Trigger | - | 触发攻击2动画 |
| Hurt | Trigger | - | 触发受伤动画 |
| Death | Trigger | - | 触发死亡动画 |

### 动画过渡建议

```
Idle → Move
  条件: Speed > 0.1
  过渡时间: 0.1秒
  Has Exit Time: ❌

Move → Idle
  条件: Speed < 0.1
  过渡时间: 0.1秒
  Has Exit Time: ❌

Any State → Attack01
  条件: Attack01 触发
  过渡时间: 0秒
  Has Exit Time: ❌

Any State → Attack02
  条件: Attack02 触发
  过渡时间: 0秒
  Has Exit Time: ❌

Any State → Hurt
  条件: Hurt 触发
  过渡时间: 0秒
  Has Exit Time: ❌

Any State → Death
  条件: Death 触发
  过渡时间: 0秒
  Has Exit Time: ❌
  (Death动画应设置为循环或保持最后一帧)
```

---

## 状态时间线

```
时间轴示例：玩家从Idle到攻击再返回

0.0s  │ Idle           │ 等待输入
      │                │
0.5s  │ 鼠标左键按下    │
      ↓                ↓
0.5s  │ Attack01       │ OnEnter() - 播放动画
0.6s  │ Attack01       │ OnUpdate() - 计时器更新
0.7s  │ Attack01       │ 
0.8s  │ Attack01       │ 
0.9s  │ Attack01       │ 
1.0s  │ Attack01结束    │ OnExit() - 清理标志
      ↓                ↓
1.0s  │ Idle           │ OnEnter() - 返回空闲
```

---

## 调试技巧

### 1. 状态切换日志
在StateMachine.ChangeState()中添加：
```csharp
Debug.Log($"[{Time.time:F2}s] {currentState?.GetType().Name} → {newState.GetType().Name}");
```

输出示例：
```
[0.00s] null → KnightIdleState
[2.34s] KnightIdleState → KnightMoveState
[4.56s] KnightMoveState → KnightAttack01State
[5.06s] KnightAttack01State → KnightIdleState
```

### 2. 可视化攻击范围
```csharp
private void OnDrawGizmosSelected()
{
    if (attackPoint == null) return;
    
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
}
```

### 3. Inspector显示当前状态
在KnightController中添加：
```csharp
[Header("调试信息")]
[SerializeField] private string currentStateName;

void Update()
{
    base.Update();
    currentStateName = stateMachine.CurrentState?.GetType().Name ?? "None";
}
```

---

## 常见状态转换错误

### ❌ 错误1：无限循环
```csharp
// 错误：每帧都强制切换
void OnUpdate()
{
    character.StateMachine.ChangeState(new IdleState(character));
    // 导致：永远进不了其他状态
}
```

### ✅ 正确：条件判断
```csharp
void OnUpdate()
{
    if (某个条件满足)
    {
        character.StateMachine.ChangeState(new NextState(character));
    }
}
```

### ❌ 错误2：忘记return
```csharp
void OnUpdate()
{
    if (character.IsDead)
    {
        character.StateMachine.ChangeState(new DeathState(character));
        // 缺少return，继续执行下面的代码
    }
    
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // 死亡后还能跳跃！
    }
}
```

### ✅ 正确：及时返回
```csharp
void OnUpdate()
{
    if (character.IsDead)
    {
        character.StateMachine.ChangeState(new DeathState(character));
        return; // 立即返回，不执行后续代码
    }
    
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // 只有活着才能执行
    }
}
```

---

## 性能考虑

### 状态切换频率
```
Idle ↔ Move：频繁（每次移动/停止）
Move → Attack：偶尔（玩家操作）
Attack → Idle：中等（攻击间隔）
Any → Death：极少（角色死亡）
```

### 优化建议
1. **状态缓存**：StateMachine已实现
2. **避免每帧检测**：移动输入除外
3. **合并相似检测**：如左右攻击可共用逻辑

---

*此图表配合完整技术文档使用效果最佳*
*参考文档：`CharacterStateMachine.md`*
