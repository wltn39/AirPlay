
using UnityEngine;
using UnityEngine.UI;

// 검색된 블루투스 기기 목록을 화면에 표시하기 위한 Class
public class BleScanResult : MonoBehaviour
{
    public string name { get; private set; }
    public string address { get; private set; }
    public string rssi { get; private set; }

    // 생성자
    public BleScanResult(string textNameValue, string textAddressValue, string textRSSIValue)
    {
        name = textNameValue;
        address = textAddressValue;
        rssi = textRSSIValue;
    }

    public void setName(string newName)
    {
        name = newName;
    }
    public void setAddress(string newAddress)
    {
        address = newAddress;
    }
    public void setRssi(string newRssi)
    {
        rssi = newRssi;
    }
}
