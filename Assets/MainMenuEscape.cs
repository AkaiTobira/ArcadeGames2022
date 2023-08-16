using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEscape : MonoBehaviour
{
    float loadDelay = 0.5f;

    void Update()
    {
        if(loadDelay < 0) return;
        if( Input.GetKeyDown(KeyCode.Escape) && 
            SceneFlowController.GetActiveIntro() != SceneManager.GetActiveScene().name){
                SceneManager.LoadScene(SceneFlowController.GetActiveIntro(), LoadSceneMode.Single);
                loadDelay = 0.5f;
        }
    }
}
