using UnityEngine;

/// <summary>
/// Boss召唤状态
/// 召唤小怪协助战斗
/// </summary>
public class BossSummonState : IState
{
    private BossController boss;
    private Animator animator;
    private float summonDuration = 1.5f; // 召唤动画持续时间
    private float summonTimer;
    private bool hasSummoned;
    
    public BossSummonState(BossController boss)
    {
        this.boss = boss;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放空闲动画（可以使用特殊召唤动画，这里用Idle代替）
        animator.SetFloat("Speed", 0f);
        summonTimer = 0f;
        hasSummoned = false;
        
        // 设置冷却时间
        boss.SummonTimer = boss.SummonCooldown;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (boss.IsDead)
        {
            boss.StateMachine.ChangeState(new BossDeathState(boss));
            return;
        }
        
        // 更新召唤计时器
        summonTimer += Time.deltaTime;
        
        // 在召唤动画中间执行召唤（约 0.7 秒时）
        if (!hasSummoned && summonTimer >= summonDuration * 0.5f)
        {
            boss.PerformSummon();
            hasSummoned = true;
        }
        
        // 召唤动画结束
        if (summonTimer >= summonDuration)
        {
            // 返回空闲状态
            boss.StateMachine.ChangeState(new BossIdleState(boss));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 召唤状态下不移动
    }
    
    public void OnExit()
    {
        // 退出召唤状态
    }
}
