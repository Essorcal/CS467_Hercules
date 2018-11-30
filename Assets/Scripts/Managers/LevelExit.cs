using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {

    public string levelToLoad;
    public string levelTransitionName;

    public float waitToLoad = 1f;
    private bool loadAfterFade;

    GameController gameController;

    void Update ()
    {
        if (loadAfterFade)
        {
            waitToLoad -= Time.deltaTime;
            if (waitToLoad <= 0)
            {
                loadAfterFade = false;
                SceneManager.LoadScene(levelToLoad);
                if (levelToLoad == "Game.MainScene")
                {
                    CountdownTimer.instance.StopTimer();
                    if ((GameController.control.twilightPortalStatus == 3) && (GameController.control.voidPortalStatus == 1) && (GameController.control.plasmaPortalStatus == 1))
                    {
                        GameMenu.instance.blur.SetActive(true);
                        GameMenu.instance.winScreen.SetActive(true);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        gameController = FindObjectOfType<GameController>();
        if (collider.tag == "Player" && gameController.canEnterPortal && gameController.isMainScene())
        {
            AudioManager.Instance.PlaySFX(6);
            loadAfterFade = true;
            UIFade.instance.Fade();
            PlayerController.instance.levelTransitionName = levelTransitionName;
        }
        else if(collider.tag == "Player" && !gameController.isMainScene())
        {
            gameController.CompletedPortal();
            loadAfterFade = true;
            UIFade.instance.Fade();
            PlayerController.instance.levelTransitionName = levelTransitionName;
        }
    }
}
