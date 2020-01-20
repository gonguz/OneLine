using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// UIManager is a class that will held the managing of the UI during the gameplay.
/// Will update the coins, timer text, show challenge or level panel...
/// </summary>
public class UIManager : MonoBehaviour
{
    //Reference to levelManager
    private LevelManager _levelManager;


    public Canvas _endLevelCanvas;

    //Images for the different panels, and images inside the panels
    public UnityEngine.UI.Image panel;
    public GameObject challengePanel;
    public GameObject winChallenge;
    public UnityEngine.UI.Image lostChallenge;

    //level shown on the level panel
    public UnityEngine.UI.Text levelText;

    //Text for timer and current category
    public GameObject timerText;
    public GameObject categoryText;
    public GameObject categoryTextInGame;

    //level shown beside the category in the upper left corner
    public UnityEngine.UI.Text levelTextInGame;

    //level buttons down (hint, clear, coins)
    public GameObject _levelButtons;

    //Panel for showing the coins
    public GameObject coinsPanel;
    
    //buttons for challenge panel
    public UnityEngine.UI.Button okButton;
    public UnityEngine.UI.Button duplicateButton;

    //bool for not clicking twice on x2 reward button
    private bool _duplicateRewardClicked;

    /// <summary>
    /// Hides every panel and activates buttons or timer depending if current level is challenge or not.
    /// Also shows the current category, and current level
    /// </summary>
    /// <param name="levelManager">reference to levelManager
    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
        _duplicateRewardClicked = false;
        //_endLevelCanvas.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
        challengePanel.SetActive(false);
        levelText.gameObject.SetActive(false);
        categoryText.SetActive(false);
        if (!GameManager.Instance.IsChallenge())
        {
            levelTextInGame.text = ((GameManager.Instance.GetCurrentLevel() + 1).ToString());
            //categoryTextInGame.GetComponent<UnityEngine.UI.Image>().sprite = categoryText.GetComponent<GetCategorySprite>().GetCategory((int)GameManager.Instance.GetCurrentCategory() - 1);
            ActiveButtons();
        }
        else
        {
            levelTextInGame.gameObject.SetActive(false);
            //categoryTextInGame.GetComponent<UnityEngine.UI.Image>().sprite = categoryText.GetComponent<GetCategorySprite>().GetCategory((int)GameManager.LEVEL_CATEGORY.CHALLENGE - 1);
        }
        timerText.SetActive(false);
    }

    /// <summary>
    /// Shows the category and level played, and the regular level panel
    /// </summary>
    public void EndLevel()
    {
        levelText.text = ((GameManager.Instance.GetCurrentLevel()+1).ToString());
        categoryText.GetComponent<UnityEngine.UI.Image>().sprite = categoryText.GetComponent<GetCategorySprite>().GetCategory((int)GameManager.Instance.GetCurrentCategory() - 1);
        panel.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        categoryText.SetActive(true);
    }

    /// <summary>
    /// Shows the panel of the challenge and shows certaint images depending if we got a win or not
    /// </summary>
    public void EndChallengeLevel(bool win)
    {
        timerText.SetActive(false);
        okButton.gameObject.SetActive(true);
        if (win)
        {
            winChallenge.SetActive(true);
            lostChallenge.gameObject.SetActive(false);
            duplicateButton.gameObject.SetActive(true);
        }
        else
        {
            lostChallenge.gameObject.SetActive(true);
            winChallenge.SetActive(false);
            duplicateButton.gameObject.SetActive(false);
        }
        challengePanel.SetActive(true);
    }

    /// <summary>
    /// Hides the regular level panel
    /// </summary>
    private void SetEndLevelInactive()
    {
        panel.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        categoryText.SetActive(false);
        timerText.SetActive(false);
    }

    /// <summary>
    ///Creats directly the next level
    /// </summary>
    public void OnClickPlay()
    {
        if (GameManager.Instance.IsChallenge())
        {
            _duplicateRewardClicked = true;
        }

        SetEndLevelInactive();
        // Start the next level
        _levelManager.NextLevel();
        levelTextInGame.text = ((GameManager.Instance.GetCurrentLevel() + 1).ToString());
        categoryTextInGame.GetComponent<UnityEngine.UI.Image>().sprite = categoryText.GetComponent<GetCategorySprite>().GetCategory((int)GameManager.Instance.GetCurrentCategory() - 1);
    }

    /// <summary>
    /// Loads MainMenu scene
    /// </summary>
    public void OnClickHome()
    {
        // Change to the main menu scene
        SceneManager.LoadScene("MainMenu");
    }


    /// <summary>
    /// Updates the timer text with the given value
    /// </summary>
    /// <param name="value">value given to update the timer</param>
    public void SetTimerText(int value)
    {
        timerText.GetComponent<UnityEngine.UI.Text>().text = value.ToString("00");
    }

    /// <summary>
    /// Actives timer
    /// </summary>
    public void ActiveTimer()
    {
        timerText.SetActive(true);
        _levelButtons.SetActive(false);
    }

    /// <summary>
    /// Active level buttons
    /// </summary>
    public void ActiveButtons()
    {
        _levelButtons.SetActive(true);
        timerText.SetActive(false);
    }

    /// <summary>
    /// Clears all the grid if there is no an ad, and level is not finished
    /// </summary>
    public void OnClickClearGrid()
    {
        if (!_levelManager.GetFinished() && GameManager.Instance.GetAdsManager().GetCanPlay())
        {
            _levelManager.ClearPath();
        }
    }

    /// <summary>
    /// Create a hint if there is no an ad, and level is not finished
    /// </summary>
    public void OnClickCreateHint()
    {
        if (!_levelManager.GetFinished() && !_levelManager.GetCellManager().GetHintsFilled() && GameManager.Instance.GetAdsManager().GetCanPlay())
        {
            if (GameManager.Instance.GetCoins() >= 25)
            {
                _levelManager.CreateHints();
                GameManager.Instance.SubstractCoins(25);
                coinsPanel.GetComponent<CoinsText>().AddCoins(GameManager.Instance.GetCoins());
                //SetTimerText(GameManager.Instance.GetCoins());
            }
        }
    }

    /// <summary>
    /// View a rewarded video if  there is no an ad, and level is not finished
    /// </summary>
    public void OnClickSeeRewardedVideo()
    {
        if (!_levelManager.GetFinished() && GameManager.Instance.GetAdsManager().GetCanPlay())
        {
            GameManager.Instance.GetAdsManager().ShowNoSkipAd();
            GameManager.Instance.GetAdsManager().SetRewardAmount(20);
            coinsPanel.GetComponent<CoinsText>().AddCoins(GameManager.Instance.GetCoins());
        }
    }

    /// <summary>
    /// View a rewarded x2 video if  there is no an ad, and level is not finished
    /// </summary>
    public void OnClickDuplicateRewardedVideo()
    {
        if (GameManager.Instance.GetAdsManager().GetCanPlay() && !_duplicateRewardClicked)
        {
            _duplicateRewardClicked = true;
            // Show an rewarded video
            GameManager.Instance.GetAdsManager().SetRewardAmount(50);
            GameManager.Instance.GetAdsManager().ShowNoSkipAd();

            coinsPanel.GetComponent<CoinsText>().AddCoins(GameManager.Instance.GetCoins());
        }   
    }


}
