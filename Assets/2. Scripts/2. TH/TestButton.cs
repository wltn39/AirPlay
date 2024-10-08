using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    public GameObject[] HideUI;

    public GameObject[] MeshRender;

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
        }
    }
}
