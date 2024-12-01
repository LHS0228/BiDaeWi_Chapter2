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

    void Update()
    {
        if (nowHP != (int)playerBase.GetHp())
        {
            //������
            if(nowHP > (int)playerBase.GetHp())
            {
                //������ ����Ʈ (AND �Ҹ�)
            }
            //ȸ��
            else
            {
                //ȸ�� ����Ʈ (AND �Ҹ�)
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
