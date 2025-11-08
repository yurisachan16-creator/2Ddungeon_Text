using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Random Wander", menuName = "Enemy Logic/Idle Logic/Random Wander")]
public class EnemyIdleRandomWander : EnemyIdleSOBase
{
    [SerializeField]public float RandomMovementRange = 5f;
    [SerializeField] public float RandomMovementSpeed = 1f;
     
     private Vector3 _targetPos;
    private Vector3 _direction;
    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _targetPos = GetRandomPointInCircle();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        //计算方向
        _direction = (_targetPos - enemy.transform.position).normalized;
        //移动敌人
        enemy.MoveEnemy(_direction * RandomMovementSpeed);
        //检查是否到达目标点,如果到达则重新选择目标点
        if((enemy.transform.position - _targetPos).magnitude < 0.01f)
        {
            _targetPos = GetRandomPointInCircle();
        }
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

    /// <summary>
    /// 获取圆形范围内的随机点
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPointInCircle()
    {
        return enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * RandomMovementRange;
    }
}
