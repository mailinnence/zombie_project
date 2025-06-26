using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set;}

    [Header("탄창이 비었을때")]
    public AudioSource EmptyManagizeSound;
    
    [Header("shooting")]
    public AudioSource ShootingChannel;
    public AudioClip pm_40;
    public AudioClip ak47;
    
    
    [Header("reloading")]
    public AudioSource ReloadingSound_ak47;
    public AudioSource ReloadingSound_pm_40;





    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.pm_40:
                ShootingChannel.PlayOneShot(pm_40);
                break;
            case WeaponModel.ak47:
                ShootingChannel.PlayOneShot(ak47);
                break;
        }
    }


    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.pm_40:
                ReloadingSound_pm_40.Play();
                break;
            case WeaponModel.ak47:
                ReloadingSound_ak47.Play();
                break;
        }
    }

}
