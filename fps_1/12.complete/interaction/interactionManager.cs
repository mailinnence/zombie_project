using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static WeadponModel; 


public class interactionManager : MonoBehaviour
{
    public static interactionManager Instance { get; set;}


    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;

    public TextMeshProUGUI plainText;
    public bool plainText_time;
    public float interactRange = 0.1f;

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


    private void Update()
    {
        
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);
    

        if (Physics.Raycast(ray, out hit, interactRange)) // ray 로 광선을 쏴서 hit의 객체를 objectHitByRaycast 에 넣는다.
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;
        
            // ammo
            if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                hoveredAmmoBox = objectHitByRaycast.gameObject.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;
                
                if (Input.GetKeyDown(KeyCode.E))
                {   
                    if(hoveredAmmoBox.GetComponent<AmmoBox>().ammoType == WeaponModel.grenade)
                    {
                        Weapon.Instance.bomo_Supply();
                        hoveredAmmoBox.gameObject.SetActive(false);
                    }
                    else
                    {
                        if(bullet_manager.instance.bulletData[hoveredAmmoBox.GetComponent<AmmoBox>().ammoType].reserveBullet != bullet_manager.instance.bulletData[hoveredAmmoBox.GetComponent<AmmoBox>().ammoType].maxBullet)
                        {
                            Weapon.Instance.Ammo_Supply(hoveredAmmoBox.GetComponent<AmmoBox>().ammoType);
                            hoveredAmmoBox.gameObject.SetActive(false);
                        }
                        else
                        {
                            if(!plainText_time)
                            {
                                plainText_time = true;
                                plainText.text = "탄약이 이미 최대입니다.";
                                StartCoroutine(HidePlainText(3f));
                            }

                        }
                    }


                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
            }

        }
    }



    private IEnumerator HidePlainText(float delay)
    {
        yield return new WaitForSeconds(delay);
        plainText_time = false;
        plainText.text = "";
    }


}

