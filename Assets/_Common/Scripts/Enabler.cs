using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enabler : MonoBehaviour
{
    [SerializeField] bool _enable;

    private void Awake() {
        gameObject.SetActive(_enable);
    }

}
