using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationDestory : MonoBehaviour
{
    private void FixedUpdate()
    {
        AnimatorStateInfo stateInfo = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0); // ���̾� 0 ����

        if (stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
