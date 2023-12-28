using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[DefaultExecutionOrder(200)] //delayed its execution because it needs to wait for Colors from ColourWheelController
public class GridItemsSpawner : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private ColourWheelController colourWheelController;

    [Header("References")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesContainer;
    [SerializeField]
    private List<Transform> SpawnedTiles = new List<Transform>();
 
    [Header("Grid Variable")]
    [SerializeField] private int gridRowCount;
    [SerializeField] private int gridColCount;
    private float tileSize = 1;
    private Vector2 tilePadding = new Vector2(0.15f, 0.15f);
    [SerializeField]
    private bool hasCheckedGameStateOnAllTilesScored = false;

    [SerializeField]
    private bool currentSessionWon;
    // Start is called before the first frame update
    void Start()
    {
        GenerateChildTiles();
    }
    private void OnEnable()
    {
        GameplayManager.Instance.OnTimeReachZero += CheckGameStateOnTimeOut;
        GameplayManager.Instance.OnNewSessionDelayCountdownEvent += ResetOnNewSessionLoad;
    }
    private void OnDisable()
    {
        if(GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnTimeReachZero -= CheckGameStateOnTimeOut;
            GameplayManager.Instance.OnNewSessionDelayCountdownEvent -= ResetOnNewSessionLoad;
        }

    }
    // Update is called once per frame
    void Update()
    {
        CheckGameStateOnAllTilesScored();
    }

    private void GenerateChildTiles()
    {
        
        CalculateTileSize();

        //calculate spawn start position, relative to the parent container 
        Vector2 containerPos = tilesContainer.position;
        Vector2 containerLocalScale = tilesContainer.localScale;
        float containerHalfWidth = containerLocalScale.x / 2f;
        float containerHalfHeight = containerLocalScale.y / 2f;

        //calculate edge of container offset with the tile size
        Vector2 tileLocalScale = new Vector2(tileSize, tileSize);
        float tileHalfWidth = tileLocalScale.x / 2f;
        float tileHalfHeight = tileLocalScale.y / 2f;

        //calculate container size so as to center all child with offset
        Vector2 gridTotalSize = new Vector2(tileSize * gridColCount + (gridColCount - 1) * tilePadding.x,
                                             tileSize * gridRowCount + (gridRowCount - 1) * tilePadding.y);
        Vector2 centerOffset = gridTotalSize / -2f + new Vector2(containerHalfWidth, containerHalfHeight); // Add half container size for proper centering


        //calculate starting position, relative to container and first tile 
        Vector2 spawnStartPos = new Vector2(containerPos.x - containerHalfWidth + tileHalfWidth + centerOffset.x,
                                            containerPos.y + containerHalfHeight - tileHalfHeight - centerOffset.y);



        List<Color> UsedTileColors = new List<Color>();
      
        //generate all child tiles
        for (int row = 0; row < gridRowCount; row++)
        {
            for(int col = 0; col < gridColCount; col++)
            {
                Vector2 tileNewPos = new Vector2(spawnStartPos.x + (row * (tileSize + tilePadding.x)) , spawnStartPos.y + (col * -(tileSize + tilePadding.y)) );
                GameObject tileNew = Instantiate(tilePrefab, tileNewPos, Quaternion.identity);
                tileNew.transform.localScale = new Vector2(tileSize, tileSize);
                tileNew.transform.parent = tilesContainer;
                SpawnedTiles.Add(tileNew.transform); //save to tiles list
                //update the tile color
                Color randomColorToUse = colourWheelController.GetRandomTileColor();
                tileNew.GetComponent<TileManager>().tileColor = randomColorToUse;
                //save the color used without repetition
                if (!UsedTileColors.Contains(randomColorToUse))
                {
                    UsedTileColors.Add(randomColorToUse);
                }
            }
        }
        //update the WheelColors with the colors used by tiles
        colourWheelController.UpdateColorsOnWheel(UsedTileColors);

    }

    private void CalculateTileSize()
    {
        //base sets
        const float MIN_BASE_TILESIZE_SCALE_FACTOR = 9f;
        const float MID_BASE_TILESIZE_SCALE_FACTOR = 36f;
        const float MAX_BASE_TILESIZE_SCALE_FACTOR = 100f;

        //calculate tile size dynamically, larger tiles for small grid counts, smaller tiles for bigger grid counts
        float gridSize = gridColCount * gridRowCount;
        float scaleFactor = 0;
        //since its always going to be a square matrix, check using row for the scale factor
        if(gridRowCount <= 5)
        {
            scaleFactor = Mathf.Clamp(MIN_BASE_TILESIZE_SCALE_FACTOR / gridSize, 0.8f, 2f);
        }
        else if(gridRowCount <= 10)
        {
            scaleFactor = Mathf.Clamp(MID_BASE_TILESIZE_SCALE_FACTOR / gridSize, 0.4f, 0.7f);
        }
        else
        {
            scaleFactor = Mathf.Clamp(MAX_BASE_TILESIZE_SCALE_FACTOR / gridSize, 0.1f, 0.35f);
        }
        
        tileSize *= scaleFactor;
    }

    private void ClearPreviousTiles()
    {
        //destroy any previous child,active/inactive, of the tiles container
        List<Transform> previousTransform = tilesContainer.GetComponentsInChildren<Transform>(true).Skip(1).ToList();
        // Iterate from the end to avoid index shifting
        for (int i = previousTransform.Count - 1; i >= 0; i--)
        {

            GameObject tileObject = previousTransform[i].gameObject;
            Destroy(tileObject);
        }

        //clear spawned tiles
        SpawnedTiles.Clear();
        SpawnedTiles = new List<Transform>();
        Debug.Log("Cleared all previous tiles");

    }

    private void CheckGameStateOnTimeOut()
    {
        int totalTileCount = gridColCount * gridRowCount;
        int scoredTiles = 0;
        foreach(Transform tileT in SpawnedTiles)
        {
            if (!tileT.gameObject.activeSelf)
            {
                //if isnt active means it was scored
                scoredTiles++;
                
            }
        }
        Debug.Log("Total scored tiles number: " + scoredTiles);

       
        currentSessionWon = GameModeManager.Instance.CheckGameWonOrLostState(scoredTiles, totalTileCount);
        GameplayManager.Instance.gameSessionWon = currentSessionWon;
        GameplayManager.Instance.gameSessionLost = !currentSessionWon;
        //after updatating game state, call the respective events
        GameplayManager.Instance.InvokeLevelWonOrLostEvents();
    }
   
    private void CheckGameStateOnAllTilesScored()
    {
        if (IsAllTilesScored())
        {
            if (!hasCheckedGameStateOnAllTilesScored)
            {
                GameplayManager.Instance.hasFinishedBeforeTimeUp = true;
                //check game state when all tiles are scored
                CheckGameStateOnTimeOut();
                hasCheckedGameStateOnAllTilesScored = true;
            }
        }
    }

    public bool IsAllTilesScored()
    {
        if (SpawnedTiles.Count == 0)
        {
            //this check is useful when a new session is been loaded
            Debug.Log("There was no child");
            return false;
        }
            

        bool allTilesScored = true;
        foreach (Transform tileT in SpawnedTiles)
        {
            if (tileT.gameObject.activeSelf)
            {
                //if is active means it hasnt scored
                allTilesScored = false;
            }
        }
        return allTilesScored;
    }

    /// <summary>
    /// Loads the next session based on ,  current game mode and whether its a win or lose for this current session
    /// </summary>
    public void LoadNextSessionRespectively()
    {
        Debug.Log("Attempting to load the next game session");
        if (!currentSessionWon)
        {
            //if this session was lost , simply reload the tiles again
            Debug.Log("Session Lost, reloading the tiles");
            ReloadSameTiles();
        }
        if (currentSessionWon)
        {
            //if this session was won, check the current game mode

            if(GameModeManager.Instance.currentGameMode == GameModeManager.GamePlayMode.QuickRush)
            {
                //if the game mode is the first mode, QuickRush, Load the same tile but change the gamemode, increase current level
                Debug.Log("Changing the game mode to TimeLapse, but reloading the same tiles");
                //reload same tiles
                ReloadSameTiles();
                //then change the current GamePlayMode
                GameModeManager.Instance.SwitchGameMode();
                
            }
            else if (GameModeManager.Instance.currentGameMode == GameModeManager.GamePlayMode.TimeLapse)
            {
                //if the game mode is the second mode, TimeLapse, Change mode and load new tiles , increase current level
                Debug.Log("Loading neew tiles, and changing mode");
                //then change the current GamePlayMode
                GameModeManager.Instance.SwitchGameMode();
                //load new tiles
                LoadNewTiles();
            }
            else
            {
                Debug.LogWarning("DONT KNOW HOW TO HANDLE NEXT SESSION LOADING");
            }


        }
    }

    private void ReloadSameTiles()
    {
        //get the currentSpawned tiles and Re-activate them
        foreach(Transform tileTransform in SpawnedTiles)
        {
            tileTransform.gameObject.SetActive(true);
            Debug.Log("All tiles were reloaded, and set active");
        }
    }

    private void LoadNewTiles()
    {
        ClearPreviousTiles();
        Debug.Log("Loading new tiles......");
        //generate new colors
        colourWheelController.GenerateColoursToWheelOnNewSessionLoad();
        //reset default tilesize
        tileSize = 1; //1 is the original scalee factor of the square sprite that was creeated
        //generate new tiles
        GenerateChildTiles();
        Debug.Log("Loading new tiles");
    }

    private void ResetOnNewSessionLoad()
    {
        hasCheckedGameStateOnAllTilesScored = false;

    }
}
