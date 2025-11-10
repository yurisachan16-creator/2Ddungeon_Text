using UnityEngine;

public class EnemyEscapeDistanceCheck : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("<color=green>玩家进入了逃跑范围！</color>");
            _enemy.SetEscapeDistanceBool(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
             Debug.Log("<color=red>玩家离开了逃跑范围！</color>");
            _enemy.SetEscapeDistanceBool(false);
        }
    }
}