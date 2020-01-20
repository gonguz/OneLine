using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ButtonLevel class allows us to manage a button of SelectLevel scene, setting it lock or
/// unlocked depending on the levels passed of each category
/// </summary>
public class ButtonLevel : MonoBehaviour
{
    //bool for button (locked or not locked sprite)
    private bool _locked;

    //index of button
    private int _index;

    //Sprite to render depending on button's state
    public Sprite unlockedSprite;
    public Sprite unlockedPressedSprite;

    /// <summary>
    /// Set button to locked
    /// </summary>
    void Start()
    {
        _locked = true;
    }


    /// <summary>
    /// Method to set button locked or unlocked. If the button is unlocked
    /// we change the sprite to the unlocked one, and put below the "star" of the sprite, 
    /// its correspondant level
    /// </summary>
    /// <param name="newValue">value to set the button state
    public void SetLocked(bool newValue)
    {
        _locked = newValue;
        if (!_locked)
        {
            this.GetComponent<UnityEngine.UI.Image>().sprite = unlockedSprite;
            string levelText = "";
            //Debug.Log(GameManager.Instance.GetCurrentLevel());
            if (_index + 1 < 10)
            {
                levelText = "00" + (_index + 1).ToString();
            }
            else if ( _index+1 < 100)
            {
                levelText = "0" + (_index + 1).ToString();
            }
            else {
                levelText = (_index + 1).ToString();
            }

            this.GetComponentInChildren<UnityEngine.UI.Text>().text = levelText;
        }
    }


    /// <summary>
    /// Getter for button state
    /// </summary>
    public bool GetLocked()
    {
        return _locked;
    }

    /// <summary>
    /// Setter for button index
    /// </summary>
    /// <param name="index">index the button will have
    public void SetButtonIndex(int index)
    {
        _index = index;
    }

    /// <summary>
    /// Getter for button index
    /// </summary>
    public int GetIndex()
    {
        return _index;
    }
}
