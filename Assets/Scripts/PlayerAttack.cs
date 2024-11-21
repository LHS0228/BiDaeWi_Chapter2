using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public enum AttackType
{
    None, //근접 공격(기본) 
    Pistol,
    ShotGun,
    Rifle,
}

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackRange;
    [SerializeField] private GameObject gunEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab_Object;
    [SerializeField] private GameObject hitEffectPrefab_Enemy;

    private PlayerController playerController;
    private BulletManagement BulletManagement;

    [Header("무기 세부 설정")]
    public AttackType attackType;
    public bool isAttackStop;
    public float maxRayDistance = 10f; // 사정거리

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

    //공격 스캔
    private void HitCastScan()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        mouseDirection = (mousePosition - transform.position).normalized;

        attackHit = Physics2D.Raycast(transform.position, mouseDirection, maxRayDistance, LayerMask.GetMask("Object"));

        rayDistance = maxRayDistance;

        if (attackHit.collider != null)
        {
            rayDistance = attackHit.distance; // 닿은 지점까지 거리로 업데이트
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
                break;

            case AttackType.Pistol:
                if (Input.GetMouseButtonDown(0))
                {
                    playerController.anim.Play("Pistol");
                    StartCoroutine(ReloadTime());

                    if (scanObject != null) // 무언가를 맞출 시
                    {
                        if (scanObject.CompareTag("Enemy"))
                        {
                            scanObject.GetComponent<MobBase>().TakeDamage(1);
                        }

                        cameraShake.Shake(0.2f, 0.05f);

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
                    else // 허공 발사
                    {
                        CreateLineEffect(transform.position, mouseDirection, rayDistance);
                    }
                }
                break;

            case AttackType.ShotGun:
                break;

            case AttackType.Rifle:
                if (Input.GetMouseButton(0))
                {
                    playerController.anim.Play("Pistol");
                    StartCoroutine(ReloadTime());

                    if (scanObject != null) // 무언가를 맞출 시
                    {
                        if (scanObject.CompareTag("Enemy"))
                        {
                            scanObject.GetComponent<MobBase>().TakeDamage(1);
                        }

                        cameraShake.Shake(0.2f, 0.05f);

                        if (scanObject.CompareTag("Enemy"))
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                            Destroy(hitEffect, 1f);
                            CreateLineEffect(transform.position, mouseDirection += new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                        }
                        else
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                            Destroy(hitEffect, 1f);
                            CreateLineEffect(transform.position, mouseDirection += new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                        }
                    }
                    else // 허공 발사
                    {
                        CreateLineEffect(transform.position, mouseDirection += new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                    }
                }
                break;

        }
    }

    // 일직선 이펙트 생성 함수
    private void CreateLineEffect(Vector3 origin, Vector2 direction, float distance)
    {
        // 일직선 이펙트 프리팹 생성
        GameObject gunEffect = Instantiate(gunEffectPrefab, origin, Quaternion.identity);

        // 이펙트 방향 설정
        gunEffect.transform.right = direction;

        // 이펙트 길이 조정
        Vector3 scale = gunEffect.transform.localScale;
        scale.x = distance * 0.725f; // 길이를 Raycast 거리만큼 설정
        gunEffect.transform.localScale = scale;

        // 일정 시간 후 이펙트 삭제
        Destroy(gunEffect, 0.3f); // 이펙트가 0.3초 후 제거
    }


    IEnumerator ReloadTime()
    {
        playerController.isMoveStop = true;
        isAttackStop = true;
        

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

            case AttackType.Rifle:
                yield return new WaitForSeconds(0.5f);
                break;
        }
        

        playerController.isMoveStop = false;
        isAttackStop = false;
    }
}
