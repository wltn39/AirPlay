using TMPro;
using UnityEngine;


public class TH_Enemy : MonoBehaviour
{
    public static TH_Enemy Instance;
    public GameObject explosionFactory;

    private float minX = -10f;
    [SerializeField] private float EnemyHp = 5f;
    private Transform playerTransform;

    void Start()
    {
        // 플레이어를 찾습니다. 플레이어 태그가 "Player"라고 가정합니다.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    public void SetMoveSpeed(float moveSpeed)
    {
        TH_Database_Manager.Instance.EnemyMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (playerTransform != null)
        { 
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * TH_Database_Manager.Instance.EnemyMoveSpeed * Time.deltaTime;
        }

        if (transform.position.x < minX)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            TH_Weapon weapon = other.gameObject.GetComponent<TH_Weapon>();
            EnemyHp -= weapon.damage;

            if (EnemyHp <= 0)
            {
                if (gameObject.tag == "Boss")
                {
                    TH_GameManager.instance.SetGameOver();
                }
                SfxType _sfxType = SfxType.Destroy;
                TH_SoundSystem.Instance.PlaySfx_Func(_sfxType);
                GameObject explosion = Instantiate(explosionFactory);
                explosion.transform.position = transform.position;
                Vector3 Coin_position = new Vector3(transform.position.x, transform.position.y - 0.85f, transform.position.z);

                Destroy(gameObject);
                Instantiate(TH_Database_Manager.Instance.coin, Coin_position, Quaternion.identity);

            }
            Destroy(other.gameObject);
        }
    }
}
