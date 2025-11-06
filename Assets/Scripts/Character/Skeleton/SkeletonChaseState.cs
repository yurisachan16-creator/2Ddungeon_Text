using UnityEngine;

/// <summary>
/// Skeleton追击状态
/// 追击玩家，尝试接近到攻击范围
/// </summary>
public class SkeletonChaseState : IState
{
    private SkeletonController skeleton;
    private Rigidbody2D rb;
    private Animator animator;
    
    public SkeletonChaseState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.rb = skeleton.Rb;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 播放行走动画（追击速度更快）
        animator.SetFloat("Speed", 1f);
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
        
        // 检测目标是否存在
        if (skeleton.Target == null)
        {
            // 目标丢失，返回巡逻状态
            skeleton.StateMachine.ChangeState(new SkeletonPatrolState(skeleton));
            return;
        }
        
        float distanceToTarget = Vector2.Distance(skeleton.transform.position, skeleton.Target.position);
        
        // 玩家在攻击范围内，切换到攻击状态
        if (distanceToTarget <= skeleton.AttackRange)
        {
            skeleton.StateMachine.ChangeState(new SkeletonAttackState(skeleton));
            return;
        }
        
        // 玩家超出侦测范围，返回巡逻状态
        if (distanceToTarget > skeleton.DetectionRange)
        {
            skeleton.StateMachine.ChangeState(new SkeletonPatrolState(skeleton));
            return;
        }
        
        // 根据移动方向翻转角色
        Vector2 direction = (skeleton.Target.position - skeleton.transform.position).normalized;
        if (direction.x != 0)
        {
            skeleton.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
    }
    
    public void OnFixedUpdate()
    {
        if (skeleton.Target != null && rb != null)
        {
            // 向玩家移动
            Vector2 direction = (skeleton.Target.position - skeleton.transform.position).normalized;
            rb.MovePosition(rb.position + direction * skeleton.ChaseSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出追击状态
    }
}
