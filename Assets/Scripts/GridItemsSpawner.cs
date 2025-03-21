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
    [SerializeField]
    private float tileSize = 1;
    [SerializeField]
    private Vector2 tilePadding = new Vector2(0.15f, 0.15f);
    private bool hasCheckedGameStateOnAllTilesScored = false;


    private bool currentSessionWon;
    [HideInInspector] public bool isCurrentWinBestScore;
    // Start is called before the first frame update
    void Start()
    {
        GetSessionGridSize();
        GenerateChildTiles();
    }
    private void OnEnable()
    {
        GameplayManager.Instance.OnTimeReachZero += CheckGameStateOnTimeOut;
        GameplayManager.Instance.OnNewSessionDelayCountdownEvent += ResetOnNewSessionLoad;
    }
    private void OnDisable()
    {
        if (GameplayManager.Instance != null)
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

    private void GetSessionGridSize()
    {
        //must remain square grid, same value for row and column
        gridColCount = GameplayManager.Instance.CalculateNewSessionGridCount();
        gridRowCount = GameplayManager.Instance.CalculateNewSessionGridCount();
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



        List<Color32> UsedTileColors = new List<Color32>();

        //generate all child tiles
        for (int row = 0; row < gridRowCount; row++)
        {
            for (int col = 0; col < gridColCount; col++)
            {
                // Vector2 tileNewPos = new Vector2(spawnStartPos.x + (row * (tileSize + tilePadding.x)),
                // spawnStartPos.y + (col * -(tileSize + tilePadding.y)));
                Vector2 tileNewPos = new Vector2(spawnStartPos.x + (row * (tileSize + tilePadding.x)), spawnStartPos.y + (col * -(tileSize + tilePadding.y)));
                GameObject tileNew = Instantiate(tilePrefab, tileNewPos, Quaternion.identity);
                tileNew.transform.localScale = new Vector2(tileSize, tileSize);
                tileNew.transform.parent = tilesContainer;
                SpawnedTiles.Add(tileNew.transform); //save to tiles list
                //update the tile color
                Color32 randomColorToUse = colourWheelController.GetRandomTileColor();
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
        //set for the first 4 levels , then calculatee the others,
        //this is to make the first set  of levels very  easy
        int currentLvl = PlayerPrefs.GetInt(GamePrefabsNames.CURRENT_LEVEL, 1);
        if(currentLvl <= 4)
        {
            if (gridColCount <= 2)
            {
                tileSize = 1.7f;
            }
            else if (gridColCount <= 5)
            {
                tileSize = 1.0f;
            }
            else
            {
                Debug.LogError("Couldn't set the right tilesize based on existing conditions");
            }
        }
        else
        {
            // Determine available space within tilesContainer
            RectTransform tilesContainerRect = tilesContainer.GetComponent<RectTransform>();
            Vector2 availableSpace = tilesContainerRect.rect.size - tilePadding; //* 2f;  // Subtract padding

            // Calculate ideal tile size based on grid size and available space
            int gridSize = gridColCount * gridRowCount;
            float idealTileSize = Mathf.Min(availableSpace.x / gridColCount, availableSpace.y / gridRowCount);

            // Apply scaling based on grid size, ensuring smooth transitions
            float scaleFactor = Mathf.Max(0.6f, 1f - Mathf.Pow(gridSize / 40f, 2f));  // Adjusted exponent, values used are tested //DON'T CHANGE
            tileSize = idealTileSize * scaleFactor;
        }

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
        foreach (Transform tileT in SpawnedTiles)
        {
            if (!tileT.gameObject.activeSelf)
            {
                //if isnt active means it was scored
                scoredTiles++;

            }
        }
        Debug.Log("Total scored tiles number: " + scoredTiles);


        currentSessionWon = GameModeManager.Instance.CheckGameWonOrLostState(scoredTiles, totalTileCount, out isCurrentWinBestScore);
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

            if (GameModeManager.Instance.currentGameMode == GameModeManager.GamePlayMode.QuickRush)
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
        foreach (Transform tileTransform in SpawnedTiles)
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
        GetSessionGridSize();
        //generate new tiles
        GenerateChildTiles();
        Debug.Log("Loading new tiles");
    }

    private void ResetOnNewSessionLoad()
    {
        hasCheckedGameStateOnAllTilesScored = false;

    }
}
