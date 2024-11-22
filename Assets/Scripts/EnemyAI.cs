using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("��Ÿ ����")]
    [SerializeField]
    private GameObject BulletPrefab; // �Ѿ� ������
    [SerializeField]
    private Transform SpawnPoint; // �Ѿ� ���� ��ġ

    [Header("���� ����")]
    [SerializeField]
    private float attackTerm = 0.5f; // ���� ��
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float attackRange = 5f;
    [SerializeField]
    private float recognizeRange = 8f;
    [SerializeField]
    private float moveSpeed = 2f;

    private Animator animator; // �ִ� ����

    private float lastAttackTime = 0f; // ���� �ҿ� ����

    [SerializeField]
    private LayerMask layerMask;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdateTarget();
    }

    public void Setup(Transform target)
    {
        this.target = target;
    }

    private void UpdateTarget()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        Collider2D collider1 = Physics2D.OverlapCircle(transform.position, recognizeRange, layerMask);
        if( collider1 != null && collider1.CompareTag("Player"))
        {
            if (distance <= recognizeRange)
            {
                Debug.Log($"�ν�1 {target}");
                RecognizeTarget();
            }
        }

        Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, layerMask);

        if (collider != null && collider.CompareTag("Player"))
        {
            if (distance <= attackRange)
            {
                Debug.Log($"�ν� {target}");
                if (Time.time - lastAttackTime >= attackTerm)
                {
                    AttackTarget();
                }
            }
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
        if (distance > attackRange)
        {
            Vector2 targetPosition = target.transform.position;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
    }
    private void AttackTarget()
    {
        lastAttackTime = Time.time;
        GameObject clone = Instantiate(BulletPrefab, SpawnPoint.position, SpawnPoint.rotation);
        clone.GetComponent<Projectile>().Setup(target.position);
    }


}