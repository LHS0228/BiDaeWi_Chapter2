using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    private MovementRigidBody2D movement2D;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator anim;

    [SerializeField] private Vector2 inputVec;

    [Header("플레이어 스테미너")]
    [SerializeField] private float stamina = 100;

    [HideInInspector] public bool isPlayerStop;
    [HideInInspector] public bool isMoveStop;


    private void Awake()
    {
        movement2D = GetComponent<MovementRigidBody2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!isPlayerStop)
        {
            if (!isMoveStop)
            {
                OnMove();
            }
            OnHide();
        }
        else
            anim.SetBool("isWalk", false);

        if(isPlayerStop || isMoveStop)
            anim.SetBool("isWalk", false);
    }

    private void OnMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        movement2D.MoveTo(new Vector3(x, 0, 0));

        if (x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        if (x > 0 || x < 0)
            anim.SetBool("isWalk", true);
        else
            anim.SetBool("isWalk", false);
    }

    private void OnHide()
    {
        if(Input.GetKey(KeyCode.F))
        {
            StartCoroutine(StaminaCountrol(-1f, 0.2f));
        }
        else if(stamina < 100){
            StartCoroutine(StaminaCountrol(1f, 0.2f));
        }
    }

    IEnumerator StaminaCountrol(float count, float time)
    {
        if(count < 0 && stamina > 0)
        {
            stamina += count;
        }
        else if(count > 0 && stamina <= 100)
        {
            stamina += count;
        }

        yield return time;
    }
}
