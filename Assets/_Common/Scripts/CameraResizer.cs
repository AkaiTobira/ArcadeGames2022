using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizer : MonoBehaviour
{

    private Camera _camera;

    private void Awake() {
        _camera = Camera.main;
    }

    private void Update() {
        transform.localScale = new Vector3(_camera.orthographicSize, _camera.orthographicSize, 1) * 0.1f;
    }
}
