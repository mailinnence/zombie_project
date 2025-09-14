using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static WeadponModel;

public class UI_Manager : MonoBehaviour
{

    public static UI_Manager instance { get; set;}

    public GameObject[] Gun_image;

    [Header("현재 무기")]  
    public string currentWeapon;

    [Header("보급 관련")]
    public TextMeshProUGUI Bullet;
    public TextMeshProUGUI bomb;

    [Header("hp 관련")]
    public Slider hpbar_L;
    public Slider hpbar_R;
    public TextMeshProUGUI hp_text;
    private float maxHp = 100;
    public float curHp = 100;
    float imsi;


    [Header("physical 관련")]
    public Slider physical_bar;
    private float physical_max = 100;
    public float physical_cur = 0;
    float physical_imsi;


    [Header("ui 효과")]
    public Volume volume;  // Global Volume 오브젝트에 연결
    private Vignette vignette;
    float duration = 0.2f;
    float duration_ = 0.5f;    
    float peakIntensity = 0.4f;
    float timer = 0f;
         
    [Header("게임 오버")]
    public TextMeshProUGUI PlainText;
    private bool once;



    private void Awake()
    {
        instance = this;

        // Volume에서 Vignette 컴포넌트 가져오기
        if(volume != null)
        {
            volume.profile.TryGet<Vignette>(out vignette);
        }
    }



    void Start()
    {
        UpdateBombUI();

        hpbar_L.value = (float) curHp / (float) maxHp;
        hpbar_R.value = (float) curHp / (float) maxHp;
        hp_text.text = $"{curHp}%";
        physical_cur = 0f;
    }




    void Update()
    {
        hp_ui();
        physical_ui();
        gameOver();

 
    }



    public void gameOver()
    {
        if(curHp<=0 && !once)
        {
            once = true;
            PlainText.text = "Game Over";
            StartCoroutine(EndGameAfterDelay(3f)); // ⏱ 3초 후 게임 종료
        }
    }


    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 여기서 게임을 종료하거나 씬을 전환하거나 타이틀로
        // 예: Application.Quit(); 또는 SceneManager.LoadScene("TitleScene");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 play모드 종료
    #else
        Application.Quit(); // 빌드에서는 게임 종료
    #endif
    }

    public void UpdateBulletUI(WeaponModel weapon)
    {
        BulletInfo info = bullet_manager.instance.bulletData[weapon];
        Bullet.text = $"{info.currentBullet} / {info.reserveBullet}";
    }


    public void UpdateBombUI()
    {
        BulletInfo info = bullet_manager.instance.bulletData[WeaponModel.grenade];
        bomb.text = $"{info.currentBullet}";
    }

    public void UpdateHpTEXTUI()
    {
        hp_text.text = $"{curHp}%";
    }



    public void hp_ui()
    {

        imsi = (float) curHp / (float) maxHp;
        HandleHp();
    }

    private void HandleHp()
    {
        hpbar_L.value = Mathf.Lerp(hpbar_L.value , imsi , Time.deltaTime * 10);
        hpbar_R.value = Mathf.Lerp(hpbar_R.value , imsi , Time.deltaTime * 10);
    }



    public void physical_ui()
    {
        if (physical_cur >= 100)
        {
            physical_cur =0;
        }

        physical_imsi = physical_cur / physical_max;
        physical_Handle();
    }

    private void physical_Handle()
    {
        physical_bar.value = Mathf.Lerp(physical_bar.value, physical_imsi, Time.deltaTime * 10);
    }






    public void PlayDamageEffect()
    {
        if(vignette != null)
        {
            StopAllCoroutines();
            StartCoroutine(DamageVignetteEffect());
        }
    }

    private IEnumerator DamageVignetteEffect()
    {

        // intensity 0 -> peak로 증가
        timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(0f, peakIntensity, timer / duration);
            vignette.smoothness.value = Mathf.Lerp(0f, peakIntensity, timer / duration);
            yield return null;
        }

        // intensity peak -> 0 으로 감소
        timer = 0f;

        while(timer < duration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(peakIntensity, 0f, timer / duration_);
            vignette.smoothness.value = Mathf.Lerp(0f, peakIntensity, timer / duration);
            yield return null;
        }

        vignette.intensity.value = 0f;
    }

}
 