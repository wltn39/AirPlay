using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TH_EnemySpawner : MonoBehaviour
{
    void Start()
    {
        StartEnemyRoutine();
    }

    void StartEnemyRoutine()
    {
        StartCoroutine(EnemyRoutine());
    }

    public void StopEnemyRoutine()
    {
        StopCoroutine("EnemyRoutine");
    }

    IEnumerator EnemyRoutine()
    {
        yield return new WaitForSeconds(1.5f);  // 초기 대기 시간

        float moveSpeed = 5f;
        int spawnCount = 0;
        int enemyIndex = 0;

        while (true)
        {
            // 각 적의 스폰마다 랜덤 대기 시간을 적용
            foreach (float posy in TH_Database_Manager.Instance.arrPosY)
            {
                int index = Random.Range(0, TH_Database_Manager.Instance.enemies.Length);
                SpawnEnemy(posy, index, moveSpeed);

                // 매번 랜덤 대기 시간을 생성
                float delayTime = Random.Range(0f, 2f);  // 0초에서 2초 사이 랜덤 대기
                yield return new WaitForSeconds(delayTime);
            }
            spawnCount += 1;
            if (spawnCount % 3 == 0)
            {
                enemyIndex += 1;
                moveSpeed += 1.5f;
            }

            if (enemyIndex >= TH_Database_Manager.Instance.BossSpawn)
            {
                SpawnBoss();
                enemyIndex = 0;
                moveSpeed = 3f;
            }

            // 추가 대기시간을 여기서 설정하면 매 루프의 끝에서 추가 대기가 발생합니다.
            float endLoopDelay = Random.Range(0f, 1f);
            yield return new WaitForSeconds(endLoopDelay);
        }
    }
    void SpawnEnemy(float posy, int index, float moveSpeed)
    {
        Vector3 spawnPos = new Vector3(transform.position.x, posy, transform.position.z);
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        GameObject enemyObject = Instantiate(TH_Database_Manager.Instance.enemies[index], spawnPos, rotation);
        TH_Enemy enemy = enemyObject.GetComponent<TH_Enemy>();
        enemy.SetMoveSpeed(moveSpeed);
    }

    void SpawnBoss()
    {
        SfxType _sfxType = SfxType.BossSpawn;
        TH_SoundSystem.Instance.PlaySfx_Func(_sfxType);
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        Vector3 bossSpawnPos = new Vector3(transform.position.x + 10f, 0f, transform.position.z);
        Instantiate(TH_Database_Manager.Instance.Boss, bossSpawnPos, rotation);
    }
    public void ResetSpawnerState()
    {
        // 적 스폰 루틴 중지
        StopEnemyRoutine();

        // 모든 적을 찾아서 제거
        foreach (var enemy in FindObjectsOfType<TH_Enemy>())
        {
            Destroy(enemy.gameObject);
        }

        // 초기 상태로 리셋
        StartEnemyRoutine();
    }
}