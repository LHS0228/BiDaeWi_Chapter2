using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

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
    private bool isSpawn = false;
    private bool isAttack = false;

    [SerializeField]
    private LayerMask layerMask;
    int typeIndex;
    private EnemyState enemyState = EnemyState.None;
    private WeaponType weaponType;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        entity = GetComponent<EntityBase>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
        StartCoroutine(UpdateTarget());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enemyState = EnemyState.None;
    }

    private void Update()
    {
        Debug.Log($"{enemyState}");
        CheckPlayerGunType();
    }
    public void Setup(Transform target)
    {
        this.target = target;
    }

    private IEnumerator UpdateTarget()
    {
        while (true)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            Collider2D collider1 = Physics2D.OverlapCircle(transform.position, recognizeRange, layerMask);
            if (collider1 != null && collider1.CompareTag("Player"))
            {
                if (distance <= recognizeRange && distance > attackRange)
                {
                    ChangeState(EnemyState.Pursuit);
                }
            }
            if (distance > recognizeRange)
            {
                ChangeState(EnemyState.Idle);
            }

            Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, layerMask);

            if (collider != null && collider.CompareTag("Player"))
            {
                if (distance <= attackRange)
                {
                    if (Time.time - lastAttackTime >= attackTerm && isDead == false)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                }
                else if (distance > attackRange)
                {
                    ChangeState(EnemyState.Pursuit);
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
        if (distance > attackRange && distance <= recognizeRange)
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
        animator.Play("Enemy_Die");

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

    }

    public void ChangeState(EnemyState state)
    {
        if (enemyState == state) return;

        StopCoroutine(enemyState.ToString());

        enemyState = state;

        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        while (enemyState == EnemyState.Idle)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(EnemyState.Die);
            }
            animator.SetBool("isWalk", false);
            animator.Play("Enemy_Shield_Idle");
            yield return null;
        }
    }

    private IEnumerator Pursuit()
    {
        while (enemyState == EnemyState.Pursuit)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(EnemyState.Die);
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
        while (enemyState == EnemyState.Attack)
        {
            animator.SetBool("isWalk", false);
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(EnemyState.Die);
            }
            else if (Vector2.Distance(transform.position, target.transform.position) > attackRange)
            {
                ChangeState(EnemyState.Pursuit);
            }
            else
            {
                AttackTarget();
                yield return new WaitForSeconds(attackTerm);
            }
        }

    }

    private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;
        if (!isSpawn)
        {
            isSpawn = true;
        }

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
        animator.SetTrigger("isAttack");

        animator.Play("Enemy_Shield_Walk");

        yield return new WaitForSeconds(0.2f);

        if (player != null)
        {
            player.TakeDamage(1);
        }

        yield return new WaitForSeconds(attackTerm - 0.2f);

        isAttack = false;
    }

    private void CheckPlayerGunType()
    {
        switch (weaponType)
        {
            case WeaponType.None:

                break;
            case WeaponType.Pistol:

                break;
            case WeaponType.Rifle:

                break;
            case WeaponType.ShotGun:

                break;
        }
    }
}