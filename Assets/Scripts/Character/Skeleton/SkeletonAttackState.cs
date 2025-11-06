using UnityEngine;

/// <summary>
/// Skeleton攻击状态
/// 执行近战攻击动画和伤害判定
/// </summary>
public class SkeletonAttackState : IState
{
    private SkeletonController skeleton;
    private Animator animator;
    private float attackDuration = 0.6f; // 攻击动画持续时间
    private float attackTimer;
    private bool hasAttacked; // 是否已执行攻击判定
    
    public SkeletonAttackState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 播放攻击动画
        animator.SetTrigger("Attack");
        attackTimer = 0f;
        hasAttacked = false;
        
        // 面向目标
        if (skeleton.Target != null)
        {
            Vector2 direction = skeleton.Target.position - skeleton.transform.position;
            if (direction.x != 0)
            {
                skeleton.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }
        }
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (skeleton.IsDead)
        {
            skeleton.StateMachine.ChangeState(new SkeletonDeathState(skeleton));
            return;
        }
        
        // 更新攻击计时器
        attackTimer += Time.deltaTime;
        
        // 在攻击动画中间执行伤害判定（约 0.3 秒时）
        if (!hasAttacked && attackTimer >= attackDuration * 0.5f)
        {
            skeleton.PerformAttack();
            hasAttacked = true;
        }
        
        // 攻击动画结束
        if (attackTimer >= attackDuration)
        {
            // 检查目标是否仍在范围内
            if (skeleton.Target != null)
            {
                float distanceToTarget = Vector2.Distance(skeleton.transform.position, skeleton.Target.position);
                
                // 玩家仍在攻击范围内，可以继续攻击
                if (distanceToTarget <= skeleton.AttackRange)
                {
                    skeleton.StateMachine.ChangeState(new SkeletonAttackState(skeleton));
                    return;
                }
                
                // 玩家在侦测范围但不在攻击范围，继续追击
                if (distanceToTarget <= skeleton.DetectionRange)
                {
                    skeleton.StateMachine.ChangeState(new SkeletonChaseState(skeleton));
                    return;
                }
            }
            
            // 目标丢失或超出侦测范围，返回巡逻状态
            skeleton.StateMachine.ChangeState(new SkeletonPatrolState(skeleton));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 攻击状态下不移动
    }
    
    public void OnExit()
    {
        // 退出攻击状态
    }
}
