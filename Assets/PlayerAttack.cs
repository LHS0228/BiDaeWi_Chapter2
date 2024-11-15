using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject attackRange;

    private Vector2 mousePos;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D rayGun = Physics2D.Raycast(transform.position, mousePos);
        Debug.DrawRay(transform.position, mousePos);
    }
}
