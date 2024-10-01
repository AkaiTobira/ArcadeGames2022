using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class B_AlphaManipulator : AlphaManipolator
{
    [SerializeField] EnableRandomChild enableRandom;
    [SerializeField] TextMeshProUGUI levelLabel;

    bool isActive = false;

    //private void OnEnable() {
    //    Hide();
    //    BLevelsManager.Paused = false;
    //}

    protected override void OnShow()
    {
        base.OnShow();

        BLevelsManager.Paused = true;
        isActive = true;
        enableRandom.Enable();

        TimersManager.Instance.FireAfter(5, ()=> { Hide(); isActive = false;});
        levelLabel.text = "LEVEL " + BLevelsManager.CurrentLevel.ToString();
    }

    public override void CUpdate()
    {
        base.CUpdate();

        if(InputHandler.GetKey(InputKey.Confirm) && isActive) {
            Hide();
            isActive = false;
        }

        enableRandom.transform.parent.GetComponent<CanvasGroup>().alpha = image.color.a;
        //levelLabel.color.a = image.color.a;
    }

    protected override void OnHide()
    {
        base.OnHide();

        BLevelsManager.Paused = false;
        
    }
}
