using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationDestory : MonoBehaviour
{
    private void FixedUpdate()
    {
        AnimatorStateInfo stateInfo = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0); // 레이어 0 기준

        if (stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
