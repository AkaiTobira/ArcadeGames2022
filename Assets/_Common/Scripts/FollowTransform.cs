using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : CUpdateMonoBehaviour
{
    [SerializeField] RectTransform rect;

    public override void CUpdate()
    {
        base.CUpdate();
        transform.position = rect.position;
    }
}
