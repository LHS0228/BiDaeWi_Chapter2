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
    [Header("[다음으로 넘어가는 스테이지]")]
    [SerializeField] private StageName nextStage;

    [Header("[텍스트들]")]
    [SerializeField] private GameObject guideText;
    [SerializeField] private TextMeshPro yesText;
    [SerializeField] private TextMeshPro noText;

    [Header("[플레이어]")]
    [SerializeField] private PlayerController playerController;

    private BoxCollider2D doorCollider;
    private Animator anim;

    private bool isChice;
    private Choice isNoYes;

    private void Awake()
    {
        doorCollider = GetComponentInChildren<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            guideText.SetActive(true);
            playerController.isPlayerStop = true;
            isChice = true;
        }
    }

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
                    Debug.Log("입장");
                    anim.enabled = true;
                    doorCollider.enabled = false;

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
                    Debug.Log("입장 거절");
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
