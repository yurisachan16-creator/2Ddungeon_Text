using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 地牢填充管理器（核心控制器）
/// 负责协调所有生成规则，在地牢生成后填充内容
/// </summary>
public class DungeonPopulationManager : MonoBehaviour
{
    [Header("规则配置")]
    [Tooltip("应用于起始房间的规则列表")]
    public List<RoomPopulationRule> startRoomRules = new List<RoomPopulationRule>();
    
    [Tooltip("应用于普通房间的规则列表")]
    public List<RoomPopulationRule> normalRoomRules = new List<RoomPopulationRule>();
    
    [Tooltip("应用于Boss房间的规则列表")]
    public List<RoomPopulationRule> bossRoomRules = new List<RoomPopulationRule>();
    
    [Header("Boss房间特殊配置")]
    [Tooltip("Boss房间出口预制体（梯子等）")]
    public GameObject bossExitPrefab;
    
    [Tooltip("出口生成位置偏移（相对于房间中心）")]
    public Vector3 exitSpawnOffset = Vector3.zero;
    
    [Header("调试配置")]
    [Tooltip("在控制台显示详细的生成日志")]
    public bool showDebugLogs = true;
    
    /// <summary>
    /// 填充整个地牢（主入口方法）
    /// </summary>
    /// <param name="allRooms">所有生成的房间列表</param>
    /// <param name="startRoom">起始房间</param>
    /// <param name="endRoom">结束房间（Boss房间）</param>
    public void PopulateDungeon(List<Room> allRooms, Room startRoom, Room endRoom)
    {
        if (allRooms == null || allRooms.Count == 0)
        {
            Debug.LogError("DungeonPopulationManager: 房间列表为空，无法填充！");
            return;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>===== 开始填充地牢 =====</color>");
            Debug.Log($"总房间数: {allRooms.Count}, 起始房间: {startRoom?.name}, Boss房间: {endRoom?.name}");
        }
        
        foreach (Room room in allRooms)
        {
            if (room == null) continue;
            
            // 判断房间类型
            bool isStartRoom = (room == startRoom);
            bool isBossRoom = (room == endRoom);
            
            if (showDebugLogs)
            {
                string roomType = isStartRoom ? "起始" : (isBossRoom ? "Boss" : "普通");
                Debug.Log($"填充房间: [{room.name}] 类型:{roomType}, 深度:{room.stepToStart}");
            }
            
            // 根据房间类型应用对应的规则
            if (isStartRoom)
            {
                ApplyRules(startRoomRules, room, isStartRoom, isBossRoom);
            }
            else if (isBossRoom)
            {
                ApplyRules(bossRoomRules, room, isStartRoom, isBossRoom);
                SpawnBossExit(room);
            }
            else
            {
                ApplyRules(normalRoomRules, room, isStartRoom, isBossRoom);
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>===== 地牢填充完成 =====</color>");
        }
    }
    
    /// <summary>
    /// 应用规则列表到指定房间
    /// </summary>
    private void ApplyRules(List<RoomPopulationRule> rules, Room room, bool isStartRoom, bool isBossRoom)
    {
        if (rules == null || rules.Count == 0) return;
        
        foreach (var rule in rules)
        {
            if (rule == null)
            {
                Debug.LogWarning($"规则列表中存在空引用，已跳过");
                continue;
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"  应用规则: [{rule.name}]");
            }
            
            rule.TryApply(room, isStartRoom, isBossRoom);
        }
    }
    
    /// <summary>
    /// 在Boss房间生成出口（满足需求4）
    /// </summary>
    private void SpawnBossExit(Room bossRoom)
    {
        if (bossExitPrefab == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("Boss房间: 未配置出口预制体（bossExitPrefab）");
            }
            return;
        }
        
        Vector3 exitPos = bossRoom.transform.position + exitSpawnOffset;
        GameObject exit = Instantiate(bossExitPrefab, exitPos, Quaternion.identity, bossRoom.transform);
        exit.name = "BossRoom_Exit";
        
        // 默认设为非激活，等待Boss被击败
        exit.SetActive(false);
        
        // 尝试关联 BossRoomController
        BossRoomController controller = bossRoom.GetComponent<BossRoomController>();
        if (controller != null)
        {
            controller.exitObject = exit;
            
            if (showDebugLogs)
            {
                Debug.Log($"  ✔ Boss房间出口已生成并关联到 BossRoomController");
            }
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"  ⚠ Boss房间缺少 BossRoomController 组件，出口将无法自动激活！");
            }
        }
    }
}
