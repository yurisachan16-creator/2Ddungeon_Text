using UnityEngine;

/// <summary>
/// Boss受伤状态
/// 播放受伤动画并短暂硬直
/// </summary>
public class BossHurtState : IState
{
    private BossController boss;
    private Animator animator;
    private float hurtDuration = 0.3f; // Boss受伤硬直时间较短
    private float hurtTimer;
    
    public BossHurtState(BossController boss)
    {
        this.boss = boss;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放受伤动画
        animator.SetTrigger("Hurt");
        hurtTimer = 0f;
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (boss.IsDead)
        {
            boss.StateMachine.ChangeState(new BossDeathState(boss));
            return;
        }
        
        // 更新受伤计时器
        hurtTimer += Time.deltaTime;
        
        // 受伤硬直结束
        if (hurtTimer >= hurtDuration)
        {
            // 重置受伤标志
            var field = typeof(CharacterBase).GetField("isHurt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(boss, false);
            
            // 返回空闲状态，由空闲状态决定下一步行动
            boss.StateMachine.ChangeState(new BossIdleState(boss));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 受伤状态下不移动
    }
    
    public void OnExit()
    {
        // 退出受伤状态
    }
}
