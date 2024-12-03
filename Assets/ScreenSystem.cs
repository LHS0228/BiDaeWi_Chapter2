using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenSystem : MonoBehaviour
{
    public static ScreenSystem instance;

    [SerializeField] Transform UICanves;
    [SerializeField] private GameObject screenPrefab;
    private GameObject MainCamera;
    private GameObject save_Screen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        MainCamera = Camera.main.gameObject;
    }

    public void ScreenPlay(bool isOn)
    {
        if(save_Screen == null)
        {
            save_Screen = Instantiate(screenPrefab, UICanves);
        }

        if(isOn)
        {
            save_Screen.GetComponent<Animator>().Play("ScreenOn");
        }
        else
        {
            save_Screen.GetComponent<Animator>().Play("ScreenOff");
        }
    }
}
