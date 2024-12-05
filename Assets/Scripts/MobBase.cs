using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBase : EntityBase
{
    private void Awake()
    {
        Setup();
    }

    private void FixedUpdate()
    {
        switch(GetMobType())
        {
            case MobType.WalkMan:
                if (IsDead)
                {
                    //gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else
                {
                    //gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                break;
        }
    }
}
