using UnityEngine;

/// <summary>
/// 角色类型枚举
/// </summary>
public enum CharacterType
{
    Player,     // 玩家
    Melee,      // 近战敌人
    Ranged,     // 远程敌人
    Boss        // Boss
}

/// <summary>
/// 角色基类，所有角色都继承此类
/// 提供通用的属性和状态机管理
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    [Header("角色信息")]
    [SerializeField] protected CharacterType characterType; // 角色类型
    [SerializeField] protected string characterName = "角色"; // 角色名称
    
    [Header("基础属性")]
    [SerializeField] protected float maxHealth = 100f; // 最大生命值
    [SerializeField] protected float currentHealth; // 当前生命值
    [SerializeField] protected float moveSpeed = 5f; // 移动速度
    [SerializeField] protected float attackDamage = 10f; // 攻击伤害
    
    [Header("状态标志")]
    [SerializeField] protected bool isDead = false; // 是否死亡
    [SerializeField] protected bool isHurt = false; // 是否受伤
    [SerializeField] protected bool isAttacking = false; // 是否正在攻击
    
    // 组件引用
    protected Rigidbody2D rb; // 刚体组件
    protected Animator animator; // 动画组件
    protected SpriteRenderer spriteRenderer; // 精灵渲染器
    
    // 状态机
    protected StateMachine stateMachine; // 状态机实例
    
    // 属性访问器
    public CharacterType CharacterType => characterType;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float MoveSpeed => moveSpeed;
    public float AttackDamage => attackDamage;
    public bool IsDead => isDead;
    public bool IsHurt => isHurt;
    public bool IsAttacking => isAttacking;
    public Rigidbody2D Rb => rb;
    public Animator Animator => animator;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public StateMachine StateMachine => stateMachine;
    
    protected virtual void Awake()
    {
        // 获取组件引用
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 初始化状态机
        stateMachine = new StateMachine();
        
        // 初始化生命值
        currentHealth = maxHealth;
    }
    
    protected virtual void Start()
    {
        // 子类可以重写此方法进行初始化
    }
    
    protected virtual void Update()
    {
        // 更新状态机
        stateMachine?.Update();
    }
    
    protected virtual void FixedUpdate()
    {
        // 物理更新状态机
        stateMachine?.FixedUpdate();
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            OnHurt();
        }
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    /// <param name="healAmount">治疗量</param>
    public virtual void Heal(float healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// 受伤时调用
    /// </summary>
    protected virtual void OnHurt()
    {
        isHurt = true;
        // 子类实现具体逻辑
    }
    
    /// <summary>
    /// 死亡时调用
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
        // 子类实现具体逻辑
    }
    
    /// <summary>
    /// 重置角色状态
    /// </summary>
    public virtual void ResetCharacter()
    {
        currentHealth = maxHealth;
        isDead = false;
        isHurt = false;
        isAttacking = false;
    }
    
    protected virtual void OnDestroy()
    {
        // 清理状态机
        stateMachine?.Clear();
    }
}
