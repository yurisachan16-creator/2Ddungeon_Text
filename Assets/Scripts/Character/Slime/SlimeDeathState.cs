using UnityEngine;

/// <summary>
/// Slime死亡状�?
/// 播放死亡动画，并在一定时间后销毁游戏对�?
/// </summary>
public class SlimeDeathState : IState
{
    private SlimeController slime;
    private Animator animator;
    private float deathDuration = 1f; // 死亡动画持续时间
    private float deathTimer;
    
    public SlimeDeathState(SlimeController slime)
    {
        this.slime = slime;
        this.animator = slime.Animator;
    }
    
    public void OnEnter()
    {
        // 播放死亡动画
        animator.SetTrigger("Death");
        deathTimer = 0f;
        
        // 禁用碰撞和移�?
        if (slime.Rb != null)
        {
            slime.Rb.velocity = Vector2.zero;
            slime.Rb.simulated = false;
        }
        
        // 禁用碰撞�?
        Collider2D collider = slime.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
    
    public void OnUpdate()
    {
        // 更新死亡计时�?
        deathTimer += Time.deltaTime;
        
        // 死亡动画播放一段时间后销毁对�?
        if (deathTimer >= deathDuration)
        {
            // 可以在这里添加掉落物品、经验等逻辑
            
            // 销毁游戏对�?
            Object.Destroy(slime.gameObject);
        }
    }
    
    public void OnFixedUpdate()
    {
        // 死亡状态无需物理更新
    }
    
    public void OnExit()
    {
        // 死亡状态不会主动退�?
    }
}
