// EnemyChaseMelee.cs
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Melee", menuName = "Enemy Logic/Chase Logic/Melee")]
public class EnemyChaseMelee : EnemyChaseSOBase
{
    [SerializeField] private float _movementSpeed = 2f;

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic(); // 执行基类逻辑 (检查是否脱离仇恨)

        if (playerTransform == null) return;

        // 如果进入了近战范围，切换到攻击状态
        if (enemy.IsWithinMeleeDistance)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
            return;
        }

        // 否则，朝玩家移动
        Vector2 moveDirection = (playerTransform.position - enemy.transform.position).normalized;
        enemy.MoveEnemy(moveDirection * _movementSpeed);
    }
}