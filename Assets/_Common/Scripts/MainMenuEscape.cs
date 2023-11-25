using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEscape : MonoBehaviour
{
    float loadDelay = 0.5f;

    private static MainMenuEscape _instance;
    private string startSceneName;

    private void OnEnable() {
        if(!Guard.IsValid(_instance)){
            _instance = this;
            DontDestroyOnLoad(this);
        }else{
            Destroy(this);
        }

        startSceneName = SceneFlowController.GetLocalizedSceneName(SceneFlowController.GetActiveIntro());
    }

    void Update()
    {
        if(loadDelay < 0) return;
        if( Input.GetKeyDown(KeyCode.V) && 
            startSceneName != SceneManager.GetActiveScene().name){
                SceneManager.LoadScene(startSceneName, LoadSceneMode.Single);
                loadDelay = 0.5f;
        }
    }
}
