using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIController : MonoBehaviour
{
    private void Start()
    {
        GamePlayController.Instance.IsOnPlay = false;
    }
    public void Play()
    {
        UIController.instance.GotoPlayUI();
        GamePlayController.Instance.NextLevel(GamePlayController.Instance.levelData.id - 1);
    }
}
