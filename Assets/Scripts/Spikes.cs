using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Spikes : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
     
        //foreach (Vector3Int position in tileMap.cellBounds.allPositionsWithin)
        //{
        //    if (tileMap.HasTile(position))
        //    {
        //        // Create a collider with trigger for a spike
        //        BoxCollider2D boxCollider = new GameObject().AddComponent<BoxCollider2D>();
        //        boxCollider.gameObject.tag = "Danger";
        //        boxCollider.isTrigger = true;
        //        boxCollider.transform.parent = tileMap.transform;
        //        boxCollider.transform.localPosition = tileMap.CellToLocal(position) +
        //            tileMap.layoutGrid.cellSize / 2;
        //    }
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Do the respawning here
        }
    }


}
