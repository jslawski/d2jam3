using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.SaveCheckpoint(new Vector3(this.transform.position.x, (this.transform.position.y + 3.0f), this.transform.position.z));
            Destroy(this.gameObject);
        }
    }
}
