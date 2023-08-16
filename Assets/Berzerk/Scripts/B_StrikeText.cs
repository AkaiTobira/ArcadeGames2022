using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class B_StrikeText : MonoBehaviour
{
    List<string> _translationTags = new List<string>(){
        "BerzerkStrike1",
        "BerzerkStrike2",
        "BerzerkStrike3",
        "BerzerkStrike4",
        "BerzerkStrike5",
        "BerzerkStrike6",
    };


    private void Awake() {
        RotationManager.Instance.RotateBy(transform, new Vector3(0,0,90), 0.25f);
        Destroy(gameObject, 2f);

        GetComponent<TextMeshProUGUI>().text = AutoTranslator.Translate(_translationTags[Random.Range(0, _translationTags.Count)]);
    }
}
