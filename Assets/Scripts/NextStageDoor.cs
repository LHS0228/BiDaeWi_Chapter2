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
    [Header("해당 문은 다음 스테이지로 가는 문인가요?"), SerializeField]
    private bool isNextDoor;

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
    private Coroutine nowCoroutine;

    private void Awake()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        anim = GetComponentInParent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isNextDoor)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                guideText.SetActive(true);
                playerController.isPlayerStop = true;
                isChice = true;
            }
        }
        else
        {
            if (nowCoroutine == null)
            {
                StartCoroutine(MapOpenDoor(collision));
            }
        }
    }

    private void Update()
    {
        ChooseOpen();
    }

    private void ChooseOpen()
    {
        if (isChice)
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

        switch (isNoYes)
        {
            case Choice.Yes:
                noText.color = Color.white;
                yesText.color = Color.red;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SoundSystem.instance.PlaySound("SFX", "DoorOpen");
                    Debug.Log("입장");
                    anim.enabled = true;
                    doorCollider.enabled = false;

                    if(nextStage == StageName.Stage_5)
                    {
                        ScreenSystem.instance.ScreenPlay(false);
                        guideText.SetActive(false);
                        SceneLoader.instance.LoadSceneDelay("Ending_Stage5", 3);
                        break;
                    }
                    else
                    {
                        guideText.SetActive(false);
                        playerController.isPlayerStop = false;
                        isChice = false;
                    }
                }
                break;

            case Choice.No:
                noText.color = Color.red;
                yesText.color = Color.white;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (nextStage != StageName.Stage_5)
                    {
                        ScreenSystem.instance.ScreenPlay(false);
                        isChice = false;
                    }

                    switch (nextStage)
                    {
                        case StageName.None:
                            guideText.SetActive(false);
                            isChice = false;
                            playerController.isPlayerStop = false;
                            break;

                        case StageName.Stage_1:
                            SceneLoader.instance.LoadSceneDelay("Ending_Stage1", 3); 
                            break;
                        case StageName.Stage_2:
                            SceneLoader.instance.LoadSceneDelay("Ending_Stage2", 3);
                            break;
                        case StageName.Stage_3:
                            SceneLoader.instance.LoadSceneDelay("Ending_Stage3", 3);
                            break;
                        case StageName.Stage_4:
                            SceneLoader.instance.LoadSceneDelay("Ending_Stage4", 3);
                            break;
                        case StageName.Stage_5:
                            guideText.SetActive(false);
                            isChice = false;
                            playerController.isPlayerStop = false;
                            break;
                    }
                }
                break;

            case Choice.None:
                noText.color = Color.white;
                yesText.color = Color.white;
                break;
        }
    }

    //스테이지 이동 X
    private IEnumerator MapOpenDoor(Collision2D collision)
    {
        collision.gameObject.GetComponent<PlayerController>().isPlayerStop = true;
        playerController.anim.Play("Kick");

        yield return new WaitForSecondsRealtime(0.2f);

        anim.enabled = true;
        doorCollider.enabled = false;

        yield return new WaitForSecondsRealtime(0.2f);
        playerController.anim.Play("Idle");
        collision.gameObject.GetComponent<PlayerController>().isPlayerStop = false;
    }
}
