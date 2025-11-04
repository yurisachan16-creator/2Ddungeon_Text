using UnityEngine;

/// <summary>
/// 远程敌人控制器
/// 展示如何使用状态机框架创建远程敌人
/// </summary>
public class RangedEnemyController : CharacterBase
{
    [Header("远程敌人特殊属性")]
    [SerializeField] private GameObject projectilePrefab; // 投射物预制体
    [SerializeField] private Transform firePoint; // 发射点
    [SerializeField] private float detectionRange = 8f; // 检测范围
    [SerializeField] private float attackRange = 6f; // 攻击范围
    
    protected override void Awake()
    {
        base.Awake();
        
        characterType = CharacterType.Ranged;
        characterName = "RangedEnemy";
    }
    
    protected override void Start()
    {
        // 初始化状态机，设置初始状态为空闲状态
        stateMachine.ChangeState(new RangedEnemyIdleState(this, detectionRange));
    }
    
    protected override void OnHurt()
    {
        base.OnHurt();
        
        if (!isDead)
        {
            stateMachine.ChangeState(new RangedEnemyHurtState(this));
        }
    }
    
    protected override void Die()
    {
        base.Die();
        
        stateMachine.ChangeState(new RangedEnemyDeathState(this));
    }
    
    /// <summary>
    /// 发射投射物
    /// 由动画事件或状态调用
    /// </summary>
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("投射物预制体或发射点未设置！");
            return;
        }
        
        // 实例化投射物
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // 设置投射物方向（朝向玩家）
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - firePoint.position).normalized;
            // 这里应该设置投射物的速度或方向
            // 例如：projectile.GetComponent<Projectile>().SetDirection(direction);
        }
    }
}
