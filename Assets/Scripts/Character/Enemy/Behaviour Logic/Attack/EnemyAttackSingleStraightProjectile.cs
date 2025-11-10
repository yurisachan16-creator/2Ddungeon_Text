using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single Straight Projectile", menuName = "Enemy Logic/Attack Logic/Single Straight Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase
{
    [SerializeField] private Rigidbody2D BulletPrefab;  // 子弹预制体
    [SerializeField] private float _timeBetweenShots = 2f;  // 射击间隔时间
    [SerializeField] private float _timeTillExit = 3f;  // 超出距离后切换状态所需时间
    [SerializeField] private float _distanceToCountExit = 3f;  // 超出该距离后开始计时切换状态
    [SerializeField] private float _bulletSpeed = 10f;  // 子弹速度

    private float _timer;   // 射击计时器
    private float _exitTimer;   // 退出计时器
    
    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);

    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        // 添加空值检查
        if (playerTransform == null)
        {
            Debug.LogWarning("EnemyAttackSingleStraightProjectile: playerTransform 为空！");
            return;
        }

        //Debug.Log($"远程骷髅状态: IsWithinEscapeDistance={enemy.IsWithinEscapeDistance}, IsWithinStrikingDistance={enemy.IsWithinStrikingDistance}");

        // --- 新增的最高优先级检查 ---
        // 如果玩家太近了，立刻停止攻击，返回追逐状态（追逐状态会处理逃跑逻辑）
        if (enemy.IsWithinEscapeDistance)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        //如果外部触发器认为玩家已经不在攻击范围内，则返回追逐状态
        if (!enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return; // 直接返回，不再执行下面的攻击代码
        }

        

        // 如果玩家在攻击范围内，敌人应该停下进行射击
        enemy.MoveEnemy(Vector2.zero);

        if (_timer > _timeBetweenShots)
        {
            _timer = 0f;

            // 发射子弹前检查预制体
            if (BulletPrefab != null)
            {
                Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;
                Rigidbody2D bullet = GameObject.Instantiate(BulletPrefab, enemy.transform.position, Quaternion.identity);
                bullet.velocity = direction * _bulletSpeed;
            }
            else
            {
                Debug.LogWarning("EnemyAttackSingleStraightProjectile: BulletPrefab 未设置！");
            }
        }
        
        _timer += Time.deltaTime;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        this.enemy = enemy;
        
        // 修改：添加空值检查
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyIdleSOBase: 未找到带有 'Player' 标签的对象。");
        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }



}
