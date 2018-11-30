using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour {

    public static CountdownTimer instance;

    public float timeLeft = 300.0f;
    private double sanityLoss =  0f;
    private float lossMult = .35f;
    public bool stop;

    private float minutes;
    private float seconds;

    public TextMeshProUGUI TextPro;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void StartTimer()
    {
            transform.GetChild(0).gameObject.SetActive(true);
            stop = false;
            Update();
            StartCoroutine(updateCoroutine());
    }

    public void StopTimer()
    {
        stop = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        if (stop)
        {            
            return;
        }

        timeLeft -= Time.deltaTime;
        sanityLoss += lossMult * Time.deltaTime;
        if (sanityLoss >= 1f)
        {
            PlayerController.instance.currentStats.currentSanity -= (int)sanityLoss;
            sanityLoss = 0f;
        }
        GameMenu.instance.sanityBar.UpdateBar(PlayerController.instance.currentStats.currentSanity, PlayerController.instance.currentStats.maxSanity);
        //Debug.Log("Player Sanity: " + PlayerController.instance.currentStats.currentSanity);
        minutes = Mathf.Floor(timeLeft / 60);
        seconds = timeLeft % 60;
        if (seconds > 59) seconds = 59;
        if (minutes < 0)
        {
            //stop = true;
            minutes = 0;
            seconds = 0;
            lossMult = 1f;
        }
        
        //        fraction = (timeLeft * 100) % 100;
    }

    private IEnumerator updateCoroutine()
    {
        while (!stop && Time.timeScale == 1)
        {
            TextPro.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(0.2f);
        }
    }
}