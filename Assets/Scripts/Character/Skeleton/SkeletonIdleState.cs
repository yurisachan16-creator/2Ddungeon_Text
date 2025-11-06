using UnityEngine;

/// <summary>
/// Skeleton空闲状态
/// 在侦测到玩家前保持静止
/// </summary>
public class SkeletonIdleState : IState
{
    private SkeletonController skeleton;
    private Animator animator;
    private float idleTimer;
    private float maxIdleTime = 3f; // 最大空闲时间，之后切换到巡逻
    
    public SkeletonIdleState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 播放空闲动画
        animator.SetFloat("Speed", 0f);
        idleTimer = 0f;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (skeleton.IsDead)
        {
            skeleton.StateMachine.ChangeState(new SkeletonDeathState(skeleton));
            return;
        }
        
        // 检测受伤
        if (skeleton.IsHurt)
        {
            skeleton.StateMachine.ChangeState(new SkeletonHurtState(skeleton));
            return;
        }
        
        // 检测玩家
        if (skeleton.Target != null)
        {
            float distanceToTarget = Vector2.Distance(skeleton.transform.position, skeleton.Target.position);
            
            // 玩家在攻击范围内，直接攻击
            if (distanceToTarget <= skeleton.AttackRange)
            {
                skeleton.StateMachine.ChangeState(new SkeletonAttackState(skeleton));
                return;
            }
            
            // 玩家在侦测范围内，开始追击
            if (distanceToTarget <= skeleton.DetectionRange)
            {
                skeleton.StateMachine.ChangeState(new SkeletonChaseState(skeleton));
                return;
            }
        }
        
        // 空闲时间过长，切换到巡逻状态
        idleTimer += Time.deltaTime;
        if (idleTimer >= maxIdleTime)
        {
            skeleton.StateMachine.ChangeState(new SkeletonPatrolState(skeleton));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 空闲状态无需物理更新
    }
    
    public void OnExit()
    {
        // 退出空闲状态
    }
}
