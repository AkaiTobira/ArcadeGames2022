using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Floor : MonoBehaviour
{
    [SerializeField] Sprite[] _terrains;
    [SerializeField] public GTerrainType _type;

    [SerializeField] public Floor Left;
    [SerializeField] public Floor Right;
    [SerializeField] public Floor Up;
    [SerializeField] public Floor Down;

    [SerializeField] public BreakInGraphical BLeft;
    [SerializeField] public BreakInGraphical BRight;
    [SerializeField] public BreakInGraphical ULeft;
    [SerializeField] public BreakInGraphical URight;

    [SerializeField] public RectTransform BLeft_position;
    [SerializeField] public RectTransform BRight_position;
    [SerializeField] public RectTransform ULeft_position;
    [SerializeField] public RectTransform URight_position;




    void Start()
    {
        GetComponent<Image>().sprite = _terrains[(int)_type];
        GetComponent<Image>().color = Color.white;

        if(BLeft)  {
            BLeft.transform.position  = BLeft_position.transform.position;
            BLeft.URight = this;
        }
        if(BRight) {
            BRight.transform.position = BRight_position.transform.position;
            BRight.ULeft = this;
        }
        if(ULeft)  {
            ULeft.transform.position  = ULeft_position.transform.position;
            ULeft.BRight = this;
        }
        if(URight) {
            URight.transform.position = URight_position.transform.position;
            URight.BLeft = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
