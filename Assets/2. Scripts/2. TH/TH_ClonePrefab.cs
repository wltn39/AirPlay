using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TH_ClonePrefab : MonoBehaviour
{
    public GameObject weapon;
    public Transform shootTransform;
    private float lastShotTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (TH_GameManager.instance.isGameOver == false && gameObject.activeSelf)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        if (Time.time - lastShotTime > TH_Database_Manager.Instance.cloneShootInterval && !Input.GetKey(KeyCode.Space))
        {
            Instantiate(weapon, shootTransform.position, Quaternion.identity);
            lastShotTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            SfxType _sfxType = SfxType.Destroy;
            TH_SoundSystem.Instance.PlaySfx_Func(_sfxType);
            gameObject.SetActive(false);
        }

    }
}
