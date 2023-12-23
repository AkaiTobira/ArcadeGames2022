using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigDug{
    public class DD_GameController : CMonoBehaviour, IListenToGameplayEvents
    {
        [SerializeField] DD_BlocksManager blocksManager;
        [SerializeField] Transform _playerPosition;
        [SerializeField] SceneLoader loader;

        public static int   NumberOfEnemies = 0;
        public static float SpeedMultiplier = 0.8f;
        float time = 0;

        DangerLevel dangerLevel = DangerLevel.Level1;
        public static HashSet<DD_BrickController> ActiveEnemies = new HashSet<DD_BrickController>();

        public void OnGameEvent(GameplayEvent gameEvent)
        {
            if(gameEvent.type == GameplayEventType.GameOver){
                HighScoreRanking.TryAddNewRecord(PointsCounter.Score);
                loader.OnSceneLoadAsync();
                AlphaManipolator.Show();
                ActiveEnemies.Clear();
            }
        }

        private void Start() {
            HighScoreRanking.LoadRanking(GameType.DigDug);

            Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
            PointsCounter.Score = 0;
            NumberOfEnemies = 0;
            SpeedMultiplier = 0;
        }

        private enum DangerLevel{
            Level1,
            Level2,
            Level3,
            Level4
        }

        public void UpdateSpeedMultiplier(){
            DangerLevel level = dangerLevel;


            if(time < 15){
                SpeedMultiplier = 0.8f;
                dangerLevel = DangerLevel.Level1;
            }else if(time < 30){
                SpeedMultiplier = 1.0f;
                dangerLevel = DangerLevel.Level2;
            }else if(time < 45){
                SpeedMultiplier = 1.2f;
                dangerLevel = DangerLevel.Level3;
            }else{
               SpeedMultiplier = 1.5f;
               dangerLevel = DangerLevel.Level4;
            }

            if(level != dangerLevel) AudioSystem.PlaySample("DigDug_DangerIncrease");
        }


        private void Update(){
            time += Time.deltaTime;
            UpdateSpeedMultiplier();

            string ss = ActiveEnemies.Count.ToString();
            foreach( DD_BrickController blok in ActiveEnemies){
                ss += blok.name + " ";
            }

            Debug.Log(ss);

            if(ActiveEnemies.Count <= 0 && blocksManager.Initilized){
                AlphaManipolator.Show();
                time = 0;

                TimersManager.Instance.FireAfter(2, ()=>{
                    DD_Player3.Instance.Setup();
                    blocksManager.SetNewLevel();
                });
                TimersManager.Instance.FireAfter(3, ()=>{
                    AlphaManipolator.Hide();
                    enabled = true;
                });

                enabled = false;
            }
        }
    }
}