using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

   [SerializeField] GameObject _activated;

    public bool OnReach(){
        if(_activated != null){
            bool wasActive = _activated.activeSelf;
            _activated.SetActive(true);
            return wasActive != _activated.activeSelf;
        }
        return false;
    }


}
