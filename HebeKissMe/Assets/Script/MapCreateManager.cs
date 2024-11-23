using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class MapCreateManager : MonoBehaviour
{
    [Header("基本設置")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;    // 1: 地板
    [SerializeField] private TileBase rockTile;      // 4: 岩石
    [SerializeField] private TileBase triggerTile;   // 9: 觸發器
    
    [Header("分段設置")]
    [SerializeField] private int segmentWidth = 5;   // 每段地圖的寬度
    [SerializeField] private int maxCachedSegments = 3; // 最大緩存段落數
    [SerializeField] private float preloadDistance = 2f; // 預載入觸發距離
    
    [Header("過渡效果")]
    [SerializeField] private float transitionDuration = 0.5f;
    // [SerializeField] private Image fadePanel; // 用於淡入淡出的UI面板
    
    private string[] fullMapData = new string[]
{
    "1111111111111111111111111111111111111111111111",
    "1000000000000000000000000000000000000000000001",
    "1004000400004000400004000004000400004000000001",
    "1000000000000000000000000000000000000000000001",
    "1001111000111100011110001111000111100011110001",
    "1001001000100010001001000100100010010001001001",
    "1004000400004000400004000400004000400004000001",
    "1000000000000000000000000000000000000000000001",
    "1111111111111111111111111111111111111111111111",
    "1000000000000000000000000000000000000000000001",
    "1000000000000000000400000000000000040000000001",
    "1000000000000000000000000000000000000000000001",
    "1111111111111111111111111111111111111111111111"
};
    private Dictionary<int, MapSegment> mapSegments = new Dictionary<int, MapSegment>();
    private HashSet<int> loadedSegments = new HashSet<int>();
    private Queue<int> segmentCache = new Queue<int>();
    private int currentSegmentIndex = 0;
    private bool isTransitioning = false;

    // 用於儲存地圖段落資訊的類
    [System.Serializable]
    public class MapSegment
    {
        public string[] data;
        public GameObject trigger;
        public Vector2 position;
        public bool isLoaded;
        public bool isPreloaded;
        public List<Vector3Int> tilePositions; // 儲存所有tile的位置

        public MapSegment(string[] data, Vector2 position)
        {
            this.data = data;
            this.position = position;
            this.isLoaded = false;
            this.isPreloaded = false;
            this.tilePositions = new List<Vector3Int>();
        }
    }

    void Start()
    {
        
        InitializeMap();
        
    }

    void InitializeMap()
    {
        SplitMapIntoSegments();
        StartCoroutine(LoadInitialSegments());
    }

    void printMapSegment()
    {
        Debug.Log("mapSegments.count:" + mapSegments.Count);
        for (int i = 0; i < mapSegments.Count; i++)
        {
            Debug.Log("mapSegments 第" + i + "個 mapSegments map key: " + mapSegments[i]);
        }
        foreach (var key in mapSegments.Keys)
        {
            Debug.Log("mapSegments contains key: " + key);
        }
    }

    IEnumerator LoadInitialSegments()
    {
        // 載入初始段落和預載入下一段
        yield return StartCoroutine(LoadSegmentWithTransition(0));
        StartCoroutine(PreloadSegment(1));
    }

    void SplitMapIntoSegments()
    {
        // 向上取整數
        int totalSegments = Mathf.CeilToInt((float)fullMapData[0].Length / segmentWidth);
        Debug.Log("totalSegments: " +totalSegments);
        Debug.Log("fullMapData.Length: " + fullMapData[0].Length + ", segmentWidth: " + segmentWidth);
        for (int i = 0; i < totalSegments; i++)
        {
            string[] segmentData = new string[fullMapData.Length];
            
            for (int y = 0; y < fullMapData.Length; y++)
            {
                int startX = i * segmentWidth;
                int length = Mathf.Min(segmentWidth, fullMapData[y].Length - startX);
                
                if (length > 0)
                {
                    string row = fullMapData[y].Substring(startX, length);
                    // 在兩端都添加觸發器，支援雙向移動
                    if (i < totalSegments - 1 && length == segmentWidth)
                    {
                        row = row.Substring(0, row.Length - 1) + "9";
                    }
                    if (i > 0 && length == segmentWidth)
                    {
                        row = "9" + row.Substring(1);
                    }
                    segmentData[y] = row.PadRight(segmentWidth, '0');
                }
                else
                {
                    segmentData[y] = new string('0', segmentWidth);
                }
            }
            Debug.Log("init MapSegment 第" + i + "個");
            mapSegments[i] = new MapSegment(
                segmentData,
                new Vector2(i * segmentWidth, 0)
            );
        }

        printMapSegment();
    }

    IEnumerator LoadSegmentWithTransition(int segmentIndex, bool isPreload = false)
    {
        if (!isPreload && isTransitioning) yield break;

        if (!isPreload)
        {
            isTransitioning = true;
            // yield return StartCoroutine(FadeOut());
        }

        yield return StartCoroutine(LoadSegmentCoroutine(segmentIndex, isPreload));

        if (!isPreload)
        {
            // yield return StartCoroutine(FadeIn());
            isTransitioning = false;
        }
    }

    IEnumerator LoadSegmentCoroutine(int segmentIndex, bool isPreload)
    {
        if (!mapSegments.ContainsKey(segmentIndex) || 
            (isPreload && mapSegments[segmentIndex].isPreloaded) ||
            (!isPreload && mapSegments[segmentIndex].isLoaded))
            yield break;

        MapSegment segment = mapSegments[segmentIndex];
        
        for (int y = 0; y < segment.data.Length; y++)
        {
            string row = segment.data[y];
            for (int x = 0; x < row.Length; x++)
            {
                char tileChar = row[x];
                Vector3Int tilePosition = new Vector3Int(
                    x + (int)segment.position.x,
                    -y + (int)segment.position.y,
                    0
                );

                switch (tileChar)
                {
                    case '1':
                        tilemap.SetTile(tilePosition, groundTile);
                        segment.tilePositions.Add(tilePosition);
                        break;
                    case '4':
                        tilemap.SetTile(tilePosition, rockTile);
                        segment.tilePositions.Add(tilePosition);
                        break;
                    case '9':
                        if (!isPreload)
                        {
                            CreateTrigger(tilePosition, segmentIndex);
                        }
                        break;
                }
            }
            
            // 每處理完一行就讓出一幀的時間
            yield return null;
        }

        if (isPreload)
        {
            segment.isPreloaded = true;
        }
        else
        {
            segment.isLoaded = true;
            loadedSegments.Add(segmentIndex);
            ManageSegmentCache(segmentIndex);
        }

        var tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = false; // 先禁用
            tilemapCollider.enabled = true;  // 再啟用，強制刷新
        }
    }

    void ManageSegmentCache(int newSegmentIndex)
    {
        segmentCache.Enqueue(newSegmentIndex);
        
        while (segmentCache.Count > maxCachedSegments)
        {
            int oldestSegment = segmentCache.Dequeue();
            if (Mathf.Abs(oldestSegment - currentSegmentIndex) > 1)
            {
                UnloadSegment(oldestSegment);
            }
            else
            {
                segmentCache.Enqueue(oldestSegment);
                break;
            }
        }
    }

    IEnumerator PreloadSegment(int segmentIndex)
    {
        if (!mapSegments.ContainsKey(segmentIndex) || mapSegments[segmentIndex].isPreloaded)
            yield break;

        yield return StartCoroutine(LoadSegmentWithTransition(segmentIndex, true));
    }

    void CreateTrigger(Vector3Int position, int segmentIndex)
    {
        GameObject trigger = new GameObject($"MapTrigger_{segmentIndex}");
        trigger.transform.position = tilemap.GetCellCenterWorld(position);
        
        BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, tilemap.size.y);

        Debug.Log($"Creating trigger for segment {segmentIndex}, position.x: {position.x}, triggersForward: {position.x < segmentIndex * segmentWidth}");
        bool isTriggerAtEnd = position.x == (segmentIndex + 1) * segmentWidth - 1; // 判斷是否位於段落尾部
        bool triggersForward = isTriggerAtEnd;
        MapTrigger triggerScript = trigger.AddComponent<MapTrigger>();
        triggerScript.Initialize(this, segmentIndex, triggersForward);

        mapSegments[segmentIndex].trigger = trigger;
    }

    // IEnumerator FadeOut()
    // {
    //     float alpha = 0;
    //     Color color = fadePanel.color;
        
    //     while (alpha < 1)
    //     {
    //         alpha += Time.deltaTime / transitionDuration;
    //         fadePanel.color = new Color(color.r, color.g, color.b, alpha);
    //         yield return null;
    //     }
    // }

    // IEnumerator FadeIn()
    // {
    //     float alpha = 1;
    //     Color color = fadePanel.color;
        
    //     while (alpha > 0)
    //     {
    //         alpha -= Time.deltaTime / transitionDuration;
    //         fadePanel.color = new Color(color.r, color.g, color.b, alpha);
    //         yield return null;
    //     }
    // }

    void UnloadSegment(int segmentIndex)
    {
        if (!mapSegments.ContainsKey(segmentIndex) || !mapSegments[segmentIndex].isLoaded)
            return;

        MapSegment segment = mapSegments[segmentIndex];
        
        foreach (Vector3Int pos in segment.tilePositions)
        {
            tilemap.SetTile(pos, null);
        }

        if (segment.trigger != null)
        {
            Destroy(segment.trigger);
        }

        segment.isLoaded = false;
        segment.isPreloaded = false;
        segment.tilePositions.Clear();
        loadedSegments.Remove(segmentIndex);
    }

    public void HandleSegmentTransition(int triggerSegmentIndex, bool movingForward)
    {
        printMapSegment();
        int targetSegmentIndex = movingForward ? triggerSegmentIndex + 1 : triggerSegmentIndex - 1;
        
        if (mapSegments.ContainsKey(targetSegmentIndex) && !isTransitioning)
        {
            Debug.Log("HandleSegmentTransition is work:, " + isTransitioning + ", contain:" + mapSegments.ContainsKey(targetSegmentIndex));
            StartCoroutine(TransitionToSegment(targetSegmentIndex, movingForward));
        }
        else
        {
            Debug.Log("HandleSegmentTransition don't work, " + isTransitioning + ", contain:" + mapSegments.ContainsKey(targetSegmentIndex) + ", triggerSegmentIndex: " + triggerSegmentIndex + "targetSegmentIndex: " + targetSegmentIndex);
        }
    }

    IEnumerator TransitionToSegment(int newSegmentIndex, bool movingForward)
    {
        // 載入目標段落
        yield return StartCoroutine(LoadSegmentWithTransition(newSegmentIndex));
        
        // 預載入下一個潛在段落
        int preloadIndex = movingForward ? newSegmentIndex + 1 : newSegmentIndex - 1;
        StartCoroutine(PreloadSegment(preloadIndex));
        
        currentSegmentIndex = newSegmentIndex;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // 檢查玩家位置，決定是否需要預載入
            int playerSegmentIndex = Mathf.FloorToInt(other.transform.position.x / segmentWidth);
            float distanceToEnd = (playerSegmentIndex + 1) * segmentWidth - other.transform.position.x;
            
            if (distanceToEnd < preloadDistance)
            {
                StartCoroutine(PreloadSegment(playerSegmentIndex + 1));
            }
            else if (distanceToEnd > segmentWidth - preloadDistance)
            {
                StartCoroutine(PreloadSegment(playerSegmentIndex - 1));
            }
        }
    }

    public void SetMapData(string[] mapData)
    {
        this.fullMapData = mapData;
        InitializeMap();
    }
}
