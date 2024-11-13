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

    private bool isGround;
    private float jumpScan = 1.5f;
    private bool isLongJump;

    [SerializeField] private Vector2 inputVec;

    [Header("[기능 활성화/비활성화]")]
    [SerializeField] private bool onMove;
    [SerializeField] private bool onJump;

    [Header("플레이어 이동속도")]
    [SerializeField] private float speed;
    [Header("플레이어 점프력")]
    [SerializeField] private float jumpPower;

    [Header("점프력")]
    [SerializeField] private float lowJump = 5;
    [SerializeField] private float highJump = 2;
    

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

        if (inputVec.x > 0 || inputVec.x < 0)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }

        RaycastHit2D jumpRay = Physics2D.Raycast(transform.position, Vector2.down, jumpScan, LayerMask.GetMask("Platform"));
        Debug.DrawRay(transform.position, Vector2.down * jumpScan, Color.red);

        if (jumpRay.collider != null && jumpRay.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            if (rigid.velocity.y == 0)
            {
                isGround = true;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isLongJump = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isLongJump = false;
        }

        if (isLongJump && rigid.velocity.y > 0)
        {
            rigid.gravityScale = highJump;
        }
        else
        {
            rigid.gravityScale = lowJump;
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
        if (isGround)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGround = false; 
        }
    }
}
