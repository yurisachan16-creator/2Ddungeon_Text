using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 门口碰撞器移除工具
/// 自动移除门位置的 Tilemap Tiles，使玩家可以通过
/// </summary>
public class DoorColliderRemover : MonoBehaviour
{
    [Header("门的位置配置")]
    public Vector2Int doorLeftOffset = new Vector2Int(-5, 0);    // 左门相对房间中心的偏移（单位：格子）
    public Vector2Int doorRightOffset = new Vector2Int(5, 0);    // 右门
    public Vector2Int doorUpOffset = new Vector2Int(0, 4);       // 上门
    public Vector2Int doorDownOffset = new Vector2Int(0, -4);    // 下门
    
    [Header("门口开口大小")]
    public int doorWidth = 2;  // 门的宽度（格子数）
    public int doorHeight = 2; // 门的高度（格子数）

    private Room roomScript;
    
    void Start()
    {
        roomScript = GetComponent<Room>();
        if (roomScript == null)
        {
            Debug.LogError("DoorColliderRemover 需要附加在有 Room 脚本的对象上！");
            return;
        }
        
        // 延迟执行，确保墙体已经生成
        StartCoroutine(RemoveDoorTilesDelayed());
    }
    
    IEnumerator RemoveDoorTilesDelayed()
    {
        // 等待一帧，确保墙体 Tilemap 已经生成
        yield return new WaitForEndOfFrame();
        
        RemoveDoorTiles();
    }
    
    /// <summary>
    /// 移除门位置的 Tiles
    /// </summary>
    void RemoveDoorTiles()
    {
        // 查找所有的 Tilemap（墙体通常在子对象中）
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        
        if (tilemaps.Length == 0)
        {
            Debug.LogWarning($"房间 {gameObject.name} 中没有找到 Tilemap 组件");
            return;
        }
        
        Vector3Int roomCenterCell = WorldToCell(transform.position, tilemaps[0]);
        
        // 根据房间的门配置，移除对应位置的 Tiles
        if (roomScript.roomLeft)
        {
            RemoveTilesAtDoor(tilemaps, roomCenterCell, doorLeftOffset, true);
        }
        if (roomScript.roomRight)
        {
            RemoveTilesAtDoor(tilemaps, roomCenterCell, doorRightOffset, true);
        }
        if (roomScript.roomUp)
        {
            RemoveTilesAtDoor(tilemaps, roomCenterCell, doorUpOffset, false);
        }
        if (roomScript.roomDown)
        {
            RemoveTilesAtDoor(tilemaps, roomCenterCell, doorDownOffset, false);
        }
    }
    
    /// <summary>
    /// 移除指定门位置的 Tiles
    /// </summary>
    void RemoveTilesAtDoor(Tilemap[] tilemaps, Vector3Int centerCell, Vector2Int doorOffset, bool isHorizontalDoor)
    {
        Vector3Int doorCenter = centerCell + new Vector3Int(doorOffset.x, doorOffset.y, 0);
        
        // 计算需要清除的区域
        int halfWidth = doorWidth / 2;
        int halfHeight = doorHeight / 2;
        
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap == null) continue;
            
            // 清除门口区域的所有 Tiles
            for (int x = -halfWidth; x <= halfWidth; x++)
            {
                for (int y = -halfHeight; y <= halfHeight; y++)
                {
                    Vector3Int cellPos = doorCenter + new Vector3Int(x, y, 0);
                    tilemap.SetTile(cellPos, null); // 移除 Tile
                }
            }
        }
        
        Debug.Log($"已移除门位置的 Tiles：{doorOffset}");
    }
    
    /// <summary>
    /// 世界坐标转换为 Tilemap Cell 坐标
    /// </summary>
    Vector3Int WorldToCell(Vector3 worldPos, Tilemap tilemap)
    {
        return tilemap.WorldToCell(worldPos);
    }
}
