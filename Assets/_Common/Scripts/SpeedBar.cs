
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BS_SpeedBar : MonoBehaviour
{
    [SerializeField] int _maxSpeed = 320;
    [SerializeField] Image _bar;
    [SerializeField] TextMeshProUGUI _text;

    public void Setup(float percent){
        _text.text = ((int)(percent * _maxSpeed)).ToString() + "\nMPH";
        _bar.fillAmount = percent;
    }
}
