using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigDug{
    public class DD_GameController : CMonoBehaviour, IListenToGameplayEvents
    {
        [SerializeField] DD_BlocksManager blocksManager;
        [SerializeField] Transform _playerPosition;
        [SerializeField] SceneLoader loader;

        public static int NumberOfEnemies = 0;


        public void OnGameEvent(GameplayEvent gameEvent)
        {
            if(gameEvent.type == GameplayEventType.GameOver){
                HighScoreRanking.TryAddNewRecord(PointsCounter.Score);
                loader.OnSceneLoadAsync();
                AlphaManipolator.Show();
            }
        }

        private void Start() {
            DD_Player3.Instance.transform.position = _playerPosition.position;
            HighScoreRanking.LoadRanking(GameType.DigDug);

            Events.Gameplay.RegisterListener(this, GameplayEventType.GameOver);
            PointsCounter.Score = 0;
            NumberOfEnemies = 0;
        }

        private void Update(){
            if(NumberOfEnemies <= 0 && blocksManager.Initilized){
                NumberOfEnemies = 0;
                AlphaManipolator.Show();

                TimersManager.Instance.FireAfter(2, ()=>{
                    DD_Player3.Instance.transform.position = _playerPosition.position;
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