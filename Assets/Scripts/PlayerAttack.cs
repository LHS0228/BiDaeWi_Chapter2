using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public enum WeaponType
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
    [SerializeField] private GameObject reloadBar;

    private PlayerController playerController;

    [Header("무기 세부 설정")]
    public WeaponType weaponType;
    public int maxBullet;
    public int loadBullet;
    public int currentBullet = 1;
    public float maxRayDistance = 10f; // 사정거리

    [Header("공격 가능 설정")]
    public bool isAttackStop;

    //기타 지역 변수값
    private GameObject scanObject;
    private Vector3 mousePosition;
    private Vector2 mouseDirection;
    private RaycastHit2D attackHit;
    private float rayDistance;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
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

        if (currentBullet > 0)
        {
            switch (weaponType)
            {
                case WeaponType.None:
                    break;

                case WeaponType.Pistol:
                    if (Input.GetMouseButtonDown(0))
                    {
                        playerController.anim.Play("Pistol");
                        StartCoroutine(DelayTime());

                        if (scanObject != null) // 무언가를 맞출 시
                        {
                            cameraShake.Shake(0.2f, 0.05f);

                            if (scanObject.CompareTag("Enemy"))
                            {
                                scanObject.GetComponent<MobBase>().TakeDamage(1);
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                                CreateLineEffect(attackRange.transform.position, mouseDirection, rayDistance);
                            }
                            else
                            {
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                                CreateLineEffect(attackRange.transform.position, mouseDirection, rayDistance);
                            }
                        }
                        else // 허공 발사
                        {
                            CreateLineEffect(transform.position, mouseDirection, rayDistance);
                        }
                    }
                    break;

                case WeaponType.ShotGun:
                    break;

                case WeaponType.Rifle:
                    if (Input.GetMouseButton(0))
                    {
                        playerController.anim.Play("Pistol");
                        StartCoroutine(DelayTime());

                        if (scanObject != null) // 무언가를 맞출 시
                        {
                            cameraShake.Shake(0.2f, 0.05f);

                            if (scanObject.CompareTag("Enemy"))
                            {
                                scanObject.GetComponent<MobBase>().TakeDamage(1);
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                                CreateLineEffect(attackRange.transform.position, mouseDirection += new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                            }
                            else
                            {
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                                CreateLineEffect(attackRange.transform.position, mouseDirection += new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
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
        else
        {
            StartCoroutine(ReloadTime());
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


    IEnumerator DelayTime()
    {
        playerController.isMoveStop = true;
        isAttackStop = true;
        currentBullet -= 1;

        switch (weaponType)
        {
            case WeaponType.None:
                yield return new WaitForSeconds(0.2f);
                break;

            case WeaponType.Pistol:
                yield return new WaitForSeconds(0.5f);
                break;

            case WeaponType.ShotGun:
                yield return new WaitForSeconds(0.5f);
                break;

            case WeaponType.Rifle:
                yield return new WaitForSeconds(0.05f);
                break;
        }
        

        playerController.isMoveStop = false;
        isAttackStop = false;
    }

    IEnumerator ReloadTime()
    {
        if (reloadBar.activeSelf)
        {
            reloadBar.GetComponent<Animator>().SetBool("isReload", true);
        }
        else
        {
            reloadBar.SetActive(true);
            reloadBar.GetComponent<Animator>().SetBool("isReload", true);
        }

        yield return new WaitForSeconds(2);

        if (currentBullet <= 0)
        {
            if (maxBullet > 0)
            {
                if (loadBullet > maxBullet)
                {
                    currentBullet = maxBullet;
                    maxBullet = 0;
                }
                else if(maxBullet != 0)
                {
                    currentBullet = loadBullet;
                    maxBullet -= loadBullet;
                }
            }
            else
            {
                weaponType = WeaponType.None;
                currentBullet = 1;
            }

            reloadBar.GetComponent<Animator>().SetBool("isReload", false);
        }
    }
}
