using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TH_Player : MonoBehaviour
{
    public static TH_Player Instance = null;

    public GameObject explosionFactory;
    public Transform shootTransform;

    public GameObject[] weapons;
    private int weaponIndex = 0;

    private float lastShotTime = 0f;
    private Vector3 minBounds = new Vector3(-8.5f, -8f, -1f);  // 최소 좌표 경계
    private Vector3 maxBounds = new Vector3(8.5f, 0.3f, 0f);  // 최대 좌표 경계

    // clone 
    private GameObject leftClone;
    private GameObject rightClone;
    private Vector3 leftCloneOffset = new Vector3(-1f, 5f, 0);
    private Vector3 rightCloneOffset = new Vector3(-1f, -3f, 0);

    Vector3 moveVector = Vector3.zero;

    private BluetoothService _service;

    public bool IsShoot = false;

    private void Awake()
    {
        Instance = this;
    }

    //void Start()
    public void Init()
    {
        // TODO: Start함수는 한번만 호출되기 때문에 아래 코드는 수정이 필요. TH_Database_Manager의 Start가 끝난 다음에 호출되게 수정 필요. - kail 2024.09.24
        // TH_Database_Manager.Instance가 null이면 초기화될 때까지 대기
        if (TH_Database_Manager.Instance == null)
        {
            Debug.LogError("TH_Database_Manager.Instance가 아직 초기화되지 않았습니다.");
            return;
        }

        rightClone = Instantiate(TH_Database_Manager.Instance.clonePrefab, transform.position + rightCloneOffset, Quaternion.identity);
        rightClone.SetActive(false);

        leftClone = Instantiate(TH_Database_Manager.Instance.clonePrefab, transform.position + leftCloneOffset, Quaternion.identity);
        leftClone.SetActive(false);
    }

    void Update()
    {
        if (TH_GameManager.instance.isGameOver == false)
        {
            Shoot();
            SyncClones();
        }
    }

    void Shoot()
    {
        //if (Time.time - lastShotTime > TH_Database_Manager.Instance.shootInterval && !Input.GetKey(KeyCode.Space))
        if (Time.time - lastShotTime > TH_Database_Manager.Instance.shootInterval && IsShoot == true)
        {
            Instantiate(weapons[weaponIndex], shootTransform.position, Quaternion.identity);
            lastShotTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            TH_GameManager.instance.SetGameOver();
            GameObject explosion = Instantiate(explosionFactory);
            explosion.transform.position = transform.position;
            SfxType _sfxType = SfxType.Destroy;
            TH_SoundSystem.Instance.PlaySfx_Func(_sfxType);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            TH_GameManager.instance.IncreaseCoin();
            Destroy(other.gameObject);
        }
    }

    public void Upgrade()
    {
        weaponIndex += 1;
        if (weaponIndex >= weapons.Length)
        {
            weaponIndex = weapons.Length - 1;
        }
        setClone();
    }

    void SyncClones()
    {
        leftClone.transform.position = transform.position + new Vector3(-1f, 3.5f, 0);
        rightClone.transform.position = transform.position + new Vector3(-1f, -1f, 0);
    }

    void setClone()
    {
        if (leftClone.activeSelf && rightClone.activeSelf) { return; }
        if (!leftClone.activeSelf && !rightClone.activeSelf)
        {
            leftClone.SetActive(true);
            return;
        }
        if (leftClone.activeSelf && !rightClone.activeSelf)
        {
            rightClone.SetActive(true);
            return;
        }
        if (rightClone.activeSelf && !leftClone.activeSelf)
        {
            leftClone.SetActive(true);
            return;
        }
    }

}
