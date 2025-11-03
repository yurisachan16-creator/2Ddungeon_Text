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
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
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
