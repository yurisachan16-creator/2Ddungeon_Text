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

    void OnTriggerEnter2D(Collider2D collision)
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
}
