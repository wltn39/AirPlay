using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class TH_DistanceBetweenImages : MonoBehaviour
{
    //public ARTrackedImageManager trackedImageManager;
    //public GameObject[] prefabsToPlace;  // 이미지별로 배치할 프리팹 배열

    //private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    //void OnEnable()
    //{
    //    trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    //}

    //void OnDisable()
    //{
    //    trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    //}

    //private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    //{
    //    // 새로 감지된 이미지 처리
    //    foreach (var trackedImage in eventArgs.added)
    //    {
    //        UpdateImage(trackedImage);
    //    }

    //    // 업데이트된 이미지 처리
    //    foreach (var trackedImage in eventArgs.updated)
    //    {
    //        UpdateImage(trackedImage);
    //    }

    //    // 제거된 이미지 처리
    //    foreach (var trackedImage in eventArgs.removed)
    //    {
    //        if (spawnedPrefabs.ContainsKey(trackedImage.referenceImage.name))
    //        {
    //            Destroy(spawnedPrefabs[trackedImage.referenceImage.name]);
    //            spawnedPrefabs.Remove(trackedImage.referenceImage.name);
    //        }
    //    }
    //}

    //private void UpdateImage(ARTrackedImage trackedImage)
    //{
    //    string imageName = trackedImage.referenceImage.name;

    //    // 이미지에 해당하는 프리팹이 이미 배치되어 있지 않으면 새로 생성
    //    if (!spawnedPrefabs.ContainsKey(imageName))
    //    {
    //        foreach (var prefab in prefabsToPlace)
    //        {
    //            if (prefab.name == imageName)
    //            {
    //                var newPrefab = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);
    //                newPrefab.transform.parent = trackedImage.transform; // 이미지와 함께 이동하도록 설정
    //                spawnedPrefabs[imageName] = newPrefab;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        // 프리팹이 이미 생성된 경우 위치 업데이트
    //        var spawnedPrefab = spawnedPrefabs[imageName];
    //        spawnedPrefab.transform.position = trackedImage.transform.position;
    //        spawnedPrefab.transform.rotation = trackedImage.transform.rotation;
    //    }
    //}

    public ARTrackedImageManager trackedImageManager;
    public string firstImageName = "qr1";  // 첫 번째 인식할 이미지의 이름
    public string secondImageName = "qr2"; // 두 번째 인식할 이미지의 이름

    private ARTrackedImage firstTrackedImage = null;
    private ARTrackedImage secondTrackedImage = null;

    public GameObject QR1;
    public GameObject QR2;

    public Text GetDistance;
    public GameObject PopupWaiting;

    bool isFirstRecongize = false;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        //Debug.Log(">>> OnTrackedImagesChanged");

        // 새로 감지된 이미지 처리
        foreach (var trackedImage in eventArgs.added)
        {
            CheckImage(trackedImage);
        }

        // 업데이트된 이미지 처리
        foreach (var trackedImage in eventArgs.updated)
        {
            CheckImage(trackedImage);
        }

        // 제거된 이미지 처리
        foreach (var trackedImage in eventArgs.removed)
        {
            Debug.Log("remove - name: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == firstImageName)
            {
                firstTrackedImage = null;
            }
            else if (trackedImage.referenceImage.name == secondImageName)
            {
                secondTrackedImage = null;
            }
        }

        // 두 이미지 모두 인식되면 거리를 계산
        if (firstTrackedImage != null && secondTrackedImage != null)
        {
            //float distance = Vector3.Distance(firstTrackedImage.transform.position, secondTrackedImage.transform.position);
            float distance = Vector3.Distance(QR1.transform.position, QR2.transform.position);
            //Debug.Log("Distance between images: " + distance + " meters.");

            //Debug.Log("firstTrackedImage position: " + firstTrackedImage.transform.position);
            //Debug.Log("secondTrackedImage position: " + secondTrackedImage.transform.position);

            // 25cm 이상 차이일때 발사 처리 하기 (3.28cm QR)- kail 2024.09.26
            // 15cm 이상 차이로 변경 (5cm QR) - kail 2024.10.24
            float cm = distance * 100.0f;

            if (cm >= 15.0f)
            {
                // 발사하기.
                TH_Player.Instance.IsShoot = true;

                if (isFirstRecongize == false)
                {
                    isFirstRecongize = true;
                    PopupWaiting.SetActive(false);
                    TH_EnemySpawner.Instance.StartEnemyRoutine();
                    TH_GameManager.instance.IsGameStart = true;
                }
            }
            else
            {
                TH_Player.Instance.IsShoot = false;
            }

            GetDistance.text = cm.ToString("F2") + " cm";
        }
        else
        {
            Debug.Log("image is null..");

            // 인식 안될때는 안쏘게 처리 - kail 2024.10.02
            TH_Player.Instance.IsShoot = false;

            GetDistance.text = "- cm";
        }        
    }

    private void CheckImage(ARTrackedImage trackedImage)
    {
        //Debug.Log(">>> CheckImage: " + trackedImage.referenceImage.name);

        // Tracking만 했더니 인식이 안된다고 해서 Limited까지 추가함 - kail 2024.10.07
        // Limited까지 얺으면 기존과 동일하게 인식 안되도 계속 거리를 보여주고 있어서 못쓸듯함. - kail 2024.10.07
        if (trackedImage.trackingState == TrackingState.Tracking)   // || trackedImage.trackingState == TrackingState.Limited)
        {
            // 첫 번째 이미지인지 확인
            if (trackedImage.referenceImage.name == firstImageName)
            {
                firstTrackedImage = trackedImage;
                //Debug.Log(">>> CheckImage set first");
                QR1.transform.position = trackedImage.transform.position;
            }

            // 두 번째 이미지인지 확인
            else if (trackedImage.referenceImage.name == secondImageName)
            {
                secondTrackedImage = trackedImage;
                //Debug.Log(">>> CheckImage set second");
                QR2.transform.position = trackedImage.transform.position;
            }
        }
        else
        {
            // 제거 처리를 여기에서 해야 할듯.
            if (trackedImage.referenceImage.name == firstImageName)
            {
                firstTrackedImage = null;
            }

            // 두 번째 이미지인지 확인
            else if (trackedImage.referenceImage.name == secondImageName)
            {
                secondTrackedImage = null;
            }
        }
        
    }
}
