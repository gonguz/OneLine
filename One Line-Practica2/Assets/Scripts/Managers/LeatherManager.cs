using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class used for managing the leathers of the game. A leather is the color used for the pointer,
/// the cells, and the hints
/// </summary>
public class LeatherManager : MonoBehaviour
{
    //Instance of the class
    private static LeatherManager _leatherManager = null;

    //Array where all leathers will be saved
    private Object[] textures;

    //Array for identifying colors, with its name
    public string[] colors;

    //Dictionary that keeps a correspondance between name and leather
    public Dictionary<string, Object[]> texturesMap = new Dictionary<string, Object[]>();

    //Enum of objects that will have a leather
    public enum OBJECTS
    {
        CELL, HINT, POINTER
    }
    
    //Color of the leather
    int _color;


    public static LeatherManager Instance
    {
        get
        {
            
            return _leatherManager;
        }
    }

    /// <summary>
    /// Creates the instance of the Leathermanager, with all its textures, since the beginning of the game
    /// </summary>
    private void Awake()
    {
        if (_leatherManager != null && _leatherManager != this)
        {
            Destroy(this.gameObject);
        }

        _leatherManager = this;
        _leatherManager.InitTextures();
        DontDestroyOnLoad(this.gameObject);


    }

    /// <summary>
    /// Selects a random color for the first time
    /// </summary>
    public void Init()
    {
        SelectRandomColor();
    }

    /// <summary>
    /// Gets the color of the pointer
    /// </summary>
    public Sprite GetPointerSprite()
    {
        Object[] textures;
       bool found = texturesMap.TryGetValue(colors[_color], out textures);
        if(found)
        {
            return textures[(int)OBJECTS.POINTER] as Sprite;
        }
        else
        {
            return null;
        }
     
    }

    /// <summary>
    /// Gets the color of the hint
    /// </summary>
    public Sprite GetHintSprite()
    {
        Object[] textures;
        bool found = texturesMap.TryGetValue(colors[_color], out textures);
        if (found)
        {
            return textures[(int)OBJECTS.HINT] as Sprite;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the color of the cell
    /// </summary>
    public Sprite GetCellSprite() {

        Object[] textures;
        bool found = texturesMap.TryGetValue(colors[_color], out textures);
        if (found)
        {
            return textures[(int)OBJECTS.CELL] as Sprite;
        }
        else
        {
            return null;
        }
      
    }

    /// <summary>
    /// Function called every time we start a new game, sets a new leather
    /// </summary>
    public void SelectRandomColor()
    {
        _color = Random.Range(0, colors.Length);
    }


    /// <summary>
    /// Starts all the textures
    /// </summary>
    private void InitTextures()
    {
        for(int i = 0; i < colors.Length; i++)
        {
            texturesMap.Add(colors[i],
            Resources.LoadAll("Textures/game/" + colors[i], typeof(Sprite)));
        }
    }



}
