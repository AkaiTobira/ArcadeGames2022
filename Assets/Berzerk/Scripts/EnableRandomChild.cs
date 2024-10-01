using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomChild : MonoBehaviour
{
    public void Enable(){
        int randomChild = CUtils.Rand(transform.childCount);
        for(int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(randomChild == i);
        }
    }
}
