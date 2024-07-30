using UnityEngine;
using UnityEngine.UI;

// 검색된 블루투스 기기 목록을 화면에 표시하기 위한 Class
public class Ble_ScannedItemScript : MonoBehaviour
{
	// public Text TextNameValue;
	// public Text TextAddressValue;
	// public Text TextRSSIValue;
	// {
	public Text TextNameValue;
	public Text TextAddressValue;
	public Text TextRSSIValue;

	// // 생성자
	// public Ble_ScannedItemScript(Text textNameValue, Text textAddressValue, Text textRSSIValue)
	// {
	// 	TextNameValue = textNameValue;
	// 	TextAddressValue = textAddressValue;
	// 	TextRSSIValue = textRSSIValue;
	// }
	public void setNameValue(string value)
	{
		TextNameValue.text = value;
	}
	public void setAddressValue(string value)
	{
		TextAddressValue.text = value;
	}
	public void setRssiValue(string value)
	{
		TextRSSIValue.text = value;
	}

	// public string getAddressValue()
	// {
	// 	return TextAddressValue.text;
	// }

}
