using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class whose aim is to create the grid of buttons at SelectLevel, and give them a callback
/// </summary>
public class PopulateGrid : MonoBehaviour
{
   
    //array of GameObjects where buttons crerated will be saved
    private GameObject[] buttonsList;

    //number of buttons to generate
    public int numberOfButtons = 100;

    //prefab from where the button will be created
    public GameObject prefab;

    /// <summary>
    /// This method just creates the "grid" of buttons
    /// </summary>
    void Start()
    {
        Populate();
    }

    /// <summary>
    /// Creates the array of buttons and gives an index and locked to each of them.
    /// If the current button index is lower than the max level of the current category
    /// we set it unlocked, and add to it the callback
    /// </summary>
    void Populate()
    {
        buttonsList = new GameObject[numberOfButtons];
        GameObject gObj;
        for(int i = 0; i < numberOfButtons; i++)
        {
            gObj = Instantiate(prefab, transform);
            gObj.GetComponent<ButtonLevel>().SetLocked(true);
            gObj.GetComponent<ButtonLevel>().SetButtonIndex(i);
            gObj.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().enabled = false; // Child 1 is the STAR image
            buttonsList[i] = gObj;

            if(buttonsList[i].GetComponent<ButtonLevel>().GetIndex() <= GameManager.Instance.GetMaxLevel(GameManager.Instance.GetCurrentCategory()))
            {
                buttonsList[i].GetComponent<ButtonLevel>().SetLocked(false);
                buttonsList[i].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().enabled = true; // Child 1 is the STAR image
                int index = buttonsList[i].GetComponent<ButtonLevel>().GetIndex();
                buttonsList[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TaskOnClick(index));
            }
        }
    }

    /// <summary>
    /// Set the current level with the given index
    /// and loads Gameplay scene
    /// </summary>
    /// <param name="buttonIndex">index of the level that will be loaded

    public void TaskOnClick(int buttonIndex)
    {
        GameManager.Instance.SetCurrentLevel(buttonIndex);
        SceneManager.LoadScene("GamePlay");
    }
}
