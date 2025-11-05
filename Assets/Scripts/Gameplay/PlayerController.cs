using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 玩家移动速度
    private Rigidbody2D rb; // 玩家刚体组件
    private Animator anim; // 玩家动画组件
    private Vector2 movement; // 玩家移动方向

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 获取输入
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 切换动画
        if (movement.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }
        SwitchAnim();
    }

    void FixedUpdate()
    {
        // 移动玩家
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void SwitchAnim()
    {
        // 根据移动方向切换动画
        anim.SetFloat("Speed", movement.magnitude);
    }
}
