using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
 //   private static bool _canLoadNextScene = true;

    [SerializeField] int sceneFlowIndex;

    public void OnSceneLoad(bool playSound = false){
        if(playSound) AudioSystem.Instance.PlayEffect("Button", 1);
        SceneManager.LoadScene(SceneFlowController.GetNextScene(sceneFlowIndex));


        Debug.Log("OnSceneLoad() :: " + SceneFlowController.GetNextScene(sceneFlowIndex));
    }

    IEnumerator LoadGameScene()
    {
      //  if(!_canLoadNextScene) yield break;

     //   _canLoadNextScene = false;
        yield return new WaitForSecondsRealtime(1f);

        Debug.Log(SceneFlowController.GetNextScene(sceneFlowIndex));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneFlowController.GetNextScene(sceneFlowIndex));

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Resources.UnloadUnusedAssets();
        asyncLoad.allowSceneActivation = true;
     //   _canLoadNextScene = true;
    }

    public void OnSceneLoadAsync(){
//        Debug.Log(SceneFlowController.GetNextScene(sceneFlowIndex) + " Called");
        //TimersManager.Instance.FireAfter(1.5f, () => {
            StartCoroutine(LoadGameScene());
        //});

        Debug.Log("OnSceneLoadAsync() :: " + SceneFlowController.GetNextScene(sceneFlowIndex));
    }
}
