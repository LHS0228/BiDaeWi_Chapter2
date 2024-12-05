using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public enum Enemy_Sniper_State { None = -1, Idle = 0, Attack, Die, }

public class Enemy_Sniper : MonoBehaviour
{
    [Header("��Ÿ ����")]
    [SerializeField]
    private Transform SpawnPoint; // �Ѿ� ���� ��ġ

    [Header("���� ����")]
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float attackRange = 30f;
    [SerializeField]
    private float aimDuration = 2f; // �÷��̾�� 2�ʵ��� ������ ��
    [SerializeField]
    private float aimCoolTime = 3f; // ��� ��Ÿ�� 


    [Header("������")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private Color laserColor = Color.magenta;


    [SerializeField]
    private PlayerController playerController;

    private Animator animator; // �ִ� ����
    private EntityBase entity;
    private bool isDead = false;
    private bool isAiming = false;
    private bool isCoolTime = false;
    private float recognizeTime = 0f; // ���� �ð� �ʱ�ȭ�Ϸ��� ��

    private BoxCollider2D boxCollider;
    private Enemy_Sniper_State enemyState = Enemy_Sniper_State.None;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        entity = GetComponent<EntityBase>();
        animator = GetComponent<Animator>();
        lineRenderer.enabled = false;
        playerController = GetComponent<PlayerController>();

        if( playerController == null ) 
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    private void OnEnable()
    {
        ChangeState(Enemy_Sniper_State.Idle);
        StartCoroutine(UpdateTarget());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enemyState = Enemy_Sniper_State.None;
    }

    private void Update()
    {
        Debug.Log($"{enemyState}");
        Debug.Log($"{playerController.isHide}");
    }
    public void Setup(Transform target)
    {
        this.target = target;
    }

    public IEnumerator UpdateTarget()
    {
        while (true)
        {
            if (enemyState == Enemy_Sniper_State.Die)
            {
                yield break;
            }
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance > attackRange)
            {
                lineRenderer.enabled = false;
                ChangeState(Enemy_Sniper_State.Idle);
            }

            Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, layerMask);

            if (collider != null && collider.CompareTag("Player"))
            {
                if (distance <= attackRange)
                {
                    if (!isAiming && !isCoolTime && isDead == false)
                    {
                        ChangeState(Enemy_Sniper_State.Attack);
                    }
                }
                else
                {
                    recognizeTime = 0f;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private IEnumerator DieAnimation()
    {
        SoundSystem.instance.PlaySound("Enemy", "EnemyDie");
        animator.Play("Enemy_Die");

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);

    }


    public void ChangeState(Enemy_Sniper_State state)
    {
        if (enemyState == state) return;

        if( enemyState == Enemy_Sniper_State.Die)
        {
            return;
        }
        StopCoroutine(enemyState.ToString());

        enemyState = state;

        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        while (enemyState == Enemy_Sniper_State.Idle)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Sniper_State.Die);
            }
            animator.Play("Enemy_Sniper_Idle");
            yield return null;
        }
    }


    private IEnumerator Attack()
    {
        recognizeTime = 0f;
        lineRenderer.enabled = true;
        while (enemyState == Enemy_Sniper_State.Attack)
        {
            if (entity.Stats.currentHP <= 0)
            {
                ChangeState(Enemy_Sniper_State.Die);
                yield return null;
            }
            if (!playerController.isHide)
            {
                lineRenderer.SetPosition(0, SpawnPoint.position);
                lineRenderer.SetPosition(1, target.position);

                recognizeTime += Time.deltaTime;
                if(recognizeTime >= aimDuration && !isCoolTime)
                {
                    AttackTarget();
                    SoundSystem.instance.PlaySound("Enemy", "EnemySniper");
                    StartCoroutine(CoolTime());
                    yield return null;
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                ChangeState(Enemy_Sniper_State.Idle);
                lineRenderer.enabled = false;
            }

        }

    }

    private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;
        boxCollider.enabled = false;
        animator.SetBool("isDead", true);
        lineRenderer.enabled = false;
        yield return StartCoroutine(DieAnimation());
    }


    private IEnumerator CoolTime()
    {
        isCoolTime = true;
        recognizeTime = 0f;
        lineRenderer.enabled = false;

        ChangeState(Enemy_Sniper_State.Idle);
        yield return new WaitForSeconds(aimCoolTime);
        isCoolTime = false;


    }

    private void AttackTarget()
    {
        RaycastHit2D hit = Physics2D.Raycast(SpawnPoint.position, (target.position - SpawnPoint.position).normalized, attackRange, layerMask);

        if (hit.collider != null)
        {
            Debug.Log($"Hit: {hit.collider.name}");

            // �÷��̾ �¾Ҵ��� Ȯ��
            if (hit.collider.CompareTag("Player"))
            {
                EntityBase player = hit.collider.GetComponent<EntityBase>();
                if (player != null)
                {
                    player.TakeDamage(2);
                }
            }
        }
    }
}