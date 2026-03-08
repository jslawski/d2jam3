using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : ProjectileController
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plantable")
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Plant")
        {
            collision.gameObject.transform.parent.gameObject.GetComponent<PlantController>().DestroyPlant();
        
            //Destroy(collision.gameObject.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
    }
}
