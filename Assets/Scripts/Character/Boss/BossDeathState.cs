using UnityEngine;

/// <summary>
/// Boss死亡状态
/// 播放死亡动画，触发Boss战结束事件
/// </summary>
public class BossDeathState : IState
{
    private BossController boss;
    private Animator animator;
    private float deathDuration = 2.5f; // Boss死亡动画较长
    private float deathTimer;
    
    public BossDeathState(BossController boss)
    {
        this.boss = boss;
        this.animator = boss.Animator;
    }
    
    public void OnEnter()
    {
        // 播放死亡动画
        animator.SetTrigger("Death");
        deathTimer = 0f;
        
        // 禁用碰撞和移动
        if (boss.Rb != null)
        {
            boss.Rb.velocity = Vector2.zero;
            boss.Rb.simulated = false;
        }
        
        // 禁用碰撞器
        Collider2D collider = boss.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // 触发Boss战结束事件（可以在这里添加）
        // 例如：GameManager.Instance.OnBossDefeated();
    }
    
    public void OnUpdate()
    {
        // 更新死亡计时器
        deathTimer += Time.deltaTime;
        
        // 死亡动画播放一段时间后销毁对象
        if (deathTimer >= deathDuration)
        {
            // 可以在这里添加掉落物品、解锁房间等逻辑
            
            // 销毁游戏对象
            Object.Destroy(boss.gameObject);
        }
    }
    
    public void OnFixedUpdate()
    {
        // 死亡状态无需物理更新
    }
    
    public void OnExit()
    {
        // 死亡状态不会主动退出
    }
}
