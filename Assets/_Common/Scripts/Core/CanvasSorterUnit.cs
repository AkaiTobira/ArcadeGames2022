using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSorterUnit : MonoBehaviour
{
    [SerializeField] int keepRelative = 0;

    private void OnEnable() {
        CanvasSorter.AddCanvas(GetComponent<Canvas>(), keepRelative);
    }

    private void OnDisable() {
        CanvasSorter.RemoveCanvas(GetComponent<Canvas>());
    }

}
