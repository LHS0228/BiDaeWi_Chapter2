using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    [SerializeField] private Vector2 inputVec;
    [Header("플레이어 이동속도")]
    [SerializeField] private float speed;
    [Header("플레이어 점프력")]
    [SerializeField] private float jumpPower;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 nextVec = new Vector2(inputVec.x * speed * Time.fixedDeltaTime, rigid.velocity.y);
        rigid.velocity = new Vector2(inputVec.x * speed, rigid.velocity.y);
        //rigid.MovePosition(new Vector2(rigid.position.x + nextVec.x, rigid.velocity.y));

        if (inputVec.x > 0 || inputVec.x < 0)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
    }

    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();

        switch(value.Get<Vector2>().x)
        {
            case 1:
                spriteRenderer.flipX = false;
                break;
            case -1:
                spriteRenderer.flipX = true;
                break;
            case 0:
                break;
        }
    }

    private void OnJump(InputValue value)
    {
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        Debug.Log("점프");
    }
}
