using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private PlayerControls _playerControls;

    // Start is called before the first frame update
    void Start()
    {
        this._playerControls = new PlayerControls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
