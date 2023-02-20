using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Fromater
{
    private const int SECONDS_IN_HOUR = 3600;
    private const int SECONDS_IN_MINUTE = 60;

    public static string FormatToTime(int points){
        string time = (points / SECONDS_IN_HOUR).ToString().PadLeft(2, '0') + ":";
        time += ((points % SECONDS_IN_HOUR)  / SECONDS_IN_MINUTE).ToString().PadLeft(2, '0') + ":";
        time += (points % SECONDS_IN_MINUTE).ToString().PadLeft(2, '0');
        return time;
    }

    public static string FormatToPoints(int points, int places = 8){
        return points.ToString().PadLeft(places, '0');
    }
}
