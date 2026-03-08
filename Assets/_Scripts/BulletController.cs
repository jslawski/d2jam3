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

        if (collision.gameObject.tag == "Plant" || collision.gameObject.tag == "DirtPlant")
        {
            //collision.gameObject.GetComponent<PlantController>().DestroyPlant();
            collision.gameObject.transform.parent.gameObject.GetComponent<PlantController>().DestroyPlant();                   
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Plantable" && collision.gameObject.layer == LayerMask.NameToLayer("Plant"))
        {
            //collision.gameObject.GetComponent<PlantController>().DestroyPlant();
            collision.gameObject.transform.parent.gameObject.GetComponent<PlantController>().DestroyPlant();
            Destroy(this.gameObject);
        }
    }
}
