using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfromEnabler : MonoBehaviour
{
    void Start()
    {
        #if UNITY_ANDROID
        #else
            gameObject.SetActive(false);
        #endif
    }

}
