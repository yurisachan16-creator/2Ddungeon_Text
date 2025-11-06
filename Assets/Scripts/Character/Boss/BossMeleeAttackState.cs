using UnityEngine;

/// <summary>
/// Boss近战攻击状态
/// 当玩家非常接近时使用的强力近战攻击
/// </summary>
public class BossMeleeAttackState : IState
{
    private BossController boss;
    private Animator animator;
    private float attackDuration = 0.7f; // 近战攻击持续时间
    private float attackTimer;
    private bool hasAttacked;
    
    public BossMeleeAttackState(BossController boss)
    {
        this.boss = boss;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放攻击动画（近战也用Attack动画，可以通过参数区分）
        animator.SetTrigger("Attack");
        attackTimer = 0f;
        hasAttacked = false;
        
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
        boss.MeleeAttackTimer = boss.MeleeAttackCooldown;
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
        
        // 在攻击动画中间执行伤害判定
        if (!hasAttacked && attackTimer >= attackDuration * 0.5f)
        {
            boss.PerformMeleeAttack();
            hasAttacked = true;
        }
        
        // 攻击动画结束
        if (attackTimer >= attackDuration)
        {
            // 返回空闲状态
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
