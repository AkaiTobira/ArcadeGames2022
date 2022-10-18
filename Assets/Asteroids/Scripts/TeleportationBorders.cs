using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationBorders : MonoBehaviour
{
    [SerializeField] private Transform _lborder;
    [SerializeField] private Transform _rborder;
    [SerializeField] private Transform _tborder;
    [SerializeField] private Transform _bborder;


    public static Transform Left;
    public static Transform Right;
    public static Transform Top;
    public static Transform Bottom;

    private void Start() {
        Left = _lborder;
        Right = _rborder;
        Top = _tborder;
        Bottom = _bborder;
    }


}
