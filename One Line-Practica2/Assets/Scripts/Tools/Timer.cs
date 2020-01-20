using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tool for creating the timer inside challenge level (30 seconds)
/// </summary>
public class Timer : MonoBehaviour
{
    //start time
    public float timer = 30.0f;

    //seconds passed
    private int seconds = 0;

    //bool that tells the timer to stop
    private bool stop = false;

    /// <summary>
    /// Updates the timer only if is not an ad being showed and timer is not stopped
    /// </summary>
    void Update()
    {
        //Debug.Log(GameManager.Instance.GetTimeDifference());
        if (GameManager.Instance.IsChallenge() && GameManager.Instance.GetAdsManager().GetCanPlay() && !stop)
        {
            timer -= Time.deltaTime;
            seconds = (int)timer % 60;
        }
    }

    /// <summary>
    /// Checks if timer is in time
    /// </summary>
    public bool InTime()
    {
        return seconds >= 0;
    }

    /// <summary>
    /// Getter for current seconds passed
    /// </summary>
    public int GetSeconds()
    {
        return seconds;
    }

    /// <summary>
    /// Checks if a given value is in time
    /// </summary>
    /// <param name="value">value to compare
    public bool CheckSecondsInTime(int value)
    {
        return value > 0;
    }

    /// <summary>
    /// Stops timer
    /// </summary>
    public void StopTimer()
    {
        stop = true;
    }
}
