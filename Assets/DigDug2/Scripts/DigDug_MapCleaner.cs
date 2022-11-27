using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigDug_MapCleaner : MonoBehaviour
{
    private void Awake() {
        DigDugPlayedMaps.ResetList();
    }
}
