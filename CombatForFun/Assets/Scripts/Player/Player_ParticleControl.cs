using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Controller))]
public class Player_ParticleControl : MonoBehaviour
{
    [Tooltip("以玩家为中心 的粒子半径")]
    public float Radius;

    [Space(10)]
    public ParticleSystem Par_RunDust;
    public ParticleSystem Par_DodgeDust;
    public ParticleSystem Par_FallDust;

    private int _maxNum_Par_RunDust;

    private Vector3 _particleOffset;
    private Transform _particleCtrl;
    private Player_Controller _playerCtrl;

    // Start is called before the first frame update

    private void Awake()
    {
        _playerCtrl = gameObject.GetComponent<Player_Controller>();
        _particleCtrl = transform.Find("ParticleControl");
    }
    void Start()
    {
        _particleOffset = (Vector2)(_particleCtrl.transform.position - _playerCtrl.transform.position);
        _maxNum_Par_RunDust = Par_RunDust.main.maxParticles;

        Par_DodgeDust.Stop();
        Par_FallDust.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //粒子相对位置
        _particleCtrl.position = (Vector3)(_playerCtrl.MoveDir * -1 * Radius ) + (_playerCtrl.transform.position + _particleOffset);
        RunDustParCtrl();
    }

    private void RunDustParCtrl()
    {
        bool isEmit = _playerCtrl.isMove;

        int parNum = Par_RunDust.main.maxParticles;
        var parMain = Par_RunDust.main;

        if(isEmit)
        {
            if (parNum == _maxNum_Par_RunDust)
                return;
            else
            {
                parMain.maxParticles = _maxNum_Par_RunDust;
                Par_RunDust.Play();
            }
        }
        else
        {
            parMain.maxParticles = 0;
            Par_RunDust.Stop();
        }
    }

    public void PlayDodgeDust()
    {
        Par_RunDust.Stop();
        Par_DodgeDust.transform.localScale *= new Vector2(_playerCtrl.isFaceRight ? 1 : -1,1);
        Par_DodgeDust.Play();
    }

    public void PlayFallDust()
    {
        Par_RunDust.Play();
        Par_FallDust.transform.localScale *= new Vector2(_playerCtrl.isFaceRight ? 1 : -1, 1);
        Par_FallDust.Play();
    }

}
