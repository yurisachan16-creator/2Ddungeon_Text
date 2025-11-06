using UnityEngine;

/// <summary>
/// Slime攻击状�?
/// Slime没有独立的攻击动画，使用Walk动画持续追击并通过碰撞造成伤害
/// </summary>
public class SlimeAttackState : IState
{
    private SlimeController slime;
    private Rigidbody2D rb;
    private Animator animator;
    private float attackStateTimer; // 攻击状态持续时�?
    private float maxAttackStateTime = 0.5f; // 最大攻击状态时�?
    
    public SlimeAttackState(SlimeController slime)
    {
        this.slime = slime;
        this.rb = slime.Rb;
        this.animator = slime.Animator;
    }
    
    public void OnEnter()
    {
        // 使用Walk动画表现攻击（Slime没有Attack动画�?
        animator.SetFloat("Speed", 1f);
        attackStateTimer = 0f;
        
        // 面向目标
        if (slime.Target != null)
        {
            Vector2 direction = slime.Target.position - slime.transform.position;
            if (direction.x != 0)
            {
                slime.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }
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
        
        // 更新攻击状态计时器
        attackStateTimer += Time.deltaTime;
        
        // 检测目�?
        if (slime.Target != null)
        {
            float distanceToTarget = Vector2.Distance(slime.transform.position, slime.Target.position);
            
            // 尝试造成伤害（通过近距离接触）
            if (distanceToTarget <= slime.AttackRange && slime.AttackTimer <= 0)
            {
                CharacterBase targetCharacter = slime.Target.GetComponent<CharacterBase>();
                if (targetCharacter != null && !targetCharacter.IsDead)
                {
                    slime.PerformAttack(targetCharacter);
                }
            }
            
            // 玩家移动到攻击范围外，继续追�?
            if (distanceToTarget > slime.AttackRange && distanceToTarget <= slime.DetectionRange)
            {
                slime.StateMachine.ChangeState(new SlimeChaseState(slime));
                return;
            }
            
            // 玩家超出侦测范围，返回游荡状�?
            if (distanceToTarget > slime.DetectionRange)
            {
                slime.StateMachine.ChangeState(new SlimeWanderState(slime));
                return;
            }
        }
        else
        {
            // 目标丢失，返回游荡状�?
            slime.StateMachine.ChangeState(new SlimeWanderState(slime));
            return;
        }
        
        // 攻击状态持续一定时间后重新评估
        if (attackStateTimer >= maxAttackStateTime)
        {
            slime.StateMachine.ChangeState(new SlimeChaseState(slime));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 继续向目标移动（粘附攻击�?
        if (slime.Target != null && rb != null)
        {
            Vector2 direction = (slime.Target.position - slime.transform.position).normalized;
            rb.MovePosition(rb.position + direction * slime.ChaseSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出攻击状�?
    }
}
