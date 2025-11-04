using UnityEngine;

/// <summary>
/// Knight空闲状态
/// 检测输入并切换到其他状态
/// </summary>
public class KnightIdleState : IState
{
    private CharacterBase character;
    private Animator animator;
    
    public KnightIdleState(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        // 播放空闲动画
        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsAttacking", false);
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (character.IsDead)
        {
            character.StateMachine.ChangeState(new KnightDeathState(character));
            return;
        }
        
        // 检测受伤
        if (character.IsHurt)
        {
            character.StateMachine.ChangeState(new KnightHurtState(character));
            return;
        }
        
        // 检测攻击输入
        if (Input.GetMouseButtonDown(0)) // 左键攻击1
        {
            character.StateMachine.ChangeState(new KnightAttack01State(character));
            return;
        }
        
        if (Input.GetMouseButtonDown(1)) // 右键攻击2
        {
            character.StateMachine.ChangeState(new KnightAttack02State(character));
            return;
        }
        
        // 检测移动输入
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        if (horizontal != 0 || vertical != 0)
        {
            character.StateMachine.ChangeState(new KnightMoveState(character));
        }
    }
    
    public void OnFixedUpdate()
    {
        // 空闲状态无需物理更新
    }
    
    public void OnExit()
    {
        // 退出空闲状态
    }
}
