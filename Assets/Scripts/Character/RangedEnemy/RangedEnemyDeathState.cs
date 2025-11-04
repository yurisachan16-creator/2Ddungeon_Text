using UnityEngine;

/// <summary>
/// 远程敌人死亡状态
/// </summary>
public class RangedEnemyDeathState : IState
{
    private CharacterBase character;
    private Animator animator;
    private float deathDuration = 1.5f;
    private float deathTimer = 0f;
    private bool hasPlayedAnimation = false;
    
    public RangedEnemyDeathState(CharacterBase character)
    {
        this.character = character;
        this.animator = character.Animator;
    }
    
    public void OnEnter()
    {
        if (!hasPlayedAnimation && animator != null)
        {
            animator.SetTrigger("Death");
            hasPlayedAnimation = true;
        }
        
        if (character.Rb != null)
        {
            character.Rb.velocity = Vector2.zero;
            character.Rb.simulated = false;
        }
        
        deathTimer = 0f;
    }
    
    public void OnUpdate()
    {
        deathTimer += Time.deltaTime;
        
        if (deathTimer >= deathDuration)
        {
            // 销毁敌人对象
            GameObject.Destroy(character.gameObject);
        }
    }
    
    public void OnFixedUpdate() { }
    
    public void OnExit()
    {
        if (character.Rb != null)
        {
            character.Rb.simulated = true;
        }
    }
}
