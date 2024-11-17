using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public enum AttackType
{
    None, //���� ����(�⺻) 
    Pistol,
    ShotGun,
    Sniper,
}

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackRange;
    [SerializeField] private GameObject gunEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab_Object;
    [SerializeField] private GameObject hitEffectPrefab_Enemy;

    private GameObject scanObject;
    public float maxRayDistance = 10f; // Raycast �Ÿ��� �����մϴ�.

    void Update()
    {
        HitCastScan();
    }

    //���� ��ĵ
    private void HitCastScan()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 direction = (mousePosition - transform.position).normalized;

        RaycastHit2D attackHit = Physics2D.Raycast(transform.position, direction, maxRayDistance, LayerMask.GetMask("Object"));

        float rayDistance = maxRayDistance;

        if (attackHit.collider != null)
        {
            rayDistance = attackHit.distance; // ���� �������� �Ÿ��� ������Ʈ
            scanObject = attackHit.collider.gameObject;
        }
        else
        {
            if (scanObject != null)
            {
                scanObject = null;
            }
        }

        Debug.DrawRay(transform.position, direction * rayDistance, Color.red);

        if (Input.GetMouseButtonDown(0))
        {
            if (scanObject != null)
            {
                Debug.Log("���� ��ü :" + attackHit.collider.name);
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();

                //����Ʈ �ߵ�
                if (cameraShake != null)
                {
                    if(scanObject.CompareTag("Enemy"))
                    {
                        scanObject.GetComponent<MobBase>().TakeDamage(1);
                    }

                    cameraShake.Shake(0.2f, 0.1f);

                    if (scanObject.CompareTag("Enemy"))
                    {
                        GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                        Destroy(hitEffect, 1f);
                        CreateLineEffect(transform.position, direction, rayDistance);
                    }
                    else
                    {
                        GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                        Destroy(hitEffect, 1f);
                        CreateLineEffect(transform.position, direction, rayDistance);
                    }
                }
            }
            else
            {
                Debug.Log("�ٴڿ� ��");
                CreateLineEffect(transform.position, direction, rayDistance);
            }
        }
    }

    // ������ ����Ʈ ���� �Լ�
    private void CreateLineEffect(Vector3 origin, Vector2 direction, float distance)
    {
        // ������ ����Ʈ ������ ����
        GameObject gunEffect = Instantiate(gunEffectPrefab, origin, Quaternion.identity);

        // ����Ʈ ���� ����
        gunEffect.transform.right = direction;

        // ����Ʈ ���� ����
        Vector3 scale = gunEffect.transform.localScale;
        scale.x = distance * 0.725f; // ���̸� Raycast �Ÿ���ŭ ����
        gunEffect.transform.localScale = scale;

        // ���� �ð� �� ����Ʈ ����
        Destroy(gunEffect, 0.3f); // ����Ʈ�� 0.3�� �� ����
    }
}
