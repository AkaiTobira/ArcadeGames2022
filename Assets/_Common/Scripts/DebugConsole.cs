

     using UnityEngine;
     
     namespace DebugStuff
     {
         public class DebugConsole : MonoBehaviour
         {
     //#if !UNITY_EDITOR
             static string myLog = "";
             private string output;
             private string stack;
             private bool display = false;
     
             void OnEnable()
             {
                 Application.logMessageReceived += Log;
             }
     
             void OnDisable()
             {
                 Application.logMessageReceived -= Log;
             }
            private void Update() {
               
                if(Input.GetKeyDown(KeyCode.T)){ display = !display; }
            }


             public void Log(string logString, string stackTrace, LogType type)
             {
                 output = logString;
                 stack = stackTrace;
                 myLog = output + "\n" + myLog;
                 if (myLog.Length > 5000)
                 {
                     myLog = myLog.Substring(0, 4000);
                 }
             }
     
             void OnGUI()
             {
                 //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
                 {

                if(display) myLog = GUI.TextArea(new Rect(10, 10, Screen.width /2, Screen.height /2), myLog);
                else {myLog = GUI.TextArea(new Rect(-1000,-1000, 1, 1), myLog);}
                 }
             }
     //#endif
         }
     }