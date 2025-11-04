using UnityEngine;

/// <summary>
/// Knight攻击1状态（左键攻击）
/// 处理第一种攻击动画和逻辑
/// </summary>
public class KnightAttack01State : IState
{
    private CharacterBase character;
    private Animator animator;
    private float attackDuration = 0.5f; // 攻击持续时间
    private float attackTimer = 0f; // 攻击计时器
    
    public KnightAttack01State(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        // 播放攻击1动画
        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("Attack01");
        attackTimer = 0f;
        
        // 设置攻击标志
        var characterBase = character as CharacterBase;
        if (characterBase != null)
        {
            // 使用反射设置私有字段（或者在CharacterBase中添加公共方法）
            var field = typeof(CharacterBase).GetField("isAttacking", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(characterBase, true);
        }
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (character.IsDead)
        {
            character.StateMachine.ChangeState(new KnightDeathState(character));
            return;
        }
        
        // 更新攻击计时器
        attackTimer += Time.deltaTime;
        
        // 攻击结束，返回空闲状态
        if (attackTimer >= attackDuration)
        {
            character.StateMachine.ChangeState(new KnightIdleState(character));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 攻击状态下不移动
    }
    
    public void OnExit()
    {
        // 重置攻击标志
        animator.SetBool("IsAttacking", false);
        
        var characterBase = character as CharacterBase;
        if (characterBase != null)
        {
            var field = typeof(CharacterBase).GetField("isAttacking", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(characterBase, false);
        }
    }
}
