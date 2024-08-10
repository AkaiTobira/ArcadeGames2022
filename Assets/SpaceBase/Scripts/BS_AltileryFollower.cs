using UnityEngine;

public class BS_AltileryFollower : BS_Follower{

    [SerializeField] RawImageAnimator _animator;

    public override float GetDamage(){ return 2; }

    protected override void PlayShootAnimation()
    {
        base.PlayShootAnimation();
        _animator.Play();
    }

    protected override void Shoot(){
        if(Guard.IsValid(player)){
            if((player.transform.position - transform.position).magnitude <= _distanceToStartMoving){
                base.Shoot();
            }
        }
    }
}
