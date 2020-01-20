using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Auxiliar class for controlling each back button all over the game
/// </summary>
public class BackButton : MonoBehaviour
{
    /// <summary>
    /// Adds a listener to each button with this component
    /// </summary>
    void Start()
    {
        this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => GoBack());
    }

    /// <summary>
    /// CallBack for the button. Gets the index of the current scene. if current level is a scene
    /// we want to go to mainmenu, if not, we go to the previous scene. Also, if we are in MainMenu 
    /// scene, whose previous index is -1, and press back button, we quit the application 
    /// </summary>
    public void GoBack()
    {
        int index = SceneManager.GetActiveScene().buildIndex - 1;
        if (index >= 0)
        {
            if (GameManager.Instance.IsChallenge())
            {
                SceneManager.LoadScene(index - 1);
            }
            else
            {
                SceneManager.LoadScene(index);
            }
            //GameManager.Instance.SetCurrentLevel(5);
        }
        else
        {
            Application.Quit();
            Serializer.Save();
        }
    }
}
