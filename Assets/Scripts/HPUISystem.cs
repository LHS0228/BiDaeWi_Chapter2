using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUISystem : MonoBehaviour
{
    [SerializeField] private MobBase playerBase;
    [SerializeField] private Sprite orginHP;
    [SerializeField] private Sprite breakHP;

    [SerializeField] private Image[] HPBar;

    private int nowHP;

    private void Awake()
    {
        nowHP = (int)playerBase.GetHp();
    }

    private void Start()
    {
        SoundSystem.instance.PlayBGM("BlackOutFight");
    }

    void Update()
    {
        if (nowHP != (int)playerBase.GetHp())
        {
            //데미지
            if(nowHP > (int)playerBase.GetHp())
            {
                SoundSystem.instance.PlaySound("Character", "Hit");
            }
            //회복
            else
            {
                //회복 이펙트 (AND 소리)
            }

            nowHP = (int)playerBase.GetHp();

            for (int i = 0; i < HPBar.Length; i++)
            {
                if (i < nowHP)
                {
                    HPBar[i].sprite = orginHP;
                }
                else
                {
                    HPBar[i].sprite = breakHP;
                }
            }
        }
    }
}
