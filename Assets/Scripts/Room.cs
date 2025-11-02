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

    public void UpdateRoom()
    {
        //这里的除于18和9是根据房间的x轴的offset和y轴的offset来的
        stepToStart = (int)(Mathf.Abs(transform.position.x) / 18) +
                    (int)(Mathf.Abs(transform.position.y) / 9);
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
}
