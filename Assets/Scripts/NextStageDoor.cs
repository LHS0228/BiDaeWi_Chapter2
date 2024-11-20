using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum StageName
{
    None,
    Stage_1,
    Stage_2,
    Stage_3,
    Stage_4,
    Stage_5,
}

public enum Choice
{
    None,
    Yes,
    No
}

public class NextStageDoor : MonoBehaviour
{
    [Header("[�������� �Ѿ�� ��������]")]
    [SerializeField] private StageName nextStage;

    [Header("[�ؽ�Ʈ��]")]
    [SerializeField] private GameObject guideText;
    [SerializeField] private TextMeshPro yesText;
    [SerializeField] private TextMeshPro noText;

    [Header("[�÷��̾�]")]
    [SerializeField] private PlayerController playerController;

    private bool isChice;
    private Choice isNoYes;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        guideText.SetActive(true);
        playerController.isPlayerStop = true;
        isChice = true;
    }

    /*
    private void OnCollisionExit2D(Collision2D collision)
    {
        guideText.SetActive(false);
        isChice = false;
        isNoYes = Choice.None;
    }
    */

    private void Update()
    {
        if(isChice)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                isNoYes = Choice.No;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                isNoYes = Choice.Yes;
            }
        }

        switch(isNoYes)
        {
            case Choice.Yes:
                noText.color = Color.white;
                yesText.color = Color.red;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("����");
                    guideText.SetActive(false);
                    gameObject.SetActive(false);
                    playerController.isPlayerStop = false;
                    isChice = false;
                }
                break;

            case Choice.No:
                noText.color = Color.red;
                yesText.color = Color.white;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("���� ����");
                    guideText.SetActive(false);
                    playerController.isPlayerStop = false;
                    isChice = false;
                }
                break;

            case Choice.None:
                noText.color = Color.white;
                yesText.color = Color.white;
                break;
        }
    }
}