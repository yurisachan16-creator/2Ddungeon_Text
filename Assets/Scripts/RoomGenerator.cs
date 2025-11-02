using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right };
    public Direction selectedDirection;


    [Header("房间信息")]
    public GameObject roomPrefab; // 房间预制体
    public int roomNumber; // 房间数量
    public Color startColor, endColor; // 起始和结束房间颜色
    private GameObject endRoom; // 结束房间引用

    [Header("位置控制")]
    public Transform generatorPoint; // 生成点
    public float xOffset; // X轴偏移量
    public float yOffset; // Y轴偏移量
    public LayerMask roomLayer; // 房间图层

    public List<GameObject> generatedRooms = new List<GameObject>(); // 已生成的房间列表

    void Start()
    {
        for (int i = 0; i < roomNumber; i++)
        {
            generatedRooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity));

            //改变Point位置
            ChangePointPos();

        }

        //给房间添加颜色
        generatedRooms[0].GetComponent<SpriteRenderer>().color = startColor;

        //找到最远的房间作为结束房间
        endRoom = generatedRooms[0];
        //遍历房间列表，找到距离生成点最远的房间
        foreach (var room in generatedRooms)
        {
            if (room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude)
            {
                endRoom = room;
            }
        }
        //改变结束房间颜色
        endRoom.GetComponent<SpriteRenderer>().color = endColor;
        
    }

    void Update()
    {
        //按下任意键获取当前房间名字并重新激活
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ChangePointPos()
    {
        do
        {
            selectedDirection = (Direction)Random.Range(0, 4);
            switch (selectedDirection)
            {
                case Direction.Up:
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.Down:
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.Left:
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.Right:
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
        } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f,roomLayer));

    }
}
