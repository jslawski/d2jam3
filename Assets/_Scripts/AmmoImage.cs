using UnityEngine;
using UnityEngine.UI;

public enum AmmoType { Basic, Dirt, Vine }

public class AmmoImage : MonoBehaviour
{
    public static AmmoImage instance;

    [SerializeField]
    private Sprite _basicSprite;
    [SerializeField]
    private Sprite _dirtSprite;
    [SerializeField]
    private Sprite _vineSprite;

    [SerializeField]
    private Image _ammoImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdateImage(AmmoType newAmmoType)
    {
        switch (newAmmoType)         
        { 
            case AmmoType.Basic:
                this._ammoImage.sprite = this._basicSprite;
                break;
            case AmmoType.Dirt:
                this._ammoImage.sprite = this._dirtSprite;
                break;
            case AmmoType.Vine:
                this._ammoImage.sprite = this._vineSprite;
                break;
            default:
                this._ammoImage.sprite = this._basicSprite;
                break;
        }
    }
}
