using UnityEngine;

/// <summary>
/// Knight角色控制器
/// 继承CharacterBase，实现Knight特有的逻辑
/// </summary>
public class KnightController : CharacterBase
{
    [Header("Knight特殊属性")]
    [SerializeField] private float attackRange = 1.5f; // 攻击范围
    [SerializeField] private LayerMask enemyLayer; // 敌人图层
    [SerializeField] private Transform attackPoint; // 攻击检测点
    
    protected override void Awake()
    {
        base.Awake();
        
        // 设置角色类型
        characterType = CharacterType.Player;
        characterName = "Knight";
    }
    
    protected override void Start()
    {
        // 初始化状态机，设置初始状态为空闲状态
        stateMachine.ChangeState(new KnightIdleState(this));
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 可以在这里添加Knight特有的更新逻辑
        // 例如：技能冷却、Buff管理等
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    
    /// <summary>
    /// 受伤时的处理
    /// </summary>
    protected override void OnHurt()
    {
        base.OnHurt();
        
        // 切换到受伤状态
        if (!isDead)
        {
            stateMachine.ChangeState(new KnightHurtState(this));
        }
    }
    
    /// <summary>
    /// 死亡时的处理
    /// </summary>
    protected override void Die()
    {
        base.Die();
        
        // 切换到死亡状态
        stateMachine.ChangeState(new KnightDeathState(this));
    }
    
    /// <summary>
    /// 执行攻击检测
    /// 由动画事件调用
    /// </summary>
    public void PerformAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("攻击点未设置！");
            return;
        }
        
        // 检测攻击范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            CharacterBase enemyCharacter = enemy.GetComponent<CharacterBase>();
            if (enemyCharacter != null)
            {
                enemyCharacter.TakeDamage(attackDamage);
                Debug.Log($"攻击了 {enemyCharacter.name}，造成 {attackDamage} 点伤害");
            }
        }
    }
    
    /// <summary>
    /// 绘制攻击范围（仅在编辑器中显示）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    
    /// <summary>
    /// 重置Knight状态
    /// </summary>
    public override void ResetCharacter()
    {
        base.ResetCharacter();
        
        // 重置到空闲状态
        stateMachine.ChangeState(new KnightIdleState(this));
    }
}
