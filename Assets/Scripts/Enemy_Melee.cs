using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public enum Enemy_Melee_State { None = -1, Idle = 0, Pursuit, Attack, Die, }

public class Enemy_Melee : MonoBehaviour
{
    // isWalk , isDead, isAttack
    [Header("공격 설정")]
    [SerializeField]
    private float attackTerm = 0.5f; // 공격 텀
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



    private Animator animator; // 애니 관련
    private EntityBase entity;
    private float lastAttackTime = 0f; // 공격 텀에 사용됨
    private bool isDead = false;
    private bool isSpawn = false;
    private bool isAttack = false;
    // Enemy_Melee_Walk, Enemy_Melee_Attack
    [SerializeField]
    private LayerMask layerMask;
    int typeIndex;
    private Enemy_Melee_State enemyState = Enemy_Melee_State.None;
    private AudioSource audioSource;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        entity = GetComponent<EntityBase>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ChangeState(Enemy_Melee_State.Idle);
        StartCoroutine(UpdateTarget());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enemyState = Enemy_Melee_State.None;
    }

    private void Update()
    {
        Debug.Log($"{enemyState}");
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
                    ChangeState(Enemy_Melee_State.Pursuit);
                }
            }
            if (distance > recognizeRange && !isDead)
            {
                ChangeState(Enemy_Melee_State.Idle);
            }

            Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, layerMask);

            if (collider != null && collider.CompareTag("Player"))
            {
                if (distance <= attackRange)
                {
                    if (Time.time - lastAttackTime >= attackTerm && isDead == false)
                    {
                        ChangeState(Enemy_Melee_State.Attack);
                    }
                }
                else if (distance > attackRange)
                {
                    ChangeState(Enemy_Melee_State.Pursuit);
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
        SoundSystem.instance.PlaySound("Enemy", "EnemyDie");
        animator.Play("Enemy_Die");

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);

    }

    public void ChangeState(Enemy_Melee_State state)
    {
        if (enemyState == state) return;

        StopCoroutine(enemyState.ToString());

        enemyState = state;

        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        while (enemyState == Enemy_Melee_State.Idle)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Melee_State.Die);
            }
            animator.SetBool("isWalk", false);
            animator.Play("Enemy_Melee_Idle");
            yield return null;
        }
    }

    private IEnumerator Pursuit()
    {
        while (enemyState == Enemy_Melee_State.Pursuit)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Melee_State.Die);
            }
            else
            {
                RecognizeTarget();
                animator.Play("Enemy_Melee_Walk");
                SoundSystem.instance.PlayDelaySounds("Character", "Footstep3", 0.5f);
                yield return null;
            }
        }
    }

    private IEnumerator Attack()
    {
        while (enemyState == Enemy_Melee_State.Attack)
        {
            animator.SetBool("isWalk", false);
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Melee_State.Die);
            }
            else if (Vector2.Distance(transform.position, target.transform.position) > attackRange)
            {
                ChangeState(Enemy_Melee_State.Pursuit);
            }
            else
            {
                AttackTarget();
                yield return new WaitForSeconds(attackTerm);
                SoundSystem.instance.PlaySound("Enemy", "EnemyMelee"); // 시간 수정 필요할수도
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

        animator.Play("Enemy_Melee_Attack");

        
        yield return new WaitForSeconds(0.2f);

        if (player != null) 
        {
            player.TakeDamage(1);
        }

        yield return new WaitForSeconds(attackTerm - 0.2f);

        isAttack = false;
    }

}