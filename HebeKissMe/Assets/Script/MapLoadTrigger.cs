using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoadTrigger : MonoBehaviour
{
    public GameObject tilemapCollider;
    public GameObject nextTilemap;
    // Start is called before the first frame update
    void Start()
    {
        nextTilemap.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("123");
        // 確認碰撞的是玩家
        if (other.CompareTag("Player"))
        {
            LoadSegment(nextTilemap);
        }
    }

    private void LoadSegment(GameObject segmentToLoad)
    {
        // 啟用選定的地圖片段，隱藏其他段
        nextTilemap.SetActive(segmentToLoad == nextTilemap);
    }
}
