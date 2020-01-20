using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Class used for managing most of the gameplay, included mainmenu and selectlevel scenes.
/// Saves the count of levels passed in each category, coins and medals.
/// Starts every variable that will be used all over the game such as level reader or ads manager
/// </summary>
public class GameManager : MonoBehaviour
{

    //Instance of the class
    private static GameManager _gameManager = null;

    //Reference to component LevelReader
    private LevelReader _levelReader;


    //Enum that contains the categories the game has
    public enum LEVEL_CATEGORY { NONE, BEGINNER, REGULAR, ADVANCED, EXPERT, MASTER, CHALLENGE };

    //Keeps the current category. Is set from the main menu, when we press a category button 
    private LEVEL_CATEGORY _currentCategory;

    //Rows and cols of the current level
    private int _rows, _cols;

    //Constants for the number of categories and the levels of each category
    private const int NUM_CATEGORIES = 6;
    private const int NUM_LEVELS = 100;

    //keeps the coins and medals
    private int _coins;
    private int _medals;

   //Bool used for save if the current level is a challenge or not
    private bool _isChallenge;

    //Reference to the AdsManager
    private AdsManager _adsManager;


    //Reference to the timeStamp that "creates" the challenge button.
    private System.DateTime _timePressStamp;

    //Tuple for managing the current level and the max level reached of each category
    public class LevelTuple
    {
        public int currentLevel;
        public int maxLevel;
    }

    
    //Dictionary for creating all the levels of a category
    private Dictionary<LEVEL_CATEGORY, List<Level>> _categoryLevels;

    //Dictionary for saving the current level and max level of each category
    private Dictionary<LEVEL_CATEGORY, LevelTuple> _categoryMaxLevel;

    public static GameManager Instance
    {
        get
        {
            return _gameManager;
        }
    }


    /// <summary>
    /// Creates the gamemanager instance
    /// </summary>
    private void Awake()
    {
        //Only if gameManager is null, we init all if it is the first game and then load if there is some data saved before
        if (_gameManager == null)
        {
            _gameManager = this;
            
            DontDestroyOnLoad(this.gameObject);

            _gameManager.Init();
        }
 
    }

    /// <summary>
    /// Inits each variable once, at the beginning of the game.
    /// Loads the last state of the game, for keeping the player progress.
    /// </summary>
    private void Init()
    {
        // Add the AdsManager Component to the object
        _adsManager = gameObject.AddComponent(typeof(AdsManager)) as AdsManager;

        _categoryLevels = new Dictionary<LEVEL_CATEGORY, List<Level>>();
        _categoryMaxLevel = new Dictionary<LEVEL_CATEGORY, LevelTuple>();

        for(int i = 1; i <= NUM_CATEGORIES; i++)
        {
            LevelTuple lt = new LevelTuple();
            lt.currentLevel = 0;
            lt.maxLevel = 0;
            _categoryMaxLevel.Add((LEVEL_CATEGORY)i, lt);
        }
        _levelReader = new LevelReader();

        _coins = 0;
        _medals = 0;
        Serializer.Load();
    }


    /// <summary>
    /// Function that loads all the levels from the given category
    /// </summary>
    /// <param name="category"> used for getting its levels
    public void StartLevelCategory(LEVEL_CATEGORY category)
    {
        if (category != LEVEL_CATEGORY.CHALLENGE) { 
            _currentCategory = category;
            // Read the levels for a given category, e.g. BEGINNER
            List<Level> levels = _levelReader.CreateLevels(_currentCategory);
            // Store the levels read in a dictionary all category levels
            _categoryLevels[_currentCategory] = levels;
            _isChallenge = false;
        }
    }

    /// <summary>
    /// Function that gets the max level
    /// </summary>
    /// <param name="category"> used for getting its max level
    public int GetMaxLevelFromCategory(LEVEL_CATEGORY category)
    {
        return _categoryMaxLevel[category].maxLevel;
    }

    /// <summary>
    /// Function that sets the new max level
    /// </summary>
    /// <param name="category"> used for setting its max level
    public void SetMaxLevelFromCategory(LEVEL_CATEGORY category, int max)
    {
        _categoryMaxLevel[category].maxLevel = max;
    }
    public Dictionary<LEVEL_CATEGORY, LevelTuple> GetAllLevels()
    {
        return _categoryMaxLevel;
    }

    /// <summary>
    /// Gets the layout of the current level of the current category
    /// </summary>
    public string[] GetCurrentLevelLayout()
    {
        if (_currentCategory != LEVEL_CATEGORY.NONE) {
            List<Level> levels = new List<Level>(); 
            _categoryLevels.TryGetValue(_currentCategory, out levels);
            return levels[_categoryMaxLevel[_currentCategory].currentLevel].layout;
        }

        else
        {
            return null;
        }
    }

    /// <summary>
    /// Function that gets the path of the current level for creating the hints.
    /// </summary>
    public List<int[]> GetCurrentLevelPath()
    {
        if (_currentCategory != LEVEL_CATEGORY.NONE)
        {
            List<Level> levels = new List<Level>(); 
            _categoryLevels.TryGetValue(_currentCategory, out levels);
            return levels[_categoryMaxLevel[_currentCategory].currentLevel].path;
        }

        else
        {
            return null;
        }   
    }

    /// <summary>
    /// Function that gets the rows of the current level for creating the grid.
    /// </summary>
    public int GetCurrentLevelRows()
    {
        if (_currentCategory != LEVEL_CATEGORY.NONE)
        {
            List<Level> levels = new List<Level>(); 
            _categoryLevels.TryGetValue(_currentCategory, out levels);

            _rows = levels[_categoryMaxLevel[_currentCategory].currentLevel].layout.Length;
            return _rows;
        }

        else
        {
            return 0;
        }   
    }

    /// <summary>
    /// Function that gets the cols of the current level for creating the grid.
    /// </summary>
    public int GetCurrentLevelCols()
    {
        //Debug.Log(_currentLevel);
        if (_currentCategory != LEVEL_CATEGORY.NONE)
        {
            List<Level> levels = new List<Level>();
            _categoryLevels.TryGetValue(_currentCategory, out levels);

             _cols = levels[_categoryMaxLevel[_currentCategory].currentLevel].layout[0].Length;
            return _cols;
        }

        else
        {
            return 0;
        }
    }

    /// <summary>
    ///Sets the current category
    /// </summary>
    /// <param name="category"> used for setting current category
    public void SetCurrentCategory(LEVEL_CATEGORY category)
    {
        _currentCategory = category;
    }

    /// <summary>
    /// Gets the current category
    /// </summary>
    public LEVEL_CATEGORY GetCurrentCategory()
    {
        return _currentCategory;
    }

    /// <summary>
    ///Gets the current level of the current category
    /// </summary>
    /// <param name="category"> used for setting current category
    public int GetCurrentLevel()
    {
        
        return _categoryMaxLevel[_currentCategory].currentLevel;
    }

    /// <summary>
    /// Gets the max level of the category given
    /// </summary>
    /// <param name="category"> category given
    public int GetMaxLevel(LEVEL_CATEGORY category)
    {
        return _categoryMaxLevel[category].maxLevel;
    }

    /// <summary>
    /// Sets the current level of the current category
    /// </summary>
    /// <param name="level"> used for setting current level
    public void SetCurrentLevel(int level)
    {
        _categoryMaxLevel[_currentCategory].currentLevel = level;
    }

    /// <summary>
    /// Selects a random category between the "difficult" ones, and gets a random level of the
    /// category selected.
    /// Sets isChallenge true
    /// </summary>
    public void SetRandomChallenge()
    {
        GameManager.LEVEL_CATEGORY category = (GameManager.LEVEL_CATEGORY)Random.Range((int)GameManager.LEVEL_CATEGORY.ADVANCED, (int)GameManager.LEVEL_CATEGORY.CHALLENGE); // [ADVANCED, CHALLENGE) MASTER INCLUIDO
        StartLevelCategory(category);
        SetCurrentLevel(Random.Range(0, 100));
        _isChallenge = true;
    }


    /// <summary>
    /// Function that checks if the current level is lower than 100 and if a new level 
    /// can be unlocked checking if the current level played is higher
    /// than the max level. If it is, increases current level and max level.
    /// if not passes to the next category. If category is Master, "restarts" the game.
    /// </summary>
    public void IncreaseLevel()
    {
        if (_categoryMaxLevel[_currentCategory].currentLevel < NUM_LEVELS - 1)
        {
            _categoryMaxLevel[_currentCategory].currentLevel++;
            if (_categoryMaxLevel[_currentCategory].currentLevel > _categoryMaxLevel[_currentCategory].maxLevel)
                _categoryMaxLevel[_currentCategory].maxLevel++;
        }
        // TODO: MIRAR SI AL TERMINAR EL ULTIMO NIVEL DE UNA CATEGORIA PASA AL PRIMERO DE LA SIGUIENTE
        else
        {

            if (_currentCategory != LEVEL_CATEGORY.MASTER)
            {
                _currentCategory++;
                _categoryMaxLevel[_currentCategory].currentLevel = 0;
                _categoryMaxLevel[_currentCategory].maxLevel = 0;
                StartLevelCategory(_currentCategory);
            }
        }

        Serializer.Save();
    }

    // Retrieve the AdsManager object
    public AdsManager GetAdsManager()
    {
        return _adsManager;
    }

    /// <summary>
    /// Add value to coins
    /// </summary>
    /// <param name="value"> value given
    public void AddCoins(int value)
    {
        _coins += value;
    }


    /// <summary>
    /// Substract value to coins
    /// </summary>
    /// <param name="value"> value given
    public void SubstractCoins(int value)
    {
        if (_coins >= value)
        {
            _coins -= value;
        }
    }

    /// <summary>
    /// Get current coins
    /// </summary>
    public int GetCoins()
    {
        return _coins;
    }

    /// <summary>
    /// Add value to medals
    /// </summary>
    /// <param name="value"> value given
    public void AddMedals(int value)
    {
        _medals += value;
    }

    /// <summary>
    /// Get current medals
    /// </summary>
    public int GetMedals()
    {
        return _medals;
    }

    /// <summary>
    /// Get if current level is challenge
    /// </summary>
    public bool IsChallenge()
    {
        return _isChallenge;
    }

    /// <summary>
    /// Get number of categories
    /// </summary>
    public int GetCategories() 
    {
        return NUM_CATEGORIES;
    }

    /// <summary>
    /// Sets the current timestamp when challenge is pressed
    /// </summary>
    /// <param name="value"> value given
    public void SetPressTimeStamp(System.DateTime time)
    {
         _timePressStamp = time;
    }

    /// <summary>
    /// Gets the current timestamp when challenge is pressed
    /// </summary>
    public System.DateTime GetPressTimeStamp()
    {
        return _timePressStamp;
    }
    /// <summary>
    /// Gets time difference between current timestamp, and the saved one
    /// </summary>
    public System.TimeSpan GetTimeDifference()
    {
        Debug.Log(GetPressTimeStamp());
        System.TimeSpan sub = System.DateTime.Now.Subtract(GetPressTimeStamp());
        return sub;
    }
    
}
