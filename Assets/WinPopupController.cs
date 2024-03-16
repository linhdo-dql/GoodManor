using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopupController : MonoBehaviour
{
    public Image victoryIcon;
    public Image targetIcon;
    public Button restartButton;
    // Start is called before the first frame update
    public void Show(int targetId)
    {
        targetIcon.sprite = Resources.Load<Sprite>("Sprites/Items/Item_" + targetId);
        LeanTween.scale(victoryIcon.gameObject, Vector3.one, 0.33f).setOnComplete(() =>
        {
            LeanTween.scale(targetIcon.gameObject, Vector3.one, 0.33f).setOnComplete(() =>
            {
                restartButton.gameObject.SetActive(true);
            });
        });
    }


    public void Next()
    {
        gameObject.SetActive(false);
        GamePlayController.Instance.NextLevel(GamePlayController.Instance.levelData.id);
    }
}
