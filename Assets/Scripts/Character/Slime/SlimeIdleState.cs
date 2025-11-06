using UnityEngine;

/// <summary>
/// Slime空闲状�?
/// 短暂停留后切换到游荡状�?
/// </summary>
public class SlimeIdleState : IState
{
    private SlimeController slime;
    private Animator animator;
    private float idleTimer;
    private float maxIdleTime = 2f; // 最大空闲时�?
    
    public SlimeIdleState(SlimeController slime)
    {
        this.slime = slime;
        this.animator = slime.Animator;
    }
    
    public void OnEnter()
    {
        // 播放空闲动画
        animator.SetFloat("Speed", 0f);
        idleTimer = 0f;
    }
    
    public void OnUpdate()
    {
        // 检测死�?
        if (slime.IsDead)
        {
            slime.StateMachine.ChangeState(new SlimeDeathState(slime));
            return;
        }
        
        // 检测玩�?
        if (slime.Target != null)
        {
            float distanceToTarget = Vector2.Distance(slime.transform.position, slime.Target.position);
            
            // 玩家在攻击范围内，进行攻�?
            if (distanceToTarget <= slime.AttackRange)
            {
                slime.StateMachine.ChangeState(new SlimeAttackState(slime));
                return;
            }
            
            // 玩家在侦测范围内，开始追�?
            if (distanceToTarget <= slime.DetectionRange)
            {
                slime.StateMachine.ChangeState(new SlimeChaseState(slime));
                return;
            }
        }
        
        // 空闲时间过长，切换到游荡状�?
        idleTimer += Time.deltaTime;
        if (idleTimer >= maxIdleTime)
        {
            slime.StateMachine.ChangeState(new SlimeWanderState(slime));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 空闲状态无需物理更新
    }
    
    public void OnExit()
    {
        // 退出空闲状�?
    }
}
