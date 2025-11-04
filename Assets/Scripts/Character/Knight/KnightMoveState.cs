using UnityEngine;

/// <summary>
/// Knight移动状态
/// 处理角色移动和方向切换
/// </summary>
public class KnightMoveState : IState
{
    private CharacterBase character;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    
    public KnightMoveState(CharacterBase character)
    {
        this.character = character;
        this.rb = character.Rb;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        // 进入移动状态
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
        
        // 检测攻击输入（移动时也可以攻击）
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
        
        // 获取移动输入
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // 如果没有输入，切换到空闲状态
        if (movement.x == 0 && movement.y == 0)
        {
            character.StateMachine.ChangeState(new KnightIdleState(character));
            return;
        }
        
        // 更新动画参数
        animator.SetFloat("Speed", movement.magnitude);
        
        // 根据移动方向翻转角色
        if (movement.x != 0)
        {
            character.transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }
    }
    
    public void OnFixedUpdate()
    {
        // 物理移动
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement * character.MoveSpeed * Time.fixedDeltaTime);
        }
    }
    
    public void OnExit()
    {
        // 退出移动状态，停止移动
        movement = Vector2.zero;
    }
}
