using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidSizes{

    public List<Asteroid.EAsteroidSize> Sizes = new List<Asteroid.EAsteroidSize>();
    public HashSet<Asteroid.EAsteroidSize> InvalidSizes = new HashSet<Asteroid.EAsteroidSize>();
}


public class Asteroids_Cell : Asteroids
{
    [SerializeField] Transform[] _mitos;
    [SerializeField] Transform[] _rybos;
    [SerializeField] Transform   _centros;
    [SerializeField] PlayerPreview _previes;

    [SerializeField] RectTransform QRCode;
    [SerializeField] RectTransform centerPoint;

    [SerializeField] SceneLoader _endingScene;
    

    List<int> CurruptedParts = new List<int>{
        0,0,0
    };


    protected override void OnTriggerEnter2D(Collider2D other) {

        if(!Guard.IsValid(other)) return;
        

        if(other.tag.Contains("cle")){
            if(other.GetComponent<Asteroid>().IsShootedDown()) return;
        
            Asteroid cellPart = other.GetComponent<Asteroid>();
            cellPart.enabled = false;
            other.enabled = false;

            Transform target = transform;
            Vector3 targetScale = new Vector3();

            switch (cellPart.GetSize()) {
                case Asteroid.EAsteroidSize.EAS_HUGE: 
                    targetScale = new Vector3(0.5f,0.5f, 1);
                    target = _centros;
                    CurruptedParts[0] = 1;
                    break;
                case Asteroid.EAsteroidSize.EAS_NORMAL: 
                    targetScale = new Vector3(0.3f,0.3f, 1);

                    if(CurruptedParts[2] >= _mitos.Length) return;
                    target = _mitos[CurruptedParts[2]];
                    CurruptedParts[2] += 1;
                
                    break;
                case Asteroid.EAsteroidSize.EAS_SMALL:  
                    targetScale = new Vector3(0.3f,0.3f, 1);
                    if(CurruptedParts[1] >= _mitos.Length) return;
                    target = _rybos[CurruptedParts[1]];
                    CurruptedParts[1] += 1;
                    break;
            }

            base.OnTriggerEnter2D(other);


            
            ScaleManager.Instance.ScaleTo( other.transform, targetScale, 0.4f);
            TweenManager.Instance.TweenTo( other.transform, target, 0.4f, () => {
                if(!Guard.IsValid(other)) return;

                (other.transform as RectTransform).SetParent(target);
                other.transform.position = target.transform.position;
            });


            ValidSizes sizes = new ValidSizes();
            if(CurruptedParts[0] > 0){sizes.InvalidSizes.Add(Asteroid.EAsteroidSize.EAS_HUGE);}
            if(CurruptedParts[1] > 2){sizes.InvalidSizes.Add(Asteroid.EAsteroidSize.EAS_SMALL);}
            if(CurruptedParts[2] > 2){sizes.InvalidSizes.Add(Asteroid.EAsteroidSize.EAS_NORMAL);}

            for(int i = 0; i < 3; i++) {
                if(sizes.InvalidSizes.Contains((Asteroid.EAsteroidSize)i)) continue;
                sizes.Sizes.Add((Asteroid.EAsteroidSize)i);
            }
            Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.ResizeAsteroids, sizes));


            AsteroidSpawner.AsteroidCount -=1;
            _movePenalty = 1.0f - ((CurruptedParts[0] * 1.5f + CurruptedParts[1] * 1.2f + CurruptedParts[2]) * 0.1f);
            _previes.Setup(CurruptedParts[0],CurruptedParts[1],CurruptedParts[2]);

            if( (CurruptedParts[0] + CurruptedParts[1] + CurruptedParts[2]) >= 7 ){
                HighScoreRanking.LoadRanking(GameType.Asteroids);
                HighScoreRanking.TryAddNewRecord(PointsCounter.Score);
                Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.GameOver));
                enabled = false;

                ScaleManager.Instance.ScaleTo(transform, new Vector3(0.01f,0.01f,1), 0.2f,() => {
                    
                _endingScene.OnSceneLoadAsync();

                });
                
                AudioSystem.Instance.PlayEffect("Asteroid_Dead", 1);
                AudioSystem.Instance.PlayMusic("Asteroid_BG", 0.2f);



//                TweenManager.Instance.TweenTo(QRCode, centerPoint, 1f, () => {

                //    Vector3 transform2 = QRCode.transform.position;
                //    transform2.z = 0;
                //    QRCode.transform.position = transform2;
                //    Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.RefreshRanking));
                
                //    AudioSystem.Instance.PlayEffect("Victory", 1);
  //              });
                
            }else{
                AudioSystem.Instance.PlayEffect("Asteroid_Hit", 1, true);
            }
        }
    }
}
