using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManagement : MonoBehaviour
{
    public int currentBullet;
    public int maxBullet;

    void GetWeapon(AttackType attackType, int maxBullet, int currentBullet)
    {
        this.currentBullet = currentBullet;
        this.maxBullet = maxBullet;
    }
}
