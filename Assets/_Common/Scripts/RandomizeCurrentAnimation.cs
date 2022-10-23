using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeCurrentAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TimersManager.Instance.FireAfter(Random.Range(0,0.5f), () => {
            if(Guard.IsValid(this)) GetComponent<Animator>().enabled = true;
        });
    }

}
