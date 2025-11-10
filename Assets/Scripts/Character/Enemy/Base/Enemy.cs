using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人类
/// </summary>
public class Enemy : MonoBehaviour, IDamageable, IMoveable, ITriggercheckable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [field: SerializeField] public float CurrentHealth { get; set; }

    public Rigidbody2D RB { get; set; }
    public bool IsFacingRight { get; set; } = true;
    public bool IsAggroed { get; set; } //激怒状态
    public bool IsWithinStrikingDistance { get; set; }  // 远程攻击距离
    public bool IsWithinMeleeDistance { get ; set ; }    // 近战攻击距离
    public bool IsWithinEscapeDistance { get; set; }   // 远程逃跑距离

    #region Enemy State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }

    #endregion

    #region ScriptableObjects Variables
    [SerializeField] private EnemyIdleSOBase EnemyIdleBase;
    [SerializeField] private EnemyChaseSOBase EnemyChaseBase;
    [SerializeField] private EnemyAttackSOBase EnemyAttackBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get; private set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; private set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get; private set; }
    
    #endregion

    void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
        EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
        EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);

        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);

        
    }
    void Start()
    {
        CurrentHealth = MaxHealth;

        RB = GetComponent<Rigidbody2D>();

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyAttackBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Movement Functions
    public void MoveEnemy(Vector2 velocity)
    {
        RB.velocity = velocity;
        CheckForLeftOrRightFacing(velocity);
    }

    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {
        if ((IsFacingRight && velocity.x < 0f) || (!IsFacingRight && velocity.x > 0f))
        {
            Flip();
        }
    }

    public void Flip()
    {
        IsFacingRight = !IsFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    #endregion

    #region Health/Die Functions

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damageAmount">伤害值</param>
    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Distance Checks

    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithStrikingDistance)
    {
        IsWithinStrikingDistance = isWithStrikingDistance;
    }

    public void SetMeleeDistanceBool(bool isWithinMeleeDistance)
    {
        IsWithinMeleeDistance = isWithinMeleeDistance;
    }

    public void SetEscapeDistanceBool(bool isWithinEscapeDistance)
    {
        IsWithinEscapeDistance = isWithinEscapeDistance;
    }


    #endregion

    #region Animation Triggers

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        //TODO: 根据不同的触发类型执行相应的逻辑
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayFootstepSound
    }

    #endregion
}
