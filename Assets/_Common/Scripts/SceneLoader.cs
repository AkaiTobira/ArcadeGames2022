using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void OnSceneLoad(bool playSound = false){
        if(playSound) AudioSystem.Instance.PlayEffect("Button", 1);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Resources.UnloadUnusedAssets();
        asyncLoad.allowSceneActivation = true;
    }

    public void OnSceneLoadAsync(){
        StartCoroutine(LoadGameScene());
    }

}
