// EnemyChaseRanged.cs
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Ranged", menuName = "Enemy Logic/Chase Logic/Ranged Keep Distance")]
public class EnemyChaseRanged : EnemyChaseSOBase
{
    [SerializeField] private float _movementSpeed = 1.5f;

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic(); // 执行基类逻辑 (检查是否脱离仇恨)

        if (playerTransform == null) return;

        // 1. 如果玩家进入了逃跑范围，立刻后退
        if (enemy.IsWithinEscapeDistance)
        {
            Vector2 runDirection = -(playerTransform.position - enemy.transform.position).normalized;
            enemy.MoveEnemy(runDirection * _movementSpeed);
        }
        // 2. 如果玩家在攻击范围内 (但不在逃跑范围内)，切换到攻击状态
        else if (enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
        // 3. 如果玩家在仇恨范围内 (但还不够近去攻击)，则追击
        else
        {
            Vector2 moveDirection = (playerTransform.position - enemy.transform.position).normalized;
            enemy.MoveEnemy(moveDirection * _movementSpeed);
        }
    }
}