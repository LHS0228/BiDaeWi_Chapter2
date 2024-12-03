using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sniper : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]
    private float attackTerm = 0.5f; // ���� ��
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float attackRange = 5f;

    private Animator animator; // �ִ� ����
    private EntityBase entity;
    private float lastAttackTime = 0f; // ���� �ҿ� ����

    [SerializeField]
    private LayerMask layerMask;
}
