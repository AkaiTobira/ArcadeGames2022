using System.Collections;
using System.Collections.Generic;
using DigDug;
using UnityEngine;

public static class DD_Levels{
    static List<string[]> levels = new List<string[]>{};
    public static string[] GetLevel(int level){
        if(levels.Count == 0){
            PropagateLevels();
        }

        return levels[level % levels.Count];
    }

    private static void PropagateLevels(){

        levels = new List<string[]>{
            new string[] {
                "                 ",
                "XXXXRXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX 1 XXXXXXXXXXX",
                "XXXX XXXXXXXXXXXX",
                "XXXRXXXX S XXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX XXXXXXXXX  XX",
                "XXX1XXXXXXXXX  XX",
                "XXX XXXXXXXXX1 XX",
                "XXXXXXXXXXXXXXXXX",
            },new string[] {
                "                 ",
                "XXXRRRXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX 1 XXXXXXXXXXX",
                "XXXX XXXXXXXX   X",
                "XXXXXXXS   XX 2 X",
                "XXXXXXXXXXXXXXXXX",
                "XXX XXXXXXXXX  XX",
                "XXX2XXXXXXXXX  XX",
                "XXX XXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
            },new string[] {
                "                 ",
                "XXXXRXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX 3 XXRRR XXXXX",
                "XXXX XXXXXXXXXXXX",
                "XXXRX    S   XXXX",
                "XXXXXXXXXXXXXXXXX",
                "XXX XXXXXXXXX  XX",
                "XXX3   XXXXXX  XX",
                "XXX XXXXXXXXX 3XX",
                "XXXXXXXXXXXXXXXXX",
            },
            new string[]{
                "                 ",
                "XXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXX",
                "X  1  1  1  1   X",
                "XXXXXXXXXXXXXXXXX",
                "XXXXXXXX   XXXXXX",
                "XXXXXXXX S XXRXXX",
                "XXX XXXXRRRX X XX",
                "XXX XXXXXXXXX  XX",
                "XXXXXXXX 1 XXXXXX",
                "XXXXXXXX   XXXXXX"
            },
            new string[]{
                "                 ",
                "XXXX XXXXXXXXXXXX",
                "XXXX XXXXXXXXRRXR",
                "RRRRRRRRXX   XX X",
                "XXXXXXXXX  X    X",
                "XXXRRRR S  RRRRXX",
                "XXXXXXXXXRXXXXXXX",
                "XXX 1  XXXX 1  XX",
                "XXX XX XXXX XX XX",
                "XXX     2      XX",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "XXXX XXXXXXXXXXXX",
                "XXX S XXXXXRRRRRR",
                "RRRRRRRRXX XXXXXX",
                "XXXXXXXXXX  3   X",
                "X      XXXXRRRRXX",
                "X      X  XXXXXXX",
                "X   3  X  X 3  XX",
                "X   XX XXXX XX XX",
                "X        3     XX",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "XXXX XXXXX XXXXXX",
                "X XXXXXXXXXXXXXRR",
                "R2RRRRRRXX    XXX",
                "X XXXXXX   S  XXX",
                "XXXXXXXXXXXRR RXX",
                "XXXX  XXXRXXX XXX",
                "XXX 3  XXXXXX XXX",
                "XXX    XXXXXX2XXX",
                "RRRRRRRRRRRRRRRRR",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "XXXXXXXXXXSXXXXXX",
                "XXXXXXXXXX XXXXXX",
                "XXX   XXX3   XXXX",
                "XXXXX3XXXX   3XXX",
                "XRRRRX XXXXRRRRXX",
                "XXXXXX XXXXXXXXXX",
                "X   2X XXXX 2XXXX",
                "X   X   XXX XX XX",
                "XXX    1 2     XX",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "XXXXXXXXXXXXXXXXX",
                "XXX  XXXXX XXXXXX",
                "XX    XXX2 XXXXXX",
                "XX S  XXXX    XXX",
                "XRRRRXXXXXX RRRXX",
                "XXXXXX XX   XXXXX",
                "X   2X XXXX 2XXXX",
                "X   XXXXXXX XX XX",
                "XXX  XX2 2XXXXXXX",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "XXXXXXXXXXXXXXXXX",
                "XXX  XXXXX XXXXXX",
                "XX    XXX2 XXSXXX",
                "XX  X2XXXX      X",
                "XRRRRXXXXXX RRR X",
                "XXXXXX XX   XXX X",
                "X   2X XXXX 2X  X",
                "X   XXXXXXX XX XX",
                "XXX  XX2 2  XX XX",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "XXXXXXXXXXXXXXXXX",
                "XXX  2XXXX   S XX",
                "XXXXXXXRRX  XXXXX",
                "XX  X2XXXX   1XXX",
                "XR2RRXX   X RRRXX",
                "XXXXXX XX  3XXXXX",
                "X   2X XXXX 2XXXX",
                "X   XXXXXXX XX XX",
                "XXX  XX2 2     XX",
                "XXXXXXXXXXXXXXXXX",
            },new string[]{
                "                 ",
                "RXRRRRRRR RRRRRXR",
                "X XXXXXXX XXXXX3X",
                "X3 3 3        3 X",
                "RX  XRRRRR   XXRR",
                "XR2RRXXXXXX RRRXX",
                "XXRXXX X   3XXXXX",
                "XXX XX XX   2XX X",
                "X   XXXXXXX XX  X",
                "XXX 1XX   S    XX",
                "XXXXXXXXXXXXXXXXX",
            }
        };
    }
}

public class DD_BlocksManager : MonoBehaviour
{
    // Start is called before the first frame update
    int currentLevel = 0;
    [SerializeField] int numberOfBricks = 0;

    [SerializeField] DD_BrickController brickControllerPrefab;
    [SerializeField] Transform background;
    public bool Initilized;

    static DD_BlocksManager Instance;

    Dictionary<char, DD_BrickState> CharToState = new Dictionary<char, DD_BrickState>{
        {'X', DD_BrickState.Full},
        {' ', DD_BrickState.Empty},
        {'1', DD_BrickState.Enemy1},
        {'2', DD_BrickState.Enemy2},
        {'3', DD_BrickState.Enemy3},
        {'R', DD_BrickState.Rock},
        {'S', DD_BrickState.PlayerPosition},
    };

    void Start(){
        StartCoroutine(RestoreSavedPositions());
        Instance = this;
    }

    private IEnumerator RestoreSavedPositions(){
        Vector2 sizes = DD_NavMesh.GetSize();

        for(int i = 0; i < numberOfBricks; i++) {
            yield return new WaitForEndOfFrame();
            DD_BrickController brick = Instantiate(brickControllerPrefab, background);
            brick.name = "Brick (" + i + ")";
            brick.Recolor(i, (int)sizes.y, (int)sizes.x);
            brick.transform.SetAsLastSibling();
        }
        yield return new WaitForEndOfFrame();
        LoadLevel();
        DD_NavMesh.InitializeNavMesh();
        AlphaManipolator.Hide();
        Initilized = true;
    }

    public void SetNewLevel(){
        currentLevel++;
        LoadLevel();
        AudioSystem.PlaySample("DigDug_LevelWin");
    }

    public static int GetCurrentLevel(){
        if(Guard.IsValid(Instance)) return Instance.currentLevel;

        return 0;
    }    

    void LoadLevel(){
        string[] level = DD_Levels.GetLevel(currentLevel);



        for(int i = 0; i < level.Length; i++) {
            string row = level[i];
            for(int j = 0; j < row.Length; j++){
                DD_BrickController brick = transform.GetChild(i * row.Length + j).GetComponent<DD_BrickController>();
                DD_GameController.ActiveEnemies.Remove(brick);

                bool right = j == 0 ;
                if(j > 0) right |= !IsEmpty(row[j]) || !IsEmpty(row[j-1]);

                //HAX
                if(j==0 && i==0) right = false;

                bool toop  = false;
                if(i > 0) toop |= !IsEmpty(level[i][j]) || !IsEmpty(level[i-1][j]);

                brick.Setup(
                    CharToState[row[j]], 
                    right, 
                    toop);
            }
        }
    }

    private bool IsEmpty(char c){
        return c == '1' || c == '2' || c == '3' || c == ' ' || c == 'S';
    }

    private bool IsNotEmpty(char c){
        return c != ' ' && c != '1';
    }
#if UNITY_EDITOR
    private void Update() {
        if(Input.GetKeyDown(KeyCode.B)){
            currentLevel++;
            LoadLevel();
        }
    }
#endif
}
