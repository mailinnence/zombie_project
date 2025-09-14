using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_delay : MonoBehaviour
{
    private MeshRenderer _renderer;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();

        // 총알을 처음엔 보이지 않게
        if (_renderer != null)
            _renderer.enabled = false;

        // 0.5초 뒤에 보이게 하기
        StartCoroutine(EnableVisualAfterDelay(0.01f));
    }

    private IEnumerator EnableVisualAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_renderer != null)
            _renderer.enabled = true;
    }

}
