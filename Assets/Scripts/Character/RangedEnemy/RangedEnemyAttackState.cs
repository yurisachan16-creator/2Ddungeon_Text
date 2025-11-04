using UnityEngine;

/// <summary>
/// 远程敌人攻击状态
/// 保持距离并发射远程攻击
/// </summary>
public class RangedEnemyAttackState : IState
{
    private CharacterBase character;
    private Animator animator;
    private Transform player;
    private float attackRange = 6f; // 攻击范围
    private float safeDistance = 4f; // 安全距离（太近会后退）
    private float attackCooldown = 2f; // 攻击冷却时间
    private float cooldownTimer = 0f; // 冷却计时器
    
    public RangedEnemyAttackState(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        cooldownTimer = 0f;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (character.IsDead)
        {
            character.StateMachine.ChangeState(new RangedEnemyDeathState(character));
            return;
        }
        
        // 检测受伤
        if (character.IsHurt)
        {
            character.StateMachine.ChangeState(new RangedEnemyHurtState(character));
            return;
        }
        
        if (player == null)
        {
            character.StateMachine.ChangeState(new RangedEnemyIdleState(character));
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(character.transform.position, player.position);
        
        // 玩家离开检测范围，返回空闲
        if (distanceToPlayer > attackRange + 2f)
        {
            character.StateMachine.ChangeState(new RangedEnemyIdleState(character));
            return;
        }
        
        // 面向玩家
        Vector2 direction = (player.position - character.transform.position).normalized;
        if (direction.x != 0)
        {
            character.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
        
        // 更新冷却计时器
        cooldownTimer += Time.deltaTime;
        
        // 攻击冷却结束，发射攻击
        if (cooldownTimer >= attackCooldown)
        {
            PerformRangedAttack();
            cooldownTimer = 0f;
        }
        
        // 玩家太近，保持安全距离（可选逻辑）
        if (distanceToPlayer < safeDistance)
        {
            // 可以添加后退逻辑
        }
    }
    
    public void OnFixedUpdate()
    {
        // 可以在这里添加保持距离的移动逻辑
    }
    
    public void OnExit()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }
    }
    
    /// <summary>
    /// 执行远程攻击
    /// </summary>
    private void PerformRangedAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // 这里应该生成投射物
        // 示例：Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        Debug.Log($"{character.name} 发射了远程攻击！");
    }
}
