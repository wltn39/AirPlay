using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Start()
    {
        SetResolution();
    }

    void SetResolution()
    {
        // 현재 화면의 너비와 높이 가져오기
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // 원하는 화면 비율 설정 (예: 16:9)
        float targetAspect = 16.0f / 9.0f;

        // 현재 화면 비율 계산
        float windowAspect = (float)screenWidth / (float)screenHeight;

        // 화면 비율에 따른 스케일 조정
        float scaleHeight = windowAspect / targetAspect;

        // 카메라 가져오기
        Camera camera = Camera.main;

        // 해상도에 따라 카메라의 Viewport Rect 조정
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else // scaleWidth가 1.0보다 작을 때 (16:9 화면 비율보다 좁을 때)
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
