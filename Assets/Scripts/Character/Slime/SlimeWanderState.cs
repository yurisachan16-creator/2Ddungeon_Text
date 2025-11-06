using UnityEngine;

/// <summary>
/// Slime游荡状�?
/// 在指定区域内随机游荡，侦测玩�?
/// </summary>
public class SlimeWanderState : IState
{
    private SlimeController slime;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 wanderTarget;
    private bool isWaiting;
    private float waitTimer;
    
    public SlimeWanderState(SlimeController slime)
    {
        this.slime = slime;
        this.rb = slime.Rb;
        this.animator = slime.Animator;
    }
    
    public void OnEnter()
    {
        // 获取随机游荡目标�?
        wanderTarget = slime.GetRandomWanderPoint();
        isWaiting = false;
        waitTimer = 0f;
        
        // 播放行走动画
        animator.SetFloat("Speed", 1f);
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
        
        // 游荡逻辑
        if (isWaiting)
        {
            // 等待阶段
            waitTimer += Time.deltaTime;
            if (waitTimer >= slime.WanderWaitTime)
            {
                // 等待结束，选择新的游荡�?
                wanderTarget = slime.GetRandomWanderPoint();
                isWaiting = false;
                animator.SetFloat("Speed", 1f);
            }
        }
        else
        {
            // 移动到游荡点
            float distance = Vector2.Distance(slime.transform.position, wanderTarget);
            
            if (distance < 0.1f)
            {
                // 到达游荡点，开始等�?
                isWaiting = true;
                waitTimer = 0f;
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                // 根据移动方向翻转角色
                Vector2 direction = (wanderTarget - (Vector2)slime.transform.position).normalized;
                if (direction.x != 0)
                {
                    slime.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
                }
            }
        }
    }
    
    public void OnFixedUpdate()
    {
        if (!isWaiting && rb != null)
        {
            // 移动到游荡目标点
            Vector2 direction = (wanderTarget - rb.position).normalized;
            rb.MovePosition(rb.position + direction * slime.WanderSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出游荡状�?
    }
}
