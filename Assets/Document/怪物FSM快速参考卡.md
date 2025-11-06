# 怪物 FSM 快速参考卡

## 三种怪物对比速查表

| 特性 | Skeleton（骷髅） | Boss（Boss） | Slime（史莱姆） |
|------|-----------------|-------------|----------------|
| **角色类型** | Melee（近战） | Boss | Melee（近战） |
| **动画数量** | 7个 | 5个 | **3个** |
| **状态数量** | 7个状态 | 8个状态 | 6个状态 |
| **血量** | 100 | 500 | 50 |
| **攻击伤害** | 15 | 30 | 10 |
| **侦测范围** | 5米 | 8米 | 4米 |
| **移动速度** | 1-2.5 | 2-3 | 0.8-2 |
| **特殊技能** | 防御格挡（30%） | 远程弹幕、召唤、冲锋 | 碰撞攻击 |
| **难度定位** | ★★☆ 中级 | ★★★ Boss | ★☆☆ 初级 |

---

## 动画资源速查

### ✅ Skeleton（完整动画）
```
✓ Idle    - 空闲站立
✓ Walk    - 行走移动
✓ Attack  - 挥剑攻击
✓ Defense - 举盾防御 ⭐特有
✓ Hurt    - 受伤反应
✓ Death   - 死亡倒地
```

### ⚠️ Boss（缺少Walk）
```
✓ Idle   - 悬浮静止
✓ Fly    - 飞行移动 ⭐代替Walk
✓ Attack - 攻击动画（远程/近战共用）
✓ Hurt   - 受伤反应
✓ Death  - 死亡动画
```

### ❌ Slime（极简动画）
```
✓ Idle  - 空闲跳动
✓ Walk  - 移动跳跃
✗ Attack - 无！使用Walk代替
✗ Hurt   - 无！使用Idle+颜色闪烁
✓ Death - 死亡消失
```

---

## 状态机架构速查

### Skeleton 状态流转
```
Idle ──2秒──> Patrol ──检测玩家──> Chase ──接近──> Attack
                                                      ↓
                                                  继续攻击
受伤 ──30%──> Defense ──2秒后──> 返回战斗
     └70%──> Hurt ──0.4秒后──> 返回战斗
```

### Boss 状态流转（技能选择）
```
Idle ──智能判断──> 选择技能:
  ├─ 玩家<2米 + CD好  → Melee Attack (1.5倍伤害)
  ├─ 玩家<6米 + CD好  → Ranged Attack (弹幕)
  ├─ 召唤CD好 + <3只  → Summon (召唤2只)
  ├─ 冲锋CD好 + >2米  → Charge (2倍伤害)
  └─ 玩家6-8米       → Chase (飞行追击)
```

### Slime 状态流转
```
Idle ──2秒──> Wander ──检测玩家──> Chase ──接近──> Attack
                                                      ↓
                                                  持续碰撞
受伤 ──> Hurt(Idle+红色) ──0.2秒──> 返回战斗
```

---

## Animator 参数设置速查

### Skeleton Controller
```csharp
Parameters:
  Speed (Float)     - 0=Idle, 1=Walk
  Attack (Trigger)  - 触发攻击
  Defense (Trigger) - 触发防御 ⭐特有
  Hurt (Trigger)    - 触发受伤
  Death (Trigger)   - 触发死亡
```

### Boss Controller
```csharp
Parameters:
  Speed (Float)    - 0=Idle, 1=NormalFly, 2=FastFly
  Attack (Trigger) - 触发攻击（远程/近战共用）
  Fly (Trigger)    - 触发飞行 ⭐特有
  Hurt (Trigger)   - 触发受伤
  Death (Trigger)  - 触发死亡
```

### Slime Controller
```csharp
Parameters:
  Speed (Float)    - 0=Idle, >0=Walk
  Death (Trigger)  - 触发死亡
  ⚠️ 无 Attack 和 Hurt 参数！
```

---

## 关键代码速查

### 1. 创建控制器实例
```csharp
// Skeleton
var skeleton = gameObject.AddComponent<SkeletonController>();
skeleton.StateMachine.ChangeState(new SkeletonPatrolState(skeleton));

// Boss
var boss = gameObject.AddComponent<BossController>();
boss.StateMachine.ChangeState(new BossIdleState(boss));

// Slime
var slime = gameObject.AddComponent<SlimeController>();
slime.StateMachine.ChangeState(new SlimeWanderState(slime));
```

### 2. 状态类模板
```csharp
public class XxxState : IState
{
    private XxxController controller;
    private Animator animator;
    
    public XxxState(XxxController controller)
    {
        this.controller = controller;
        this.animator = controller.Animator;
    }
    
    public void OnEnter()
    {
        // 进入状态：设置动画
        animator.SetFloat("Speed", 1f);
    }
    
    public void OnUpdate()
    {
        // 每帧更新：检测条件，切换状态
        if (controller.IsDead)
        {
            controller.StateMachine.ChangeState(new XxxDeathState(controller));
            return;
        }
    }
    
    public void OnFixedUpdate()
    {
        // 物理更新：移动角色
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
    
    public void OnExit()
    {
        // 退出状态：清理工作
    }
}
```

### 3. 伤害检测模式
```csharp
// 方式1：范围检测（Skeleton, Boss近战）
Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);
foreach (var hit in hits)
{
    hit.GetComponent<CharacterBase>()?.TakeDamage(damage);
}

// 方式2：弹幕碰撞（Boss远程）
void OnTriggerEnter2D(Collider2D collision)
{
    collision.GetComponent<CharacterBase>()?.TakeDamage(damage);
    Destroy(gameObject);
}

// 方式3：直接调用（Slime）
slime.PerformAttack(targetCharacter); // 内部有冷却判断
```

---

## Inspector 必填项速查

### 所有怪物共同项
```
[必需组件]
✓ Rigidbody2D (Gravity Scale=0, Freeze Rotation Z)
✓ Collider2D (Circle/Box)
✓ Animator (设置对应的Controller)
✓ SpriteRenderer

[CharacterBase]
✓ Max Health
✓ Move Speed
✓ Attack Damage

[图层设置]
✓ Player Layer: 选择 "Player"
```

### Skeleton 专属
```
✓ Attack Point (Transform)
✓ Detection Range: 5
✓ Attack Range: 1.5
✓ Defense Chance: 0.3
```

### Boss 专属
```
✓ Attack Point (Transform)
✓ Projectile Prefab (GameObject) ⚠️重要
✓ Summon Prefab (GameObject) ⚠️重要
✓ Detection Range: 8
✓ Ranged Attack Range: 6
```

### Slime 专属
```
✓ Detection Range: 4
✓ Attack Range: 0.8
✓ Wander Radius: 3
```

---

## 常见错误速查

| 错误现象 | 可能原因 | 解决方案 |
|---------|---------|---------|
| 怪物不移动 | Rigidbody2D 未设置 | Gravity Scale=0, Body Type=Dynamic |
| 动画不播放 | Animator 参数名错误 | 检查参数名拼写（区分大小写） |
| 攻击无效果 | Attack Point 未设置 | 创建子对象并拖入 Inspector |
| Boss 不发射弹幕 | Projectile Prefab 为空 | 创建弹幕预制体并设置 Projectile 脚本 |
| Slime 卡在墙里 | Collider 尺寸过大 | 调整 Collider Radius 或添加物理材质 |
| 状态切换卡顿 | Transition Has Exit Time | 取消勾选，设置 Duration=0.1 |
| Boss 召唤无限怪 | MaxSummonCount 检测失效 | 确保召唤物死亡调用 OnSummonDeath() |
| 怪物自相残杀 | Layer Mask 设置错误 | PlayerLayer 仅选择 "Player" |

---

## 性能优化速查

### ✓ 推荐做法
```csharp
// 1. 缓存组件引用
private Rigidbody2D rb;
void Awake() { rb = GetComponent<Rigidbody2D>(); }

// 2. 使用 sqrMagnitude 避免开方
float sqrDistance = (target.position - transform.position).sqrMagnitude;
if (sqrDistance < detectionRange * detectionRange) { ... }

// 3. 限制侦测频率
float detectTimer = 0f;
if (detectTimer <= 0) {
    DetectPlayer();
    detectTimer = 0.2f; // 每0.2秒检测一次
}
```

### ✗ 避免做法
```csharp
// 1. 避免每帧 GetComponent
❌ GetComponent<Animator>().SetFloat("Speed", 1f);

// 2. 避免过大的侦测范围
❌ detectionRange = 50f; // 太大！建议4-8米

// 3. 避免频繁 Instantiate
❌ 每帧生成弹幕 // 应该使用对象池
```

---

## 调试命令速查

### Scene 视图可视化（Gizmos）
```csharp
// 在 OnDrawGizmosSelected() 中：
Gizmos.color = Color.yellow;
Gizmos.DrawWireSphere(transform.position, detectionRange); // 侦测范围

Gizmos.color = Color.red;
Gizmos.DrawWireSphere(attackPoint.position, attackRange); // 攻击范围
```

### 控制台日志
```csharp
// 状态切换日志
Debug.Log($"[{name}] 切换到状态: {this.GetType().Name}");

// 伤害日志
Debug.Log($"[{name}] 造成 {damage} 点伤害给 {target.name}");

// AI决策日志
Debug.Log($"[{name}] 检测到玩家，距离: {distance:F2}米");
```

### 运行时修改参数（测试用）
```csharp
// 在 Inspector 中勾选 Debug 模式，可以看到所有 public 属性
// 运行时可以直接修改 detectionRange、attackDamage 等
```

---

## 快速测试清单

### Skeleton 测试
- [ ] 巡逻：在巡逻范围内随机移动
- [ ] 追击：检测到玩家后追击
- [ ] 攻击：接近后挥剑攻击
- [ ] 防御：受伤时30%概率举盾
- [ ] 死亡：血量归零播放死亡动画

### Boss 测试
- [ ] 空闲：悬浮在空中
- [ ] 追击：飞向玩家
- [ ] 远程攻击：发射弹幕（冷却3秒）
- [ ] 近战攻击：玩家近身时使用（冷却2秒）
- [ ] 召唤：召唤2只小怪（最多3只）
- [ ] 冲锋：快速冲向玩家（冷却5秒）
- [ ] 死亡：播放长动画（2.5秒）

### Slime 测试
- [ ] 游荡：随机移动并停留
- [ ] 追击：检测到玩家后追击
- [ ] 攻击：接近玩家持续造成伤害（冷却1.5秒）
- [ ] 受伤：变红0.2秒后恢复
- [ ] 死亡：播放死亡动画（1秒）

---

## 扩展建议速查

### Skeleton 扩展
```csharp
// 1. 格挡反击
void OnExitDefenseState() {
    if (Target != null && Vector2.Distance(...) < attackRange) {
        StateMachine.ChangeState(new SkeletonAttackState(this));
    }
}

// 2. 巡逻路径点
public Transform[] patrolPoints; // 在 Inspector 中设置
int currentPointIndex = 0;
Vector2 patrolTarget = patrolPoints[currentPointIndex].position;
```

### Boss 扩展
```csharp
// 1. 阶段血量机制
void Update() {
    float healthPercent = currentHealth / maxHealth;
    if (healthPercent < 0.5f) {
        summonCooldown = 5f; // 血量<50%时召唤冷却减半
    }
}

// 2. 弹幕模式（扇形射击）
void FireProjectileSpread(int count = 5) {
    float angleStep = 30f;
    float startAngle = -60f;
    for (int i = 0; i < count; i++) {
        float angle = startAngle + angleStep * i;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        // 生成弹幕...
    }
}
```

### Slime 扩展
```csharp
// 1. 分裂机制
void Die() {
    if (transform.localScale.x > 0.5f) { // 不是最小体型
        for (int i = 0; i < 2; i++) {
            var child = Instantiate(slimePrefab, transform.position, Quaternion.identity);
            child.transform.localScale = transform.localScale * 0.6f;
            child.GetComponent<SlimeController>().maxHealth = maxHealth * 0.5f;
        }
    }
    base.Die();
}

// 2. 跳跃攻击
public void JumpAttack() {
    rb.AddForce(Vector2.up * jumpForce + direction * jumpDistance, ForceMode2D.Impulse);
}
```

---

## 文件路径速查

```
项目根目录/
├── Assets/
│   ├── Scripts/
│   │   └── Character/
│   │       ├── CharacterBase.cs ⭐基类
│   │       ├── Skeleton/
│   │       │   ├── SkeletonController.cs
│   │       │   └── (7个状态类)
│   │       ├── Boss/
│   │       │   ├── BossController.cs
│   │       │   └── (8个状态类)
│   │       └── Slime/
│   │           ├── SlimeController.cs
│   │           └── (6个状态类)
│   │
│   ├── Animation/
│   │   ├── Boss/Boss.controller
│   │   ├── Skeleton/Skeleton.controller
│   │   └── Slime/Slime.controller
│   │
│   └── Document/
│       ├── 怪物FSM实现总结.md ⭐完整文档
│       └── 怪物FSM快速参考卡.md ⭐本文档
│
└── ProjectSettings/
    └── TagManager.asset (配置 "Player" 图层)
```

---

## 联系方式 & 支持

- **文档版本**: v1.0
- **Unity 版本**: 2022.3.62f2c1
- **更新日期**: 2024

**遇到问题？**
1. 查看 `怪物FSM实现总结.md` 获取详细说明
2. 检查本文档的"常见错误速查"部分
3. 在 Scene 视图中启用 Gizmos 查看侦测范围

---

**Happy Coding! 🎮**
