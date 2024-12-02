using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public enum WeaponType
{
    None, //근접 공격(기본) 
    Pistol,
    Rifle,
    ShotGun,
}

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackRange;
    [SerializeField] private GameObject gunEffectPrefab;
    [SerializeField] private GameObject knifeEffectPrefab;
    [SerializeField] private GameObject shotGunEffectPrefab;

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
    private Vector3 knifeDistance;
    private Vector3 shotGunDistance;

    private bool isRifleAttacking; //라이플 관련
    private Coroutine rifleCoroutine;
    private Coroutine reloadCoroutine;

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
                    if (Input.GetMouseButtonDown(0))
                    {
                        playerController.anim.Play("Knife");
                        SoundSystem.instance.PlaySound("Weapon", "Knife");
                        StartCoroutine(moveStopTime());
                        StartCoroutine(DelayTime());
                        AttackTurn();
                        knifeDistance = (Vector2)transform.position + new Vector2(1 * (gameObject.GetComponent<SpriteRenderer>().flipX ? -1 : 1), 0);
                        GameObject knifeEffect = Instantiate(knifeEffectPrefab, knifeDistance, Quaternion.identity);
                        knifeEffect.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<SpriteRenderer>().flipX;
                        Destroy(knifeEffect, 1f);

                        Collider2D hit = Physics2D.OverlapBox((Vector2)knifeDistance, new Vector2(2, 2.5f), 0, LayerMask.GetMask("Object"));

                        if (hit != null) // 무언가를 맞출 시
                        {
                            cameraShake.Shake(0.2f, 0.050f);

                            if (hit.CompareTag("Enemy"))
                            {
                                hit.gameObject.GetComponent<MobBase>().TakeDamage(1);
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, hit.gameObject.transform.position, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                            }
                            else
                            {
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Object, hit.gameObject.transform.position, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                            }
                        }
                    }
                    break;

                case WeaponType.Pistol:
                    if (Input.GetMouseButtonDown(0))
                    {
                        AttackTurn();
                        SoundSystem.instance.PlaySound("Weapon", "PistolShot");
                        playerController.anim.Play("Pistol");
                        StartCoroutine(moveStopTime());
                        StartCoroutine(DelayTime());
                        cameraShake.Shake(0.2f, 0.075f);


                        if (scanObject != null) // 무언가를 맞출 시
                        {
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        AttackTurn();
                        playerController.anim.Play("ShotGun");
                        SoundSystem.instance.PlaySound("Weapon", "ShotgunShot");
                        StartCoroutine(moveStopTime());
                        StartCoroutine(DelayTime());

                        shotGunDistance = (Vector2)transform.position + new Vector2(3 * (gameObject.GetComponent<SpriteRenderer>().flipX ? -1 : 1), 0);
                        GameObject ShotEffect = Instantiate(shotGunEffectPrefab, shotGunDistance, Quaternion.identity);
                        ShotEffect.GetComponentInChildren<SpriteRenderer>().flipX = gameObject.GetComponent<SpriteRenderer>().flipX;

                        Collider2D[] hit = Physics2D.OverlapBoxAll((Vector2)shotGunDistance, new Vector2(5.25f, 2.5f), 0, LayerMask.GetMask("Object"));

                        cameraShake.Shake(0.2f, 0.4f);

                        if (hit != null) // 무언가를 맞출 시
                        {
                            for (int i = 0; i < hit.Length; i++)
                            {
                                if (hit[i].CompareTag("Enemy"))
                                {
                                    hit[i].gameObject.GetComponent<MobBase>().TakeDamage(1); // 데미지 입히기
                                    GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, hit[i].gameObject.transform.position, Quaternion.identity);
                                    Destroy(hitEffect, 1f);
                                }
                                else
                                {
                                    GameObject hitEffect = Instantiate(hitEffectPrefab_Object, hit[i].gameObject.transform.position, Quaternion.identity);
                                    Destroy(hitEffect, 1f);
                                }
                            }
                        }
                    }
                    break;

                case WeaponType.Rifle:
                    if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
                    {
                        isRifleAttacking = true;
                        playerController.isMoveStop = true;

                        AttackTurn();
                        playerController.anim.Play("Rifle");
                        SoundSystem.instance.PlaySound("Weapon", "RifleShot");
                        StartCoroutine(DelayTime());

                        cameraShake.Shake(0.2f, 0.1f);

                        if (scanObject != null) // 무언가를 맞출 시
                        {
                            if (scanObject.CompareTag("Enemy"))
                            {
                                scanObject.GetComponent<MobBase>().TakeDamage(1);
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Enemy, attackHit.point, Quaternion.identity);
                                Destroy(hitEffect, 1f);

                                CreateLineEffect(attackRange.transform.position, mouseDirection + new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                            }
                            else
                            {
                                GameObject hitEffect = Instantiate(hitEffectPrefab_Object, attackHit.point, Quaternion.identity);
                                Destroy(hitEffect, 1f);
                                CreateLineEffect(attackRange.transform.position, mouseDirection + new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                            }
                        }
                        else // 허공 발사
                        {
                            CreateLineEffect(transform.position, mouseDirection + new Vector2(0, Random.Range(-0.1f, 0.1f)), rayDistance);
                        }
                    }

                    if(isRifleAttacking)
                    {
                        if (rifleCoroutine != null)
                        {
                            StopCoroutine(rifleCoroutine);
                            rifleCoroutine = StartCoroutine(moveStopTime());
                            isRifleAttacking = false;
                        }
                        else
                        {
                            rifleCoroutine = StartCoroutine(moveStopTime());
                            isRifleAttacking = false;
                        }
                    }
                    break;
            }
        }
        else
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(ReloadTime());
            }
        }
    }


    private void AttackTurn()
    {
        if (mouseDirection.x <= Camera.main.ScreenToWorldPoint(transform.position).normalized.x)
        {
            Debug.Log("왼쪽");
            playerController.spriteRenderer.flipX = true;

            // 캐릭터의 위치를 기준으로 `attackRange` 위치를 설정
            attackRange.transform.position = transform.position + new Vector3(-1, 0, 0);
        }
        else
        {
            Debug.Log("오른쪽");
            playerController.spriteRenderer.flipX = false;

            // 캐릭터의 위치를 기준으로 `attackRange` 위치를 설정
            attackRange.transform.position = transform.position + new Vector3(1, 0, 0);
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
        isAttackStop = true;

        switch (weaponType)
        {
            case WeaponType.None:
                yield return new WaitForSeconds(0.2f);
                break;

            case WeaponType.Pistol:
                currentBullet -= 1;
                yield return new WaitForSeconds(0.5f);
                break;

            case WeaponType.ShotGun:
                currentBullet -= 1;
                yield return new WaitForSeconds(0.5f);
                break;

            case WeaponType.Rifle:
                currentBullet -= 1;
                yield return new WaitForSeconds(0.05f);
                break;
        }
        
        isAttackStop = false;
    }

    IEnumerator moveStopTime()
    {
        playerController.isMoveStop = true;
        playerController.anim.SetBool("isWalk", false);
        playerController.anim.SetBool("isAttack", true);

        switch (weaponType)
        {
            case WeaponType.None:

                yield return new WaitForSeconds(0.1f);
                break;

            case WeaponType.Pistol:
                yield return new WaitForSeconds(0.5f);
                break;

            case WeaponType.ShotGun:
                yield return new WaitForSeconds(2);
                break;

            case WeaponType.Rifle:
                yield return new WaitForSeconds(1);
                break;
                
        }

        playerController.isMoveStop = false;
        playerController.anim.SetBool("isAttack", false);
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

        reloadCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (weaponType == WeaponType.None)  
        {
            knifeDistance = (Vector2)transform.position + new Vector2(1 * (gameObject.GetComponent<SpriteRenderer>().flipX ? -1 : 1), 0);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(knifeDistance, new Vector3(2, 2.5f));
        }

        if (weaponType == WeaponType.ShotGun)
        {
            shotGunDistance = (Vector2)transform.position + new Vector2(3 * (gameObject.GetComponent<SpriteRenderer>().flipX ? -1 : 1), 0);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(shotGunDistance, new Vector3(5.25f, 2.5f));
        }
    }
}
