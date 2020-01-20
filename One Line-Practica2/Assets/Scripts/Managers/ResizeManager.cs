using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeManager : MonoBehaviour
{
    public GameObject botCanvas;
    public GameObject topCanvas;

    public Canvas mainCanvas;
   

    private float gameZoneTilesWidth = 6.0f;
    private float gameZoneTilesHeight = 8.0f;

    
    public void Init()
    {

        //gameZoneTilesWidth = (GameManager.Instance.GetCurrentLevelCols() + 1) * 1.1f;
        //gameZoneTilesHeight = (GameManager.Instance.GetCurrentLevelRows() + 1) *1.1f;

        Canvas.ForceUpdateCanvases();

        // Obtain the total screen canvas size in pixels
        float screenHeight = Screen.height;

        // Calculate the top Canvas height in pixels
        float topCanvasHeight = topCanvas.GetComponent<RectTransform>().rect.height * mainCanvas.GetComponent<RectTransform>().localScale.y;

        // Calculate the bot Canvas height in pixels
        float botCanvasHeight = botCanvas.GetComponent<RectTransform>().rect.height * mainCanvas.GetComponent<RectTransform>().localScale.y;
      

        //Calculate the size of the space leave for the board in pixels
        float gameZoneHeight = screenHeight - botCanvasHeight - topCanvasHeight;

        //Calculate pixels per unit
        float pixelPerTileHeight = gameZoneHeight / gameZoneTilesHeight;

        // Obatin the size in units that the we want the Canvas to take
        float botCanvasUnitHeight = topCanvasHeight / pixelPerTileHeight;
        float topCanvasUnitHeight = botCanvasHeight / pixelPerTileHeight;

        //Now, we set the camera size regarding bot and top Canvases and set the position right in the middle
        float screenCenter = ((gameZoneTilesHeight + botCanvasUnitHeight + topCanvasUnitHeight) / 2.0f);
        Camera.main.orthographicSize = screenCenter;
        float yPosition = Camera.main.transform.position.y - (topCanvasUnitHeight - botCanvasUnitHeight) / 2.0f;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yPosition, Camera.main.transform.position.z);

        //At this point, the size of the camera is setted correctly to see all the board, but if the screen is taller than wider,
        //we need to adjust it, so we set the width of the camera to the board width. 
        float cameraWidthUnits = (Camera.main.orthographicSize * 2) * Camera.main.aspect;
        if (cameraWidthUnits < gameZoneTilesWidth)
        {

            float newCameraHeightSize = gameZoneTilesWidth / Camera.main.aspect;
            Camera.main.orthographicSize = (newCameraHeightSize / 2);

        }

        Destroy(gameObject);
    }
}
