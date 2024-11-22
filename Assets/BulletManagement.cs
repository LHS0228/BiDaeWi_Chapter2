using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManagement : MonoBehaviour
{
    WeaponType weaponType;
    public int maxBullet;
    public int loadBullet;
    public int currentBullet;

    void GetWeapon(WeaponType weaponType, int maxBullet, int loadBullet,int currentBullet)
    {
        weaponType = this.weaponType;
        loadBullet = this.loadBullet;
        currentBullet = this.currentBullet;
        maxBullet = this.maxBullet;
    }
}
