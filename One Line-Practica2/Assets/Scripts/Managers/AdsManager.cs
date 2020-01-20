using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

/// <summary>
/// Class used for managing ads in-game. We can create ads rewarded or not. If the ad created
/// is rewarded, we can skip it but reward will be not added. No rewarded ads, can be skipped.
/// </summary>
public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    //Id for the game
    private string _gameId;

    private bool _testMode = true;

    //Number used for the reward given
    private int _rewardCoins = 0;

    //Bool used for avoid user plays while an ad is being showed
    private bool canPlay = true;


    /// <summary>
    /// Creates and prepare the component that will manage the ads
    /// </summary>
    private void Awake()
    {

        #if UNITY_IOS
            _gameId = "3413994"; // Game id for Apple Store from Developer Dashboard 
        #elif UNITY_ANDROID
            _gameId = "3413995"; // Game id for Play Store from Developer Dashboard 
        #elif UNTIY_STANDALONE_WIN || UNITY_EDITOR
                _gameId = "1234567";
        #endif

        if (!Advertisement.isInitialized)
        {
            // Initialize the Ads listener and service:
            Advertisement.AddListener(this);
            Advertisement.Initialize(_gameId, _testMode);
        }
    }

    /// <summary>
    /// Function used for checking if the video has been skipped or not.
    /// If has been watched, we add its reward, if not, we do nothing
    /// </summary>
    /// <param name="placementId">id used for the ads
    /// <param name="showResult">Variable used for getting the final state of the add (has been watched or not)
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(placementId == "rewardedVideo")
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                // Reward the user for watching the ad to completion.

                Serializer.Save();

#if UNITY_EDITOR
                Debug.Log("The ad " + placementId + " was successfully shown.");
#endif
#if !UNTIY_STANDALONE_WIN
                canPlay = true;
#endif
                //GameManager.Instance.AddCoins(_rewardCoins);
            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
#if UNITY_EDITOR
                Debug.Log("The ad " + placementId + " was skipped before reaching the end.");
#endif
                //canPlay = true;
            }
            else if (showResult == ShowResult.Failed)
            {
#if UNITY_EDITOR
                Debug.LogWarning("The ad " + placementId + " did not finish due to an error.");
#endif
                //canPlay = true;
            }
        }

       
    }

    /// <summary>
    /// Function used for preparing the next ad to show.
    /// </summary>

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == "rewardedVideo")
        {
#if UNITY_EDITOR
            Debug.Log("Rewarded video is ready");
#endif
            //myButton.interactable = true;
        }
        else if (placementId == "video")
        {
#if UNITY_EDITOR
            Debug.Log("Video is ready");
#endif
        }

#if !UNTIY_STANDALONE_WIN
        canPlay = true;
#endif
    }

    /// <summary>
    /// for managing ad errors.
    /// </summary>
    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }


    /// <summary>
    /// for managing start of the add.
    /// </summary>
    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
#if UNITY_EDITOR
        Debug.Log("The ad has started");
#endif
#if !UNTIY_STANDALONE_WIN
        canPlay = false;
#endif
    }


    // -------------------------------------------------




    /// <summary>
    ///  Show an ad that you can skip
    /// </summary>
    public void ShowAd()
    {
        StartCoroutine(ShowAdCoroutine());
    }


    /// <summary>
    ///  Waits until ad is not ready.
    /// </summary>
    IEnumerator ShowAdCoroutine()
    {
        while (!Advertisement.IsReady("video"))
        {
            yield return new WaitForSeconds(0.5f);
        }

        canPlay = false;
#if UNITY_EDITOR
        Debug.Log("VIDEO");
#endif
        Advertisement.Show("video");

    }

    /// <summary>
    ///  Show an ad that can be skipped
    /// </summary>
    public void ShowNoSkipAd()
    {
        StartCoroutine(ShowNoSkippableAdCoroutine());
    }


    /// <summary>
    ///  Waits until ad is not ready.
    /// </summary>
    IEnumerator ShowNoSkippableAdCoroutine()
    {
        while (!Advertisement.IsReady("rewardedVideo"))
        {
            yield return new WaitForSeconds(0.5f);
        }

        canPlay = false;

#if UNITY_EDITOR
        Debug.Log("REWARDED VIDEO");
#endif
        Advertisement.Show("rewardedVideo");
    }

    /// <summary>
    ///  Sets the reward given for the rewarded ad
    /// </summary>
    public void SetRewardAmount(int rewardCoins)
    {
        _rewardCoins = rewardCoins;
        GameManager.Instance.AddCoins(_rewardCoins);
    }

#if !UNTIY_STANDALONE_WIN
    public bool GetCanPlay()
    {
        return canPlay;
    }
#endif
}
