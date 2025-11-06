using UnityEngine;

/// <summary>
/// Boss远程攻击状态
/// 发射弹幕攻击玩家（Boss的主要攻击方式）
/// </summary>
public class BossRangedAttackState : IState
{
    private BossController boss;
    private Animator animator;
    private float attackDuration = 0.8f; // 攻击动画持续时间
    private float attackTimer;
    private bool hasFired; // 是否已发射弹幕
    
    public BossRangedAttackState(BossController boss)
    {
        this.boss = boss;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放攻击动画
        animator.SetTrigger("Attack");
        attackTimer = 0f;
        hasFired = false;
        
        // 面向目标
        if (boss.Target != null)
        {
            Vector2 direction = boss.Target.position - boss.transform.position;
            if (direction.x != 0)
            {
                boss.transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }
        }
        
        // 设置冷却时间
        boss.RangedAttackTimer = boss.RangedAttackCooldown;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (boss.IsDead)
        {
            boss.StateMachine.ChangeState(new BossDeathState(boss));
            return;
        }
        
        // 更新攻击计时器
        attackTimer += Time.deltaTime;
        
        // 在攻击动画中间发射弹幕（约 0.4 秒时）
        if (!hasFired && attackTimer >= attackDuration * 0.5f)
        {
            boss.FireProjectile();
            hasFired = true;
        }
        
        // 攻击动画结束
        if (attackTimer >= attackDuration)
        {
            // 返回空闲状态，让空闲状态决定下一步行动
            boss.StateMachine.ChangeState(new BossIdleState(boss));
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
