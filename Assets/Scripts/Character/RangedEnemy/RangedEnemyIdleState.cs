using UnityEngine;

/// <summary>
/// 远程敌人空闲状态
/// 检测玩家并决定是否进入攻击或追击状态
/// </summary>
public class RangedEnemyIdleState : IState
{
    private CharacterBase character;
    private Animator animator;
    private float detectionRange = 8f; // 检测范围
    private Transform player; // 玩家引用
    
    public RangedEnemyIdleState(CharacterBase character, float detectionRange = 8f)
    {
        this.character = character;
        this.animator = character.Animator;
        this.detectionRange = detectionRange;
    }
    
    public void OnEnter()
    {
        // 播放空闲动画
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
        
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    public void OnUpdate()
    {
        // 检测死亡
        if (character.IsDead)
        {
            character.StateMachine.ChangeState(new RangedEnemyDeathState(character));
            return;
        }
        
        // 检测受伤
        if (character.IsHurt)
        {
            character.StateMachine.ChangeState(new RangedEnemyHurtState(character));
            return;
        }
        
        // 检测玩家距离
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(character.transform.position, player.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                // 玩家在检测范围内，进入攻击状态
                character.StateMachine.ChangeState(new RangedEnemyAttackState(character));
            }
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
