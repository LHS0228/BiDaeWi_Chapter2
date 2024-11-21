using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementRigidBody2D : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    private Rigidbody2D rigid2D;

    void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    public void MoveTo(Vector3 direction)
    {
        rigid2D.velocity = direction * moveSpeed;
    }
}
