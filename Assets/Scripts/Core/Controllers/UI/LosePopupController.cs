using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LosePopupController : MonoBehaviour
{
    public Image loseIcon;
    public GameObject progressObject;
    public Image targetIcon;
    public Image progressFillImage;
    public TextMeshProUGUI targetText;
    public Button restartButton;
    // Start is called before the first frame update
    public void Show(float value, float maxValue, int targetId)
    {
        progressObject.SetActive(maxValue > 0);
        LeanTween.scale(loseIcon.gameObject, Vector3.one, 0.33f).setOnComplete(() =>
        {
            progressFillImage.fillAmount = value / maxValue;
            targetText.text = value + "/" + maxValue;
            targetIcon.sprite = Resources.Load<Sprite>("Sprites/Items/Item_" + targetId);
            LeanTween.scale(progressObject, Vector3.one, 0.33f).setOnComplete(() =>
            {
                restartButton.gameObject.SetActive(true);
            });
        });
    }


    public void Replay()
    {
        gameObject.SetActive(false);
        GamePlayController.Instance.Replay();
    }
}
