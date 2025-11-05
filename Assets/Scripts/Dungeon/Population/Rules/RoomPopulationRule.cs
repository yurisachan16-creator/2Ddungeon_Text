using UnityEngine;

/// <summary>
/// 房间填充规则抽象基类
/// 使用策略模式，每个具体规则继承此类并实现 Apply 方法
/// </summary>
public abstract class RoomPopulationRule : ScriptableObject
{
    [Header("规则描述")]
    [TextArea(3, 5)]
    [Tooltip("此规则的功能说明（供策划备注）")]
    public string description = "在此填写规则说明...";
    
    [Header("启用控制")]
    [Tooltip("是否启用此规则")]
    public bool isEnabled = true;
    
    [Header("房间类型过滤")]
    [Tooltip("是否只应用于起始房间")]
    public bool applyToStartRoom = false;
    
    [Tooltip("是否只应用于Boss房间")]
    public bool applyToBossRoom = false;
    
    [Tooltip("是否应用于普通房间")]
    public bool applyToNormalRooms = true;
    
    /// <summary>
    /// 应用此规则到指定房间
    /// </summary>
    /// <param name="room">要填充的房间</param>
    /// <param name="isStartRoom">是否为起始房间</param>
    /// <param name="isBossRoom">是否为Boss房间</param>
    public void TryApply(Room room, bool isStartRoom, bool isBossRoom)
    {
        if (!isEnabled) return;
        
        // 根据房间类型过滤
        if (isStartRoom && !applyToStartRoom) return;
        if (isBossRoom && !applyToBossRoom) return;
        if (!isStartRoom && !isBossRoom && !applyToNormalRooms) return;
        
        Apply(room, isStartRoom, isBossRoom);
    }
    
    /// <summary>
    /// 具体规则实现（由子类重写）
    /// </summary>
    /// <param name="room">要填充的房间</param>
    /// <param name="isStartRoom">是否为起始房间</param>
    /// <param name="isBossRoom">是否为Boss房间</param>
    protected abstract void Apply(Room room, bool isStartRoom, bool isBossRoom);
}
