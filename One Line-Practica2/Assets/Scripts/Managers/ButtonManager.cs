using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
///  Class used for managing every button of the MainMenu scene. Creates the callback for the level buttons
///  and creates "some actions" for the rest, such as, exit button, or challenge panel buttons
/// </summary>
public class ButtonManager : MonoBehaviour
{
    //Array for the level buttons
    public UnityEngine.UI.Button[] _buttons;

    //Panel showed before starting the challenge
    public GameObject scrollViewPanelChallenge;

    //Checks if a button can be pressed
    private bool _canPress;

    //Text that will be shown over the challenge button
    public GameObject timerText;

    /// <summary>
    ///  Function that creates a callback for every button in the MainMenu scene
    /// </summary>
    private void Start()
    {
        scrollViewPanelChallenge.SetActive(false);
        for (int i = 0; i < _buttons.Length; i++)
        {
            int closureIndex = _buttons.Length - 1 - i; // Prevents the closure problem
            _buttons[i].GetComponentInChildren<UnityEngine.UI.Text>().text = (GameManager.Instance.GetMaxLevel((GameManager.LEVEL_CATEGORY)i+1)+1).ToString() + "/100";
            _buttons[closureIndex].onClick.AddListener(() => TaskOnClick(closureIndex));
        }

        _buttons[_buttons.Length - 1].GetComponentInChildren<UnityEngine.UI.Text>().text = GameManager.Instance.GetMedals().ToString();
    }

    /// <summary>
    ///  Callback function. Gets the index of the button. If its index is not the challenge one,
    ///  loads the select level scene, if not, it sets a challenge to be passed, and shows a panel.
    ///  Challenge button can only be pressed if 30 minutes has passed since the last play.
    /// </summary>
    ///  <param name="buttonIndex">index of the button pressed
    public void TaskOnClick(int buttonIndex)
    {
        if (buttonIndex + 1 != (int)GameManager.LEVEL_CATEGORY.CHALLENGE)
        {
            GameManager.Instance.StartLevelCategory((GameManager.LEVEL_CATEGORY)buttonIndex + 1);
            SceneManager.LoadScene("SelectLevel");
        }
        else
        {
            if((int)(Mathf.Round((float)GameManager.Instance.GetTimeDifference().TotalMinutes)) >= 30)
            {
                _canPress = true;
                Debug.Log("YES");
                GameManager.Instance.SetPressTimeStamp(System.DateTime.Now);
                Serializer.Save();
            }

            if (_canPress)
            {
                GameManager.Instance.SetRandomChallenge();
                scrollViewPanelChallenge.SetActive(true);
                _canPress = false;
            }
        
        }
    }

    /// <summary>
    ///  Function that closes the challengePanel and "restarts" the timestamp of the challenge button
    /// </summary>
    public void ClosePanelChallenge()
    {
        scrollViewPanelChallenge.SetActive(false);
        GameManager.Instance.SetPressTimeStamp(System.DateTime.Now);
        _canPress = true;
    }

    /// <summary>
    ///  Function That starts a challenge paying 25 coins
    /// </summary>
    public void InitChallenge()
    {
        if (GameManager.Instance.GetCoins() >= 25)
        {
            SceneManager.LoadScene("GamePlay");
            GameManager.Instance.SubstractCoins(25);
        }

    }


    /// <summary>
    ///  Function that starts a challenge showing an ad without reward
    /// </summary>
    public void InitFreeChallenge()
    {
        // Offer 0 coins for the video because is displayed for a free challenge
        GameManager.Instance.GetAdsManager().SetRewardAmount(0);
        GameManager.Instance.GetAdsManager().ShowNoSkipAd();

        SceneManager.LoadScene("GamePlay");
    }


    /// <summary>
    ///  Exit application
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
