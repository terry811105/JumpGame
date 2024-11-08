using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject tilemapSegment1;
    public GameObject tilemapSegment2;
    public GameObject tilemapSegment3;
    void Start()
    {
        tilemapSegment2.SetActive(false);
        tilemapSegment3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 確認碰撞的是玩家
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == tilemapSegment1)
            {
                LoadSegment(tilemapSegment1);
            }
            else if (other.gameObject == tilemapSegment2)
            {
                LoadSegment(tilemapSegment2);
            }
            else if (other.gameObject == tilemapSegment3)
            {
                LoadSegment(tilemapSegment3);
            }
        }
    }

    private void LoadSegment(GameObject segmentToLoad)
    {
        // 啟用選定的地圖片段，隱藏其他段
        tilemapSegment1.SetActive(segmentToLoad == tilemapSegment1);
        tilemapSegment2.SetActive(segmentToLoad == tilemapSegment2);
        tilemapSegment3.SetActive(segmentToLoad == tilemapSegment3);
    }
}
