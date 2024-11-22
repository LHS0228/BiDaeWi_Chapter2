using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManagement : MonoBehaviour
{
    public WeaponType weaponType;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<PlayerAttack>().weaponType = weaponType;
        collision.GetComponent<PlayerAttack>().maxBullet = maxBullet;
        collision.GetComponent<PlayerAttack>().loadBullet = loadBullet;
        collision.GetComponent<PlayerAttack>().currentBullet = currentBullet;

        Destroy(gameObject);
    }
}
