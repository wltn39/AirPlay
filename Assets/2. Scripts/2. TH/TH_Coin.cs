using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TH_Coin : MonoBehaviour
{
    public Transform coinTransform;

    private float destroyTime = 5f; // 오브젝트가 사라질 시간 (초)


    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        Invoke("DestroyObject", destroyTime);
        Jump();
    }

    void Jump()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();

        float randomJumpForce = Random.Range(4f, 8f);
        Vector3 jumpVelocity = Vector3.left * randomJumpForce;
        jumpVelocity.y = Random.Range(-1f, 1f);
        rigidBody.AddForce(jumpVelocity, ForceMode.Impulse);
    }

    // 오브젝트를 파괴하는 메서드
    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
