using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Boss房间控制器
/// 负责管理Boss战斗流程和出口激活（满足需求4）
/// </summary>
public class BossRoomController : MonoBehaviour
{
    [Header("Boss配置")]
    [Tooltip("Boss敌人引用（如果为空则自动搜索子对象）")]
    public CharacterBase bossEnemy;
    
    [Header("出口配置")]
    [Tooltip("出口物体（梯子等）- 通常由 DungeonPopulationManager 自动设置")]
    public GameObject exitObject;
    
    [Header("特效配置")]
    [Tooltip("Boss死亡后播放的特效")]
    public GameObject victoryEffectPrefab;
    
    [Tooltip("特效生成位置偏移")]
    public Vector3 effectOffset = Vector3.zero;
    
    [Header("音效配置")]
    [Tooltip("Boss死亡音效")]
    public AudioClip victorySound;
    
    [Header("调试")]
    [Tooltip("显示调试日志")]
    public bool showDebugLogs = true;
    
    private bool bossDefeated = false;
    
    void Start()
    {
        // 如果没有手动设置Boss引用，尝试自动查找
        if (bossEnemy == null)
        {
            bossEnemy = GetComponentInChildren<CharacterBase>();
        }
        
        // 订阅Boss死亡事件
        if (bossEnemy != null)
        {
            // 假设 CharacterBase 有 OnDeath 事件（需要确认你的角色基类实现）
            // 如果没有，可能需要使用其他方式监听Boss死亡
            
            if (showDebugLogs)
            {
                Debug.Log($"BossRoomController: 已关联Boss [{bossEnemy.name}]");
            }
            
            // 注意：这里假设 CharacterBase 有 OnDeath 事件
            // 如果你的实现不同，需要调整这部分代码
            SubscribeToBossDeath();
        }
        else
        {
            Debug.LogWarning($"BossRoomController: 未找到Boss敌人引用！");
        }
        
        // 确保出口初始为非激活状态
        if (exitObject != null)
        {
            exitObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 订阅Boss死亡事件（需要根据你的 CharacterBase 实现调整）
    /// </summary>
    private void SubscribeToBossDeath()
    {
        // 方案1：如果 CharacterBase 有 OnDeath 事件
        // bossEnemy.OnDeath += OnBossDefeated;
        
        // 方案2：如果使用轮询检测（临时方案）
        StartCoroutine(CheckBossStatus());
    }
    
    /// <summary>
    /// 轮询检测Boss状态（临时方案）
    /// 更好的做法是在 CharacterBase 中实现死亡事件
    /// </summary>
    private System.Collections.IEnumerator CheckBossStatus()
    {
        while (!bossDefeated && bossEnemy != null)
        {
            yield return new WaitForSeconds(0.5f);
            
            // 检查Boss是否被销毁或禁用（表示死亡）
            if (bossEnemy == null || !bossEnemy.gameObject.activeInHierarchy)
            {
                OnBossDefeated();
                break;
            }
            
            // 如果 CharacterBase 有 isDead 属性
            // if (bossEnemy.isDead)
            // {
            //     OnBossDefeated();
            //     break;
            // }
        }
    }
    
    /// <summary>
    /// Boss被击败时调用
    /// </summary>
    private void OnBossDefeated()
    {
        if (bossDefeated) return; // 防止重复触发
        
        bossDefeated = true;
        
        if (showDebugLogs)
        {
            Debug.Log($"<color=yellow>🎉 Boss被击败！</color>");
        }
        
        // 延迟激活出口（增加戏剧性）
        Invoke(nameof(ActivateExit), 1f);
        
        // 播放特效
        if (victoryEffectPrefab != null)
        {
            Vector3 effectPos = transform.position + effectOffset;
            Instantiate(victoryEffectPrefab, effectPos, Quaternion.identity);
        }
        
        // 播放音效
        if (victorySound != null)
        {
            AudioSource.PlayClipAtPoint(victorySound, transform.position);
        }
    }
    
    /// <summary>
    /// 激活出口
    /// </summary>
    private void ActivateExit()
    {
        if (exitObject != null)
        {
            exitObject.SetActive(true);
            
            if (showDebugLogs)
            {
                Debug.Log($"✔ Boss房间出口已激活: [{exitObject.name}]");
            }
            
            // 可以添加出口激活的特效
            // 例如：光效、粒子效果等
        }
        else
        {
            Debug.LogWarning("BossRoomController: 出口物体未设置，无法激活！");
        }
    }
    
    void OnDestroy()
    {
        // 取消订阅事件（如果使用事件方案）
        // if (bossEnemy != null)
        // {
        //     bossEnemy.OnDeath -= OnBossDefeated;
        // }
    }
}
