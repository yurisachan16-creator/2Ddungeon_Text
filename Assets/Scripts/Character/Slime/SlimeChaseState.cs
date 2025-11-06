using UnityEngine;

/// <summary>
/// Slime追击状�?
/// 追击玩家，尝试接近到攻击范围
/// </summary>
public class SlimeChaseState : IState
{
    private SlimeController slime;
    private Rigidbody2D rb;
    private Animator animator;
    
    public SlimeChaseState(SlimeController slime)
    {
        this.slime = slime;
        this.rb = slime.Rb;
        this.animator = slime.Animator;
    }
    
    public void OnEnter()
    {
        // 播放行走动画（追击时移动更快�?
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
        
        // 检测目标是否存�?
        if (slime.Target == null)
        {
            // 目标丢失，返回游荡状�?
            slime.StateMachine.ChangeState(new SlimeWanderState(slime));
            return;
        }
        
        float distanceToTarget = Vector2.Distance(slime.transform.position, slime.Target.position);
        
        // 玩家在攻击范围内，切换到攻击状�?
        if (distanceToTarget <= slime.AttackRange)
        {
            slime.StateMachine.ChangeState(new SlimeAttackState(slime));
            return;
        }
        
        // 玩家超出侦测范围，返回游荡状�?
        if (distanceToTarget > slime.DetectionRange)
        {
            slime.StateMachine.ChangeState(new SlimeWanderState(slime));
            return;
        }
        
        // 根据移动方向翻转角色
        Vector2 direction = (slime.Target.position - slime.transform.position).normalized;
        if (direction.x != 0)
        {
            slime.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
    }
    
    public void OnFixedUpdate()
    {
        if (slime.Target != null && rb != null)
        {
            // 向玩家移�?
            Vector2 direction = (slime.Target.position - slime.transform.position).normalized;
            rb.MovePosition(rb.position + direction * slime.ChaseSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出追击状�?
    }
}
