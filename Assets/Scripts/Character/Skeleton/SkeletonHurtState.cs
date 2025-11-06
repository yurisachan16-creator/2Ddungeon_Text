using UnityEngine;

/// <summary>
/// Skeleton受伤状态
/// 播放受伤动画并短暂硬直
/// </summary>
public class SkeletonHurtState : IState
{
    private SkeletonController skeleton;
    private Animator animator;
    private float hurtDuration = 0.4f; // 受伤硬直时间
    private float hurtTimer;
    
    public SkeletonHurtState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 播放受伤动画
        animator.SetTrigger("Hurt");
        hurtTimer = 0f;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (skeleton.IsDead)
        {
            skeleton.StateMachine.ChangeState(new SkeletonDeathState(skeleton));
            return;
        }
        
        // 更新受伤计时器
        hurtTimer += Time.deltaTime;
        
        // 受伤硬直结束
        if (hurtTimer >= hurtDuration)
        {
            // 重置受伤标志
            var field = typeof(CharacterBase).GetField("isHurt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(skeleton, false);
            
            // 根据情况决定下一个状态
            if (skeleton.Target != null)
            {
                float distanceToTarget = Vector2.Distance(skeleton.transform.position, skeleton.Target.position);
                
                // 玩家在攻击范围内，进行攻击
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
            
            // 没有目标，返回巡逻状态
            skeleton.StateMachine.ChangeState(new SkeletonPatrolState(skeleton));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 受伤状态下不移动
    }
    
    public void OnExit()
    {
        // 退出受伤状态
    }
}
