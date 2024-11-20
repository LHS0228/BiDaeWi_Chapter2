using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public enum AttackType
{
    None, //근접 공격(기본) 
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
    public float maxRayDistance = 10f; // Raycast 거리를 설정합니다.

    void Update()
    {
        HitCastScan();
    }

    //공격 스캔
    private void HitCastScan()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 direction = (mousePosition - transform.position).normalized;

        RaycastHit2D attackHit = Physics2D.Raycast(transform.position, direction, maxRayDistance, LayerMask.GetMask("Object"));

        float rayDistance = maxRayDistance;

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

        Debug.DrawRay(transform.position, direction * rayDistance, Color.red);

        if (Input.GetMouseButtonDown(0))
        {
            if (scanObject != null)
            {
                Debug.Log("공격 물체 :" + attackHit.collider.name);
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();

                //이펙트 발동
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
                Debug.Log("바닥에 쏨");
                CreateLineEffect(transform.position, direction, rayDistance);
            }
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
}
