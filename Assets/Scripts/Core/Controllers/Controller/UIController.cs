using GogoGaga.TME;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public List<GameObject> gamePopups = new List<GameObject>();
    public GameObject gamePlayUI;
    public GameObject homeUI;
    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }
    public void CloseAllPopup()
    {
        foreach(GameObject popup in gamePopups)
        {
            popup.SetActive(false);
        }
    }

    internal void BackToHome()
    {
        CloseAllPopup();
        if(gamePlayUI.activeSelf)
        {
            gamePlayUI.SetActive(false);
        }
        homeUI.SetActive(true);

        GamePlayController.Instance.IsOnPlay = false;
    }

    internal void GotoPlayUI()
    {
        CloseAllPopup();
        if (homeUI.activeSelf)
        {
            homeUI.SetActive(false);
        }
        gamePlayUI.SetActive(true);
        GamePlayController.Instance.IsOnPlay = true;
    }
}
