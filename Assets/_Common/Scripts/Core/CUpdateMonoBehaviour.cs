
public class CUpdateMonoBehaviour : CMonoBehaviour
{
    protected virtual void Start(){
        CUpdateManger.Register(this);
    }

    public virtual void CUpdate(){}
    public virtual void CFixedUpdate(){}
    public virtual void CLateUpdate(){}
}
