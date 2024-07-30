using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TH_Database_Manager : ScriptableObject
{
    public static TH_Database_Manager Instance;

    [Header("플레이어")]
    public float playerMoveSpeed = 1f; // 
    public float shootInterval; // 미사일 발사속도
    public GameObject clonePrefab;
    public float cloneShootInterval; // 발사간격
    public float upgradeInterval; // 업그레이드 필요 코인

    [Header("적")]
    public GameObject[] enemies;
    public float EnemyMoveSpeed = 10f;
    public float[] arrPosY = { -2.4f, -1.5f, -0.6f, 0.3f, 1.2f, 2.1f }; // 적 출현 좌표
    public GameObject Boss;
    public float SpawnInterval = 3f; // 적 출현 속도
    public float BossSpawn = 5f;

    [Header("아이템&폭탄")]
    public GameObject coin;

    [Header("사운드")]
    [SerializeField] private SfxData[] sfxDataArr = null;
    private Dictionary<SfxType, SfxData> sfxDataDic;

    public void Init_Func()
    {
        Instance = this;
        this.sfxDataDic = new Dictionary<SfxType, SfxData>();
        foreach (SfxData _sfxData in this.sfxDataArr)
        {
            this.sfxDataDic.Add(_sfxData.sfxType, _sfxData);
        }
    }

    public SfxData GetSfxData_Func(SfxType _sfxType)
    {
        return this.sfxDataDic[_sfxType];
    }
    [System.Serializable]
    public class SfxData
    {
        public SfxType sfxType;
        public AudioClip clip;
        public float volume = 1f;
    }

}
