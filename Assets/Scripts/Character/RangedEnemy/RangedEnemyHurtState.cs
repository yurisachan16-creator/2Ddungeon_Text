using UnityEngine;

/// <summary>
/// 远程敌人受伤状态
/// </summary>
public class RangedEnemyHurtState : IState
{
    private CharacterBase character;
    private Animator animator;
    private float hurtDuration = 0.4f;
    private float hurtTimer = 0f;
    
    public RangedEnemyHurtState(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        hurtTimer = 0f;
    }
    
    public void OnUpdate()
    {
        if (character.IsDead)
        {
            character.StateMachine.ChangeState(new RangedEnemyDeathState(character));
            return;
        }
        
        hurtTimer += Time.deltaTime;
        
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
            
            character.StateMachine.ChangeState(new RangedEnemyIdleState(character));
        }
    }
    
    public void OnFixedUpdate() { }
    
    public void OnExit() { }
}
