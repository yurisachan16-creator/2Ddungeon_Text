using UnityEngine;

/// <summary>
/// Skeleton（骷髅）怪物控制器
/// 继承CharacterBase，实现近战骷髅敌人的AI逻辑
/// 拥有巡逻、追击、攻击、防御等行为
/// </summary>
public class SkeletonController : CharacterBase
{
    [Header("骷髅特殊属性")]
    [SerializeField] private float detectionRange = 5f; // 侦测范围
    [SerializeField] private float attackRange = 1.5f; // 攻击范围
    [SerializeField] private float patrolSpeed = 1f; // 巡逻速度
    [SerializeField] private float chaseSpeed = 2.5f; // 追击速度
    [SerializeField] private LayerMask playerLayer; // 玩家图层
    [SerializeField] private Transform attackPoint; // 攻击检测点
    
    [Header("巡逻设置")]
    [SerializeField] private float patrolWaitTime = 2f; // 巡逻停留时间
    [SerializeField] private float patrolRadius = 3f; // 巡逻半径
    
    [Header("防御设置")]
    [SerializeField] private float defenseChance = 0.3f; // 防御触发概率（30%）
    [SerializeField] private float defenseDuration = 2f; // 防御持续时间
    
    // 公共属性，供状态机访问
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public LayerMask PlayerLayer => playerLayer;
    public Transform AttackPoint => attackPoint;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolRadius => patrolRadius;
    public float DefenseChance => defenseChance;
    public float DefenseDuration => defenseDuration;
    
    // 状态标志
    public bool IsDefending { get; private set; }
    public Transform Target { get; private set; } // 追击目标
    
    protected override void Awake()
    {
        base.Awake();
        
        // 设置角色类型
        characterType = CharacterType.Melee;
        characterName = "Skeleton";
    }
    
    protected override void Start()
    {
        base.Start();
        
        // 初始化状态机，设置初始状态为巡逻状态
        stateMachine.ChangeState(new SkeletonPatrolState(this));
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 检测玩家
        DetectPlayer();
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
        
        // 有一定概率进入防御状态而不是受伤状态
        if (!isDead && !IsDefending)
        {
            if (Random.value < defenseChance)
            {
                stateMachine.ChangeState(new SkeletonDefenseState(this));
            }
            else
            {
                stateMachine.ChangeState(new SkeletonHurtState(this));
            }
        }
    }
    
    /// <summary>
    /// 死亡时的处理
    /// </summary>
    protected override void Die()
    {
        base.Die();
        
        // 切换到死亡状态
        stateMachine.ChangeState(new SkeletonDeathState(this));
    }
    
    /// <summary>
    /// 设置防御状态
    /// </summary>
    public void SetDefending(bool defending)
    {
        IsDefending = defending;
    }
    
    /// <summary>
    /// 执行攻击检测
    /// 由动画事件调用
    /// </summary>
    public void PerformAttack()
    {
        if (attackPoint == null || isDead)
        {
            return;
        }
        
        // 检测攻击范围内的玩家
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        
        foreach (Collider2D player in hitPlayers)
        {
            CharacterBase playerCharacter = player.GetComponent<CharacterBase>();
            if (playerCharacter != null && !playerCharacter.IsDead)
            {
                playerCharacter.TakeDamage(attackDamage);
            }
        }
    }
    
    /// <summary>
    /// 获取随机巡逻点
    /// </summary>
    public Vector2 GetRandomPatrolPoint()
    {
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
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
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        
        // 绘制巡逻范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
    
    /// <summary>
    /// 重置骷髅状态
    /// </summary>
    public override void ResetCharacter()
    {
        base.ResetCharacter();
        
        IsDefending = false;
        Target = null;
        
        // 重置到巡逻状态
        stateMachine.ChangeState(new SkeletonPatrolState(this));
    }
}
