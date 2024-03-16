using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUIController : MonoBehaviour
{
    // Start is called before the first frame update
   public void Continue()
   {
        gameObject.SetActive(false);
        GamePlayController.Instance.Continue();
   }

   public void Restart()
    {
        gameObject.SetActive(false);
        GamePlayController.Instance.NextLevel(GamePlayController.Instance.levelData.id-1);
    }

    public void GoBackHome()
    {
        GamePlayController.Instance.GobackHome();
    }
}
