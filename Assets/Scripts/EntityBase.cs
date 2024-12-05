using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    [SerializeField]
    protected EntityStats stats;

    private Enemy_Shield shield;
    public EntityStats Stats => stats;
    public bool IsDead => stats.currentHP <= 0;
    private bool isInvincible;
    public bool IsInvincible
    {
        get { return isInvincible; }
        set { isInvincible = value; }
    }


    protected virtual void Setup()
    {
        stats.currentHP = stats.maxHP;
    }

    public float GetHp()
    {
        return stats.currentHP;
    }

    public void TakeDamage(float damage)
    {
        if(IsDead) return;

        if(IsInvincible)
        {
            stats.currentHP -= 0;
        }
        else if(!IsInvincible)
        {
            stats.currentHP = stats.currentHP - damage > 0 ? stats.currentHP - damage : 0;
        }
        

        if(stats.currentHP <= 0)
        {
            //»ç¸ÁÃ³¸®
        }
    }

    public MobType GetMobType()
    {
        return stats.mobType;
    }

    public void HealHP()
    {
        if(stats.currentHP < 6) stats.currentHP += 1;
    }
}
