using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single Straight Projectile", menuName = "Enemy Logic/Attack Logic/Single Straight Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase
{
    [SerializeField] private Rigidbody2D BulletPrefab;
    [SerializeField] private float _timeBetweenShots = 2f;
    [SerializeField] private float _timeTillExit = 3f;
    [SerializeField] private float _distanceToCountExit = 3f;
    [SerializeField] private float _bulletSpeed = 10f;

    private float _timer;
    private float _exitTimer;
    
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

        enemy.MoveEnemy(Vector2.zero);

        if (_timer > _timeBetweenShots)
        {
            _timer = 0f;

            // 发射子弹
            Vector2 direction = (playerTransform.position - enemy.transform.position).normalized;
            Rigidbody2D bullet = GameObject.Instantiate(BulletPrefab, enemy.transform.position, Quaternion.identity);
            bullet.velocity = direction * _bulletSpeed;
        }
        if (Vector2.Distance(playerTransform.position, enemy.transform.position) > _distanceToCountExit)
        {
            _exitTimer += Time.deltaTime;
            if (_exitTimer >= _timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
        }
        else
        {
            _timer += Time.deltaTime;
        }
        
        _timer += Time.deltaTime;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }



}
