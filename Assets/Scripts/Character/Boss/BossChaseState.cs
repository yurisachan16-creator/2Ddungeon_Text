using UnityEngine;

/// <summary>
/// Boss追击状态
/// Boss飞行追击玩家，尝试进入攻击范围
/// </summary>
public class BossChaseState : IState
{
    private BossController boss;
    private Rigidbody2D rb;
    private Animator animator;
    
    public BossChaseState(BossController boss)
    {
        this.boss = boss;
        this.rb = boss.Rb;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放飞行动画
        animator.SetTrigger("Fly");
        animator.SetFloat("Speed", 1f);
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (boss.IsDead)
        {
            boss.StateMachine.ChangeState(new BossDeathState(boss));
            return;
        }
        
        // 检测受伤
        if (boss.IsHurt)
        {
            boss.StateMachine.ChangeState(new BossHurtState(boss));
            return;
        }
        
        // 检测目标是否存在
        if (boss.Target == null)
        {
            // 目标丢失，返回空闲状态
            boss.StateMachine.ChangeState(new BossIdleState(boss));
            return;
        }
        
        float distanceToTarget = Vector2.Distance(boss.transform.position, boss.Target.position);
        
        // 进入攻击范围，选择攻击方式
        if (distanceToTarget <= boss.MeleeAttackRange && boss.MeleeAttackTimer <= 0)
        {
            boss.StateMachine.ChangeState(new BossMeleeAttackState(boss));
            return;
        }
        else if (distanceToTarget <= boss.RangedAttackRange && boss.RangedAttackTimer <= 0)
        {
            boss.StateMachine.ChangeState(new BossRangedAttackState(boss));
            return;
        }
        
        // 玩家超出侦测范围，返回空闲状态
        if (distanceToTarget > boss.DetectionRange)
        {
            boss.StateMachine.ChangeState(new BossIdleState(boss));
            return;
        }
        
        // 根据移动方向翻转角色
        Vector2 direction = (boss.Target.position - boss.transform.position).normalized;
        if (direction.x != 0)
        {
            boss.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
    }
    
    public void OnFixedUpdate()
    {
        if (boss.Target != null && rb != null)
        {
            // 飞向玩家
            Vector2 direction = (boss.Target.position - boss.transform.position).normalized;
            rb.MovePosition(rb.position + direction * boss.ChaseSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出追击状态
    }
}
