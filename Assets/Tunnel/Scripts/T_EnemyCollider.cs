using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class T_EnemyCollider : MonoBehaviour
{
    Collider2D _collider;
    [SerializeField] float _showWarningAt;
    [SerializeField] float _enableAt;
    [SerializeField] float _disableAt;

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] Canvas _childCanvas;
    [SerializeField] Image _image;
    [SerializeField] Color _warningColor;
    [SerializeField] Color _enabledColor;

    private void Awake() {
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {

        bool shouldBeEnabled = transform.parent.parent.localScale.x > _enableAt && transform.parent.parent.localScale.x < _disableAt;

        _collider.enabled = shouldBeEnabled;
    //    transform.GetChild(0).gameObject.SetActive(_collider.enabled);

        _childCanvas.gameObject.SetActive(transform.parent.parent.localScale.x > _showWarningAt && transform.parent.parent.localScale.x < _disableAt);
        
        _image.color = transform.parent.parent.localScale.x > _enableAt ? _enabledColor : _warningColor;
        
        
        _childCanvas.sortingOrder = _parentCanvas.sortingOrder + 1;


    }
}
