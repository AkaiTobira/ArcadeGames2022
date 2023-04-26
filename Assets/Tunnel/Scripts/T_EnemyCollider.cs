using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class T_EnemyCollider : MonoBehaviour
{
    Collider2D _collider;
    [SerializeField] float _enableAt;
    [SerializeField] float _disableAt;

    private void Awake() {
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        _collider.enabled = transform.parent.parent.localScale.x > _enableAt && transform.parent.parent.localScale.x < _disableAt;
    }
}
