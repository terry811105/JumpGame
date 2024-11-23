using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTrigger : MonoBehaviour
{
    private MapCreateManager generator;
    private int segmentIndex;
    private bool triggersForward;

    public void Initialize(MapCreateManager generator, int segmentIndex, bool triggersForward)
    {
        this.generator = generator;
        this.segmentIndex = segmentIndex;
        this.triggersForward = triggersForward;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
             Debug.Log("player enter, index: " + segmentIndex);
            generator.HandleSegmentTransition(segmentIndex, triggersForward);
        } 
        else 
        {
            Debug.Log("no player enter");
        }
    }
}
