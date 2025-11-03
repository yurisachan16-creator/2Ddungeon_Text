using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机控制器，用于跟随玩家
/// 后续会使用Cinemachine替代
/// </summary>
public class CameraController : MonoBehaviour
{

    public static CameraController instance; // 单例实例
    public Transform target; // 目标对象（玩家）
    public float smoothSpeed = 0.125f; // 平滑移动速度
    
    private Transform currentRoom; // 当前房间
    private Transform player; // 玩家Transform引用
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 自动查找玩家
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                player = target;
            }
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition;
            
            // 如果有当前房间，相机锁定在房间中心
            if (currentRoom != null)
            {
                desiredPosition = new Vector3(currentRoom.position.x, currentRoom.position.y, transform.position.z);
            }
            else
            {
                // 没有房间时跟随玩家
                desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            }
            
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
    
    /// <summary>
    /// 切换到新房间
    /// </summary>
    /// <param name="roomTransform">房间的Transform</param>
    public void ChangeRoom(Transform roomTransform)
    {
        currentRoom = roomTransform;
    }
}
