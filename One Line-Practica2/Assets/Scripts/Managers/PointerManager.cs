using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that manages the pointer shown when we press or touch over the screen.
/// </summary>
public class PointerManager : MonoBehaviour
{
    //Reference to the gameObject that "represents" the pointer
    public GameObject pointer;

    //Reference to levelManager
    private LevelManager _levelManager;

    //Bool to make pointer visible or not
    private bool pointerVisible;

    /// <summary>
    ///Inits the pointer with a selected color, and starts the levelManager reference
    /// </summary>
    /// <param name="levelManager">Inits the levelManagerReference
    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
        UpdatePointerColor();
        HidePointer();
    }

    /// <summary>
    ///Updates the pointer color with the leather selected from the LeatherManager
    /// </summary>
    public void UpdatePointerColor()
    {
        pointer.GetComponent<SpriteRenderer>().sprite = LeatherManager.Instance.GetPointerSprite();
    }

    /// <summary>
    ///Updates the input
    /// </summary>
    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    ///Updates the pointer position to the given position
    /// </summary>
    /// <param name="pos">position that the pointer will have
    void UpdatePointerPosition(Vector2 pos)
    {
        if (!pointerVisible)
        {
            pointerVisible = true;
            pointer.GetComponent<SpriteRenderer>().enabled = true;
        }

        pointer.transform.position = new Vector2(pos.x, pos.y);
    }

    /// <summary>
    /// Hides the pointer
    /// </summary>
    void HidePointer()
    {
        // if (pointerVisible)
        //{
        pointer.GetComponent<SpriteRenderer>().enabled = false;
        pointerVisible = false;
        // }
    }

    /// <summary>
    /// HandleInput of the game. Is platform dependant.
    /// If we are on Windows: Check if mouse has been pressed. If it is, take the position of the press
    /// and throw a ray. if the ray collides with a game object with a component cell, we "color" the cell pressed
    /// with its direction. Also checks if level is not finsihed yet. Pointer will be shown only if we click
    /// inside the grid.
    /// For Android: The same as Windows, but using touch, not mouse.
    /// </summary>
    void HandleInput()
    {

        // Platform dependent compilation:
        // In case the platform is Windows or Unity Editor
#if UNITY_STANDALONE_WIN || UNITY_EDITOR


        if (Input.GetMouseButton(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            UpdatePointerPosition(worldPos);

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if(hit.collider != null && hit.collider.GetComponent<Cell>() != null && !_levelManager.GetFinished() && GameManager.Instance.GetAdsManager().GetCanPlay())
            {
                _levelManager.GetCellManager().MakeMove(hit.collider.GetComponent<Cell>()); // param = Cell
                // Check if the level is completed
                _levelManager.LevelFinished();
            }
            // This check if the pointer is in the limits of the tile grid in order to show the pointer or not
            /*if (!_levelManager.GetCellManager().IsInsideGrid(worldPos))
            {
                HidePointer();
            }*/

        }

        else if (Input.GetMouseButtonUp(0))
        {
           // The left mouse button is up so the pointer must no be shown
           HidePointer();
        }

        // In other case, if the platform is Android and is not Unity Editor
#elif UNITY_ANDROID && !UNITY_EDITOR

        // Handle screen touches.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Move the cube if the screen has the finger moving.
            if (touch.phase != TouchPhase.Ended)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);

                UpdatePointerPosition(worldPos);

                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

                if (hit.collider != null && hit.collider.GetComponent<Cell>() != null && !_levelManager.GetFinished() && GameManager.Instance.GetAdsManager().GetCanPlay())
                {
                    _levelManager.GetCellManager().MakeMove(hit.collider.GetComponent<Cell>()); // param = Cell
                                                                                                // Check if the level is completed
                    _levelManager.LevelFinished();
                }
                // This check if the pointer is in the limits of the tile grid in order to show the pointer or not
                /*if (!_levelManager.GetCellManager().IsInsideGrid(worldPos))
                {
                    HidePointer();
                }*/
            }
            else
            {
                // The touch is finished so the pointer must no be shown
                HidePointer();
            }
        }
#endif
    }
}
