using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum Enemy_Shield_State { None = -1, Idle = 0, Pursuit, Attack, Die, }

public class Enemy_Shield : MonoBehaviour
{
    [Header("asd")]
    [SerializeField]
    private float attackTerm = 0.5f; // 
    [SerializeField]
    private Transform target;
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    private float attackRange = 5f;
    [SerializeField]
    private float recognizeRange = 8f;
    [SerializeField]
    private float moveSpeed = 2f;



    private Animator animator; 
    private EntityBase entity;
    private float lastAttackTime = 0f; // 
    private bool isDead = false;
    private bool isAttack = false;
    private bool isMove = true;
    private EntityStats stats;

    [SerializeField]
    private LayerMask layerMask;
    int typeIndex;
    private Enemy_Shield_State enemyState = Enemy_Shield_State.None;
    [SerializeField]
    private PlayerAttack playerAttack;

    private float moveStopDuration = 1.5f;
    private float stopTime = 0f;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        entity = GetComponent<EntityBase>();
        animator = GetComponent<Animator>();
        if( playerAttack != null ) 
        {
            playerAttack = FindObjectOfType<PlayerAttack>();
        }
    }

    private void OnEnable()
    {
        ChangeState(Enemy_Shield_State.Idle);
        StartCoroutine(UpdateTarget());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enemyState = Enemy_Shield_State.None;
    }

    private void Update()
    {
        playerAttack.GetWeaponType();
        Debug.Log($"{playerAttack.GetWeaponType()}");
        PlayerWeapon();
    }
    public void Setup(Transform target)
    {
        this.target = target;
    }

    private IEnumerator UpdateTarget()
    {
        while (true)
        {

            if (enemyState == Enemy_Shield_State.Die)
            {
                yield break;
            }
            float distance = Vector2.Distance(transform.position, target.transform.position);

            Collider2D collider1 = Physics2D.OverlapCircle(transform.position, recognizeRange, layerMask);
            if (collider1 != null && collider1.CompareTag("Player"))
            {
                if (distance <= recognizeRange && distance > attackRange)
                {
                    ChangeState(Enemy_Shield_State.Pursuit);
                }
            }
            if (distance > recognizeRange)
            {
                ChangeState(Enemy_Shield_State.Idle);
            }

            Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, layerMask);

            if (collider != null && collider.CompareTag("Player"))
            {
                if (distance <= attackRange)
                {
                    if (Time.time - lastAttackTime >= attackTerm && isDead == false)
                    {
                        ChangeState(Enemy_Shield_State.Attack);
                    }
                }
                else if (distance > attackRange)
                {
                    ChangeState(Enemy_Shield_State.Pursuit);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, recognizeRange);
    }
    private void RecognizeTarget()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance > attackRange && distance <= recognizeRange && isMove)
        {
            Vector2 targetPosition = target.transform.position;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
            animator.SetBool("isWalk", true);
        }
    }
    private void AttackTarget()
    {
        lastAttackTime = Time.time;
    }

    private IEnumerator DieAnimation()
    {
        animator.Play("Enemy_Shield_Die");
        SoundSystem.instance.PlaySound("Enemy", "EnemyDie");

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

    }

    public void ChangeState(Enemy_Shield_State state)
    {
        if (enemyState == state) return;

        if (enemyState == Enemy_Shield_State.Die)
        {
            return;
        }

        StopCoroutine(enemyState.ToString());

        enemyState = state;

        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        while (enemyState == Enemy_Shield_State.Idle)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Shield_State.Die);
            }
            animator.SetBool("isWalk", false);
            animator.Play("Enemy_Shield_Idle");
            yield return null;
        }
    }

    private IEnumerator Pursuit()
    {
        while (enemyState == Enemy_Shield_State.Pursuit)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Shield_State.Die);
            }
            else
            {
                RecognizeTarget();
                animator.Play("Enemy_Shield_Walk");
                yield return null;
            }
        }
    }

    private IEnumerator Attack()
    {
        while (enemyState == Enemy_Shield_State.Attack)
        {
            animator.SetBool("isWalk", false);
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Shield_State.Die);
            }
            else if (Vector2.Distance(transform.position, target.transform.position) > attackRange)
            {
                ChangeState(Enemy_Shield_State.Pursuit);
            }
            else
            {
                stopTime += Time.deltaTime;
                AttackTarget();
                yield return new WaitForSeconds(attackTerm);
            }
        }

    }

    private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;
        boxCollider.enabled = false;
        animator.SetBool("isDead", true);

        yield return StartCoroutine(DieAnimation());
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player") && !isAttack)
        {
            StartCoroutine(AttackAnim(collision.GetComponent<EntityBase>()));
        }
    }

    private IEnumerator AttackAnim(EntityBase player)
    {
        isAttack = true;

        animator.Play("Enemy_Shield_Walk");

        yield return new WaitForSeconds(0.2f);

        if (player != null)
        {
            player.TakeDamage(1);
        }
        StartCoroutine(MoveStop());

        yield return new WaitForSeconds(1f);

        isAttack = false;
    }

    private void PlayerWeapon()
    {
        if(playerAttack.GetWeaponType() != WeaponType.ShotGun)
        {
            entity.IsInvincible = true;
        }
        else
        {
            entity.IsInvincible = false;
        }
    }

    private IEnumerator MoveStop()
    {
        isMove = false;
        ChangeState(Enemy_Shield_State.Idle);
        yield return new WaitForSeconds(moveStopDuration);
        isMove = true;
    }
}