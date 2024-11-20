using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer BulletPrefab;
    [SerializeField]
    private Transform SpawnPoint;

    private Animator animator;
    private void Update()
    {
        animator = GetComponent<Animator>();
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 5f);

        if( collider != null && collider.CompareTag("Player"))
        {
            AttackTarget(collider.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }

    private void AttackTarget(GameObject spawnPoint)
    {
        Instantiate(BulletPrefab, SpawnPoint.position, Quaternion.identity);
    }
}
