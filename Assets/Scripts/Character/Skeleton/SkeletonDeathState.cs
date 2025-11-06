using UnityEngine;

/// <summary>
/// Skeleton死亡状态
/// 播放死亡动画，并在一定时间后销毁游戏对象
/// </summary>
public class SkeletonDeathState : IState
{
    private SkeletonController skeleton;
    private Animator animator;
    private float deathDuration = 1.5f; // 死亡动画持续时间
    private float deathTimer;
    private bool hasDisabled; // 是否已禁用碰撞
    
    public SkeletonDeathState(SkeletonController skeleton)
    {
        this.skeleton = skeleton;
        this.animator = skeleton.Animator;
    }
    
    public void OnEnter()
    {
        // 播放死亡动画
        animator.SetTrigger("Death");
        deathTimer = 0f;
        hasDisabled = false;
        
        // 禁用碰撞和移动
        if (skeleton.Rb != null)
        {
            skeleton.Rb.velocity = Vector2.zero;
            skeleton.Rb.simulated = false;
        }
        
        // 禁用碰撞器
        Collider2D collider = skeleton.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
    
    public void OnUpdate()
    {
        // 更新死亡计时器
        deathTimer += Time.deltaTime;
        
        // 死亡动画播放一段时间后销毁对象
        if (deathTimer >= deathDuration)
        {
            // 可以在这里添加掉落物品、经验等逻辑
            
            // 销毁游戏对象
            Object.Destroy(skeleton.gameObject);
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
