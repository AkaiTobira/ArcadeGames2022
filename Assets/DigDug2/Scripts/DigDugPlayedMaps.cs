using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DigDugPlayedMaps 
{
    private static HashSet<int> _maps = new HashSet<int>();

    public static void ResetList(){
        _maps.Clear();
    }

    public static void LockMap(int mapID){
        _maps.Add(mapID);
    }

    public static bool IsLocked(int mapID){
        return _maps.Contains(mapID);
    }

    public static bool IsFull(){
        return _maps.Count == LevelManager.NUMBER_OF_LEVELS;
    }
}
