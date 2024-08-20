using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class G_AlertDisplayer : MonoBehaviour
{
    [SerializeField] GameObject _warning;
    [SerializeField] GameObject _warningTextBox;
    [SerializeField] TextMeshProUGUI _warningText;
    [SerializeField] G_DurationBar _bar;

    public void Show(G_RandomEvents @event, float duration = 2){

        _warning.SetActive(true);
        switch (@event){
            case G_RandomEvents.InverseMovement: 
                _warningText.text = AutoTranslator.Translate("GInverseMovement");
            break;
            case G_RandomEvents.MorePlanties: 
                _warningText.text = AutoTranslator.Translate("GMorePlanties");
            break;
            case G_RandomEvents.CodeBreak: 
                _warningText.text = AutoTranslator.Translate("GCodeBreak");
            break;
        }

        RotationManager.Instance.RotateBy(_warning.transform, new Vector3(0,0,360), 0.2f);
        ScaleManager.Instance.ScaleTo(_warning.transform, new Vector3(1,1,1), 0.2f, () => { 
            _warningText.gameObject.SetActive(true);
            _warningTextBox.gameObject.SetActive(true);
            _bar.FillIn(duration-0.2f, true);
        });

        TimersManager.Instance.FireAfter(duration, () => Hide());
    }

    private void Hide(){
        ScaleManager.Instance.ScaleTo(_warning.transform, new Vector3(0.01f,0.01f,0.01f), 0.2f, () => { 
            _warningText.gameObject.SetActive(false);
            _warningTextBox.gameObject.SetActive(false);
            _warning.SetActive(false);
        });
    }
}
