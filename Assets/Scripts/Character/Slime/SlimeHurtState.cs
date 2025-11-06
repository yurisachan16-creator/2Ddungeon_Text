using UnityEngine;

/// <summary>
/// Slime受伤状�?
/// 注意：Slime没有Hurt动画，使用Idle动画短暂替代受伤效果
/// </summary>
public class SlimeHurtState : IState
{
    private SlimeController slime;
    private Animator animator;
    private float hurtDuration = 0.2f; // 受伤硬直时间（较短）
    private float hurtTimer;
    
    public SlimeHurtState(SlimeController slime)
    {
        this.slime = slime;
        this.animator = slime.Animator;
    }
    
    public void OnEnter()
    {
        // 使用Idle动画替代受伤动画
        animator.SetFloat("Speed", 0f);
        hurtTimer = 0f;
        
        // 可以添加颜色闪烁效果表现受伤
        var spriteRenderer = slime.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 变红效果
            spriteRenderer.color = Color.red;
        }
    }
    
    public void OnUpdate()
    {
        // 检测死�?
        if (slime.IsDead)
        {
            slime.StateMachine.ChangeState(new SlimeDeathState(slime));
            return;
        }
        
        // 更新受伤计时�?
        hurtTimer += Time.deltaTime;
        
        // 受伤硬直结束
        if (hurtTimer >= hurtDuration)
        {
            // 重置受伤标志
            var field = typeof(CharacterBase).GetField("isHurt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(slime, false);
            
            // 根据情况决定下一个状�?
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
            
            // 没有目标，返回游荡状�?
            slime.StateMachine.ChangeState(new SlimeWanderState(slime));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 受伤状态下不移�?
    }
    
    public void OnExit()
    {
        // 恢复正常颜色
        var spriteRenderer = slime.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
