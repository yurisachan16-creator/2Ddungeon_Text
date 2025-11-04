using UnityEngine;

/// <summary>
/// Knight死亡状态
/// 处理死亡动画和后续逻辑
/// </summary>
public class KnightDeathState : IState
{
    private CharacterBase character;
    private Animator animator;
    private float deathDuration = 1.5f; // 死亡动画持续时间
    private float deathTimer = 0f; // 死亡计时器
    private bool hasPlayedAnimation = false; // 是否已播放动画
    
    public KnightDeathState(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        // 播放死亡动画
        if (!hasPlayedAnimation)
        {
            animator.SetTrigger("Death");
            hasPlayedAnimation = true;
            deathTimer = 0f;
        }
        
        // 禁用角色控制
        if (character.Rb != null)
        {
            character.Rb.velocity = Vector2.zero;
            character.Rb.simulated = false; // 禁用物理模拟
        }
        
        Debug.Log($"{character.name} 死亡了！");
    }
    
    public void OnUpdate()
    {
        // 更新死亡计时器
        deathTimer += Time.deltaTime;
        
        // 死亡动画结束后的处理
        if (deathTimer >= deathDuration)
        {
            // 可以在这里添加：
            // 1. 销毁对象
            // 2. 触发复活
            // 3. 显示游戏结束界面等
            
            // 示例：销毁对象
            // GameObject.Destroy(character.gameObject);
        }
    }
    
    public void OnFixedUpdate()
    {
        // 死亡状态下不进行物理更新
    }
    
    public void OnExit()
    {
        // 退出死亡状态（如果有复活逻辑）
        if (character.Rb != null)
        {
            character.Rb.simulated = true; // 重新启用物理模拟
        }
    }
}
