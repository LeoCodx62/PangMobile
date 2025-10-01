
using UnityEngine;

public class WeaponController : Item
{

    [SerializeField]
    private WeaponType weaponType;

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

}
