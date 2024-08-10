using UnityEngine;

public class RotationMenu : CUpdateMonoBehaviour
{
    [SerializeField] private CButton[] buttons;
    [SerializeField] private RectTransform[] points;
    [SerializeField] private float scaleSize = 2;

    bool canMove = true;
    int buttonIndex = 0;

    private void OnEnable() {
        Vector2 startPoint = new Vector2(0,1);
        float angle = -2*Mathf.PI/buttons.Length;

        float distance = (points[0].transform.position - transform.position).magnitude;
        for (int i = 0; i < points.Length; i++){
            points[i].position = CUtils.RotateVector(startPoint,i * angle) * distance;
        }

        SetupButton(3, scaleSize, 0);
    }

    private void SetupButton(int sortingOrder, float scale, float time = 0.2f){
        ScaleManager.Instance.ScaleTo(buttons[buttonIndex].transform, new Vector3(scale,scale,0), 0.2f);
        buttons[buttonIndex].GetComponent<Canvas>().sortingOrder = sortingOrder;
    }

    public override void CUpdate()
    {
        base.CUpdate();

        float horizontalMove = InputHandler.GetHorizontal();
        if(canMove && Mathf.Abs(horizontalMove) > 0.5f){
            canMove = false;

            SetupButton(2, 1);
            buttonIndex = (buttonIndex + (int)Mathf.Sign(horizontalMove) + buttons.Length) % buttons.Length;
            buttons[buttonIndex].OnPointerExit(null);
            RotationManager.Instance.RotateBy(
                transform, 
                new Vector3(0,0, Mathf.Sign(horizontalMove) * 360.0f/buttons.Length), 
                0.2f, () => {
                    canMove = true;
                    buttons[buttonIndex].OnPointerEnter(null);
            });
            SetupButton(3,scaleSize);
        }

        if(InputHandler.GetKey(InputKey.Confirm)){
            buttons[buttonIndex].OnPointerDown(null);
        }
    }
}
