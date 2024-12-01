using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnTriggered : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody && collision.attachedRigidbody.GetComponent<PlayerController>())
        {
            collision.attachedRigidbody.GetComponent<PlayerController>().SetLastCheckpoint(this.transform.position);
        }
    }
}
