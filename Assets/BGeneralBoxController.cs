using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGeneralBoxController : MonoBehaviour
{

    [SerializeField] BGeneralBox Left;
    [SerializeField] BGeneralBox Right;

    public static float Retributed = 0;
    public static bool TankSend = false;
    public static BGeneralBoxController Instance;

    private void Awake() {
        Instance = this;
    }

    // Update is called once per frame
    
    bool _previouseLeftActive = false;
    
    void Update()
    {
        bool enableLeft = Berzerk.Instance.transform.position.x > 0;

        if(enableLeft != _previouseLeftActive){
            Left.Adjust();
            Right.Adjust();
        }
        _previouseLeftActive = enableLeft;

        Left.gameObject.SetActive(enableLeft);
        Right.gameObject.SetActive(!enableLeft);
    }


    public void Setup(){
        Retributed = 0;
        TankSend = false;

        Left.Setup();
        Right.Setup();
    }
}
