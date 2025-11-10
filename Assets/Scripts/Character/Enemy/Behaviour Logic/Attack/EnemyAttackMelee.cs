// EnemyAttackMelee.cs

using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Melee", menuName = "Enemy Logic/Attack Logic/Melee")]
public class EnemyAttackMelee : EnemyAttackSOBase
{
    [SerializeField] private float _timeBetweenAttacks = 1.5f;
    [SerializeField] private float _damage = 10f;

    [Header("Melee Attack Area")]
    [SerializeField] private Transform _attackPoint; // 需要在敌人预制件上创建一个攻击点
    [SerializeField] private float _attackRadius = 0.8f;
    [SerializeField] private LayerMask _playerLayer; // 设置为玩家所在的层

    private float _timer;
    private bool _isAttacking;  // 是否正在攻击

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _isAttacking = false;   // 重置攻击状态
        _timer = 0f;    // 重置计时器
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        enemy.MoveEnemy(Vector2.zero); // 保持站立

        _timer += Time.deltaTime;

        Debug.Log($"近战骷髅状态: IsWithinMeleeDistance={enemy.IsWithinMeleeDistance}, isAttacking={_isAttacking}");

        // 如果不在攻击动画/冷却中，才检查是否要退出
        if (!_isAttacking)
        {
            if (!enemy.IsWithinMeleeDistance)
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
                return;
            }

            if (_timer >= _timeBetweenAttacks)
            {
                _timer = 0f;
                _isAttacking = true; // 标记开始攻击
                // 在这里可以触发动画
                // enemy.Animator.SetTrigger("Attack");
                
                // 为了演示，我们延迟执行伤害和重置状态
                // 理想情况下，伤害应该由动画事件触发
                PerformAttack(); 
            }
        }
        
        // 模拟攻击动画/动作的持续时间
        // 假设攻击动作需要0.5秒
        if (_isAttacking && _timer > 0.5f)
        {
            _isAttacking = false; // 攻击动作结束
        }
    }

    private void PerformAttack()
    {
        // 在攻击点创建一个圆形检测区域
        Collider2D hitPlayer = Physics2D.OverlapCircle(_attackPoint.position, _attackRadius, _playerLayer);

        // 如果检测到了玩家
        if (hitPlayer != null)
        {
            // 尝试获取IDamageable接口并造成伤害
            if (hitPlayer.TryGetComponent<IDamageable>(out IDamageable player))
            {
                player.TakeDamage(_damage);
            }
        }
    }

    // 你可以在Unity编辑器的Scene视图中绘制这个攻击范围，方便调试
    public void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
    }
    
    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
        // 为了让SO能访问到场景中的引用，你可能需要敌人动态查找它
        _attackPoint = transform.Find("AttackPoint"); // 假设你的攻击点叫 "AttackPoint"
    }
}