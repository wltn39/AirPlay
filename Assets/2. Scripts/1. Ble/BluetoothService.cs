using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// AutoConnect 관련 처리도 해야 함
public enum BluetoothState // 수정된 부분: public으로 변경
{
    Initialize,
    Scanning,
    ScanComplete,
    Connecting,
    Connected,
    Subscribing,
    FailConnect,
    Disconnected,
    Reset,
    Error,
    End,
}

public class BluetoothService : MonoBehaviour
{
    static string[] ServiceUUIDs = new string[] { "0000fff0-0000-1000-8000-00805f9b34fb" };
    static string ServiceUUID = "0000fff0-0000-1000-8000-00805f9b34fb";
    static string wheelyxSpeedCharacteristicUUID = "0000fff1-0000-1000-8000-00805f9b34fb";
    static uint MAXPOSVALUE = 4294967295;

    // 싱글 톤 패턴으로 하나의 인스턴스만 생성 되도록!!!
    private static BluetoothService _instance;
    public static BluetoothService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BluetoothService>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(BluetoothService).ToString());
                    _instance = singleton.AddComponent<BluetoothService>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }
    // private Dictionary<string, string> scanResults = new Dictionary<string, string>();

    // public Dictionary<string, string> ScanResults
    // {
    //     get => scanResults;
    //     private set
    //     {
    //         scanResults = value;
    //         OnScanResultsChanged?.Invoke(this, EventArgs.Empty);
    //     }
    // }

    private Dictionary<string, BleScanResult> _scanResults;
    public Dictionary<string, BleScanResult> ScanResults
    {
        get => _scanResults;
        private set
        {
            Debug.Log("OnScanResultsChanged 호출");
            _scanResults = value;

        }
    }

    private void _setScanResult(string name, BleScanResult value)
    {
        if (!ScanResults.ContainsKey(value.address))
        {
            Debug.Log("SetScanResult호출");
            // var _temp = ScanResults;
            ScanResults[name] = value;
            Debug.Log(value.address + "가 추가됨");
            // ScanResults = _temp;
            OnScanResultsChanged?.Invoke(this, EventArgs.Empty);
        }

    }
    public event EventHandler OnScanResultsChanged;

    private string sensorAddress;
    public string SensorAddress
    {
        get => sensorAddress;
        private set
        {
            sensorAddress = value;
            OnSensorAddressChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler OnSensorAddressChanged;

    private bool isScanning;
    public bool IsScanning
    {
        get => isScanning;
        private set
        {
            isScanning = value;
            OnIsScanningChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler OnIsScanningChanged;

    private Ble_WheelyHubSpeedVO leftWheelyx;
    public Ble_WheelyHubSpeedVO LeftWheelyx
    {
        get => leftWheelyx;
        private set
        {
            leftWheelyx = value;
            OnLeftWheelyxChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler OnLeftWheelyxChanged;

    private Ble_WheelyHubSpeedVO rightWheelyx;
    public Ble_WheelyHubSpeedVO RightWheelyx
    {
        get => rightWheelyx;
        private set
        {
            rightWheelyx = value;
            OnRightWheelyxChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler OnRightWheelyxChanged;

    private BluetoothState _state;
    public BluetoothState State
    {
        get => _state;
        private set
        {
            _state = value;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler OnStateChanged;

    public static Ble_WheelyHubData previous { get; set; } = new Ble_WheelyHubData(0, 0, 0, 0);
    public static Ble_WheelyHubData current { get; private set; } = new Ble_WheelyHubData(0, 0, 0, 0);

    public void Awake()
    {
        ScanResults = new Dictionary<string, BleScanResult>();
        Initialize();

    }
    private void Initialize()
    {
        BluetoothLEHardwareInterface.Initialize(true, false, () =>
        {
            SetState(BluetoothState.Initialize);
            // _timeout = _startScanDelay;
        },
        (error) =>
        {
            SetState(BluetoothState.Error);
            BluetoothLEHardwareInterface.Log("Error: " + error);
            // StatusMessage = "Error during initialize: " + error;
            if (error.Contains("Bluetooth LE Not Enabled"))
                BluetoothLEHardwareInterface.BluetoothEnable(true);
        });
    }

    void Start()
    {

    }

    public void StartScan()
    {
        SetState(BluetoothState.Scanning);
        IsScanning = true;
        BluetoothLEHardwareInterface.Log("StartScan()진입");
        //! TimeOut 관련된 기능 어디있는지(스캔 언제 마칠건지)
        //  (_, __) =>
        //  {
        //      // action 하지만 사용하지 않음
        //  },
        //actionAdrtisingInfo, Advertising 이란 디바이스 정보를 알려주는 과정임
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(
             ServiceUUIDs, null, (address, name, rssi, bytes) =>
             {

                 BluetoothLEHardwareInterface.Log("item scanned: " + address);
                 Debug.Log("ScanResults 갱신전");

                 if (ScanResults.ContainsKey(address))
                 {
                     BluetoothLEHardwareInterface.Log("scanResults.ContainsKey(address) -> true");
                     BluetoothLEHardwareInterface.Log("ScanResults already contains the address: " + address);
                     var scannedItem = ScanResults[address];
                     scannedItem.setRssi(rssi.ToString());
                     _setScanResult(address, scannedItem);

                 }
                 else
                 {
                     BluetoothLEHardwareInterface.Log("scanResults.ContainsKey(address) -> false");
                     BleScanResult scannedItem = new(name, address, rssi.ToString());
                     //  ScanResults[address] = scannedItem;
                     _setScanResult(address, scannedItem);
                     BluetoothLEHardwareInterface.Log("Adding new device to ScanResults: " + address);
                     //  ScanResults[address] = name;
                 }
                 //  ScanResults = scanResults;
                 Debug.Log("ScanResults 갱신 완료");
             }, true);
    }

    public void StopScan()
    {
        IsScanning = false;
        SetState(BluetoothState.ScanComplete);
        BluetoothLEHardwareInterface.StopScan();
    }

    public void StartConnect(string deviceAddress)
    {
        StartCoroutine(ConnectCoroutine(deviceAddress));
    }

    private bool isConnectedDevice = false;
    private bool isSubscribed = false;

    private IEnumerator ConnectCoroutine(string deviceAddress)
    {
        int epoch = 0; //연결시도를 3번정도
        int max_epoch = 3;

        while (epoch < max_epoch)
        {
            bool connectionAttempted = false;

            BluetoothLEHardwareInterface.ConnectToPeripheral(
                deviceAddress,
                (_) =>
                {
                    SetState(BluetoothState.Connecting); // 상태를 연결 중으로 변경
                    connectionAttempted = true;
                },
                null, // serviceAction은 사용 안 함
                (address, serviceUUID, characteristicUUID) =>
                { // 연결 성공 시 Action
                    SetState(BluetoothState.Connected);
                    SensorAddress = deviceAddress;
                    isConnectedDevice = true;
                    BluetoothLEHardwareInterface.StopScan();
                },
                (_) =>
                { // 연결 실패 시 Action
                    epoch++;
                    SetState(BluetoothState.FailConnect);
                    SensorAddress = null;
                    isConnectedDevice = false;
                    connectionAttempted = true;
                }
            );

            // 연결 시도 완료될 때까지 대기
            yield return new WaitUntil(() => connectionAttempted);

            // 연결 성공 시 코루틴 종료
            if (SensorAddress != null)
            {
                StartCoroutine(SubscirbeCoroutine());
                yield break;
            }

            // 연결 시도 간 대기 시간 설정 (옵션)
            yield return new WaitForSeconds(1f);
        }
        //todo 최대 시도 횟수에 도달한 경우 처리
        // ShowMaxConnectTryError
    }

    private IEnumerator SubscirbeCoroutine()
    {
        yield return new WaitForSeconds(2f);
        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(SensorAddress, ServiceUUID, wheelyxSpeedCharacteristicUUID, (notifyAddress, notifyCharacteristic) =>
                 {
                     new WaitForSeconds(2f);
                     BluetoothLEHardwareInterface.ReadCharacteristic(SensorAddress, ServiceUUID, wheelyxSpeedCharacteristicUUID, (characteristic, bytes) =>
                             {
                                 SetState(BluetoothState.Subscribing);
                             });
                 },
                 (address, wheelyxSpeedCharacteristicUUID, bytes) =>
                 {
                     UpdateWheelyHubData(Ble_WheelyHubData.FromHex(bytes));
                 });
    }
    public void DisconnectAll()
    {
        //todo DisconnectAll 되었는지 확인 필요
        BluetoothLEHardwareInterface.DisconnectAll();
        SetState(BluetoothState.Disconnected);
    }

    public void Reset()
    {
        StartCoroutine(ResetCoroutine());
    }

    private void SetState(BluetoothState newState)
    {
        State = newState;
    }



    private IEnumerator ResetCoroutine()
    {
        SetState(BluetoothState.Reset);
        DisconnectAll();
        StopScan();
        yield break;
    }

    public void UpdateWheelyHubData(Ble_WheelyHubData calCurrent)
    {
        current = calCurrent;
        LeftWheelyx = Ble_WheelyHubSpeedVO.FromRawLeftWheelyxModel(previous, current);
        RightWheelyx = Ble_WheelyHubSpeedVO.FromRawRightWheelyxModel(previous, current);

        previous.Battery = current.Battery;
        previous.LeftWheelRev = current.LeftWheelRev;
        previous.RightWheelRev = current.RightWheelRev;
        previous.MeasureTime = current.MeasureTime;
    }



    public void HandleTestHubDate(Ble_WheelyHubSpeedVO value)
    {
        // Debug.Log("handleTestHubDate");
        // Debug.Log(value.SpeedABS);
        LeftWheelyx = value;
        RightWheelyx = value;

    }
}
