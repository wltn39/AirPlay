
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

public class Ble_WheelyHubData
{
    private const uint MAX_VALUE = 4294967295;
    public byte[] SensorData { get; set; }
    public int Battery { get; set; }
    public uint MeasureTime { get; set; }
    public uint LeftWheelRev { get; set; }
    public uint RightWheelRev { get; set; }

    public Ble_WheelyHubData(byte[] sensorData, int battery, uint measureTime, uint leftWheelRev, uint rightWheelRev)
    {
        SensorData = sensorData;
        Battery = battery;
        MeasureTime = measureTime;
        LeftWheelRev = leftWheelRev;
        RightWheelRev = rightWheelRev;
    }
    public Ble_WheelyHubData(int battery, uint measureTime, uint leftWheelRev, uint rightWheelRev)
    {

        Battery = battery;
        MeasureTime = measureTime;
        LeftWheelRev = leftWheelRev;
        RightWheelRev = rightWheelRev;
    }

    public static Ble_WheelyHubData FromHex(byte[] data)
    {
        // string hex = BitConverter.ToString(data.ToArray()).Replace("-", string.Empty);
        string hex = "";
        foreach (byte b in data)
        {
            hex += string.Format("{0:X2}", b);
        }

        if (hex.Length < 1)
            return new Ble_WheelyHubData(data, 0, 0, 0, 0);

        int battery = GetBattery(hex);
        uint measureTime = GetMeasureTime(hex);
        uint leftWheelRev = GetLeftWheelRev(hex);
        uint rightWheelRev = GetRightWheelRev(hex);

        try
        {
            return new Ble_WheelyHubData(data, battery, measureTime, leftWheelRev, rightWheelRev);
        }
        catch (Exception)
        {
            return new Ble_WheelyHubData(data, 0, 0, 0, 0); // Equivalent to returning null in Dart
        }
    }

    private static int GetBattery(string hex)
    {
        string result = hex.Substring(6, 2) + hex.Substring(4, 2);
        // int battery = Convert.ToInt32(result, 16);
        int battery = int.Parse(result, NumberStyles.HexNumber);
        return battery;
    }

    private static uint GetMeasureTime(string hex)
    {
        string result = hex.Substring(14, 2) + hex.Substring(12, 2) + hex.Substring(10, 2) + hex.Substring(8, 2);
        // int measureTime = Convert.ToInt32(result, 16);
        uint measureTime = uint.Parse(result, NumberStyles.HexNumber);
        return measureTime;
    }

    private static uint GetLeftWheelRev(string hex)
    {
        string result = hex.Substring(22, 2) + hex.Substring(20, 2) + hex.Substring(18, 2) + hex.Substring(16, 2);
        // int lWheelPos = Convert.ToInt32(result, 16);
        // if (lWheelPos == MAX_VALUE)
        // {
        //     lWheelPos = 0;
        // }
        uint lWheelPos = uint.Parse(result, NumberStyles.HexNumber);

        if (lWheelPos == MAX_VALUE)
        {
            lWheelPos = 0;
        }
        return lWheelPos;
    }

    private static uint GetRightWheelRev(string hex)
    {
        string result = hex.Substring(30, 2) + hex.Substring(28, 2) + hex.Substring(26, 2) + hex.Substring(24, 2);
        // int rWheelPos = Convert.ToInt32(result, 16);
        // if (rWheelPos == MAX_VALUE)
        // {
        //     rWheelPos = 0;
        // }
        uint rWheelPos = uint.Parse(result, NumberStyles.HexNumber);

        if (rWheelPos == MAX_VALUE)
        {
            rWheelPos = 0;
        }
        return rWheelPos;
    }
}

