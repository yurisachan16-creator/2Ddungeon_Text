using UnityEngine;
using UnityEditor;

/// <summary>
/// Room 编辑器扩展 - 可视化生成区域
/// 在 Scene 视图中绘制生成区域的 Gizmos
/// </summary>
[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    private void OnSceneGUI()
    {
        Room room = (Room)target;
        
        if (room == null) return;
        
        Vector3 roomPos = room.transform.position;
        
        // 绘制地板生成区域
        Handles.color = new Color(0f, 1f, 0f, 0.2f); // 绿色半透明
        DrawRectangle(roomPos, room.floorAreaSize);
        
        // 绘制地板区域边框
        Handles.color = Color.green;
        Handles.DrawWireCube(roomPos, new Vector3(room.floorAreaSize.x, room.floorAreaSize.y, 0f));
        
        // 绘制墙壁装饰区域
        Handles.color = new Color(1f, 0.5f, 0f, 0.3f); // 橙色半透明
        
        // 上墙装饰区域
        if (!room.roomUp)
        {
            Vector3 topWallPos = roomPos + new Vector3(0, room.wallDecoYOffset, 0);
            DrawRectangle(topWallPos, new Vector2(room.wallDecoAreaSize.x, 1f));
            Handles.Label(topWallPos + Vector3.up * 0.5f, "Top Wall Deco", EditorStyles.boldLabel);
        }
        
        // 下墙装饰区域
        if (!room.roomDown)
        {
            Vector3 bottomWallPos = roomPos + new Vector3(0, -room.wallDecoYOffset, 0);
            DrawRectangle(bottomWallPos, new Vector2(room.wallDecoAreaSize.x, 1f));
            Handles.Label(bottomWallPos - Vector3.up * 0.5f, "Bottom Wall Deco", EditorStyles.boldLabel);
        }
        
        // 左墙装饰区域
        if (!room.roomLeft)
        {
            Vector3 leftWallPos = roomPos + new Vector3(-room.wallDecoAreaSize.x / 2f, 0, 0);
            DrawRectangle(leftWallPos, new Vector2(1f, room.wallDecoAreaSize.y));
            Handles.Label(leftWallPos - Vector3.right * 0.5f, "Left", EditorStyles.boldLabel);
        }
        
        // 右墙装饰区域
        if (!room.roomRight)
        {
            Vector3 rightWallPos = roomPos + new Vector3(room.wallDecoAreaSize.x / 2f, 0, 0);
            DrawRectangle(rightWallPos, new Vector2(1f, room.wallDecoAreaSize.y));
            Handles.Label(rightWallPos + Vector3.right * 0.5f, "Right", EditorStyles.boldLabel);
        }
        
        // 在房间中心显示信息
        Handles.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.cyan;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string info = $"Depth: {room.stepToStart}\nDoors: {room.doorNumber}";
        Handles.Label(roomPos + Vector3.down * 3f, info, style);
    }
    
    /// <summary>
    /// 绘制填充的矩形区域
    /// </summary>
    private void DrawRectangle(Vector3 center, Vector2 size)
    {
        Vector3 halfSize = new Vector3(size.x / 2f, size.y / 2f, 0f);
        
        Vector3[] corners = new Vector3[4]
        {
            center + new Vector3(-halfSize.x, -halfSize.y, 0),
            center + new Vector3(halfSize.x, -halfSize.y, 0),
            center + new Vector3(halfSize.x, halfSize.y, 0),
            center + new Vector3(-halfSize.x, halfSize.y, 0)
        };
        
        Handles.DrawSolidRectangleWithOutline(corners, Handles.color, Color.clear);
    }
}
