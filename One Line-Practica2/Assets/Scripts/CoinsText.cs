using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tool for set the coins panel text
/// </summary>
public class CoinsText : MonoBehaviour
{
    /// <summary>
    /// Every time is invoked for first time, gets the current coins
    /// </summary>
    void Start()
    {   
        GetComponentInChildren<UnityEngine.UI.Text>().text = GameManager.Instance.GetCoins().ToString();
        
    }

    /// <summary>
    /// Updates text with the given value
    /// </summary>
    /// <param name="value"> value to be shown in the coins panel
    public void AddCoins(int value) {

        GetComponentInChildren<UnityEngine.UI.Text>().text = value.ToString();
    }
}
