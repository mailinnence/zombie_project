using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeadponModel;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set;}


    [Header("탄창이 비었을때")]
    public AudioSource EmptyManagizeSound_source;
    public AudioClip EmptyManagizeSound;
    private bool delay_empty;


    [Header("shooting")]
    public AudioSource ShootingChannel; 
    public AudioClip pistol; // 연속적으로 소리가 날 경우를 끊기지 않게 하기 위한 코드
    public AudioClip akm;
    public AudioClip ak47;
    public AudioClip shortgun;
    public AudioClip reaper;
    public AudioClip mac;
    
    
    [Header("reloading")]
    public AudioSource ReloadingSound_pistol;
    public AudioSource ReloadingSound_akm;
    public AudioSource ReloadingSound_ak47;
    public AudioSource ReloadingSound_shortgun;
    public AudioSource ReloadingSound_reaper;
    public AudioSource ReloadingSound_mac;


    [Header("movement")]
    public AudioSource dash; 
    public AudioClip dash_;
    public AudioSource jump;
    public AudioClip jump_;
    public AudioSource foot;
    public AudioClip foot_;
    



    [Header("bomb")]
    public AudioSource throwablesChannel; 
    public AudioClip grenadeSound;

    [Header("explode")]
    public AudioSource explode_gun; 


    [Header("zombie")]
    public AudioSource zombie_idle_sound; 
    public AudioClip idle_1;
    public AudioSource zombie_attack_sound; 
    public AudioClip attack;  
    public AudioSource zombie_chasing_sound; 
    public AudioClip chasing;
    public AudioSource zombie_death_sound;
    public AudioClip death; 
    



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
            case WeaponModel.pistol:
                ShootingChannel.PlayOneShot(pistol);
                break;
            case WeaponModel.akm:
                ShootingChannel.PlayOneShot(akm);
                break;
            case WeaponModel.ak47:
                ShootingChannel.PlayOneShot(ak47);
                break;
            case WeaponModel.shortgun:
                ShootingChannel.PlayOneShot(shortgun);
                break;
            case WeaponModel.reaper:
                ShootingChannel.PlayOneShot(reaper);
                break;
            case WeaponModel.mac:
                ShootingChannel.PlayOneShot(mac);
                break;
        }
    }


    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.pistol:
                ReloadingSound_pistol.Play();
                break;
            case WeaponModel.akm:
                ReloadingSound_akm.Play();
                break;
            case WeaponModel.ak47:
                ReloadingSound_ak47.Play();
                break;
            case WeaponModel.shortgun:
                ReloadingSound_shortgun.Play();
                break;
            case WeaponModel.reaper:
                ReloadingSound_reaper.Play();
                break;
            case WeaponModel.mac:
                ReloadingSound_mac.Play();
                break;

        }
    }


    public void empty()
    {
        if(delay_empty) { return; }
        delay_empty = true;        
        StartCoroutine(ResetEmptyDelay());
    }

    private IEnumerator ResetEmptyDelay()
    {
        yield return new WaitForSeconds(0.7f);
        EmptyManagizeSound_source.PlayOneShot(EmptyManagizeSound);
        delay_empty = false;
    }





    public void PlayZombieSound(string type)
    {
        // 먼저 전부 정지
        zombie_idle_sound.Stop();
        zombie_chasing_sound.Stop();
        zombie_death_sound.Stop();
        zombie_attack_sound.Stop();

        // 타입에 따라 하나만 재생
        switch (type)
        {
            case "idle":
                zombie_idle_sound.PlayOneShot(idle_1);
                break;
            case "chasing":
                zombie_chasing_sound.PlayOneShot(chasing);
                break;
            case "death":
                zombie_death_sound.PlayOneShot(death);
                break;
            case "attack":
                zombie_death_sound.PlayOneShot(attack);
                break;
                                
        }
    }  
}
