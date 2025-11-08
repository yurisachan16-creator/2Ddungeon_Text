using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可移动接口
/// </summary>
public interface IMoveable
{
    Rigidbody2D RB { get; set; }
    bool IsFacingRight { get; set; }
    void MoveEnemy(Vector2 velocity);
    void CheckForLeftOrRightFacing(Vector2 velocity);
}
