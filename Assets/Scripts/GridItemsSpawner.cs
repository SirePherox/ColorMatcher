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

    [SerializeField]
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
        //calculate spawn start position, relative to the parent container 
        Vector2 containerPos = tilesContainer.position;
        Vector2 containerLocalScale = tilesContainer.localScale;
        float containerHalfWidth = containerLocalScale.x / 2f;
        float containerHalfHeight = containerLocalScale.y / 2f;

        //calculate edge of container offset with the tile size
        Vector2 tileLocalScale = new Vector2(tileSize, tileSize);
        float tileHalfWidth = tileLocalScale.x / 2f;
        float tileHalfHeight = tileLocalScale.y / 2f;

        //calculate size so as to center all child
        Vector2 gridTotalSize = new Vector2(tileSize * gridColCount + (gridColCount - 1) * tilePadding.x, tileSize * gridRowCount + (gridRowCount - 1) * tilePadding.y);
        Vector2 centerOffset = gridTotalSize / -2f;

        //calculate starting position, relative to container and first tile 
        Vector2 spawnStartPos = new Vector2(containerPos.x - containerHalfWidth + tileHalfWidth, containerPos.y + containerHalfHeight - tileHalfHeight);
        //Vector2 spawnStartPos = new Vector2(containerPos.x - containerHalfWidth + tileHalfWidth + centerOffset.x, containerPos.y + containerHalfHeight - tileHalfHeight + centerOffset.y);

        GameObject tilesParent = new GameObject("TilesParent");
        tilesParent.transform.parent = tilesContainer.transform;

        //generate all child tiles
        for (int row = 0; row < gridRowCount; row++)
        {
            for(int col = 0; col < gridColCount; col++)
            {
                Vector2 tileNewPos = new Vector2(spawnStartPos.x + (row * (tileSize + tilePadding.x)) , spawnStartPos.y + (col * -(tileSize + tilePadding.y)) );
                GameObject tileNew = Instantiate(tilePrefab, tileNewPos, Quaternion.identity);
                Debug.Log("POarent :" + containerPos + "child pos:" + spawnStartPos);
                tileNew.transform.parent = tilesParent.transform;
            }
        }

        // Center the tilesParent in the container
        tilesParent.transform.position = containerPos;
    }

    #region 
    public void GenerateGrid(int rowCount, int colCount)
    {
       CalculateTileSize(rowCount,colCount);

        // Calculate the total size of the grid
        float parentWidth =tilesContainer.transform.localScale.x;
        float parentHeight = tilesContainer.transform.localScale.y;

        // Calculate the total size of the grid
        float gridWidth = gridColCount * tileSize;
        float gridHeight = gridRowCount * tileSize;

        // Calculate the starting position to center the grid within the parent
        float startX = -parentWidth / 2 + gridWidth / 2;
        float startY = -parentHeight / 2 + gridHeight / 2;


        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                GameObject tileNew = Instantiate(tilePrefab, tilesContainer);
                float posX = startX + col * tileSize;
                float posY = startY +  row * -tileSize;
                tileNew.transform.position = new Vector2(posX, posY);

                /// Set the size of the tile dynamically
                tileNew.transform.localScale = new Vector3(tileSize, tileSize, 1.0f);
            }
        }

        //position the whole grid at the center
        //float gridHeight = rowCount * tileSize;
        //float gridWidth = colCount * tileSize;

        //tilesContainer.transform.position = new Vector2(-gridWidth / 2, gridHeight / 2);
    }

    private void CalculateTileSize(int rowCount, int colCount)
    {
        // Calculate the tile size based on the container's size and grid size
        float containerWidth = tilesContainer.GetComponent<SpriteRenderer>().bounds.size.x;
        float containerHeight = tilesContainer.GetComponent<SpriteRenderer>().bounds.size.y;

       tileSize = Mathf.Min(containerWidth / gridRowCount, containerHeight / gridColCount);
        Debug.Log("Tile size:" + tileSize);
    }
    #endregion
}
