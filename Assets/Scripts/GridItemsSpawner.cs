using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItemsSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesContainer;


    [Header("Grid Variable")]
    [SerializeField] private int gridRowCount;
    [SerializeField] private int gridColCount;
    [SerializeField] private float tileSize;


    private Vector2 tilePadding = new Vector2(0.15f, 0.15f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GenerateChildTiles();
        }
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
        //Vector2 spawnStartPos = new Vector2(containerPos.x - containerHalfWidth + tileHalfWidth, containerPos.y + containerHalfHeight - tileHalfHeight);
        Vector2 spawnStartPos = new Vector2(containerPos.x - containerHalfWidth + tileHalfWidth + centerOffset.x,
                                            containerPos.y + containerHalfHeight - tileHalfHeight - centerOffset.y);



        //generate all child tiles
        for (int row = 0; row < gridRowCount; row++)
        {
            for(int col = 0; col < gridColCount; col++)
            {
                Vector2 tileNewPos = new Vector2(spawnStartPos.x + (row * (tileSize + tilePadding.x)) , spawnStartPos.y + (col * -(tileSize + tilePadding.y)) );
                GameObject tileNew = Instantiate(tilePrefab, tileNewPos, Quaternion.identity);
                tileNew.transform.localScale = new Vector2(tileSize, tileSize);
                Debug.Log("POarent :" + containerPos + "child pos:" + spawnStartPos);
                tileNew.transform.parent = tilesContainer;
            }
        }

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

}
