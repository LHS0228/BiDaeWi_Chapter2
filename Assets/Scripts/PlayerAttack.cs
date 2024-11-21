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

    private PlayerController playerController;
    private BulletManagement BulletManagement;

    public AttackType attackType;
    public bool isAttackStop;
    public float maxRayDistance = 10f; // �����Ÿ�

    private GameObject scanObject;
    private Vector3 mousePosition;
    private Vector2 mouseDirection;
    private RaycastHit2D attackHit;
    private float rayDistance;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        BulletManagement = GetComponent<BulletManagement>();
    }

    void Update()
    {
        if (!isAttackStop)
        {
            HitCastScan();
        }
    }

    //���� ��ĵ
    private void HitCastScan()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        mouseDirection = (mousePosition - transform.position).normalized;

        attackHit = Physics2D.Raycast(transform.position, mouseDirection, maxRayDistance, LayerMask.GetMask("Object"));

        rayDistance = maxRayDistance;

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

        WeaponAttack();
    }

    private void WeaponAttack()
    {
        Debug.DrawRay(transform.position, mouseDirection * rayDistance, Color.red);

        CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();

        switch (attackType)
        {
            case AttackType.None:
                if (Input.GetMouseButtonDown(0))
                {
                    playerController.anim.Play("Pistol");
                    StartCoroutine(ReloadTime());

                    if (scanObject != null) // ���𰡸� ���� ��
                    {
                        if (scanObject.CompareTag("Enemy"))
                        {
                            scanObject.GetComponent<MobBase>().TakeDamage(1);
                        }

                        cameraShake.Shake(0.2f, 0.1f);

                        if (scanObject.CompareTag("Enemy"))
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                            Destroy(hitEffect, 1f);
                            CreateLineEffect(transform.position, mouseDirection, rayDistance);
                        }
                        else
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                            Destroy(hitEffect, 1f);
                            CreateLineEffect(transform.position, mouseDirection, rayDistance);
                        }
                    }
                    else // ��� �߻�
                    {
                        CreateLineEffect(transform.position, mouseDirection, rayDistance);
                    }
                }
                break;

            case AttackType.Pistol:
                break;

            case AttackType.ShotGun:
                if (Input.GetMouseButton(0))
                {

                }
                break;

            case AttackType.Sniper:
                break;

        }

        //������
        if (Input.GetMouseButton(0))
        {
            playerController.anim.Play("Pistol");
            StartCoroutine(ReloadTime());
            if (scanObject != null)
            {
                Debug.Log("���� ��ü :" + attackHit.collider.name);

                //����Ʈ �ߵ�
                if (cameraShake != null)
                {

                    if (scanObject.CompareTag("Enemy"))
                    {
                        scanObject.GetComponent<MobBase>().TakeDamage(1);
                    }

                    cameraShake.Shake(0.2f, 0.1f);

                    if (scanObject.CompareTag("Enemy"))
                    {
                        GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                        Destroy(hitEffect, 1f);
                        CreateLineEffect(transform.position, mouseDirection, rayDistance);
                    }
                    else
                    {
                        GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                        Destroy(hitEffect, 1f);
                        CreateLineEffect(transform.position, mouseDirection, rayDistance);
                    }
                }
            }
            else
            {
                Debug.Log("�ٴڿ� ��");
                CreateLineEffect(transform.position, mouseDirection, rayDistance);
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


    IEnumerator ReloadTime()
    {
        playerController.isJumpStop = true;
        playerController.isMoveStop = true;
        isAttackStop = true;

        yield return new WaitForSeconds(0.1f);

        /*
        switch (attackType)
        {
            case AttackType.None:
                yield return new WaitForSeconds(0.5f);
                break;

            case AttackType.Pistol:
                yield return new WaitForSeconds(0.5f);
                break;

            case AttackType.ShotGun:
                yield return new WaitForSeconds(0.5f);
                break;

            case AttackType.Sniper:
                yield return new WaitForSeconds(0.5f);
                break;
        }
        */

        playerController.isJumpStop = false;
        playerController.isMoveStop = false;
        isAttackStop = false;
    }
}
