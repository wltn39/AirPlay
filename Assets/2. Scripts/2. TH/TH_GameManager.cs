using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TH_GameManager : MonoBehaviour
{
    [SerializeField] private TH_Database_Manager database_Manager = null;
    public static TH_GameManager instance = null;

    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI overText;
    [SerializeField] private TextMeshProUGUI countdownText; // 카운트다운 텍스트 추가
    private int coin = 0;
    [SerializeField] private TH_SoundSystem soundSystem;

    [HideInInspector]
    public bool isGameOver = false;
    public bool IsGameStart = false;    // 시작 용도로 추가함 - kail 2024.10.25

    //  private void Awake()
    // {
    // StartCoroutine(StartGameWithDelay(3f)); // 3초 지연 후 게임 시작                
    // }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        this.database_Manager.Init_Func();
        this.soundSystem.Init_Func();
        TH_Player.Instance.Init();  // 싱글턴 이용해서 초기화 - kail 2024.09.24

        // Awake로 이동 처리 - kail 2024.09.24
        //if (instance == null)
        //{
        //    instance = this;
        //}
    }

    public void IncreaseCoin()
    {
        SfxType _sfxType = SfxType.Coin;
        TH_SoundSystem.Instance.PlaySfx_Func(_sfxType);
        coin += 1;
        scoreText.SetText(coin.ToString());
        overText.SetText(coin.ToString());

        if (coin % TH_Database_Manager.Instance.upgradeInterval == 0)
        {
            TH_Player player = FindObjectOfType<TH_Player>();
            if (player != null)
            {
                player.Upgrade();
                if (coin <= TH_Database_Manager.Instance.upgradeInterval * 2)
                {
                    SfxType _sfxType2 = SfxType.PlayerLevelUp;
                    TH_SoundSystem.Instance.PlaySfx_Func(_sfxType2);
                }
            }
        }
    }

    public void SetGameOver()
    {
        isGameOver = true;
        TH_EnemySpawner enemySpawner = FindObjectOfType<TH_EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.StopEnemyRoutine();
        }
        Invoke("ShowGameOverPanel", 1f);
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain_Easy()
    {
        // ResetGameEnvironment();
        SceneManager.LoadScene("TH_Game_Easy");
    }

    public void PlayAgain_Normal()
    {
        // ResetGameEnvironment();
        SceneManager.LoadScene("TH_Game_Normal");
    }

    public void PlayAgain_Hard()
    {
        SceneManager.LoadScene("TH_Game_Hard");
    }

    public void MoveHome()
    {
        // ResetGameEnvironment();
        SceneManager.LoadScene("Ble");
    }
}
