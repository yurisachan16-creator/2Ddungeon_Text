using UnityEngine;

/// <summary>
/// 地板贴花生成规则
/// 满足需求(2)：在地板上随机生成新的纹路地板，视觉覆盖原先的层
/// </summary>
[CreateAssetMenu(fileName = "Rule_FloorDecals", menuName = "Dungeon/Population Rule/Floor Decals", order = 4)]
public class Rule_SpawnFloorDecals : RoomPopulationRule
{
    [Header("生成配置")]
    [Tooltip("地板贴花生成表（不同纹路的Sprite）")]
    public SpawnTable decalTable;
    
    [Tooltip("最少生成数量")]
    public int minAmount = 1;
    
    [Tooltip("最多生成数量")]
    public int maxAmount = 5;
    
    [Header("视觉配置")]
    [Tooltip("贴花的Sorting Layer名称（确保在基础地板之上）")]
    public string sortingLayerName = "Floor";
    
    [Tooltip("贴花的Sorting Order（相对于基础地板）")]
    public int sortingOrder = 1;
    
    [Header("随机旋转")]
    [Tooltip("是否随机旋转贴花")]
    public bool randomRotation = true;
    
    [Header("随机缩放")]
    [Tooltip("是否随机缩放贴花")]
    public bool randomScale = false;
    
    [Tooltip("缩放范围")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    
    protected override void Apply(Room room, bool isStartRoom, bool isBossRoom)
    {
        if (decalTable == null)
        {
            Debug.LogWarning($"Rule [{name}]: DecalTable 未设置！");
            return;
        }
        
        // 重置生成表的房间缓存
        decalTable.ResetRoomCache();
        
        int count = Random.Range(minAmount, maxAmount + 1);
        
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = room.GetRandomFloorPosition();
            
            GameObject prefab = decalTable.GetRandomPrefab(room.stepToStart);
            
            if (prefab != null)
            {
                // 计算旋转
                Quaternion rotation = randomRotation 
                    ? Quaternion.Euler(0, 0, Random.Range(0f, 360f)) 
                    : Quaternion.identity;
                
                GameObject decal = Instantiate(prefab, spawnPos, rotation, room.transform);
                decal.name = $"{prefab.name}_Decal_{i}";
                
                // 设置 Sorting Layer 和 Order（确保视觉覆盖）
                SpriteRenderer renderer = decal.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = sortingLayerName;
                    renderer.sortingOrder = sortingOrder;
                }
                else
                {
                    Debug.LogWarning($"贴花预制体 [{prefab.name}] 缺少 SpriteRenderer 组件！");
                }
                
                // 随机缩放
                if (randomScale)
                {
                    float scale = Random.Range(scaleRange.x, scaleRange.y);
                    decal.transform.localScale = new Vector3(scale, scale, 1f);
                }
            }
        }
    }
}
