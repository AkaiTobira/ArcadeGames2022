using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class G_WaterMissle : CUpdateMonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] float distance;
    [SerializeField] float speed;
    [SerializeField] float animationSpeed;
    [SerializeField] int points;
    
    private Image _image;
    private G_PlantInstance _plantie;
    private PlayerIndex _index;
    private Vector3 _direction;
    private float _animationTime;
    private int _animationIndex;

    protected override void Awake()
    {
        base.Awake();
        _image = GetComponent<Image>();
    }

    public void Setup(G_PlantInstance target, PlayerIndex index){
        _plantie = target;
        _index = index;
        _direction = (_plantie.transform.position - transform.position).normalized;
    }

    public override void CUpdate()
    {
        base.CUpdate();

        _animationTime -= Time.deltaTime;
        if(_animationTime < 0){
            _animationTime += animationSpeed;
            _animationIndex = (_animationIndex + 1) % sprites.Length; 
            _image.sprite = sprites[_animationIndex];
        }

        transform.position += _direction * speed * Time.deltaTime;
        distance -= speed * Time.deltaTime;
        if(distance < 0) {
            Destroy(gameObject);

            if(!_plantie.IsWaterd()){
                PointsCounter.AddPoints(_index, points);
            }
            _plantie.Regenerate();
        }
    }
}
