using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 墙壁碰撞器管理器
/// 自动禁用门位置的碰撞器，确保玩家可以通过
/// </summary>
public class WallColliderManager : MonoBehaviour
{
    [Header("门的位置偏移（相对于房间中心）")]
    public float doorWidth = 2f; // 门的宽度
    public float doorHeight = 2f; // 门的高度
    
    [Header("门的位置（与房间中心的偏移）")]
    public Vector2 doorUpOffset = new Vector2(0, 4.5f);
    public Vector2 doorDownOffset = new Vector2(0, -4.5f);
    public Vector2 doorLeftOffset = new Vector2(-8f, 0);
    public Vector2 doorRightOffset = new Vector2(8f, 0);
    
    void Start()
    {
        // 获取父对象（房间）的信息
        Room room = GetComponentInParent<Room>();
        if (room == null)
        {
            Debug.LogWarning("WallColliderManager: 未找到父对象的 Room 组件");
            return;
        }
        
        // 根据房间的门配置，禁用相应位置的碰撞器
        DisableCollidersAtDoors(room);
    }
    
    /// <summary>
    /// 禁用门位置的碰撞器
    /// </summary>
    private void DisableCollidersAtDoors(Room room)
    {
        // 获取所有墙壁碰撞器
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        
        foreach (Collider2D collider in allColliders)
        {
            // 跳过触发器
            if (collider.isTrigger) continue;
            
            Vector2 colliderPos = collider.transform.position;
            Vector2 roomPos = transform.position;
            
            // 检查碰撞器是否在门的位置
            bool shouldDisable = false;
            
            if (room.roomUp && IsInDoorArea(colliderPos, roomPos + doorUpOffset, doorWidth, doorHeight))
            {
                shouldDisable = true;
            }
            if (room.roomDown && IsInDoorArea(colliderPos, roomPos + doorDownOffset, doorWidth, doorHeight))
            {
                shouldDisable = true;
            }
            if (room.roomLeft && IsInDoorArea(colliderPos, roomPos + doorLeftOffset, doorHeight, doorWidth))
            {
                shouldDisable = true;
            }
            if (room.roomRight && IsInDoorArea(colliderPos, roomPos + doorRightOffset, doorHeight, doorWidth))
            {
                shouldDisable = true;
            }
            
            if (shouldDisable)
            {
                collider.enabled = false;
                Debug.Log($"禁用门位置的碰撞器: {collider.gameObject.name}");
            }
        }
    }
    
    /// <summary>
    /// 检查碰撞器是否在门的区域内
    /// </summary>
    private bool IsInDoorArea(Vector2 colliderPos, Vector2 doorPos, float width, float height)
    {
        return Mathf.Abs(colliderPos.x - doorPos.x) < width / 2f &&
               Mathf.Abs(colliderPos.y - doorPos.y) < height / 2f;
    }
}
