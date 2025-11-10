using UnityEngine;

public class EnemyMeleeDistanceCheck : MonoBehaviour
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
             Debug.Log("<color=red>玩家进入了近战范围！</color>");
            _enemy.SetMeleeDistanceBool(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
             Debug.Log("<color=red>玩家离开了近战范围！</color>");
            _enemy.SetMeleeDistanceBool(false);
        }
    }
}