using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionScreamer : MonoBehaviour
{
    private void Awake() {
        Debug.Log(name + " : " + transform.position);
    }

}
