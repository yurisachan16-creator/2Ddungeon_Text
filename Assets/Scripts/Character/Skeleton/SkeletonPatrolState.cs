using UnityEngine;

/// <summary>
/// Skeleton巡逻状态
/// 在指定区域内随机巡逻，侦测玩家
/// </summary>
public class SkeletonPatrolState : IState
{
    private SkeletonController skeleton;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 patrolTarget;
    private bool isWaiting;
    private float waitTimer;
    
    public SkeletonPatrolState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.rb = skeleton.Rb;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 获取随机巡逻目标点
        patrolTarget = skeleton.GetRandomPatrolPoint();
        isWaiting = false;
        waitTimer = 0f;
        
        // 播放行走动画
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
        
        // 巡逻逻辑
        if (isWaiting)
        {
            // 等待阶段
            waitTimer += Time.deltaTime;
            if (waitTimer >= skeleton.PatrolWaitTime)
            {
                // 等待结束，选择新的巡逻点
                patrolTarget = skeleton.GetRandomPatrolPoint();
                isWaiting = false;
                animator.SetFloat("Speed", 1f);
            }
        }
        else
        {
            // 移动到巡逻点
            float distance = Vector2.Distance(skeleton.transform.position, patrolTarget);
            
            if (distance < 0.1f)
            {
                // 到达巡逻点，开始等待
                isWaiting = true;
                waitTimer = 0f;
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                // 根据移动方向翻转角色
                Vector2 direction = (patrolTarget - (Vector2)skeleton.transform.position).normalized;
                if (direction.x != 0)
                {
                    skeleton.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
                }
            }
        }
    }
    
    public void OnFixedUpdate()
    {
        if (!isWaiting && rb != null)
        {
            // 移动到巡逻目标点
            Vector2 direction = (patrolTarget - rb.position).normalized;
            rb.MovePosition(rb.position + direction * skeleton.PatrolSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出巡逻状态
    }
}
