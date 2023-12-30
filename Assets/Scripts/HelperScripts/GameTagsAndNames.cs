using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTagsAndNames : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class GamePrefabsNames
{
    public const string CURRENT_LEVEL = "CURRENT_LEVEL";
}

public class GameObjectNames
{
    public const string GRID_MANAGER = "GridManager"; //object with the GridItemsSpawner script
    public const string TIMER_MANAGER = "GameManager"; //oobject with the TimerManager Script
    
}

public class SceneIndex
{
    public const int MAIN_MENU = 0;
    public const int GAME_SCENE = 1;
}
