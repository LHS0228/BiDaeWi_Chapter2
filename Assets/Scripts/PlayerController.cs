using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    private MovementRigidBody2D movement2D;
    private MobBase mobBase;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator anim;

    [SerializeField] private Vector2 inputVec;

    public bool isHighGratify;
    public bool isHide;

    [Header("플레이어 정지")]
    public bool isPlayerStop;
    public bool isMoveStop;

    [Header("플레이어 스테미너")]
    [SerializeField] private GameObject staminaBar;
    [SerializeField] private float stamina = 100;
    private Coroutine nowCoroutine;

    [SerializeField] private Vector3 barOriganlSize;
    private Vector3 barOriganlTransform;

    private void Awake()
    {
        movement2D = GetComponent<MovementRigidBody2D>();
        mobBase = GetComponent<MobBase>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        barOriganlSize = staminaBar.transform.localScale;
        barOriganlTransform = staminaBar.transform.position;
    }

    private void Update()
    {
        if (!isPlayerStop)
        {
            OnHide();
        }
    }

    private void FixedUpdate()
    {
        StaminaBarUpdate();
        if (!isPlayerStop)
        {
            if (!isMoveStop)
            {
                OnMove();
            }
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
        else if( x > 0)
            spriteRenderer.flipX = false;

        if (x > 0 || x < 0)
        {
            anim.SetBool("isWalk", true);
            SoundSystem.instance.PlayDelaySounds("Character", "Footstep3", 0.25f);
        }
        else
            anim.SetBool("isWalk", false);
    }

    private void OnHide()
    {
        if(Input.GetKey(KeyCode.F) && isHighGratify && stamina > 0 && !gameObject.GetComponent<PlayerAttack>().isAttackStop)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SoundSystem.instance.PlaySound("Character", "Hide");
            }

            isHide = true;
            anim.SetBool("isHide", true);
            isMoveStop = true;

            if (nowCoroutine == null)
            {
                nowCoroutine = StartCoroutine(StaminaCountrol(-1f, 0.2f));
            }
        }

        else if(!isHide && stamina < 100)
        {
            isHide = false;
            if (nowCoroutine == null)
            {
                nowCoroutine = StartCoroutine(StaminaCountrol(1f, 0.2f));
            }
        }

        if (isHide)
        {
            if (Input.GetKeyUp(KeyCode.F) || Input.GetKey(KeyCode.F) && stamina < 1 || Input.GetKey(KeyCode.F) && gameObject.GetComponent<PlayerAttack>().isAttackStop)
            {
                isMoveStop = false;
                isHide = false;
                anim.SetBool("isHide", false);
            }
        }
    }

    private void StaminaBarUpdate()
    {
        if(stamina == 100)
        {
            if (staminaBar.activeSelf)
            {
                staminaBar.SetActive(false);
            }
        }
        else
        {
            if (!staminaBar.activeSelf)
            {
                staminaBar.SetActive(true);
            }
        }
        staminaBar.transform.localScale = new Vector3(barOriganlSize.x * (stamina/100), barOriganlSize.y, barOriganlSize.z);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hiding"))
        {
            staminaBar.transform.position = new Vector2(staminaBar.transform.position.x, -0.2f);
        }
        else
        {
            staminaBar.transform.position = new Vector2(staminaBar.transform.position.x, barOriganlTransform.y);
        }

    }

    private IEnumerator StaminaCountrol(float count, float time)
    {
        yield return new WaitForSeconds(time);

        if (count < 0 && stamina > 0)
        {
            stamina += count;
        }
        else if (count > 0 && stamina <= 100)
        {
            stamina += count;
        }

        nowCoroutine = null;
    }

    private void OnDead()
    {
        if(mobBase.GetHp() < 0)
        {
            //플레이어 죽음 애니메이션 실행
            //게임 오버 모션
            //씬 넘어가는 장면
            isPlayerStop = true;
            anim.Play("Die");
        }
    }
}
