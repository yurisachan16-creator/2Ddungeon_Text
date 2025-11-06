using UnityEngine;

/// <summary>
/// Skeleton防御状态
/// 骷髅举盾防御，减少受到的伤害（骷髅特有技能）
/// </summary>
public class SkeletonDefenseState : IState
{
    private SkeletonController skeleton;
    private Animator animator;
    private float defenseTimer;
    
    public SkeletonDefenseState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 播放防御动画
        animator.SetTrigger("Defense");
        defenseTimer = 0f;
        
        // 设置防御标志
        skeleton.SetDefending(true);
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (skeleton.IsDead)
        {
            skeleton.StateMachine.ChangeState(new SkeletonDeathState(skeleton));
            return;
        }
        
        // 更新防御计时器
        defenseTimer += Time.deltaTime;
        
        // 防御时间结束
        if (defenseTimer >= skeleton.DefenseDuration)
        {
            // 结束防御，根据情况决定下一个状态
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
        // 防御状态下不移动
    }
    
    public void OnExit()
    {
        // 取消防御标志
        skeleton.SetDefending(false);
    }
}
