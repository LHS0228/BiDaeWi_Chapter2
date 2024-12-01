using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    [SerializeField]
    private PlayerAttack playerAttackBase;

    //0: 기본무기, 1: 피스톨, 2: 라이플, 3: 샷구너 
    private int[] currentBullet;
    private int[] maxBullet;

    [SerializeField] private TextMeshProUGUI[] currentBulletText;
    [SerializeField] private GameObject[] gunOjbect;
    [SerializeField] private float[] gunRotation;

    [SerializeField] private WeaponType latestWeaponType;
    private Coroutine nowCoroutine;

    private void Awake()
    {
        gunRotation = new float[4];
        gunRotation[0] = gunOjbect[0].GetComponent<RectTransform>().localEulerAngles.z;
        gunRotation[1] = gunOjbect[1].GetComponent<RectTransform>().localEulerAngles.z;
        gunRotation[2] = gunOjbect[2].GetComponent<RectTransform>().localEulerAngles.z;
        gunRotation[3] = gunOjbect[3].GetComponent<RectTransform>().localEulerAngles.z;

        currentBullet = new int[4];
        maxBullet = new int[4];
    }

    private void Update()
    {
        if(latestWeaponType != playerAttackBase.weaponType)
        {
            latestWeaponType = playerAttackBase.weaponType;

            for(int i=0; i<gunOjbect.Length; i++)
            {
                gunOjbect[i].SetActive(false);
            }

            switch(latestWeaponType)
            {
                case WeaponType.None:
                    maxBullet[0] = playerAttackBase.maxBullet;
                    currentBullet[0] = playerAttackBase.currentBullet;
                    gunOjbect[0].SetActive(true);
                    break;
                case WeaponType.Pistol:
                    maxBullet[1] = playerAttackBase.maxBullet;
                    currentBullet[1] = playerAttackBase.currentBullet;
                    gunOjbect[1].SetActive(true);
                    break;
                case WeaponType.Rifle:
                    maxBullet[2] = playerAttackBase.maxBullet;
                    currentBullet[2] = playerAttackBase.currentBullet;
                    gunOjbect[2].SetActive(true);
                    break;
                case WeaponType.ShotGun:
                    maxBullet[3] = playerAttackBase.maxBullet;
                    currentBullet[3] = playerAttackBase.currentBullet;
                    gunOjbect[3].SetActive(true);
                    break;
            }
        }

        int weaponIndex = (int)playerAttackBase.weaponType;

        if (weaponIndex >= 0 && weaponIndex < currentBullet.Length && currentBullet[weaponIndex] != playerAttackBase.currentBullet)
        {
            maxBullet[weaponIndex] = playerAttackBase.maxBullet;
            currentBullet[weaponIndex] = playerAttackBase.currentBullet;

            if (playerAttackBase.loadBullet != currentBullet[weaponIndex])
            {
                switch (weaponIndex)
                {
                    case 0:
                        Rebound(WeaponType.None);
                        break;
                    case 1:
                        Rebound(WeaponType.Pistol);
                        break;
                    case 2:
                        Rebound(WeaponType.Rifle);
                        break;
                    case 3:
                        Rebound(WeaponType.ShotGun);
                        break;
                }
            }
        }

        switch (latestWeaponType)
        {
            case WeaponType.None:
                if (gunOjbect[0].gameObject.GetComponent<RectTransform>().eulerAngles.z > gunRotation[0])
                {
                    if (nowCoroutine == null)
                    {
                        nowCoroutine = StartCoroutine(GunDownDelay(latestWeaponType));
                    }
                }
                break;
            case WeaponType.Pistol:
                if (gunOjbect[1].gameObject.GetComponent<RectTransform>().eulerAngles.z > gunRotation[1])
                {
                    if (nowCoroutine == null)
                    {
                        nowCoroutine = StartCoroutine(GunDownDelay(latestWeaponType));
                    }
                }
                break;
            case WeaponType.Rifle:
                if (gunOjbect[2].gameObject.GetComponent<RectTransform>().eulerAngles.z > gunRotation[2])
                {
                    if (nowCoroutine == null)
                    {
                        nowCoroutine = StartCoroutine(GunDownDelay(latestWeaponType));
                    }
                }
                break;
            case WeaponType.ShotGun:
                if (gunOjbect[3].gameObject.GetComponent<RectTransform>().eulerAngles.z > gunRotation[3])
                {
                    if (nowCoroutine == null)
                    {
                        nowCoroutine = StartCoroutine(GunDownDelay(latestWeaponType));
                    }
                }
                break;
        }

        currentBulletUptateText();
    }

    private void Rebound(WeaponType weaponType)
    {
        switch(weaponType)
        {
            case WeaponType.None:
                break;

            case WeaponType.Pistol:

                if (gunOjbect[1].gameObject.GetComponent<RectTransform>().eulerAngles.z < 30)
                {
                    gunOjbect[1].gameObject.GetComponent<RectTransform>().eulerAngles += new Vector3(0, 0, 25);
                }
                break;
            case WeaponType.Rifle:
                if (gunOjbect[2].gameObject.GetComponent<RectTransform>().eulerAngles.z < 47)
                {
                    gunOjbect[2].gameObject.GetComponent<RectTransform>().eulerAngles += new Vector3(0, 0, 6);
                }
                break;
            case WeaponType.ShotGun:
                gunOjbect[3].gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 15);
                break;
        }
    }

    private IEnumerator GunDownDelay(WeaponType weaponType)
    {
        yield return new WaitForSecondsRealtime(0.02f);

        switch(weaponType)
        {
            case WeaponType.None:
                break;
            case WeaponType.Pistol:
                gunOjbect[1].GetComponent<RectTransform>().eulerAngles += new Vector3(0, 0, -1.5f);
                break;
            case WeaponType.Rifle:
                gunOjbect[2].GetComponent<RectTransform>().eulerAngles += new Vector3(0, 0, -1.5f);
                break;
            case WeaponType.ShotGun:
                gunOjbect[3].GetComponent<RectTransform>().eulerAngles += new Vector3(0, 0, -0.25f);
                break;
        }

        nowCoroutine = null;
    }

    private void currentBulletUptateText()
    {
        for(int i =0; i < currentBulletText.Length; i++)
        {
            if (gunOjbect[i].activeSelf)
            {
                currentBulletText[i].text = maxBullet[i] + " / " + currentBullet[i];
            }
        }
    }
}
