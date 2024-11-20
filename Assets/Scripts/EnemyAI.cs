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

    private Animator animator; // �ִ� ����

    private float lastAttackTime = 0f; // ���� �ҿ� ����


    private void Update()
    {
        animator = GetComponent<Animator>();
        UpdateTarget();
    }

    public void Setup(Transform target)
    {
        this.target = target;
    }

    private void UpdateTarget()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 5f);

        if (collider != null && collider.CompareTag("Player"))
        {
            if(Time.time - lastAttackTime >= attackTerm)
            {
                AttackTarget();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }

    private void AttackTarget()
    {
        lastAttackTime = Time.time;
        GameObject clone = Instantiate(BulletPrefab, SpawnPoint.position, SpawnPoint.rotation);
        clone.GetComponent<Projectile>().Setup(target.position);
    }


}
