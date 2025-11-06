using UnityEngine;

/// <summary>
/// Boss空闲状态
/// Boss悬浮在空中，侦测玩家并决定下一步行动
/// </summary>
public class BossIdleState : IState
{
    private BossController boss;
    private Animator animator;
    private float idleTimer;
    
    public BossIdleState(BossController boss)
    {
        this.boss = boss;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放空闲动画（悬浮效果）
        animator.SetFloat("Speed", 0f);
        idleTimer = 0f;
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
        
        idleTimer += Time.deltaTime;
        
        // 检测玩家
        if (boss.Target != null)
        {
            float distanceToTarget = Vector2.Distance(boss.transform.position, boss.Target.position);
            
            // 决定使用哪种攻击方式
            if (distanceToTarget <= boss.MeleeAttackRange && boss.MeleeAttackTimer <= 0)
            {
                // 近战范围内，使用近战攻击
                boss.StateMachine.ChangeState(new BossMeleeAttackState(boss));
                return;
            }
            else if (distanceToTarget <= boss.RangedAttackRange && boss.RangedAttackTimer <= 0)
            {
                // 远程范围内，使用远程攻击
                boss.StateMachine.ChangeState(new BossRangedAttackState(boss));
                return;
            }
            else if (boss.SummonTimer <= 0 && boss.CurrentSummonCount < boss.MaxSummonCount && idleTimer > 2f)
            {
                // 召唤技能可用，召唤小怪
                boss.StateMachine.ChangeState(new BossSummonState(boss));
                return;
            }
            else if (boss.ChargeTimer <= 0 && distanceToTarget > boss.MeleeAttackRange && idleTimer > 3f)
            {
                // 冲锋技能可用，向玩家冲锋
                boss.StateMachine.ChangeState(new BossChargeState(boss));
                return;
            }
            else if (distanceToTarget > boss.RangedAttackRange)
            {
                // 玩家太远，追击
                boss.StateMachine.ChangeState(new BossChaseState(boss));
                return;
            }
        }
    }
    
    public void OnFixedUpdate()
    {
        // 空闲状态可以有轻微的悬浮效果（可选）
    }
    
    public void OnExit()
    {
        // 退出空闲状态
    }
}
