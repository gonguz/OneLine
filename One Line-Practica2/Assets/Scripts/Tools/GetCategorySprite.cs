using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tool class that allows us to draw in gameplay the name of the current category. It's a quite simple class
/// </summary>
public class GetCategorySprite : MonoBehaviour
{
    //Array of the category names sprites
    public Sprite[] categories;

    /// <summary>
    /// Getter for the sprite of the category given
    /// </summary>
    /// <param name="c">category from where we want to get the sprite
    /// 
    private void Start()
    {
        GameManager.LEVEL_CATEGORY category = GameManager.Instance.GetCurrentCategory();
        if (!GameManager.Instance.IsChallenge())
        {
            this.GetComponent<UnityEngine.UI.Image>().sprite = this.GetCategory((int)category - 1);
        }
        else
        {
            this.GetComponent<UnityEngine.UI.Image>().sprite = this.GetCategory((int)GameManager.LEVEL_CATEGORY.CHALLENGE - 1);
        }
    }
    public Sprite GetCategory(int c)
    {
        return categories[c];
    }
}
