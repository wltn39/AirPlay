using TMPro;
using UnityEngine;

public class TH_GameTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI elapsedTimeText;
    private float elapsedTime;

    void Start()
    {
        elapsedTime = 0f;
    }

    void Update()
    {
        // 게임 종료시 타이머 멈추게 처리 - kail 2024.09.24
        if (TH_GameManager.instance.isGameOver == true)
        {
            return;
        }

        if (TH_GameManager.instance.IsGameStart == false)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        DisplayElapsedTime(elapsedTime);
    }

    void DisplayElapsedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);
        elapsedTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
