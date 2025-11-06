using UnityEngine;

/// <summary>
/// Slime（史莱姆）怪物控制器
/// 继承CharacterBase，实现简单的近战史莱姆敌人AI逻辑
/// 拥有游荡、追击和碰撞攻击行为
/// 注意：Slime只有3个动画（Idle、Walk、Death），没有Attack和Hurt动画
/// </summary>
public class SlimeController : CharacterBase
{
    [Header("史莱姆特殊属性")]
    [SerializeField] private float detectionRange = 4f; // 侦测范围
    [SerializeField] private float attackRange = 0.8f; // 攻击范围（碰撞距离）
    [SerializeField] private float wanderSpeed = 0.8f; // 游荡速度
    [SerializeField] private float chaseSpeed = 2f; // 追击速度
    [SerializeField] private LayerMask playerLayer; // 玩家图层
    
    [Header("游荡设置")]
    [SerializeField] private float wanderWaitTime = 2f; // 游荡停留时间
    [SerializeField] private float wanderRadius = 3f; // 游荡半径
    
    [Header("攻击设置")]
    [SerializeField] private float attackCooldown = 1.5f; // 攻击冷却时间
    
    // 公共属性，供状态机访问
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;
    public LayerMask PlayerLayer => playerLayer;
    public float WanderWaitTime => wanderWaitTime;
    public float WanderRadius => wanderRadius;
    public float AttackCooldown => attackCooldown;
    
    // 状态标�?
    public Transform Target { get; private set; } // 追击目标
    public float AttackTimer { get; set; } // 攻击冷却计时�?
    
    protected override void Awake()
    {
        base.Awake();
        
        // 设置角色类型
        characterType = CharacterType.Melee;
        characterName = "Slime";
    }
    
    protected override void Start()
    {
        base.Start();
        
        // 初始化冷却计时器
        AttackTimer = 0f;
        
        // 初始化状态机，设置初始状态为游荡状�?
        stateMachine.ChangeState(new SlimeWanderState(this));
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 更新攻击冷却
        if (AttackTimer > 0)
            AttackTimer -= Time.deltaTime;
        
        // 检测玩�?
        DetectPlayer();
    }
    
    /// <summary>
    /// 检测玩家是否在侦测范围�?
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
    /// 注意：Slime没有Hurt动画，使用Idle动画代替并快速恢�?
    /// </summary>
    protected override void OnHurt()
    {
        base.OnHurt();
        
        // Slime没有单独的受伤状态，直接在当前状态处�?
        // 使用Idle状态短暂替代受伤效�?
        if (!isDead)
        {
            stateMachine.ChangeState(new SlimeHurtState(this));
        }
    }
    
    /// <summary>
    /// 死亡时的处理
    /// </summary>
    protected override void Die()
    {
        base.Die();
        
        // 切换到死亡状�?
        stateMachine.ChangeState(new SlimeDeathState(this));
    }
    
    /// <summary>
    /// 执行碰撞攻击
    /// Slime通过碰撞造成伤害，没有单独的攻击动画
    /// </summary>
    public void PerformAttack(CharacterBase target)
    {
        if (target == null || isDead || AttackTimer > 0)
        {
            return;
        }
        
        // 造成伤害
        target.TakeDamage(attackDamage);
        
        // 设置攻击冷却
        AttackTimer = attackCooldown;
    }
    
    /// <summary>
    /// 获取随机游荡�?
    /// </summary>
    public Vector2 GetRandomWanderPoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        return (Vector2)transform.position + randomDirection;
    }
    
    /// <summary>
    /// 绘制检测范围和攻击范围（仅在编辑器中显示）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 绘制侦测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 绘制游荡范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
    
    /// <summary>
    /// 重置史莱姆状�?
    /// </summary>
    public override void ResetCharacter()
    {
        base.ResetCharacter();
        
        Target = null;
        AttackTimer = 0f;
        
        // 重置到游荡状�?
        stateMachine.ChangeState(new SlimeWanderState(this));
    }
}
