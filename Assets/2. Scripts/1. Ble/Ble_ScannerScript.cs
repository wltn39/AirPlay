using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine.SceneManagement;

public class Ble_ScannerScript : MonoBehaviour
{
	public GameObject Content; // 스크롤뷰의 Content 오브젝트
	public GameObject ButtonPrefab2;
	public Text StatusText;
	public Text ReceiveDataText;
	public Text BatteryDataText;
	public Text LeftWheelSpeedDataText;
	public Text RightWheelSpeedDataText;

	private float _timeout;
	private float _startScanTimeout = 3f;
	private float _startScanDelay = 0.1f;
	private bool _startScan = true;
	private Dictionary<string, Ble_ScannedItemScript> _scannedItems;
	private string[] serviceUUIDs = new string[] { "0000fff0-0000-1000-8000-00805f9b34fb" };
	public string ServiceUUID = "0000fff0-0000-1000-8000-00805f9b34fb";
	public string wheelyxSpeedCharacteristicUUID = "0000fff1-0000-1000-8000-00805f9b34fb";
	private bool _connected = false;
	private string _deviceAddress;
	private bool _foundButtonUUID = false;
	private bool _foundLedUUID = false;
	private bool _rssiOnly = false;
	private int _rssi = 0;
	private uint _maxValue = 4294967295;
	public double leftCumulativeDistance = 0;
	public double rightCumulativeDistance = 0;

	public BluetoothState State { get; private set; }
	// private List<GameObject> buttonList = new List<GameObject>();
	private List<Ble_ScannedItemScript> scannedItemScriptsList = new List<Ble_ScannedItemScript>();

	private string StatusMessage
	{
		set
		{
			// BluetoothLEHardwareInterface.Log(value);
			StatusText.text = value.ToString();
		}
	}

	private BluetoothService _service;


	//? Arthur: 이유는 모르겠지만 Awake에 넣어서 BleScannerScript가 Instanciate할 때 한번 값들을 만져주면 이후에도 잘 작동합니다.
	void Awake()
	{

		// Initialize Bluetooth Service
		_service = BluetoothService.Instance;
		_scannedItems = new Dictionary<string, Ble_ScannedItemScript>();
		StatusMessage = "Init";

	}
	void OnEnable()
	{
		_service.OnScanResultsChanged += HandleScanResultsChanged;
		_service.OnSensorAddressChanged += HandleSensorAddressChanged;
		_service.OnIsScanningChanged += HandleIsScanningChanged;
		_service.OnLeftWheelyxChanged += HandleLeftWheelyxChanged;
		_service.OnRightWheelyxChanged += HandleRightWheelyxChanged;
		_service.OnStateChanged += HandleStateChanged;
	}

	void OnDisable()
	{
		_service.OnScanResultsChanged -= HandleScanResultsChanged;
		_service.OnSensorAddressChanged -= HandleSensorAddressChanged;
		_service.OnIsScanningChanged -= HandleIsScanningChanged;
		_service.OnLeftWheelyxChanged -= HandleLeftWheelyxChanged;
		_service.OnRightWheelyxChanged -= HandleRightWheelyxChanged;
		_service.OnStateChanged -= HandleStateChanged;
	}

	void Start()
	{


		// BluetoothLEHardwareInterface.Log("Start");
		// _service.OnScanResultsChanged += HandleScanResultsChanged;
		// _service.OnSensorAddressChanged += HandleSensorAddressChanged;
		// _service.OnIsScanningChanged += HandleIsScanningChanged;
		// _service.OnLeftWheelyxChanged += HandleLeftWheelyxChanged;
		// _service.OnRightWheelyxChanged += HandleRightWheelyxChanged;
		// _service.OnStateChanged += HandleStateChanged;





	}



	public void OnClickStartGame()
	{
		SceneManager.LoadScene("VL_GameScene");
	}

	public void OnClickTH_Start_Easy()
	{
		SceneManager.LoadScene("TH_Game_Easy");
	}

	public void OnClickTH_Start_Normal()
	{
		SceneManager.LoadScene("TH_Game_Normal");
	}
	public void OnClickTH_Start_Hard()
	{
		SceneManager.LoadScene("TH_Game_Hard");
	}



	public void OnClickStopScanning()
	{
		_startScan = true;
		// _timeout = _startScanDelay;
		StatusMessage = "StopScan";
		_service.StopScan();
	}

	public void OnClickStartScanning()
	{
		leftCumulativeDistance = 0;
		rightCumulativeDistance = 0;
		StatusMessage = "Scanning ... ";
		_startScan = false;
		_timeout = _startScanTimeout;
		Debug.Log("_service.StartScan()");
		_service.StartScan();
	}

	private IEnumerator OnClickConnecting(string deviceAddress)
	{
		StatusMessage = "Connecting...";
		_foundButtonUUID = false;
		_foundLedUUID = false;
		_service.StartConnect(deviceAddress);
		yield break;
	}

	private void HandleScanResultsChanged(object sender, EventArgs _)
	{
		foreach (var kvp in _service.ScanResults)
		{

			Debug.Log("kvp.Value.address" + kvp.Value.address);
			if (!_scannedItems.ContainsKey(kvp.Value.address))
			{
				Debug.Log("!_scannedItems.ContainsKey(kvp.Value.address) -> true");
				Debug.Log("kvp.Value.address" + kvp.Value.address);
				AddToScannedItemsAndButton(kvp.Key, kvp.Value);
			}
			else
			{
				Debug.Log("!_scannedItems.ContainsKey(kvp.Value.address) -> false");

			}



		}
	}

	private void HandleSensorAddressChanged(object sender, EventArgs e)
	{
		StatusMessage = $"Connected to {_service.SensorAddress}";
	}

	private void HandleIsScanningChanged(object sender, EventArgs e)
	{
		if (!_service.IsScanning)
		{
			StatusMessage = "Scan Complete";
		}
	}

	private void HandleLeftWheelyxChanged(object sender, EventArgs e)
	{
		LeftWheelSpeedDataText.text = _service.LeftWheelyx.RealSpeed.ToString(CultureInfo.InvariantCulture);
	}

	private void HandleRightWheelyxChanged(object sender, EventArgs e)
	{
		RightWheelSpeedDataText.text = _service.RightWheelyx.RealSpeed.ToString(CultureInfo.InvariantCulture);
	}

	private void HandleStateChanged(object sender, EventArgs e)
	{
		State = _service.State;
		StatusMessage = $"State changed to: {State}";
	}

	private void AddToScannedItemsAndButton(string key, BleScanResult scanResult)
	{
		// foreach (GameObject button in buttonList)
		// {
		// 	!buttonList.ContainsKey(kvp.Value.address)
		// 	Ble_ScannedItemScript script = button.GetComponent<Ble_ScannedItemScript>();
		// 	Debug.Log("Button with address " + scanResult.address);
		// 	if (script != null && script.getAddressValue() == scanResult.address)
		// 	{
		// 		Debug.Log("Button with address " + scanResult.address + " already exists.");
		// 		return; // 이미 리스트에 같은 주소의 버튼이 있으면 추가하지 않고 종료
		// 	}
		// }

		GameObject newButton = Instantiate(ButtonPrefab2, Content.transform);
		if (newButton != null)
		{
			if (newButton.TryGetComponent<Ble_ScannedItemScript>(out var scannedItem))
			{
				var buttonScript = newButton.GetComponent<Ble_ScannedItemScript>();
				buttonScript.setNameValue(scanResult.name);
				buttonScript.setAddressValue(scanResult.address);
				buttonScript.setRssiValue(scanResult.rssi);
				_scannedItems.Add(scanResult.address, buttonScript); // 이름표 추가
																	 // _scannedItems[scanResult.address] = buttonScript;

				// BluetoothLEHardwareInterface.Log("item set: " + address);
				newButton.name = "Button_" + scanResult.name;


				Button buttonComponent = newButton.GetComponent<Button>();
				if (buttonComponent != null)
				{
					buttonComponent.onClick.AddListener(() => StartCoroutine(OnClickConnecting(scanResult.address)));
				}

				// buttonList.Add(newButton); // 버튼 추가 

			}
		}




	}



}
