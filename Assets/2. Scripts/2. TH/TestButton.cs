using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    public GameObject[] HideUI;

    public GameObject[] MeshRender;

    public Camera CameraSprite;
    public Camera CameraAR;

    bool isTest = false;

    public void ClickTest()
    {
        isTest = !isTest;

        if (isTest == true)
        {
            // Hide
            for (int i = 0; i < HideUI.Length; i++)
            {
                HideUI[i].SetActive(false);
            }

            // 박스는 보이게 처리.
            for (int i = 0; i < MeshRender.Length; i++)
            {
                MeshRender[i].GetComponent<MeshRenderer>().enabled = true;
            }

            // 카메라 설정. (테스트)
            CameraSprite.clearFlags = CameraClearFlags.Nothing;

            CameraAR.rect = new Rect(0, 0, 1, 1);
        }
        else
        {
            // Show
            for (int i = 0; i < HideUI.Length; i++)
            {
                HideUI[i].SetActive(true);
            }

            for (int i = 0; i < MeshRender.Length; i++)
            {
                MeshRender[i].GetComponent<MeshRenderer>().enabled = false;
            }

            // 카메라 설정. (게임)
            CameraSprite.clearFlags = CameraClearFlags.SolidColor;
        }
    }
}
