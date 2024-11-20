using UnityEngine;

public enum MobType
{
    Player,
    WalkMan,
    PistolMan,
    SildeMan,
    SniperMan
}

[System.Serializable]
public struct EntityStats
{
    [Header("[몹 타입]")]
    public MobType mobType;

    [Header("[공격 설정]")]
    public float damage;
    public float cooldownTime;

    [Header("[체력 설정]")]
    public float currentHP;
    public float maxHP;
}
