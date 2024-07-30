using System;
using System.Diagnostics;


// 속도, 거리 계산 해주는 VO 
// Class인데, 값이 들어있는 클래스에요.
// 데이터 전달을 위한 클래스 입니다.
[System.Serializable]
public class Ble_WheelyHubSpeedVO
{
    private static readonly double Pi = 3.1415926535897932;
    private static readonly uint UINT16_MAX = 65536 - 1; // 2^16
    private static readonly long UINT32_MAX = 4294967296 - 1; // 2^32

    private readonly double _speed;
    public double SpeedABS => Math.Abs(_speed);
    public double RealSpeed => _speed;
    public double Rpm { get; private set; }
    public double Distance { get; private set; }
    public double Battery { get; private set; }

    public Ble_WheelyHubSpeedVO(double battery, double speed, double distance)
    {
        Battery = battery;
        _speed = speed;
        Distance = distance;
    }

    private static double TransformMsToKmH(double speedMs)
    {
        return speedMs * 3.6;
    }

    public static double WheelyxWheelPosConst = 8;
    public static double RollerR = 0.03815;
    public static double RollerC = 2 * Pi * RollerR; // 0.2395
    public static double NewWheelyHubPeriod = 0.25;


    public static Ble_WheelyHubSpeedVO FromRawLeftWheelyxModel(Ble_WheelyHubData previous, Ble_WheelyHubData current)
    {
        uint timeDiff = CalWheelyXTimeDiff(previous.MeasureTime, current.MeasureTime);
        long leftWheelPosDiff = CalWheelyXPosDiff(previous.LeftWheelRev, current.LeftWheelRev);
        double time = timeDiff * NewWheelyHubPeriod;

        // Debug.Log($"Left Wheel - Time Diff: {timeDiff}, Pos Diff: {leftWheelPosDiff}, Time: {time}");



        //단위: m
        double distance = RollerC / WheelyxWheelPosConst * 4 * leftWheelPosDiff;
        double distanceAbs = Math.Abs(distance);
        // double speed = TransformMsToKmH(distance / time);
        double speed = TransformMsToKmH(RollerC / WheelyxWheelPosConst * 4 * leftWheelPosDiff / time);
        double battery = CalculateWheelyxBattery(current.Battery);

        // Debug.Log($"Left Wheel - Speed: {speed}, Distance: {distance}, Battery: {battery}");

        return new Ble_WheelyHubSpeedVO(battery, speed, distanceAbs);
    }

    public static Ble_WheelyHubSpeedVO FromRawRightWheelyxModel(Ble_WheelyHubData previous, Ble_WheelyHubData current)
    {
        uint timeDiff = CalWheelyXTimeDiff(previous.MeasureTime, current.MeasureTime);
        long rightWheelPosDiff = CalWheelyXPosDiff(previous.RightWheelRev, current.RightWheelRev);
        double time = timeDiff * NewWheelyHubPeriod;

        // Debug.Log($"Right Wheel - Time Diff: {timeDiff}, Pos Diff: {rightWheelPosDiff}, Time: {time}");


        //단위: m
        double distance = RollerC / WheelyxWheelPosConst * 4 * rightWheelPosDiff;
        double distanceAbs = Math.Abs(distance);
        // double speed = TransformMsToKmH(distance / time);
        double speed = TransformMsToKmH(RollerC / WheelyxWheelPosConst * 4 * rightWheelPosDiff / time);
        double battery = CalculateWheelyxBattery(current.Battery);

        // Debug.Log($"Right Wheel - Speed: {speed}, Distance: {distance}, Battery: {battery}");

        return new Ble_WheelyHubSpeedVO(battery, speed, distanceAbs);
    }

    private static long CalWheelyXPosDiff(uint prevPos, uint currentPos)
    {
        uint _currentPos = currentPos;
        if (Math.Abs(_currentPos) > ((Math.Abs(prevPos) + 1) * 1000))
        {
            _currentPos = prevPos;
        }
        long diff = (long)_currentPos - prevPos;

        if (diff < 0 && Math.Abs(diff) > 100000)
        {
            diff += UINT32_MAX;
        }
        else if (diff > 0 && Math.Abs(diff) > 100000)
        {
            diff -= UINT32_MAX;
        }
        //    Debug.Log($"CalWheelyXPosDiff - PrevPos: {prevPos}, CurrentPos: {currentPos}, Diff: {diff}");
        return diff;
    }

    private static uint CalWheelyXTimeDiff(uint prevPos, uint currentPos)
    {
        uint diff = currentPos - prevPos;

        if (diff < 0 && Math.Abs(diff) > 10000)
        {
            diff += UINT16_MAX;
        }
        else if (diff > 0 && Math.Abs(diff) > 10000)
        {
            diff -= UINT16_MAX;
        }
        //  Debug.Log($"CalWheelyXTimeDiff - PrevPos: {prevPos}, CurrentPos: {currentPos}, Diff: {diff}");
        return diff;
    }

    private static double CalculateWheelyxBattery(double battery)
    {
        double result = (battery - 2800) / 14;
        if (result < 11)
        {
            result = 10;
        }
        return result;
    }


}
