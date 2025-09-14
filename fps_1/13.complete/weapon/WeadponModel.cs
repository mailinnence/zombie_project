using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeadponModel : MonoBehaviour
{

    public static WeadponModel instance { get; private set; }

    void Awake()
    {
        instance = this;
    }


    public enum WeaponModel
    {
        pistol,
        akm,
        ak47,
        shortgun,
        reaper,
        mac,
        grenade
    }
}
