using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Player2 : T_Player
{

    float horizontalBreakPoint = 0.3f;
    float previousHorizontalValue = 0;



    protected override void Update()
    {
        base.Update();

        transform.Rotate(new Vector3(0,0, T_Segment.RotationSpeed * (T_SegmentSpawner.MULTIPLER - 1.0f) * Time.deltaTime));
    }

    protected override void ProcessMove_Horizontal(float horizontal)
    {
// /        base.ProcessMove_Horizontal(horizontal);

        if(Mathf.Abs(horizontal) > horizontalBreakPoint && previousHorizontalValue != horizontal){
            
            float rotationAngle = Mathf.Sign(horizontal)  * 20f;  //> 0 ? angles.Forward : -angles.Backward;
            transform.Rotate(new Vector3(0,0, rotationAngle));
        }

        previousHorizontalValue = horizontal;
    }
}
