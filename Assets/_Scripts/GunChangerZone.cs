using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunChangerZone : MonoBehaviour
{
    [SerializeField]
    private GameObject _seedPrefab;

    [SerializeField]
    private AudioClip _annoucement;

    private void OnTriggerEnter(Collider other)
    {        
        if (other.tag == "Player")
        {
            other.gameObject.GetComponentInParent<PlayerCharacterController>().gunController._seedPrefab = this._seedPrefab;

            AudioChannelSettings channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.5f, "");

            AudioManager.instance.Play(this._annoucement, channelSettings);
        }
    }
}
