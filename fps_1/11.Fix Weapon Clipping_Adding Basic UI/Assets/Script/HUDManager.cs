using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    // UI
    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;

    // 캐싱된 스프라이트
    private Dictionary<Weapon.WeaponModel, Sprite> weaponSprites;
    private Dictionary<Weapon.WeaponModel, Sprite> ammoSprites;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Sprite 캐싱
        weaponSprites = new Dictionary<Weapon.WeaponModel, Sprite>()
        {
            { Weapon.WeaponModel.pm_40, LoadSpriteFromPrefab("pm_40_Weapon") },
            { Weapon.WeaponModel.ak47, LoadSpriteFromPrefab("ak47_Weapon") }
        };

        ammoSprites = new Dictionary<Weapon.WeaponModel, Sprite>()
        {
            { Weapon.WeaponModel.pm_40, LoadSpriteFromPrefab("Pistol_Ammo") },
            { Weapon.WeaponModel.ak47, LoadSpriteFromPrefab("Rifle_Ammo") }
        };
    }

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot()?.GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            // magazineAmmoUI.text = $"{activeWeapon.bulletsLeft} / {activeWeapon.bulletsPerBurst}";
            // totalAmmoUI.text = $"{activeWeapon.magazineSize}*{activeWeapon.bulletsPerBurst}";

            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
            totalAmmoUI.text = $"/{activeWeapon.magazineSize}";


            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;

            ammoTypeUI.sprite = GetAmmoSprite(model);
            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
            else
            {
                unActiveWeaponUI.sprite = emptySlot;
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";
            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        return weaponSprites.ContainsKey(model) ? weaponSprites[model] : null;
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        return ammoSprites.ContainsKey(model) ? ammoSprites[model] : null;
    }

    private Sprite LoadSpriteFromPrefab(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab != null)
        {
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
                return sr.sprite;
        }
        Debug.LogWarning($"[HUDManager] Could not load sprite from prefab at: {path}");
        return null;
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null;
    }
}
