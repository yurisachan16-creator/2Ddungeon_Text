# 怪物有限状态机（FSM）实现总结

## 概述
本文档总结了三种怪物（Skeleton、Boss、Slime）的有限状态机实现细节。所有怪物继承自 `CharacterBase` 基类，使用统一的状态机系统。

## 实现日期
2024年（根据项目时间）

## 怪物类型对比

### 1. Skeleton（骷髅）- 近战敌人
**角色类型**: `CharacterType.Melee`  
**动画资源**: 7个动画（Attack, Death, Defense, Hurt, Idle, Walk）  
**特点**: 拥有独特的防御技能

#### 状态机组成
| 状态类 | 功能描述 | 动画 | 特殊说明 |
|--------|---------|------|---------|
| `SkeletonIdleState` | 空闲等待 | Idle (Speed=0) | 超过3秒自动切换到巡逻 |
| `SkeletonPatrolState` | 区域巡逻 | Walk (Speed=1) | 随机巡逻点，到达后等待2秒 |
| `SkeletonChaseState` | 追击玩家 | Walk (Speed=1) | 使用 ChaseSpeed (2.5) 移动 |
| `SkeletonAttackState` | 近战攻击 | Attack (Trigger) | 持续0.6秒，中间时刻执行伤害判定 |
| `SkeletonDefenseState` | 防御状态 | Defense (Trigger) | **特有**，受伤时30%概率触发，持续2秒 |
| `SkeletonHurtState` | 受伤硬直 | Hurt (Trigger) | 持续0.4秒，硬直期间无法行动 |
| `SkeletonDeathState` | 死亡动画 | Death (Trigger) | 1.5秒后销毁对象 |

#### 关键参数
```csharp
侦测范围: 5f
攻击范围: 1.5f
巡逻速度: 1f
追击速度: 2.5f
防御概率: 30%
防御持续时间: 2秒
```

#### AI 决策流程
```
空闲/巡逻 → 检测到玩家（5米内）→ 追击状态
追击状态 → 接近到1.5米 → 攻击状态
攻击后 → 继续攻击 或 继续追击 或 返回巡逻
受伤时 → 30%概率进入防御状态（格挡），70%进入受伤状态
```

---

### 2. Boss（Boss）- 远程Boss
**角色类型**: `CharacterType.Boss`  
**动画资源**: 5个动画（Attack, Death, Fly, Hurt, Idle）  
**特点**: 飞行移动、远程弹幕攻击、召唤小怪、冲锋技能

#### 状态机组成
| 状态类 | 功能描述 | 动画 | 特殊说明 |
|--------|---------|------|---------|
| `BossIdleState` | 悬浮等待 | Idle (Speed=0) | 分析战况，选择最优技能 |
| `BossChaseState` | 飞行追击 | Fly (Trigger) + Speed=1 | **Fly动画代替Walk**，追击速度3 |
| `BossRangedAttackState` | 远程攻击 | Attack (Trigger) | **主要攻击**，发射弹幕，冷却3秒 |
| `BossMeleeAttackState` | 近战攻击 | Attack (Trigger) | 玩家过于接近时使用，伤害1.5倍，冷却2秒 |
| `BossChargeState` | 冲锋突进 | Fly + Speed=2 | 快速冲向玩家，碰撞伤害2倍，冷却5秒 |
| `BossSummonState` | 召唤小怪 | Idle | 每次召唤2只，最多3只，冷却10秒 |
| `BossHurtState` | 受伤硬直 | Hurt (Trigger) | 持续0.3秒（较短） |
| `BossDeathState` | 死亡动画 | Death (Trigger) | 2.5秒后销毁，触发Boss战结束 |

#### 关键参数
```csharp
侦测范围: 8f（比普通怪物更大）
近战范围: 2f
远程范围: 6f
飞行速度: 2f
追击速度: 3f
远程攻击冷却: 3秒
近战攻击冷却: 2秒
召唤冷却: 10秒
冲锋冷却: 5秒
最大召唤数: 3只
```

#### AI 决策流程（智能选择技能）
```
空闲状态 → 根据情况选择：
  1. 玩家在近战范围(2米) + 近战CD好 → 近战攻击（1.5倍伤害）
  2. 玩家在远程范围(6米) + 远程CD好 → 远程弹幕（主要攻击）
  3. 召唤CD好 + 召唤物<3只 + 空闲>2秒 → 召唤小怪
  4. 冲锋CD好 + 玩家距离>2米 + 空闲>3秒 → 冲锋攻击（2倍伤害）
  5. 玩家在6-8米 → 飞行追击
```

#### 弹幕系统
Boss使用 `Projectile` 组件发射弹幕：
- 在 `BossRangedAttackState` 的 0.4 秒时调用 `FireProjectile()`
- 弹幕速度：5 单位/秒
- 弹幕存在时间：2秒（自动销毁）
- 弹幕伤害：继承Boss的 `attackDamage`

---

### 3. Slime（史莱姆）- 简单近战敌人
**角色类型**: `CharacterType.Melee`  
**动画资源**: **仅3个动画**（Death, Idle, Walk）  
**特点**: 最简单的敌人，通过碰撞造成伤害，没有Attack和Hurt动画

#### 状态机组成
| 状态类 | 功能描述 | 动画 | 特殊说明 |
|--------|---------|------|---------|
| `SlimeIdleState` | 短暂停留 | Idle (Speed=0) | 2秒后自动游荡 |
| `SlimeWanderState` | 随机游荡 | Walk (Speed=1) | 使用 WanderSpeed (0.8) 移动 |
| `SlimeChaseState` | 追击玩家 | Walk (Speed=1) | 使用 ChaseSpeed (2) 移动 |
| `SlimeAttackState` | 碰撞攻击 | **Walk (Speed=1)** | **无Attack动画**，通过接近造成伤害 |
| `SlimeHurtState` | 受伤闪烁 | **Idle (Speed=0)** | **无Hurt动画**，用Idle+红色闪烁表现 |
| `SlimeDeathState` | 死亡动画 | Death (Trigger) | 1秒后销毁对象 |

#### 关键参数
```csharp
侦测范围: 4f
攻击范围: 0.8f（碰撞距离）
游荡速度: 0.8f
追击速度: 2f
攻击冷却: 1.5秒
```

#### AI 决策流程
```
游荡状态 → 检测到玩家（4米内）→ 追击状态
追击状态 → 接近到0.8米 → 攻击状态（持续碰撞）
攻击状态 → 每1.5秒造成一次伤害（通过 PerformAttack()）
受伤时 → 进入HurtState（使用Idle动画+颜色变红0.2秒）
```

#### 攻击机制（特殊）
由于 Slime 没有独立的 Attack 动画，使用**碰撞攻击**机制：
1. 在 `SlimeAttackState` 中持续使用 Walk 动画
2. 持续向玩家移动（粘附效果）
3. 检测到玩家在攻击范围(0.8米)内且冷却结束
4. 调用 `PerformAttack()` 造成伤害
5. 设置 1.5 秒冷却时间
6. 攻击状态持续 0.5 秒后重新评估

---

## 技术实现细节

### 1. 状态机基础结构
所有状态类实现 `IState` 接口：
```csharp
public interface IState
{
    void OnEnter();      // 进入状态时调用
    void OnUpdate();     // 每帧调用
    void OnFixedUpdate(); // 物理帧调用（移动）
    void OnExit();       // 退出状态时调用
}
```

### 2. 控制器基类
所有怪物继承 `CharacterBase`，提供：
- `StateMachine stateMachine` - 状态机实例
- `Rigidbody2D Rb` - 物理组件
- `Animator Animator` - 动画控制器
- `bool IsDead / IsHurt` - 状态标志
- `void TakeDamage(float)` - 受伤接口
- `void Die()` - 死亡处理
- `void OnHurt()` - 受伤处理（可重写）

### 3. 动画参数约定
| 参数名 | 类型 | 用途 |
|--------|------|------|
| `Speed` | Float | 控制 Idle/Walk 动画切换（0=Idle, >0=Walk） |
| `Attack` | Trigger | 触发攻击动画 |
| `Defense` | Trigger | 触发防御动画（仅Skeleton） |
| `Hurt` | Trigger | 触发受伤动画 |
| `Death` | Trigger | 触发死亡动画 |
| `Fly` | Trigger | 触发飞行动画（仅Boss） |

### 4. 角色翻转逻辑
所有怪物使用统一的翻转方法：
```csharp
// 根据移动方向翻转
Vector2 direction = (target.position - transform.position).normalized;
if (direction.x != 0)
{
    transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
}
```

### 5. 伤害判定方式
- **Skeleton**: 在 `SkeletonAttackState` 的 0.3 秒时调用 `PerformAttack()`，使用 `Physics2D.OverlapCircleAll` 检测范围内玩家
- **Boss 近战**: 同 Skeleton，检测近战范围（2米）
- **Boss 远程**: 发射 `Projectile` 弹幕，弹幕碰撞时造成伤害
- **Slime**: 在 `SlimeAttackState` 中直接调用 `PerformAttack(targetCharacter)`，通过距离判断

### 6. 死亡处理流程
```csharp
// 1. 播放死亡动画
animator.SetTrigger("Death");

// 2. 禁用物理
rb.velocity = Vector2.zero;
rb.simulated = false;

// 3. 禁用碰撞
GetComponent<Collider2D>().enabled = false;

// 4. 等待动画播放完毕
deathTimer >= deathDuration

// 5. 销毁对象
Object.Destroy(gameObject);
```

---

## 动画资源对应表

### Skeleton 动画控制器参数设置
```
Animator Parameters:
- Speed (Float): 0 = Idle, 1 = Walk
- Attack (Trigger): 触发攻击动画
- Defense (Trigger): 触发防御动画
- Hurt (Trigger): 触发受伤动画
- Death (Trigger): 触发死亡动画

Transitions:
Any State → Death (when Death trigger)
Idle ↔ Walk (Speed > 0 / Speed = 0)
Any State → Attack (when Attack trigger, can interrupt)
Any State → Defense (when Defense trigger)
Any State → Hurt (when Hurt trigger)
```

### Boss 动画控制器参数设置
```
Animator Parameters:
- Speed (Float): 0 = Idle, 1 = Normal Fly, 2 = Fast Fly
- Attack (Trigger): 触发攻击动画（远程/近战共用）
- Fly (Trigger): 触发飞行动画
- Hurt (Trigger): 触发受伤动画
- Death (Trigger): 触发死亡动画

Transitions:
Any State → Death (when Death trigger)
Idle → Fly (when Fly trigger)
Fly ↔ Idle (Speed changes)
Any State → Attack (when Attack trigger)
Any State → Hurt (when Hurt trigger)
```

### Slime 动画控制器参数设置
```
Animator Parameters:
- Speed (Float): 0 = Idle, >0 = Walk
- Death (Trigger): 触发死亡动画

Transitions:
Any State → Death (when Death trigger)
Idle ↔ Walk (Speed > 0 / Speed = 0)

注意：没有 Attack 和 Hurt 参数！
```

---

## Inspector 设置指南

### Skeleton 组件配置
```
SkeletonController:
  [角色基础属性] (继承自 CharacterBase)
  - Max Health: 100
  - Current Health: 100
  - Move Speed: 1.5
  - Attack Damage: 15
  
  [骷髅特殊属性]
  - Detection Range: 5
  - Attack Range: 1.5
  - Patrol Speed: 1
  - Chase Speed: 2.5
  - Player Layer: 选择 "Player" 图层
  - Attack Point: 拖入子对象 "AttackPoint"
  
  [巡逻设置]
  - Patrol Wait Time: 2
  - Patrol Radius: 3
  
  [防御设置]
  - Defense Chance: 0.3
  - Defense Duration: 2

必需组件:
- Rigidbody2D (Gravity Scale = 0, Freeze Rotation Z)
- Collider2D
- Animator (Controller = Skeleton)
- SpriteRenderer
```

### Boss 组件配置
```
BossController:
  [角色基础属性]
  - Max Health: 500
  - Current Health: 500
  - Move Speed: 2
  - Attack Damage: 30
  
  [Boss特殊属性]
  - Detection Range: 8
  - Melee Attack Range: 2
  - Ranged Attack Range: 6
  - Fly Speed: 2
  - Chase Speed: 3
  - Player Layer: 选择 "Player" 图层
  - Attack Point: 拖入子对象 "AttackPoint"
  - Projectile Prefab: 拖入弹幕预制体
  
  [技能冷却]
  - Ranged Attack Cooldown: 3
  - Melee Attack Cooldown: 2
  - Summon Cooldown: 10
  - Charge Cooldown: 5
  
  [召唤设置]
  - Summon Prefab: 拖入 Skeleton 或 Slime 预制体
  - Max Summon Count: 3
  - Summon Radius: 3

必需组件:
- Rigidbody2D (Gravity Scale = 0)
- Collider2D
- Animator (Controller = Boss)
- SpriteRenderer
```

### Slime 组件配置
```
SlimeController:
  [角色基础属性]
  - Max Health: 50
  - Current Health: 50
  - Move Speed: 1.5
  - Attack Damage: 10
  
  [史莱姆特殊属性]
  - Detection Range: 4
  - Attack Range: 0.8
  - Wander Speed: 0.8
  - Chase Speed: 2
  - Player Layer: 选择 "Player" 图层
  
  [游荡设置]
  - Wander Wait Time: 2
  - Wander Radius: 3
  
  [攻击设置]
  - Attack Cooldown: 1.5

必需组件:
- Rigidbody2D (Gravity Scale = 0)
- Collider2D
- Animator (Controller = Slime)
- SpriteRenderer
```

---

## 使用建议

### 1. 性能优化
- **侦测范围**: 避免设置过大，建议 4-8 米
- **攻击检测**: 使用 `Physics2D.OverlapCircle` 而非 `OnTriggerEnter2D`（更精确）
- **状态切换**: 在 `OnUpdate` 中优先检测死亡和受伤，避免无效计算

### 2. 关卡设计
- **Slime**: 适合作为初级敌人，大量生成
- **Skeleton**: 中级敌人，可以配合防御技能制造威胁
- **Boss**: 作为Boss战核心，配合召唤的小怪增加战斗复杂度

### 3. 难度调整参数
- **简单模式**: 降低 `attackDamage` 和 `chaseSpeed`，增加攻击冷却时间
- **困难模式**: 提高 `maxHealth` 和 `detectionRange`，降低攻击冷却
- **Boss战**: 调整 `MaxSummonCount` 控制召唤物数量

### 4. 扩展建议
- **Skeleton**: 可以添加格挡反击机制（在 `SkeletonDefenseState` 退出时触发攻击）
- **Boss**: 可以根据血量阶段切换行为（如血量<50%时更频繁召唤）
- **Slime**: 可以添加分裂机制（死亡时生成2个小史莱姆）

---

## 调试工具

### Gizmos 可视化
所有怪物在 Scene 视图中显示：
- **黄色圆圈**: 侦测范围
- **红色圆圈**: 攻击范围
- **蓝色圆圈**: 远程攻击范围（仅Boss）
- **绿色圆圈**: 巡逻/游荡范围

### 日志建议
在关键位置添加调试日志：
```csharp
Debug.Log($"{name} 切换到状态: {stateName}");
Debug.Log($"{name} 攻击了 {target.name}，造成 {damage} 点伤害");
```

---

## 常见问题

### Q1: Slime 的 Hurt 状态没有动画怎么办？
**A**: `SlimeHurtState` 使用 Idle 动画 + 颜色闪烁（变红 0.2 秒）替代受伤表现。这是因为 Slime 的动画资源中没有 Hurt.anim 文件。

### Q2: Boss 的 Walk 动画在哪里？
**A**: Boss 没有 Walk 动画，使用 `Fly` 动画代替移动表现。Boss 是飞行单位，悬浮在空中。

### Q3: 怪物卡在墙角怎么办？
**A**: 
1. 确保 Rigidbody2D 的 Collision Detection 设置为 "Continuous"
2. 为墙壁添加物理材质，设置 Friction = 0
3. 在巡逻/游荡状态中添加射线检测，避开障碍物

### Q4: 状态切换不流畅？
**A**: 检查 Animator Controller 的 Transition Settings：
- Has Exit Time: 取消勾选（除了循环动画如 Idle/Walk）
- Transition Duration: 设置为 0.1-0.2 秒
- Interruption Source: 设置为 "Current State Then Next State"

### Q5: Boss 召唤的小怪死亡后计数不减少？
**A**: 需要在召唤物的 `Die()` 方法中调用 `BossController.OnSummonDeath()`。可以通过事件系统或直接引用实现。

---

## 文件清单

### Skeleton 文件 (8个)
```
Assets/Scripts/Character/Skeleton/
├── SkeletonController.cs
├── SkeletonIdleState.cs
├── SkeletonPatrolState.cs
├── SkeletonChaseState.cs
├── SkeletonAttackState.cs
├── SkeletonDefenseState.cs (特有)
├── SkeletonHurtState.cs
└── SkeletonDeathState.cs
```

### Boss 文件 (9个)
```
Assets/Scripts/Character/Boss/
├── BossController.cs
├── BossIdleState.cs
├── BossChaseState.cs
├── BossRangedAttackState.cs (主要攻击)
├── BossMeleeAttackState.cs
├── BossChargeState.cs (特有)
├── BossSummonState.cs (特有)
├── BossHurtState.cs
└── BossDeathState.cs
```

### Slime 文件 (7个)
```
Assets/Scripts/Character/Slime/
├── SlimeController.cs
├── SlimeIdleState.cs
├── SlimeWanderState.cs
├── SlimeChaseState.cs
├── SlimeAttackState.cs (碰撞攻击)
├── SlimeHurtState.cs (无动画版本)
└── SlimeDeathState.cs
```

### 共享文件
```
Assets/Scripts/Character/
├── CharacterBase.cs (基类)
└── [IState.cs 在 Core/StateMachine/ 中]

Assets/Animation/
├── Boss/Boss.controller
├── Skeleton/Skeleton.controller
└── Slime/Slime.controller
```

---

## 版本历史
- **v1.0** (2024): 初始实现，完成三种怪物的基础FSM
- 包含：Skeleton（7状态）、Boss（8状态）、Slime（6状态）
- 所有代码使用 UTF-8 编码，中文注释

---

## 鸣谢
本实现参考了 Unity 2D Roguelike 项目和地牢生成器项目的设计模式。

