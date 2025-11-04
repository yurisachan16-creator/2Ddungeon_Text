using UnityEngine;

/// <summary>
/// Knight受伤状态
/// 处理受伤动画和无敌帧
/// </summary>
public class KnightHurtState : IState
{
    private CharacterBase character;
    private Animator animator;
    private float hurtDuration = 0.4f; // 受伤持续时间
    private float hurtTimer = 0f; // 受伤计时器
    
    public KnightHurtState(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        // 播放受伤动画
        animator.SetTrigger("Hurt");
        hurtTimer = 0f;
        
        // 可以在这里添加受伤特效、音效等
        Debug.Log($"{character.name} 受伤了！");
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (character.IsDead)
        {
            character.StateMachine.ChangeState(new KnightDeathState(character));
            return;
        }
        
        // 更新受伤计时器
        hurtTimer += Time.deltaTime;
        
        // 受伤结束，返回空闲状态
        if (hurtTimer >= hurtDuration)
        {
            // 重置受伤标志
            var characterBase = character as CharacterBase;
            if (characterBase != null)
            {
                var field = typeof(CharacterBase).GetField("isHurt", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                field?.SetValue(characterBase, false);
            }
            
            character.StateMachine.ChangeState(new KnightIdleState(character));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 受伤状态下不移动（可以添加击退效果）
    }
    
    public void OnExit()
    {
        // 退出受伤状态
    }
}
