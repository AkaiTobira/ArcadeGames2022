using UnityEngine;
using UnityEngine.UI;

public class G_PlantInstance : CMonoBehaviour
{
    [SerializeField] Sprite[] _sprites;
    [SerializeField] Image _image;

    private int _index = 0;

    protected override void Awake() { _image.sprite = _sprites[_index]; base.Awake(); }
    public bool CanDegradate(){ return _index < _sprites.Length; }
    public bool IsWaterd(){ return _index == 0; }
    public void Setup(){ 
        AudioSystem.PlaySample("Garden_FlowerNew", 1.6f);
        gameObject.SetActive(true);
        _image.sprite = _sprites[0]; 
        _index = 0; 
    }

    public void Degradate(){
        if(!CanDegradate()) return;

        _index++;
        if(_index < _sprites.Length){
            _image.sprite = _sprites[_index];
        }
        
        if(_index == _sprites.Length-1){
           Events.Gameplay.RiseEvent(new GameplayEvent(GameplayEventType.PlayerDied, this));
            gameObject.SetActive(false);

            AudioSystem.PlaySample("Garden_FlowerLost");
        }
    }

    public void Regenerate(){
        if(_index > 0) _index--;
        _image.sprite = _sprites[_index];
    }
}
