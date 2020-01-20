using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LevelManager allows us to keep the control of the current level, checking if has been passed, or not. Wether it
/// is or not, we'll do different actions.
/// Level manager saves references to cellmanager to check how many cells has been filled
/// </summary>
public class LevelManager : MonoBehaviour
{

    //References to different classes that will be used inside LevelManager
    public CellManager cellManager;
    public PointerManager pointerManager;
    public UIManager UIManager;
    public ResizeManager resizeManager;
    public Timer timer;

    //Keeps the time of the challenge
    private int time;

    //Bool for controlling if a level is finsihed or not
    private bool finished;

    //Gets from the game manager if current level is challenge
    private bool _levelIsChallenge;

    //Panel of coins in the upper right corner
    public GameObject panelCoins;

    /// <summary>
    ///Function called every time a level starts. If current level is challenge timer is activated
    ///if not, we activate hint, coins and clear buttons.
    /// </summary>
    private void Start()
    {
        Init();
        _levelIsChallenge = GameManager.Instance.IsChallenge();
        if (_levelIsChallenge)
        {
            UIManager.ActiveTimer();
        }
        else
        {
            UIManager.ActiveButtons();
        }
        panelCoins.GetComponentInChildren<UnityEngine.UI.Text>().text = GameManager.Instance.GetCoins().ToString();
    }

    /// <summary>
    /// Updates the timer if the current level is a challenge
    /// </summary>
    private void Update()
    {
        if (_levelIsChallenge)
        {
            if (timer.InTime())
            {

                time = timer.GetSeconds();
                UIManager.SetTimerText(time);
            }
            else
            {
                finished = true;
                StartCoroutine(WaitForChallengePanel());
            }
        }
      
        //HACK
        /*//LLamar a una pista cuesta 25 monedas, hay que administrat las monedas desde el gamemanager
        if (Input.GetKeyDown(KeyCode.A))
        {
            cellManager.CreateHintsPath(this);
        
        }*/
        
    }

    /// <summary>
    /// Inits all the variables needed for starting a level
    /// </summary>
    public void Init()
    {
        finished = false;
        time = 0;
        resizeManager.Init();
        LeatherManager.Instance.Init();
        cellManager.Init();
        cellManager.CreateGrid();
        pointerManager.Init(this);
        UIManager.Init(this);
    }

    /// <summary>
    /// Gets a reference to cell manager
    /// </summary>
    public CellManager GetCellManager()
    {
        return cellManager;
    }


    /// <summary>
    /// Checks if the current level has been completed. If has been, waits for a second (during that
    /// you can do nothing), and shows the respective panel, depending on the current level type (challenge or not)
    /// </summary>
    public void LevelFinished()
    {
        if(cellManager.GetCellsFilled())
        {
            //GameManager.Instance.GetAdsManager().ShowAd();

            finished = true;
            if (GameManager.Instance.IsChallenge())
            {
                timer.StopTimer();
                StartCoroutine(WaitForChallengePanel());
            }
            else
            {
                StartCoroutine(WaitForNormalPanel());
            }
        }
    }

    /*public bool CheckLevelFinished()
    {
        return cellManager.GetCellsFilled();
    }*/

    /// <summary>
    /// Creates next level clearing the previous one, and selecting a new leather.
    /// </summary>
    public void NextLevel()
    {
        finished = false;

        ClearLevel();

        LeatherManager.Instance.SelectRandomColor();
        pointerManager.UpdatePointerColor();
        StartCoroutine(WaitForNewLevel());

    }

    /// <summary>
    /// Coroutines that wait for a minutes and shows the normal or challenge panel. if the level
    /// is challenge, if has been completed in time, we got a win and get a reward.
    /// </summary>

    IEnumerator WaitForNormalPanel()
    {
        yield return new WaitForSeconds(1);
        UIManager.EndLevel();
        GameManager.Instance.GetAdsManager().ShowAd();
        GameManager.Instance.IncreaseLevel();       
    }
    IEnumerator WaitForNewLevel()
    {
        yield return new WaitForSeconds(1);
        cellManager.Init();
        cellManager.CreateGrid();
    }

    IEnumerator WaitForChallengePanel()
    {
        yield return new WaitForSeconds(1);
        if (timer.CheckSecondsInTime(time))
        {
            UIManager.EndChallengeLevel(true);
            GameManager.Instance.GetAdsManager().ShowAd();
            GameManager.Instance.AddMedals(1);
            GameManager.Instance.AddCoins(50);

            Serializer.Save();
        }
        else
        {
            UIManager.EndChallengeLevel(false);
        }       
    }

    // BUTTONS ACTIONS FOR GAMEPLAY

    /// <summary>
    /// Says to cell manager, that has to clear the grid
    /// </summary>
    public void ClearLevel()
    {
        cellManager.ClearGrid();
    }

    /// <summary>
    /// If cellManager has not every cell with a hint, we create a new hint.
    /// </summary>
    public void CreateHints()
    {
        if (!cellManager.GetHintsFilled())
        {
            cellManager.CreateHintsPath(this);
        }
    }

    /// <summary>
    /// Clears all the path
    /// </summary>
    public void ClearPath()
    {
        cellManager.ClearAllPath();
    }

    /// <summary>
    /// Getter for the finished bool
    /// </summary>
    public bool GetFinished()
    {
        return finished;
    }
}
