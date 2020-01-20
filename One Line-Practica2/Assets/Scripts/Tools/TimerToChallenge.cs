using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for controlling timer text of the challenge
/// </summary>
public class TimerToChallenge : MonoBehaviour
{
    //Minutes of the timer
    public float timer = 30;
    //seconds passed of the timer
    private int seconds = 0;
    //minutes passed of the timer
    private int minutes = 0;
    //timer text to be updated
    public UnityEngine.UI.Text timeText;

    /// <summary>
    /// Inits timer in seconds
    /// </summary>
    private void Start()
    {
        timer = timer*60 - (float)GameManager.Instance.GetTimeDifference().TotalSeconds;
    }

    /// <summary>
    /// Updates values of minutes and seconds and updates the text
    /// </summary>
    void Update()
    {

        timer -= Time.deltaTime;
        minutes = (int)Mathf.Floor(timer / 60);
        seconds = (int)timer % 60;

        if(minutes <= 0)
        {
            minutes = 0;
        }

        if(seconds <= 0)
        {
            seconds = 0;
        }

        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
