 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that represents a single cell. Each cell has different "components",
/// such as color, hints, direction lines..., also has its own direction, and coordinates
/// </summary>
public class Cell : MonoBehaviour
{
    // Enable sorite renderer for showing the color
    private bool _active;

    //Array of the sprites that a cell has
    private SpriteRenderer[] _childs;

    //Enum with all the "objects" a cell can have
    private enum CellObjects
    {
        COLOR,
        HORIZONTAL_LINE,
        VERTICAL_LINE,
        BLOCK,
        HORIZONTAL_HINT,
        VERTICAL_HINT
    }

    //Enum with all the directions a cell can have
    public enum DIRS
    {
        UP, DOWN, RIGHT, LEFT, NONE
    }
    DIRS _direction;

    //Coordinates of the cell
    public struct Coordinates
    {
        public int x; public int y;

    }

    Coordinates coords;

    /// <summary>
    /// Sets the direction to NONE, and fills a cell
    /// </summary>
    void Start()
    {
        _direction = DIRS.NONE;
        FillCell();
        _childs[(int)CellObjects.HORIZONTAL_HINT].GetComponent<SpriteRenderer>().sprite = _childs[(int)CellObjects.VERTICAL_HINT].GetComponent<SpriteRenderer>().sprite = LeatherManager.Instance.GetHintSprite();
    }

    /// <summary>
    /// Method that fills a cell with a color given from the leather manager
    /// </summary>
    public void FillCell()
    {
        _childs = GetComponentsInChildren<SpriteRenderer>();
        _childs[(int)CellObjects.COLOR].GetComponent<SpriteRenderer>().sprite = LeatherManager.Instance.GetCellSprite();
    }

    /// <summary>
    /// Method that actives a cell if this is not activated, and "paint" it, activating the
    /// color sprite
    /// </summary>
    public void SetActive()
    {
        if (!_active)
        {
            _active = true;
            _childs[(int)CellObjects.COLOR].GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    /// <summary>
    /// Sets the horizontal hint sprite active
    /// </summary>

    public void SetHHintActive()
    {
            _childs[(int)CellObjects.HORIZONTAL_HINT].GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Sets the vertical hint sprite active
    /// </summary>
    public void SetVHintActive()
    {
            _childs[(int)CellObjects.VERTICAL_HINT].GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Sets the horizontal hint sprite inactive
    /// </summary>
    public void DeactiveHHint()
    {
        if (_childs[(int)CellObjects.HORIZONTAL_HINT].GetComponent<SpriteRenderer>().enabled)
        {
            _childs[(int)CellObjects.HORIZONTAL_HINT].GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Sets the vertical hint sprite inactive
    /// </summary>
    public void DeactiveVHint()
    {
        if (_childs[(int)CellObjects.VERTICAL_HINT].GetComponent<SpriteRenderer>().enabled)
        {
            _childs[(int)CellObjects.VERTICAL_HINT].GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Sets the horizontal and vertical hint sprites inactive
    /// </summary>
    public void DeactivateHint()
    {
        DeactiveHHint();
        DeactiveVHint();
    }


    /// <summary>
    /// Sets the direction sprite of the cell depending on the dir given
    /// </summary>
    /// <param name="dir">direction to be activated
    public void SetDirectionLine(DIRS dir)
    {
        switch (dir)
        {
            case DIRS.LEFT:
                ActivateHorizontal();
                break;
            case DIRS.RIGHT:
                ActivateHorizontal();
                break;
            case DIRS.UP:
            case DIRS.DOWN:
                ActivateVertical();
                break;
            case DIRS.NONE:
                break;
        }
    }

    /// <summary>
    /// Sets the horizontal and vertical direction sprites inactive
    /// </summary>
    public void DeactivateDirectionLine()
    {
        DeactivateHorizontal();
        DeactivateVertical();
    }

    /// <summary>
    /// Sets the horizontal direction sprite active
    /// </summary>
    public void ActivateHorizontal()
    {
        _childs[(int)CellObjects.HORIZONTAL_LINE].GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Sets the horizontal direction sprite inactive
    /// </summary>

    public void DeactivateHorizontal()
    {
        _childs[(int)CellObjects.HORIZONTAL_LINE].GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Sets the vertical direction sprite inactive
    /// </summary>
    public void DeactivateVertical()
    {
        _childs[(int)CellObjects.VERTICAL_LINE].GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Sets the vertical direction sprite active
    /// </summary>
    public void ActivateVertical()
    {
        _childs[(int)CellObjects.VERTICAL_LINE].GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Getter for the active state of the cell
    /// </summary>
    public bool IsActive()
    {
        return _active;
    }

    /// <summary>
    /// Sets the color block sprite inactive
    /// </summary>
    void DeactivateColor()
    {
        _childs[(int)CellObjects.COLOR].GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Sets the cell inactive, deactivating directions and color sprites
    /// </summary>
    public void DeactivateCell()
    {
        _active = false;
        DeactivateHorizontal();
        DeactivateVertical();
        DeactivateColor();
    }


    /// <summary>
    /// Sets the cell coordinates
    /// </summary>
    /// <param name="x, y">coordinates the cell will have
    public void SetCoordinates(int x, int y)
    {
        coords.x = x;
        coords.y = y;
    }

    /// <summary>
    /// Getter for the cell coordinates
    /// </summary>
    public Coordinates GetCoordinates()
    {
        return coords;
    }

    /// <summary>
    /// Sets the cell position
    /// </summary>
    /// <param name="x, y">position the cell will have
    public void SetPosition(float x, float y)
    {
        this.transform.position = new Vector2(x, y);
    }

    /// <summary>
    /// Getter for the cell position
    /// </summary>
    public Vector2 GetPosition()
    {
        return this.transform.position;
    }

    /// <summary>
    /// Sets the cell direction to default
    /// </summary>
    public void SetNoneDirection()
    {
        _direction = DIRS.NONE;
    }

    /// <summary>
    /// Sets the cell direction depending on the previous cell in the path
    /// </summary>
    /// <param name="lastCell">previous cell on the path, from where we'll get its position in order to set
    /// currents cell's position
    public void SetDirection(Cell lastCell)
    {

        if (transform.position.x > lastCell.transform.position.x && transform.position.y == lastCell.transform.position.y)
        {
            _direction = DIRS.RIGHT;
        }
        if (transform.position.x < lastCell.transform.position.x && transform.position.y == lastCell.transform.position.y)
        {
            _direction = DIRS.LEFT;
        }
        if (transform.position.y > lastCell.transform.position.y && transform.position.x == lastCell.transform.position.x)
        {
            _direction = DIRS.UP;
        }
        if (transform.position.y < lastCell.transform.position.y && transform.position.x == lastCell.transform.position.x)
        {
            _direction = DIRS.DOWN;
        }
    }

    /// <summary>
    /// Getter for the cell direction
    /// </summary>
    public DIRS GetDirection()
    {
        return _direction;
    }

    /// <summary>
    ///Checks if this cell is adjacent to given coordinates
    /// </summary>
    /// <param name="c">coordinates given to check if it's adjacent
    public bool IsAdjacent(Coordinates c)
    {
        bool checkX = (coords.x == c.x) && ((coords.y + 1 == c.y) || (coords.y - 1 == c.y));
        bool checkY = (coords.y == c.y) && ((coords.x + 1 == c.x) || (coords.x - 1 == c.x));
        return checkX || checkY;
    }

    /// <summary>
    /// Deactivates the grey block
    /// </summary>
    public void DeactivateBlock()
    {
        _childs[(int)CellObjects.BLOCK].GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Deactivates all the cell, included the grey block
    /// </summary>
    public void SetInactive()
    {
        DeactivateHorizontal();
        DeactivateVertical();
        DeactivateBlock();
    }

    /// <summary>
    /// Coroutine that "animates" a cell to a lower or bigger scale, during a time.
    /// If we scale positive, the objective scale will be 1, if not, will be 0. The scale animation
    /// will be proportional to time passed as parameter. If its scale is lower than 0, we destroy the cell.
    /// </summary>
    /// <param name = "positive, time" > positive allows us to indicate if we want to scale it lower
    /// or bigger during a time (parameter)
    public IEnumerator ScaleCell(bool positive, int time)
    {
        Vector3 originalScale = this.transform.localScale;
        Vector3 destinationScale;
        if (positive)
        {
            destinationScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            destinationScale = new Vector3(0f, 0f, 0f);
        }

        float currentTime = 0.0f;

        do
        {
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
        if(transform.localScale.sqrMagnitude < 0.1f)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Starts scale cell coroutine
    /// </summary>
    /// <param name = "positive"> bool positive to pass to coroutine
    public void StartCoroutineCell(bool positive)
    {
        StartCoroutine(ScaleCell(positive, 1));
    }
}
