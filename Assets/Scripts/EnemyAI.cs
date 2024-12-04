using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum EnemyState { None = -1, Idle = 0, Pursuit, Attack, Die, }

public class EnemyAI : MonoBehaviour
{
    [Header("기타 설정")]
    [SerializeField]
    private GameObject BulletPrefab; // 총알 프리팹
    [SerializeField]
    private Transform SpawnPoint; // 총알 생성 위치
    [SerializeField]
    private GameObject[] GunObjectPrefab;

    [Header("공격 설정")]
    [SerializeField]
    private float attackTerm = 0.5f; // 공격 텀
    [SerializeField]
    private Transform target;

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

    [SerializeField]
    private LayerMask layerMask;
    int typeIndex;
    private EnemyState enemyState = EnemyState.None;

    private void Awake()
    {
        entity = GetComponent<EntityBase>();
        animator = GetComponent<Animator>();
        typeIndex = (int)entity.Stats.mobType;
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
            if (distance > recognizeRange && !isDead)
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
        GameObject clone = Instantiate(BulletPrefab, SpawnPoint.position, SpawnPoint.rotation);
        clone.GetComponent<Projectile>().Setup(target.position);
    }

    private IEnumerator DieAnimation()
    {
        SoundSystem.instance.PlaySound("Enemy", "EnemyDie");
        animator.Play("Enemy_Die");

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);

    }

    private IEnumerator SpawnItem()
    {
        switch (typeIndex)
        {
            case 2:
                GameObject gunpistol = Instantiate(GunObjectPrefab[0], transform.position, transform.rotation);
                isSpawn = false;
                break;
            case 3:
                GameObject gunrifle = Instantiate(GunObjectPrefab[1], transform.position, transform.rotation);
                isSpawn = false;
                break;
        }

        yield return null;
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
            animator.Play("Enemy_Idle");
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
                animator.Play("Enemy_Walk");
                SoundSystem.instance.PlayDelaySounds("Character", "Footstep3", 0.5f);
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
                SoundSystem.instance.PlaySound("Enemy", "EnemyPistol");
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
            yield return StartCoroutine(SpawnItem());
        }

        animator.SetBool("isDead", true);

        yield return StartCoroutine(DieAnimation());

    }

}