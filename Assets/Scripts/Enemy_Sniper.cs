using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sniper : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField]
    private float attackTerm = 0.5f; // 공격 텀
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float attackRange = 5f;

    private Animator animator; // 애니 관련
    private EntityBase entity;
    private float lastAttackTime = 0f; // 공격 텀에 사용됨

    [SerializeField]
    private LayerMask layerMask;
}
