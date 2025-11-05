using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameObject doorLeft, doorRight, doorUp, doorDown;// 房间四个方向的门对象
    public bool roomLeft, roomRight, roomUp, roomDown;// 房间四个方向是否有门
    public Text textStep; // 显示从起始房间到该房间的步数
    public int stepToStart; // 从起始房间到该房间的步数
    public int doorNumber;// 房间门的数量
    
    [Header("生成区域配置 (Population System)")]
    [Tooltip("地板可生成区域大小（相对于房间中心）")]
    public Vector2 floorAreaSize = new Vector2(14f, 8f);
    
    [Tooltip("墙壁装饰生成区域大小")]
    public Vector2 wallDecoAreaSize = new Vector2(16f, 1.5f);
    
    [Tooltip("墙壁装饰Y轴偏移（相对于房间中心）")]
    public float wallDecoYOffset = 4.5f;
    
    [Tooltip("碰撞检测图层（用于避免生成位置重叠）")]
    public LayerMask obstacleLayer;
    
    [Tooltip("生成位置重试次数上限")]
    public int maxRetryAttempts = 10;
    void Start()
    {
        // 激活/隐藏门，并确保门不会阻挡玩家
        SetupDoor(doorLeft, roomLeft);
        SetupDoor(doorRight, roomRight);
        SetupDoor(doorUp, roomUp);
        SetupDoor(doorDown, roomDown);
    }
    
    /// <summary>
    /// 设置门的状态，确保门不会阻挡玩家通行
    /// </summary>
    /// <param name="door">门对象</param>
    /// <param name="shouldBeActive">是否应该激活</param>
    private void SetupDoor(GameObject door, bool shouldBeActive)
    {
        if (door == null) return;
        
        door.SetActive(shouldBeActive);
        
        if (shouldBeActive)
        {
            // 如果门被激活（有相邻房间），确保碰撞器设置为触发器或禁用
            Collider2D doorCollider = door.GetComponent<Collider2D>();
            if (doorCollider != null)
            {
                // 将碰撞器设置为触发器，这样玩家可以穿过
                doorCollider.isTrigger = true;
            }
            
            // 也检查子对象的碰撞器
            Collider2D[] childColliders = door.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in childColliders)
            {
                collider.isTrigger = true;
            }
        }
    }

    public void UpdateRoom(float xOffset, float yOffset)
    {
        //这里的除于18和9是根据房间的x轴的offset和y轴的offset来的
        stepToStart = (int)(Mathf.Abs(transform.position.x) / xOffset) +
                    (int)(Mathf.Abs(transform.position.y) / yOffset);
        textStep.text = stepToStart.ToString();

        // 重置doorNumber，避免重复累加导致的错误墙体生成
        doorNumber = 0;
        
        if (roomUp)
        {
            doorNumber++;
        }
        if (roomDown)
        {
            doorNumber++;
        }
        if (roomLeft)
        {
            doorNumber++;
        }
        if (roomRight)
        {
            doorNumber++;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 将相机目标设置为当前房间
            CameraController.instance.ChangeRoom(this.transform);
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 玩家离开房间时不做处理，由进入新房间时触发切换
        }
    }
    
    #region 生成系统辅助方法 (Population System Helpers)
    
    /// <summary>
    /// 获取一个随机的地板生成位置
    /// </summary>
    /// <returns>房间内的随机位置（世界坐标）</returns>
    public Vector3 GetRandomFloorPosition()
    {
        float x = Random.Range(-floorAreaSize.x / 2f, floorAreaSize.x / 2f);
        float y = Random.Range(-floorAreaSize.y / 2f, floorAreaSize.y / 2f);
        return transform.position + new Vector3(x, y, 0);
    }
    
    /// <summary>
    /// 获取一个有效的地板生成位置（带碰撞检测）
    /// </summary>
    /// <param name="checkRadius">碰撞检测半径</param>
    /// <returns>有效位置，如果失败返回 Vector3.zero</returns>
    public Vector3 GetValidFloorPosition(float checkRadius = 0.5f)
    {
        for (int i = 0; i < maxRetryAttempts; i++)
        {
            Vector3 pos = GetRandomFloorPosition();
            
            // 检查该位置是否被占用
            if (!Physics2D.OverlapCircle(pos, checkRadius, obstacleLayer))
            {
                return pos;
            }
        }
        
        Debug.LogWarning($"Room [{gameObject.name}]: 无法找到有效的地板生成位置（重试{maxRetryAttempts}次失败）");
        return Vector3.zero; // 返回标记值表示失败
    }
    
    /// <summary>
    /// 获取指定墙壁方向的随机装饰位置
    /// </summary>
    /// <param name="direction">墙壁方向</param>
    /// <returns>墙壁装饰位置（世界坐标），如果该方向无墙返回 null</returns>
    public Vector3? GetRandomWallPosition(WallDirection direction)
    {
        // 检查该方向是否有墙（即该方向没有相邻房间）
        bool hasWall = false;
        Vector3 offset = Vector3.zero;
        
        switch (direction)
        {
            case WallDirection.Up:
                hasWall = !roomUp;
                offset = new Vector3(0, wallDecoYOffset, 0);
                break;
            case WallDirection.Down:
                hasWall = !roomDown;
                offset = new Vector3(0, -wallDecoYOffset, 0);
                break;
            case WallDirection.Left:
                hasWall = !roomLeft;
                offset = new Vector3(-wallDecoAreaSize.x / 2f, 0, 0);
                break;
            case WallDirection.Right:
                hasWall = !roomRight;
                offset = new Vector3(wallDecoAreaSize.x / 2f, 0, 0);
                break;
        }
        
        if (!hasWall) return null; // 该方向没有墙
        
        // 根据方向生成随机位置
        float randomX = 0f, randomY = 0f;
        
        if (direction == WallDirection.Up || direction == WallDirection.Down)
        {
            randomX = Random.Range(-wallDecoAreaSize.x / 2f, wallDecoAreaSize.x / 2f);
        }
        else
        {
            randomY = Random.Range(-wallDecoAreaSize.y / 2f, wallDecoAreaSize.y / 2f);
        }
        
        return transform.position + offset + new Vector3(randomX, randomY, 0);
    }
    
    /// <summary>
    /// 检查某个方向是否有墙（没有相邻房间）
    /// </summary>
    public bool HasWall(WallDirection direction)
    {
        switch (direction)
        {
            case WallDirection.Up: return !roomUp;
            case WallDirection.Down: return !roomDown;
            case WallDirection.Left: return !roomLeft;
            case WallDirection.Right: return !roomRight;
            default: return false;
        }
    }
    
    #endregion
}

/// <summary>
/// 墙壁方向枚举
/// </summary>
public enum WallDirection
{
    Up,
    Down,
    Left,
    Right
}
