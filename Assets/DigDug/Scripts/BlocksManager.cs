using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlocksManager_Levels{
    static List<string[]> levels = new List<string[]>{};
    public static string[] GetLevel(int level){
        if(levels.Count == 0){
            PropagateLevels();
        }

        return levels[level % levels.Count];
    }

    private static void PropagateLevels(){

        levels = new List<string[]>{
            /*new string[] {
                "                 ",
                "XXXXXXXXXXXXXXXXX",
                "XXXX XXXXXXXXXXXX",
                "XXX 1 XXXXXXXXXXX",
                "XXXX XXXXXXXXXXXX",
                "XXXXRXXX 3 XXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX XXXXXXXXX  XX",
                "XXX XXXXXXXXX 2XX",
                "XXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
            },
            
            new string[]{
                "                 ",
                "XXXX XXXXXXXXXXXX",
                "XXXX XXXXXXXXXXXX",
                "XXX 1  1 1 1 1XXX",
                "XXXX XXXXXXXXXXXX",
                "XXXXRXXX 3 XXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX XXXXXXXXX2 XX",
                "XXX XXXXXXXXX22XX",
                "XXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
            
            },
            */
            new string[]{
                "             1   ",
                "XXXX XXXXXXRRRRRR",
                "XXXX XXXXXXXXXXXX",
                "XXXXXXXRXX   XX X",
                "XXXX XXXXXXX    X",
                "XXXXRXXXXXXXXXXXX",
                "XXXXXXXXXRXXXXXXX",
                "XXX XXXXXXX 1 XXX",
                "XXX XXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
            
            },
        };
    }
}

public class BlocksManager : MonoBehaviour
{
    // Start is called before the first frame update
    int currentLevel = 0;
    void Start(){
        LoadLevel();
    }

    void LoadLevel(){
        string[] level = BlocksManager_Levels.GetLevel(currentLevel);
        for(int i = 0; i < level.Length; i++) {
            string row = level[i];
            for(int j = 0; j < row.Length; j++){
                DD_BrickController brick = transform.GetChild(i * row.Length + j).GetComponent<DD_BrickController>();
                brick.Setup();

                bool right = j > 0 && IsNotEmpty(row[j-1]);
                bool up    = i > 0 && IsNotEmpty(level[i-1][j]);

                switch(row[j]){
                    case 'X': break;
                    case ' ':
                        brick.Disable(up, right, false);
                    break;
                    case 'R':
                        brick.Disable(up, right, true);
                        brick.MakeRock();
                    break;
                    case '1':
                        brick.Disable(up, right, false);
                        brick.MakeEnemy(0);
                    break;
                    case '2':
                        brick.Disable(up, right, false);
                        brick.MakeEnemy(1);
                    break;
                    case '3':
                        brick.Disable(up, right, false);
                        brick.MakeEnemy(2);
                    break;
                }
            }
        }
    }

    private bool IsNotEmpty(char c){
        return c != ' ' && c != '1';
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.B)){
            LoadLevel();
        }
    }
}
