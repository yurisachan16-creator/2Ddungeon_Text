using UnityEngine;

/// <summary>
/// Boss（Boss）控制器
/// 继承CharacterBase，实现远程Boss的AI逻辑
/// 拥有飞行、远程攻击、近战攻击、召唤等技能
/// </summary>
public class BossController : CharacterBase
{
    [Header("Boss特殊属性")]
    [SerializeField] private float detectionRange = 8f; // 侦测范围（比普通怪物更大）
    [SerializeField] private float meleeAttackRange = 2f; // 近战攻击范围
    [SerializeField] private float rangedAttackRange = 6f; // 远程攻击范围
    [SerializeField] private float flySpeed = 2f; // 飞行速度
    [SerializeField] private float chaseSpeed = 3f; // 追击速度
    [SerializeField] private LayerMask playerLayer; // 玩家图层
    [SerializeField] private Transform attackPoint; // 攻击检测点
    [SerializeField] private GameObject projectilePrefab; // 远程攻击弹幕预制体
    
    [Header("技能冷却")]
    [SerializeField] private float rangedAttackCooldown = 3f; // 远程攻击冷却时间
    [SerializeField] private float meleeAttackCooldown = 2f; // 近战攻击冷却时间
    [SerializeField] private float summonCooldown = 10f; // 召唤技能冷却时间
    [SerializeField] private float chargeCooldown = 5f; // 冲锋技能冷却时间
    
    [Header("召唤设置")]
    [SerializeField] private GameObject summonPrefab; // 召唤物预制体（可以是Skeleton或Slime）
    [SerializeField] private int maxSummonCount = 3; // 最大召唤数量
    [SerializeField] private float summonRadius = 3f; // 召唤范围
    
    // 公共属性，供状态机访问
    public float DetectionRange => detectionRange;
    public float MeleeAttackRange => meleeAttackRange;
    public float RangedAttackRange => rangedAttackRange;
    public float FlySpeed => flySpeed;
    public float ChaseSpeed => chaseSpeed;
    public LayerMask PlayerLayer => playerLayer;
    public Transform AttackPoint => attackPoint;
    public GameObject ProjectilePrefab => projectilePrefab;
    public GameObject SummonPrefab => summonPrefab;
    public int MaxSummonCount => maxSummonCount;
    public float SummonRadius => summonRadius;
    
    // 技能冷却计时器
    public float RangedAttackTimer { get; set; }
    public float MeleeAttackTimer { get; set; }
    public float SummonTimer { get; set; }
    public float ChargeTimer { get; set; }
    
    public float RangedAttackCooldown => rangedAttackCooldown;
    public float MeleeAttackCooldown => meleeAttackCooldown;
    public float SummonCooldown => summonCooldown;
    public float ChargeCooldown => chargeCooldown;
    
    // 状态标志
    public Transform Target { get; private set; } // 追击目标
    public int CurrentSummonCount { get; private set; } // 当前召唤物数量
    
    protected override void Awake()
    {
        base.Awake();
        
        // 设置角色类型
        characterType = CharacterType.Boss;
        characterName = "Boss";
    }
    
    protected override void Start()
    {
        base.Start();
        
        // 初始化冷却计时器
        RangedAttackTimer = 0f;
        MeleeAttackTimer = 0f;
        SummonTimer = 0f;
        ChargeTimer = 0f;
        CurrentSummonCount = 0;
        
        // 初始化状态机，设置初始状态为空闲状态
        stateMachine.ChangeState(new BossIdleState(this));
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 更新技能冷却
        UpdateCooldowns();
        
        // 检测玩家
        DetectPlayer();
    }
    
    /// <summary>
    /// 更新技能冷却计时器
    /// </summary>
    private void UpdateCooldowns()
    {
        if (RangedAttackTimer > 0)
            RangedAttackTimer -= Time.deltaTime;
        
        if (MeleeAttackTimer > 0)
            MeleeAttackTimer -= Time.deltaTime;
        
        if (SummonTimer > 0)
            SummonTimer -= Time.deltaTime;
        
        if (ChargeTimer > 0)
            ChargeTimer -= Time.deltaTime;
    }
    
    /// <summary>
    /// 检测玩家是否在侦测范围内
    /// </summary>
    private void DetectPlayer()
    {
        if (isDead) return;
        
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        
        if (playerCollider != null)
        {
            Target = playerCollider.transform;
        }
        else
        {
            Target = null;
        }
    }
    
    /// <summary>
    /// 受伤时的处理
    /// </summary>
    protected override void OnHurt()
    {
        base.OnHurt();
        
        // Boss受伤后切换到受伤状态
        if (!isDead)
        {
            stateMachine.ChangeState(new BossHurtState(this));
        }
    }
    
    /// <summary>
    /// 死亡时的处理
    /// </summary>
    protected override void Die()
    {
        base.Die();
        
        // 切换到死亡状态
        stateMachine.ChangeState(new BossDeathState(this));
    }
    
    /// <summary>
    /// 执行近战攻击检测
    /// 由动画事件调用
    /// </summary>
    public void PerformMeleeAttack()
    {
        if (attackPoint == null || isDead)
        {
            return;
        }
        
        // 检测近战范围内的玩家
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, meleeAttackRange, playerLayer);
        
        foreach (Collider2D player in hitPlayers)
        {
            CharacterBase playerCharacter = player.GetComponent<CharacterBase>();
            if (playerCharacter != null && !playerCharacter.IsDead)
            {
                playerCharacter.TakeDamage(attackDamage * 1.5f); // Boss近战伤害更高
            }
        }
    }
    
    /// <summary>
    /// 发射远程弹幕
    /// 由动画事件或状态调用
    /// </summary>
    public void FireProjectile()
    {
        if (projectilePrefab == null || Target == null || isDead)
        {
            return;
        }
        
        // 在攻击点生成弹幕
        Vector2 direction = (Target.position - attackPoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        
        // 设置弹幕方向和伤害（需要弹幕脚本配合）
        // 这里假设弹幕有 Projectile 组件
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Initialize(direction, attackDamage, playerLayer);
        }
    }
    
    /// <summary>
    /// 召唤小怪
    /// </summary>
    public void PerformSummon()
    {
        if (summonPrefab == null || CurrentSummonCount >= maxSummonCount)
        {
            return;
        }
        
        // 在Boss周围随机位置召唤小怪
        for (int i = 0; i < 2; i++) // 每次召唤2只
        {
            if (CurrentSummonCount >= maxSummonCount)
                break;
            
            Vector2 summonPos = (Vector2)transform.position + Random.insideUnitCircle * summonRadius;
            GameObject summon = Instantiate(summonPrefab, summonPos, Quaternion.identity);
            
            // 监听召唤物死亡，减少计数
            CharacterBase summonCharacter = summon.GetComponent<CharacterBase>();
            if (summonCharacter != null)
            {
                CurrentSummonCount++;
                // 这里需要实现死亡回调机制
            }
        }
    }
    
    /// <summary>
    /// 召唤物死亡回调
    /// </summary>
    public void OnSummonDeath()
    {
        CurrentSummonCount = Mathf.Max(0, CurrentSummonCount - 1);
    }
    
    /// <summary>
    /// 绘制检测范围和攻击范围（仅在编辑器中显示）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 绘制侦测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // 绘制近战范围
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, meleeAttackRange);
            
            // 绘制远程范围
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, rangedAttackRange);
        }
        
        // 绘制召唤范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, summonRadius);
    }
    
    /// <summary>
    /// 重置Boss状态
    /// </summary>
    public override void ResetCharacter()
    {
        base.ResetCharacter();
        
        Target = null;
        CurrentSummonCount = 0;
        RangedAttackTimer = 0f;
        MeleeAttackTimer = 0f;
        SummonTimer = 0f;
        ChargeTimer = 0f;
        
        // 重置到空闲状态
        stateMachine.ChangeState(new BossIdleState(this));
    }
}

/// <summary>
/// 弹幕脚本（简单实现）
/// 需要单独创建完整的 Projectile.cs 文件
/// </summary>
public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float damage;
    private float speed = 5f;
    private LayerMask targetLayer;
    
    public void Initialize(Vector2 dir, float dmg, LayerMask layer)
    {
        direction = dir;
        damage = dmg;
        targetLayer = layer;
        
        // 2秒后自动销毁
        Destroy(gameObject, 2f);
    }
    
    private void Update()
    {
        // 沿方向移动
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检测是否击中目标
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            CharacterBase character = collision.GetComponent<CharacterBase>();
            if (character != null && !character.IsDead)
            {
                character.TakeDamage(damage);
            }
            
            // 销毁弹幕
            Destroy(gameObject);
        }
    }
}
