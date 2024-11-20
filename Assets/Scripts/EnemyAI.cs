using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("기타 설정")]
    [SerializeField]
    private GameObject BulletPrefab; // 총알 프리팹
    [SerializeField]
    private Transform SpawnPoint; // 총알 생성 위치

    [Header("공격 설정")]
    [SerializeField]
    private float attackTerm = 0.5f; // 공격 텀
    [SerializeField]
    private Transform target;

    private Animator animator; // 애니 관련

    private float lastAttackTime = 0f; // 공격 텀에 사용됨


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
