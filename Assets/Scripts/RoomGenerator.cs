using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WallType
{
    //墙体预制体,根据不同的门的组合生成不同的墙体
    public GameObject wallUp, wallDown, wallLeft, wallRight, wallUpDown, wallLeftRight,
        wallUpLeft, wallUpRight, wallDownLeft, wallDownRight,
        wallUpLeftRight, wallDownLeftRight, wallLeftUpDown, wallRightUpDown,
        wallAll;

}

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
    public int maxStepCount; // 最大步数限制
    public List<Room> generatedRooms = new List<Room>(); // 已生成的房间列表
    List<GameObject> farRooms = new List<GameObject>();//存放最远房间列表
    List<GameObject> lessfarRooms = new List<GameObject>();//存放次远房间列表
    List<GameObject> oneWayRooms = new List<GameObject>();//存放单向房间列表
    public WallType wallType; //墙体类型

    void Start()
    {
        for (int i = 0; i < roomNumber; i++)
        {
            generatedRooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>());
            ChangePointPos();
        }

        // 给房间添加颜色
        generatedRooms[0].GetComponent<SpriteRenderer>().color = startColor;

        // 找到最远的房间作为结束房间
        endRoom = generatedRooms[0].gameObject;
        // 遍历房间列表，找到距离生成点最远的房间
        foreach (var room in generatedRooms)
        {
            // if (room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude)
            // {
            //     endRoom = room.gameObject;
            // }

            SetupRoom(room, room.transform.position);
        }
        FindEndRoom();
        // 改变结束房间颜色
        endRoom.GetComponent<SpriteRenderer>().color = endColor;
    }

    void Update()
    {
        //按下R键获取当前房间名字并重新激活
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// 改变生成点位置
    /// </summary>
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
        } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));

    }

    /// <summary>
    /// 设置房间
    /// </summary>
    /// <param name="newRoom"></param>
    /// <param name="roomPosition"></param>
    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);

        newRoom.UpdateRoom(xOffset, yOffset);

        //根据门的数量和位置生成对应的墙体
        switch (newRoom.doorNumber)
        {
            case 1:
                if (newRoom.roomUp && !newRoom.roomDown && !newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallUp, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && newRoom.roomDown && !newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallDown, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && !newRoom.roomDown && newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallLeft, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && !newRoom.roomDown && !newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallRight, roomPosition, Quaternion.identity);
                }
                break;
            case 2:
                if (newRoom.roomUp && newRoom.roomDown && !newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallUpDown, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && !newRoom.roomDown && newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallLeftRight, roomPosition, Quaternion.identity);
                }
                else if (newRoom.roomUp && !newRoom.roomDown && newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallUpLeft, roomPosition, Quaternion.identity);
                }
                else if (newRoom.roomUp && !newRoom.roomDown && !newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallUpRight, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && newRoom.roomDown && newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallDownLeft, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && newRoom.roomDown && !newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallDownRight, roomPosition, Quaternion.identity);
                }
                break;
            case 3:
                if (newRoom.roomUp && newRoom.roomDown && newRoom.roomLeft && !newRoom.roomRight)
                {
                    Instantiate(wallType.wallLeftUpDown, roomPosition, Quaternion.identity);
                }
                else if (newRoom.roomUp && newRoom.roomDown && !newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallRightUpDown, roomPosition, Quaternion.identity);
                }
                else if (newRoom.roomUp && !newRoom.roomDown && newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallUpLeftRight, roomPosition, Quaternion.identity);
                }
                else if (!newRoom.roomUp && newRoom.roomDown && newRoom.roomLeft && newRoom.roomRight)
                {
                    Instantiate(wallType.wallDownLeftRight, roomPosition, Quaternion.identity);
                }
                break;
            case 4:
                Instantiate(wallType.wallAll, roomPosition, Quaternion.identity);
                break;

        }
        //Debug.Log($"Room at {roomPosition}: Up={newRoom.roomUp}, Down={newRoom.roomDown}, Left={newRoom.roomLeft}, Right={newRoom.roomRight}, DoorNum={newRoom.doorNumber}");
    }

    /// <summary>
    /// 查找结束房间
    /// </summary>
    public void FindEndRoom()
    {
        //找到最大步数
        for (int i = 0; i < generatedRooms.Count; i++)
        {
            if (generatedRooms[i].stepToStart > maxStepCount)
            {
                maxStepCount = generatedRooms[i].stepToStart;
            }
        }

        //找到最大步数和次大步数的房间
        foreach (var room in generatedRooms)
        {
            if (room.stepToStart == maxStepCount)
            {
                farRooms.Add(room.gameObject);
            }
            else if (room.stepToStart == maxStepCount - 1)
            {
                lessfarRooms.Add(room.gameObject);
            }
        }

        //找到单向房间
        for (int i = 0; i < farRooms.Count; i++)
        {
            if (farRooms[i].GetComponent<Room>().doorNumber == 1)
            {
                oneWayRooms.Add(farRooms[i]);
            }
        }

        //找到次单向房间
        for (int i = 0; i < lessfarRooms.Count; i++)
        {
            if (lessfarRooms[i].GetComponent<Room>().doorNumber == 1)
            {
                oneWayRooms.Add(lessfarRooms[i]);
            }
        }

        //选择最终房间
        if (oneWayRooms.Count != 0)
        {
            endRoom = oneWayRooms[Random.Range(0, oneWayRooms.Count)];
        }
        else
        {
            endRoom = farRooms[Random.Range(0, farRooms.Count)];
        }
    }

    //举例“以撒的结合”，最终的房间只有一个单向门，然后在里面打败boss，然后结束关卡；
    //制作方法：找到相对初始点较远的房间，在这个房间旁边单独生成一个房间，把这个房间作为最终房间。

    //接下来的选择方案，没有包含U型房间设置。
    //最终的房间，会有一个单独的门通向场景，场景BOSS战斗。
    //在设定的最终的房间，没有门或隐藏的一侧，生成一个单独的门传送到BOSS房间。
    //在预制体的房间添加文本，从初始点出发经过多少的网格步数，能够到达最终房间。

    //获得最大步数的房间以及比它小1的房间列表，在他们当中找到单一出口的房间，获得这个房间。

    //特殊情况，生成正方形房间时，最大数的房间会有两个出口，随机选择一个出口生成最终房间。这不是最佳方案，后续可以优化。

    //为每一个预制体的门都添加一个collider2D作为触发器，只有满足条件时，门会消失，才能够触发传送。
}
