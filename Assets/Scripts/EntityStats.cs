using UnityEngine;

public enum MobType
{
    Player,
    WalkMan,
    PistolMan,
    ShieldMan,
    SniperMan
}

[System.Serializable]
public struct EntityStats
{
    [Header("[�� Ÿ��]")]
    public MobType mobType;

    [Header("[���� ����]")]
    public float damage;
    public float cooldownTime;

    [Header("[ü�� ����]")]
    public float currentHP;
    public float maxHP;
}
