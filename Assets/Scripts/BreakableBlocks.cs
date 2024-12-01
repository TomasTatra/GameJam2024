using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableBlocks : MonoBehaviour
{
    private Tilemap _tileMap;

    // Start is called before the first frame update
    void Start()
    {
        _tileMap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile")) // shoulld be projectile later
        {
            Vector3 impactPoint = Vector3.zero;
            List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
            collision.GetContacts(contactPoints);
            foreach (ContactPoint2D impact in contactPoints)
            {
                impactPoint.x = impact.point.x - 0.04f * impact.normal.x;
                impactPoint.y = impact.point.y - 0.04f * impact.normal.y;
                Vector3Int cellPosition = _tileMap.WorldToCell(impactPoint);
                _tileMap.SetTile(cellPosition, null);
            }
            Destroy(collision.gameObject);
        }


        //if (collision.gameObject.CompareTag("Player")) // shoulld be projectile later
        //{
        // 
        //}
        //_tileMap.SetTile(cellPosition, null);
    }
}
