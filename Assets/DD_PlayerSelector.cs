using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DD_PlayerSelector : MonoBehaviour
{
    [SerializeField] Sprite[] inactivePlayers;
    [SerializeField] Sprite[] activePlayers;
    [SerializeField] Image[] image;

    public static int PlayerSelected = 0;

    private void Awake() {
        SetImageActive(PlayerSelected);
    }

    void Update()
    {
        float inputHozrionatal = Input.GetAxisRaw("Horizontal");
        if(Mathf.Abs(inputHozrionatal) > 0.3f){
            if(inputHozrionatal > 0) SetImageActive(1);
            if(inputHozrionatal < 0) SetImageActive(0);
        }

        if(Input.GetKeyDown(KeyCode.C)){
            Events.Gameplay.RiseEvent(GameplayEventType.ContinueAnimation);
        }
    }

    private void SetImageActive(int imageIndex){
        for(int i =0; i < image.Length; i++){
            image[i].sprite = imageIndex == i ? activePlayers[i] : inactivePlayers[i];
        }

        PlayerSelected = imageIndex;
    }
}
