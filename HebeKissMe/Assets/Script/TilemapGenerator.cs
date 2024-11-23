using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [Header("基本設置")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;    // 1: 地板
    // [SerializeField] private TileBase treeTile;      // 2: 樹木
    // [SerializeField] private TileBase animalTile;    // 3: 動物
    [SerializeField] private TileBase rockTile;      // 4: 岩石
    private string[] mapData = new string[]
    {
        "00000000000000000",
        "00000400000004440",
        "00000400000000000",
        "00000400000000000",
        "111111111111111111111111111"
    };
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMap()
    {
        // 確保Tilemap有碰撞體組件
        TilemapCollider2D tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            tilemapCollider = tilemap.gameObject.AddComponent<TilemapCollider2D>();
        }

        // 如果需要複合碰撞體（優化性能），可以添加複合碰撞體
        CompositeCollider2D compositeCollider = tilemap.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null)
        {
            compositeCollider = tilemap.gameObject.AddComponent<CompositeCollider2D>();
            tilemap.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            tilemapCollider.usedByComposite = true;
        }

        // 清空現有的 tilemap
        tilemap.ClearAllTiles();

        // 遍歷地圖數據
        for (int y = 0; y < mapData.Length; y++)
        {
            string row = mapData[y];
            for (int x = 0; x < row.Length; x++)
            {
                char tileChar = row[x];
                Vector3Int tilePosition = new Vector3Int(x, -y, 0);

                // 根據數字放置對應的 Tile
                switch (tileChar)
                {
                    case '1':
                        tilemap.SetTile(tilePosition, groundTile);
                        break;
                    // case '2':
                    //     tilemap.SetTile(tilePosition, treeTile);
                    //     break;
                    // case '3':
                    //     tilemap.SetTile(tilePosition, animalTile);
                    //     break;
                    case '4':
                        tilemap.SetTile(tilePosition, rockTile);
                        break;
                }
            }
        }
    }

    // 提供一個公開方法來更新地圖數據
    public void UpdateMapData(string[] newMapData)
    {
        mapData = newMapData;
        GenerateMap();
    }
}
