# 角色状态机系统文档

## 概述
本项目实现了一个通用的有限状态机（FSM）系统，用于管理不同类型角色的状态和行为。系统采用面向对象设计，支持快速扩展新角色类型。

## 目录结构

```
Assets/Scripts/
├── StateMachine/              # 状态机核心框架
│   ├── IState.cs             # 状态接口
│   └── StateMachine.cs       # 状态机管理类
│
├── Character/                 # 角色系统
│   ├── CharacterBase.cs      # 角色基类
│   │
│   ├── Knight/               # Knight角色（近战玩家）
│   │   ├── KnightController.cs
│   │   ├── KnightIdleState.cs
│   │   ├── KnightMoveState.cs
│   │   ├── KnightAttack01State.cs
│   │   ├── KnightAttack02State.cs
│   │   ├── KnightHurtState.cs
│   │   └── KnightDeathState.cs
│   │
│   └── RangedEnemy/          # 远程敌人（示例）
│       ├── RangedEnemyController.cs
│       ├── RangedEnemyIdleState.cs
│       ├── RangedEnemyAttackState.cs
│       ├── RangedEnemyHurtState.cs
│       └── RangedEnemyDeathState.cs
```

## 核心组件

### 1. IState 接口
所有状态类必须实现此接口。

**方法说明：**
- `OnEnter()`: 进入状态时调用，用于初始化
- `OnUpdate()`: 每帧调用，处理状态逻辑
- `OnFixedUpdate()`: 物理更新时调用，处理移动等物理操作
- `OnExit()`: 退出状态时调用，用于清理

### 2. StateMachine 类
管理状态的切换和生命周期。

**主要功能：**
- 状态缓存机制，避免重复创建对象
- 安全的状态切换（自动调用OnExit和OnEnter）
- 防止切换到相同状态

**使用方法：**
```csharp
// 创建状态机
StateMachine stateMachine = new StateMachine();

// 切换状态（泛型方式）
stateMachine.ChangeState<IdleState>();

// 切换状态（实例方式）
stateMachine.ChangeState(new MoveState(this));

// 在Update中更新
stateMachine.Update();

// 在FixedUpdate中更新
stateMachine.FixedUpdate();
```

### 3. CharacterBase 抽象类
所有角色的基类，提供通用属性和方法。

**核心属性：**
- `characterType`: 角色类型（玩家/近战/远程/Boss）
- `maxHealth/currentHealth`: 生命值系统
- `moveSpeed`: 移动速度
- `attackDamage`: 攻击伤害
- `isDead/isHurt/isAttacking`: 状态标志

**核心方法：**
- `TakeDamage(float damage)`: 受到伤害
- `Heal(float healAmount)`: 治疗
- `OnHurt()`: 受伤处理（虚方法，子类重写）
- `Die()`: 死亡处理（虚方法，子类重写）
- `ResetCharacter()`: 重置角色状态

## Knight角色实现

### 状态图
```
┌─────────┐
│  Idle   │ ◄─┐
└─────────┘   │
     │        │
     ├──► Move ──┘
     │        │
     ├──► Attack01
     │        │
     ├──► Attack02
     │        │
     └──► Hurt ──┘
          │
          ▼
       Death
```

### 输入映射
- **WASD**: 移动控制（切换到MoveState）
- **鼠标左键**: 攻击1（切换到Attack01State）
- **鼠标右键**: 攻击2（切换到Attack02State）
- **受伤**: 自动切换到HurtState
- **死亡**: 自动切换到DeathState

### 各状态详解

#### KnightIdleState（空闲状态）
- **功能**: 等待输入，作为默认状态
- **转换条件**:
  - 检测到移动输入 → MoveState
  - 检测到攻击输入 → Attack01State/Attack02State
  - 受到伤害 → HurtState
  - 生命值归零 → DeathState

#### KnightMoveState（移动状态）
- **功能**: 处理角色移动和方向翻转
- **特性**:
  - 使用Rigidbody2D.MovePosition实现物理移动
  - 根据移动方向自动翻转角色
  - 更新动画参数（Speed）
- **转换条件**:
  - 停止输入 → IdleState
  - 攻击输入 → Attack01State/Attack02State

#### KnightAttack01State/Attack02State（攻击状态）
- **功能**: 播放攻击动画，执行攻击逻辑
- **特性**:
  - 攻击期间锁定移动
  - 通过计时器控制攻击持续时间
  - 支持动画事件触发伤害判定
- **攻击检测**: 使用`Physics2D.OverlapCircleAll`检测范围内敌人

#### KnightHurtState（受伤状态）
- **功能**: 播放受伤动画，提供无敌帧
- **特性**:
  - 短暂的硬直时间（0.4秒）
  - 可扩展击退效果
  - 自动返回IdleState

#### KnightDeathState（死亡状态）
- **功能**: 播放死亡动画，禁用控制
- **特性**:
  - 禁用物理模拟（`Rigidbody2D.simulated = false`）
  - 死亡动画结束后可销毁对象或触发复活

### Unity设置指南

#### 1. 创建Knight对象
1. 在场景中创建空GameObject，命名为"Knight"
2. 添加以下组件：
   - `KnightController`
   - `Rigidbody2D`（设置Gravity Scale = 0，Freeze Rotation Z）
   - `Animator`
   - `SpriteRenderer`
   - `Collider2D`（如BoxCollider2D）

#### 2. 设置攻击检测
1. 在Knight下创建子对象"AttackPoint"
2. 调整AttackPoint位置到武器前方
3. 在KnightController中分配AttackPoint引用
4. 设置Enemy Layer和Attack Range参数

#### 3. 动画控制器参数
需要在Animator Controller中创建以下参数：
- **Float**: `Speed` - 控制移动动画
- **Bool**: `IsAttacking` - 攻击状态标志
- **Trigger**: `Attack01` - 触发攻击1动画
- **Trigger**: `Attack02` - 触发攻击2动画
- **Trigger**: `Hurt` - 触发受伤动画
- **Trigger**: `Death` - 触发死亡动画

#### 4. 动画事件（可选）
在攻击动画的关键帧添加事件，调用：
- `PerformAttack()` - 执行伤害判定

## 远程敌人实现

### 特点
- **AI驱动**: 无需玩家输入，自动检测并攻击玩家
- **保持距离**: 在攻击范围内保持安全距离
- **投射物攻击**: 发射远程攻击而非近战

### 主要状态

#### RangedEnemyIdleState（空闲状态）
- **功能**: 巡逻或等待，检测玩家
- **检测范围**: 8格（可配置）
- **转换**: 发现玩家 → AttackState

#### RangedEnemyAttackState（攻击状态）
- **功能**: 保持距离并发射投射物
- **攻击冷却**: 2秒（可配置）
- **安全距离**: 4格（玩家太近会后退）

### Unity设置指南
1. 创建RangedEnemy对象，添加`RangedEnemyController`
2. 创建投射物预制体（Projectile）
3. 设置FirePoint（发射点位置）
4. 分配Projectile Prefab引用

## 扩展新角色

### 步骤1：创建状态类
为新角色创建状态类，继承`IState`接口：

```csharp
public class NewCharacterIdleState : IState
{
    private CharacterBase character;
    
    public NewCharacterIdleState(CharacterBase character)
    {
        this.character = character;
    }
    
    public void OnEnter() { }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit() { }
}
```

### 步骤2：创建控制器类
继承`CharacterBase`：

```csharp
public class NewCharacterController : CharacterBase
{
    protected override void Awake()
    {
        base.Awake();
        characterType = CharacterType.Melee; // 设置类型
        characterName = "NewCharacter";
    }
    
    protected override void Start()
    {
        // 设置初始状态
        stateMachine.ChangeState(new NewCharacterIdleState(this));
    }
    
    protected override void OnHurt()
    {
        base.OnHurt();
        // 自定义受伤逻辑
    }
    
    protected override void Die()
    {
        base.Die();
        // 自定义死亡逻辑
    }
}
```

### 步骤3：实现状态转换逻辑
在每个状态的`OnUpdate()`中检测条件并切换状态。

## 最佳实践

### 1. 状态设计原则
- **单一职责**: 每个状态只处理一种行为
- **清晰转换**: 明确定义状态间的转换条件
- **避免循环**: 防止状态A→B→A的无限循环

### 2. 性能优化
- 状态机使用缓存池，避免频繁创建对象
- 物理操作放在`OnFixedUpdate()`中
- 避免在Update中频繁查找GameObject

### 3. 调试技巧
```csharp
// 在状态切换时打印日志
public void ChangeState(IState newState)
{
    Debug.Log($"状态切换: {currentState?.GetType().Name} → {newState.GetType().Name}");
    // ... 切换逻辑
}
```

### 4. 常见问题

**Q: 状态无法切换？**
- 检查是否有死循环（如每帧都强制切换回原状态）
- 确认转换条件是否正确

**Q: 攻击无效果？**
- 确认AttackPoint已分配
- 检查敌人Layer设置是否正确
- 验证攻击范围（通过Gizmos可视化）

**Q: 动画不播放？**
- 确认Animator Controller中有对应参数
- 检查动画过渡条件
- 确保Has Exit Time设置正确

## 角色类型对比

| 特性 | Knight（近战玩家） | RangedEnemy（远程敌人） |
|------|-------------------|----------------------|
| 控制方式 | 玩家输入 | AI自动 |
| 攻击方式 | 近战范围检测 | 发射投射物 |
| 移动特点 | WASD完全控制 | AI自动追踪/保持距离 |
| 适用场景 | 玩家角色 | 敌人AI |

## 版本信息
- Unity版本: 2022.3.62f2c1
- 编码格式: UTF-8
- 语言: 简体中文

## 更新日志
- 2025-11-04: 创建初始版本，实现Knight和RangedEnemy
