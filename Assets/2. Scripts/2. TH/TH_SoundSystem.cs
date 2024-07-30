using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TH_SoundSystem : MonoBehaviour
{
    public static TH_SoundSystem Instance;

    [SerializeField] private AudioSource sfxAS = null;
    public void Init_Func()
    {
        Instance = this;
    }
    public void PlaySfx_Func(SfxType _sfxType)
    {
        TH_Database_Manager.SfxData _sfxData = TH_Database_Manager.Instance.GetSfxData_Func(_sfxType);
        this.sfxAS.volume = _sfxData.volume;
        this.sfxAS.PlayOneShot(_sfxData.clip);
    }

}
public enum SfxType
{
    None = 0,
    Coin = 10,
    Destroy = 20,
    BossSpawn = 30,
    PlayerDie = 40,
    PlayerLevelUp = 50,
    Countdown = 60,
}
