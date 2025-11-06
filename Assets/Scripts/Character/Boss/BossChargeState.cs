using UnityEngine;

/// <summary>
/// Boss冲锋状态
/// 快速冲向玩家的位置，造成冲击伤害
/// </summary>
public class BossChargeState : IState
{
    private BossController boss;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 chargeDirection;
    private Vector2 chargeTarget;
    private float chargeDuration = 0.8f; // 冲锋持续时间
    private float chargeSpeed = 8f; // 冲锋速度
    private float chargeTimer;
    
    public BossChargeState(BossController boss)
    {
        this.boss = boss;
        this.rb = boss.Rb;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 记录目标位置和方向
        if (boss.Target != null)
        {
            chargeTarget = boss.Target.position;
            chargeDirection = (chargeTarget - (Vector2)boss.transform.position).normalized;
            
            // 面向冲锋方向
            if (chargeDirection.x != 0)
            {
                boss.transform.localScale = new Vector3(Mathf.Sign(chargeDirection.x), 1, 1);
            }
        }
        else
        {
            chargeDirection = Vector2.right * boss.transform.localScale.x;
        }
        
        // 播放飞行动画（快速移动）
        animator.SetTrigger("Fly");
        animator.SetFloat("Speed", 2f);
        chargeTimer = 0f;
        
        // 设置冷却时间
        boss.ChargeTimer = boss.ChargeCooldown;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (boss.IsDead)
        {
            boss.StateMachine.ChangeState(new BossDeathState(boss));
            return;
        }
        
        // 更新冲锋计时器
        chargeTimer += Time.deltaTime;
        
        // 冲锋结束
        if (chargeTimer >= chargeDuration)
        {
            // 返回空闲状态
            boss.StateMachine.ChangeState(new BossIdleState(boss));
        }
    }
    
    public void OnFixedUpdate()
    {
        if (rb != null)
        {
            // 沿冲锋方向快速移动
            rb.MovePosition(rb.position + chargeDirection * chargeSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出冲锋状态
        animator.SetFloat("Speed", 0f);
    }
    
    /// <summary>
    /// 碰撞检测（在BossController中添加OnCollisionEnter2D）
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 冲锋碰撞到玩家造成伤害
        CharacterBase character = collision.gameObject.GetComponent<CharacterBase>();
        if (character != null && character.CharacterType == CharacterType.Player)
        {
            character.TakeDamage(boss.AttackDamage * 2f); // 冲锋伤害更高
        }
    }
}
