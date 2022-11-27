using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigDug_ButtonPlayAgain : MonoBehaviour
{
    private void Awake() {
        gameObject.SetActive(!DigDugPlayedMaps.IsFull());
    }
}
