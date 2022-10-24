using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfromEnabler : MonoBehaviour
{
    private bool forceEnabled = true;


    void Start()
    {
        if(forceEnabled){gameObject.SetActive(true);};


        #if UNITY_ANDROID
        #else
            gameObject.SetActive(false);
        #endif
    }

}
