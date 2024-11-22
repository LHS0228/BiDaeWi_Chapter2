using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public WeaponType weaponType;
    public int maxBullet;
    public int loadBullet;
    public int currentBullet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerAttack>().weaponType = weaponType;
            collision.GetComponent<PlayerAttack>().maxBullet = maxBullet;
            collision.GetComponent<PlayerAttack>().loadBullet = loadBullet;
            collision.GetComponent<PlayerAttack>().currentBullet = currentBullet;

            Destroy(gameObject);
        }
    }
}
