
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Class used for managing a grid of objects. In this case, we use GameObjects with a 
/// component Cell, which will be the component we'll use for checking actions inside the grid.
/// </summary>

public class CellManager : MonoBehaviour
{
    //2D array of cols, rows
    GameObject[,] grid;

    //position of the first cell in the grid
    float iniPosX, iniPosY;

    //separation between cells
    float separation;

  
    int cols, rows;

    //Numbers for managing the "progress" inside the grid
    int _cellsFilled;
    int _totalCells;
    int _hintCells;


    // Stack that contains the cells activated in order, being at top the very last activated cell
    Stack<Cell> cellStack;
    // Used for managing the last activated cell
    Cell _lastActivatedCell;

    //Used for getting the coordinates of the first cell
    Cell.Coordinates firstCellCoordinates;

    //Used for getting the coordinates of the current last cell (hints)
    Cell.Coordinates currentCellCoordinates;

    //GameObject the grid contains
    public GameObject prefab;

    /// <summary>
    /// Inits the component
    /// </summary>
    public void Init()
    {
        _hintCells = 0;
        _cellsFilled = 0;
    }

    /// <summary>
    /// This method is used everytime a cell has received the mouse input. 
    /// Makes the respective action depending on the cell status.
    /// </summary>
    /// <param name="cell">The cell that has received the mouse input</param>
    public void MakeMove(Cell cell)
    {
        if (cell.IsActive())
        {
            ClearPath(cell);
        }
        else
        {
           
            _lastActivatedCell = cellStack.Peek();
            if (cell.IsAdjacent(_lastActivatedCell.GetCoordinates()))
            {
                
                AddCellToPath(cell);
            }
        }
    }

    /// <summary>
    /// This method is used everytime a cell has received the instruction of being "painted". 
    /// Makes the respective action depending on the cell status.
    /// </summary>
    /// <param name="cellToPush">The cell that has to be painted
    private void AddCellToPath(Cell cellToPush)
    {
        cellToPush.SetActive();
        cellToPush.SetDirection(_lastActivatedCell);

        if (cellToPush.GetDirection() == Cell.DIRS.LEFT || cellToPush.GetDirection() == Cell.DIRS.DOWN)
        {
            cellToPush.SetDirectionLine(cellToPush.GetDirection());
        }
        else
        {
            _lastActivatedCell.SetDirectionLine(cellToPush.GetDirection());
        }
        cellStack.Push(cellToPush);
        currentCellCoordinates = cellToPush.GetCoordinates();
        _cellsFilled++;
    }

    /// <summary>
    /// This method is used everytime a cell has received the instruction of being "erased". 
    /// Makes the respective action depending on the cell direction.
    /// </summary>
    /// <param name="cellToClear">The cell that has to be erased
    private void ClearPath(Cell cellToClear)
    {
        while (cellStack.Count > 1 && cellToClear != cellStack.Peek())
        {
            _lastActivatedCell = cellStack.Peek();
            cellStack.Peek().DeactivateCell();
            switch (cellStack.Peek().GetDirection())
            {
                //In case the direction of the cell that is gonna be erased:
                /*
                 LEFT: Deactivate any direction of any cell.
                 */
                case Cell.DIRS.LEFT:
                    cellStack.Peek().DeactivateDirectionLine();
                    if (cellToClear.GetDirection() == Cell.DIRS.NONE)
                    {
                        cellToClear.DeactivateDirectionLine();
                    }
                    if (cellToClear.GetDirection() == Cell.DIRS.DOWN)
                    {
                        cellToClear.ActivateVertical();
                    }
                    if (cellToClear.GetDirection() == Cell.DIRS.UP)
                    {
                        cellToClear.DeactivateVertical();
                    }
                    break;

                /*
                RIGHT: Deactivate any direction of any cell and deactive direction of itself depending of its own direction
                */
                case Cell.DIRS.RIGHT:
                    cellStack.Peek().DeactivateDirectionLine();
                    if (cellToClear.GetDirection() == Cell.DIRS.DOWN)
                    {
                        cellToClear.ActivateVertical();
                        cellToClear.DeactivateHorizontal();
                    }
                    else
                    {
                        cellToClear.DeactivateHorizontal();
                    }
                    if (cellToClear.GetDirection() == Cell.DIRS.NONE)
                    {
                        cellToClear.DeactivateDirectionLine();
                    }
                    break;
                /*
                 UP: Deactivate vertical direction of itself and activate its horizontal direction depending of previous cell's direction
               */
                case Cell.DIRS.UP:
                    cellToClear.DeactivateVertical();
                    if(cellToClear.GetDirection() == Cell.DIRS.LEFT)
                    {
                        cellToClear.ActivateHorizontal();
                    }
                    if (cellToClear.GetDirection() == Cell.DIRS.NONE)
                    {
                        cellToClear.DeactivateDirectionLine();
                    }
                    break;

                /*
                 DOWN: Deactivate any direction of any cell and activate or deactivate its own directions depending of previous cell's direction
               */
                case Cell.DIRS.DOWN:
                    cellStack.Peek().DeactivateDirectionLine();
                    if (cellToClear.GetDirection() == Cell.DIRS.LEFT)
                    {
                        cellToClear.ActivateHorizontal();
                        cellToClear.DeactivateVertical();
                    }
                    else
                    {
                        cellToClear.DeactivateHorizontal();
                        cellToClear.DeactivateVertical();
                    }
                    if(cellToClear.GetDirection() == Cell.DIRS.DOWN || cellToClear.GetDirection() == Cell.DIRS.UP)
                    {
                        cellToClear.ActivateVertical();
                    }
                    if (cellToClear.GetDirection() == Cell.DIRS.NONE)
                    {
                        cellToClear.DeactivateDirectionLine();
                    }
                    break;
                /*
                 NONE: Deactivate everything
               */
                case Cell.DIRS.NONE:
                    cellToClear.DeactivateDirectionLine();
                    break;
            }
            cellStack.Pop();
            _cellsFilled--;
            currentCellCoordinates = cellToClear.GetCoordinates();
        }
    }

    /// <summary>
    /// This method is used everytime we want to clear all the path. Pops out every cell (cleaning it out) 
    /// of the stack until only left the first cell.
    /// </summary>
    public void ClearAllPath()
    {
        while(cellStack.Count > 1)
        {
            cellStack.Peek().DeactivateCell();
            cellStack.Pop();
            _cellsFilled--;
        }
        cellStack.Peek().DeactivateDirectionLine();
        currentCellCoordinates = cellStack.Peek().GetCoordinates();
    }

    /// <summary>
    /// This method is used for creating a cell in concrete coordinates.
    /// The created cell can be of Type "Empty" or "First", or "Regular". Depending
    /// on its type we generate a cell with different parameters.
    /// </summary>
    /// <param name="empty">Cell is empty
    /// <param name="first">Cell is the first
    /// <param name="coords">Coordinates the cell will have
    private void GenerateCell(bool empty, bool first, Cell.Coordinates coords)
    {
        grid[coords.x, coords.y] = Instantiate(prefab);
        grid[coords.x, coords.y].GetComponent<Cell>().FillCell();
        grid[coords.x, coords.y].GetComponent<Cell>().DeactivateHint();
        grid[coords.x, coords.y].transform.SetParent(this.transform);
        if (!empty)
        {
            grid[coords.x, coords.y].GetComponent<Cell>().SetCoordinates(coords.x, coords.y);
            grid[coords.x, coords.y].GetComponent<Cell>().SetNoneDirection();
            if (first)
            {
                grid[coords.x, coords.y].GetComponent<Cell>().SetActive();
                firstCellCoordinates = coords;
                currentCellCoordinates = coords;
            }
        }
        else
        {
            grid[coords.x, coords.y].GetComponent<Cell>().SetCoordinates(-1, -1);
            grid[coords.x, coords.y].GetComponent<Cell>().SetInactive();
        }
        grid[coords.x, coords.y].GetComponent<Cell>().SetPosition(iniPosX + (separation * coords.y), iniPosY - (separation * coords.x));
    }

    /// <summary>
    /// This method is used for creating the path of hints. First of all we get which are our current coordinates
    /// and search for them in the hints layout of the level.
    /// Once we got them, we calculate the number of hints that we can create. 
    /// For each hint, calculate the coordinates of the previous one, and depending on that,
    /// activate the horizontal of vertical line.
    /// Maximum number of hints created at once, is five.
    /// </summary>
    /// /// <param name="_lManager">reference to LevelManager for getting the current coordinates for creating
    /// the hint
    public void CreateHintsPath(LevelManager _lManager)
    {
        /*bool found = false;
        int aux = 0;*/
        if(_hintCells+5 <= _totalCells - 1)
        {
            for(int i = 0; i < 5; i++)
            {
                int previousX = GameManager.Instance.GetCurrentLevelPath()[_hintCells][1];
                int previousY = GameManager.Instance.GetCurrentLevelPath()[_hintCells][0];
                int x = GameManager.Instance.GetCurrentLevelPath()[1 + _hintCells][1];
                int y = GameManager.Instance.GetCurrentLevelPath()[1 + _hintCells][0];
                if (previousX != x)
                {
                    if (previousX > x)
                    {
                        grid[y, x].GetComponent<Cell>().SetHHintActive();
                    }
                    else
                    {
                        grid[previousY, previousX].GetComponent<Cell>().SetHHintActive();
                    }
                }
                else
                {
                    if (previousY < y)
                    {
                        grid[y, x].GetComponent<Cell>().SetVHintActive();
                    }
                    else
                    {
                        grid[previousY, previousX].GetComponent<Cell>().SetVHintActive();
                    }
                }
                _hintCells++;
            }
        }
        else
        {
            for(int i = 0; i < _totalCells - _hintCells - 1; i++)
            {
                int previousX = GameManager.Instance.GetCurrentLevelPath()[_hintCells][1];
                int previousY = GameManager.Instance.GetCurrentLevelPath()[_hintCells][0];
                int x = GameManager.Instance.GetCurrentLevelPath()[_hintCells + 1][1];
                int y = GameManager.Instance.GetCurrentLevelPath()[_hintCells + 1][0];
                if (previousX != x)
                {
                    if (previousX > x)
                    {
                        grid[y, x].GetComponent<Cell>().SetHHintActive();
                    }
                    else
                    {
                        grid[previousY, previousX].GetComponent<Cell>().SetHHintActive();
                    }
                }
                else
                {
                    if (previousY < y)
                    {
                        grid[y, x].GetComponent<Cell>().SetVHintActive();
                    }
                    else
                    {
                        grid[previousY, previousX].GetComponent<Cell>().SetVHintActive();
                    }
                }
                _hintCells++;
            }
        }

        //DONE FOR CREATING A HINT FROM CURRENT PLAYER CELL
       /* for (int c = 0; c < GameManager.Instance.GetCurrentLevelPath().Count; c++)
        {
            int a = GameManager.Instance.GetCurrentLevelPath()[c][0];
            int b = GameManager.Instance.GetCurrentLevelPath()[c][1];
            if (GameManager.Instance.GetCurrentLevelPath()[c][1] == currentCellCoordinates.y && GameManager.Instance.GetCurrentLevelPath()[c][0] == currentCellCoordinates.x)
            {
                found = true;
                aux = c;
                break;
            }
        }*/
        /*if (found)
        {
           if (aux+5 <= GameManager.Instance.GetCurrentLevelPath().Count - 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    int previousX = GameManager.Instance.GetCurrentLevelPath()[aux][1];
                    int previousY = GameManager.Instance.GetCurrentLevelPath()[aux][0];
                    int x = GameManager.Instance.GetCurrentLevelPath()[aux+1][1];
                    int y = GameManager.Instance.GetCurrentLevelPath()[aux+1][0];
                    if (previousX != x)
                    {
                        if (previousX > x)
                        {
                            grid[y, x].GetComponent<Cell>().SetHHintActive();
                        }
                        else
                        {
                            grid[previousY, previousX].GetComponent<Cell>().SetHHintActive();
                        }
                    }
                    else
                    {
                        if (previousY < y)
                        {
                            grid[y, x].GetComponent<Cell>().SetVHintActive();
                        }
                        else
                        {
                            grid[previousY, previousX].GetComponent<Cell>().SetVHintActive();
                        }
                    }
                    if (aux+1 <= GameManager.Instance.GetCurrentLevelPath().Count)
                    {
                        aux++;
                    }
                    _hintCells ++;
                }
            }
            else
            {
                for (int i = 0; i < GameManager.Instance.GetCurrentLevelPath().Count - aux - 1; i++)
                {
                    int previousX = GameManager.Instance.GetCurrentLevelPath()[aux][1];
                    int previousY = GameManager.Instance.GetCurrentLevelPath()[aux][0];
                    int x = GameManager.Instance.GetCurrentLevelPath()[aux + 1][1];
                    int y = GameManager.Instance.GetCurrentLevelPath()[aux + 1][0];
                    if (previousX != x)
                    {
                        if (previousX > x)
                        {
                            grid[y, x].GetComponent<Cell>().SetHHintActive();
                        }
                        else
                        {
                            grid[previousY, previousX].GetComponent<Cell>().SetHHintActive();
                        }
                    }
                    else
                    {
                        if (previousY < y)
                        {
                            grid[y, x].GetComponent<Cell>().SetVHintActive();
                        }
                        else
                        {
                            grid[previousY, previousX].GetComponent<Cell>().SetVHintActive();
                        }
                    }
                    if (aux + 1 <= GameManager.Instance.GetCurrentLevelPath().Count)
                    {
                        aux++;
                    }
                    _hintCells++;
                }
            }
        }*/
    }


    /// <summary>
    /// This method is used for creating a grid of cells. Grid is always centered on screen.
    /// Depending on the dimensions of the grid, we scale the cells for a better fit on screen.
    /// </summary>
    /// <param name="cols, rows"> Dimensions of the grid
    public void CreateGrid()
    {
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        cellStack = new Stack<Cell>();
        _cellsFilled = 0;
        _hintCells = 0;
        
        cols = GameManager.Instance.GetCurrentLevelCols();
        rows = GameManager.Instance.GetCurrentLevelRows();
        _totalCells = cols * rows;
        separation = 1.1f;
        iniPosX = -(cols - 1) * (separation / 2);
        iniPosY = (rows - 1) * (separation / 2);

         grid = new GameObject[rows, cols];
        GameObject[,] auxGrid = grid;

         for (int i = 0; i < rows; i++)
         {
             for (int j = 0; j < cols; j++)
             {
                Cell.Coordinates cellCoords;
                cellCoords.x = i;
                cellCoords.y = j;
                if (GameManager.Instance.GetCurrentLevelLayout()[i][j] == '0') {
                    GenerateCell(true, false, cellCoords);
                    _totalCells--;
                }
                else
                {
                    if(GameManager.Instance.GetCurrentLevelLayout()[i][j] == '1')
                    {
                        GenerateCell(false, false, cellCoords);
                    }
                    else
                    {
                        GenerateCell(false, true, cellCoords);
                    }
                    auxGrid[i, j].GetComponent<Cell>().StartCoroutineCell(true);
                }
             }
         }
         cellStack.Push(auxGrid[firstCellCoordinates.x, firstCellCoordinates.y].GetComponent<Cell>());
        _lastActivatedCell = auxGrid[(int)firstCellCoordinates.x, (int)firstCellCoordinates.y].GetComponent<Cell>();

        //First cell counted
        _cellsFilled++;


        //Don't know if this can be made
        if(GameManager.Instance.GetCurrentLevelCols() <= 5)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1.0f);
        }
        else
        {
            this.transform.localScale = new Vector3(0.9f, 0.9f, 1.0f);
        }

    }

    /// <summary>
    /// Checks if all cells have been filled.
    /// </summary>
    public bool GetCellsFilled()
    {
        return _cellsFilled == _totalCells;  
    }

    /// <summary>
    /// Checks if we can not create more hints.
    /// </summary>
    public bool GetHintsFilled()
    {
        return _hintCells == _totalCells - 1;  // Substract the first cell
    }

    /// <summary>
    /// Prepares the grid for the next level, destroying the cell, with an animation.
    /// </summary>
    public void ClearGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j].GetComponent<Cell>().DeactivateHint();
                grid[i, j].GetComponent<Cell>().DeactivateDirectionLine();
                grid[i, j].GetComponent<Cell>().StartCoroutineCell(false);
            }
        }
    }



    /// <summary>
    /// Checks if the position passed is inside the grid
    /// </summary>
    /// <param name="position"> Position to check
    public bool IsInsideGrid(Vector3 position)
    {
        bool limitX = (position.x >= (grid[0, 0].transform.position.x - (grid[0, 0].transform.lossyScale.x / 2)) && position.x <= (grid[0, cols - 1].transform.position.x + (grid[0, cols - 1].transform.lossyScale.x / 2)));
        // In the world space the y is higher when the object is higher and lower when the object is lower in the space
        bool limitY = (position.y <= (grid[0, 0].transform.position.y + (grid[0, 0].transform.lossyScale.y / 2)) && position.y >= (grid[rows - 1, 0].transform.position.y - (grid[rows - 1, 0].transform.lossyScale.y / 2)));

        return (limitX && limitY);
      
    }

}

