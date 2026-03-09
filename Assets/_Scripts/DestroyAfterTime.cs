using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeBeforeDestroy;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", this.timeBeforeDestroy);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

}
