using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunChangerZone : MonoBehaviour
{
    [SerializeField]
    private GameObject _seedPrefab;

    [SerializeField]
    private AudioClip _annoucement;

    [SerializeField]
    private GameObject _pickupParticleObject;

    [SerializeField]
    private AmmoType _ammoType;

    private void OnTriggerEnter(Collider other)
    {        
        if (other.tag == "Player")
        {
            GunController gunController = other.gameObject.GetComponentInParent<PlayerCharacterController>().gunController;

            if (gunController._seedPrefab != this._seedPrefab)
            {
                gunController._seedPrefab = this._seedPrefab;

                AudioChannelSettings channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 0.1f, "");
                AudioManager.instance.Play(this._annoucement, channelSettings);

                Instantiate(this._pickupParticleObject, this.gameObject.transform.position, new Quaternion());

                AmmoImage.instance.UpdateImage(this._ammoType);
            }
        }
    }
}
