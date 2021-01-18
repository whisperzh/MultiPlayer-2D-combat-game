using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Bullet : MonoBehaviour
{
    public float FlyingDistance;

    private Rigidbody2D m_Rigidbody;
    private Animator anim;
    private Player_Controller _owner;
    private Vector2 _flyDir;
    private int _damage;
    private float _speed;

    public LayerMask glass;

    // Start is called before the first frame update

    void Start()
    {
        Destroy(this.gameObject, FlyingDistance/_speed);
        m_Rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Rigidbody.velocity= _flyDir.normalized * _speed * 10;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, _flyDir, 0.4f, glass);
        Debug.DrawRay(transform.position, _flyDir);
        if (raycastHit2D)
        {
            TileMapMorpher tmc = raycastHit2D.collider.gameObject.GetComponent<TileMapMorpher>();
            if (tmc != null)
            {
                tmc.des(raycastHit2D.point,transform.rotation, _flyDir);
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// 设置子弹属性
    /// </summary>
    /// <param name="Owner">子弹所属</param>
    /// <param name="BulletDamage">子弹伤害</param>
    /// <param name="BulletSpeed">子弹速度</param>
    /// <param name="FlyDir">子弹飞行速度</param>
    public void SetBulletAttribution(Player_Controller Owner,int BulletDamage,float BulletSpeed,Vector2 FlyDir)
    {
        _owner = Owner;
        _damage = BulletDamage;
        _speed = BulletSpeed;
        _flyDir = FlyDir;
    }

    private void OnCollisionEnter2D(Collision2D collision)  
    {
        Player_Controller victim = collision.gameObject.GetComponent<Player_Controller>();  
        if (victim != null)
        {
            if (victim == _owner)
                return;
            victim.Hurt(_damage);
            _speed = 0;
            _damage = 0;
            anim.Play("BulletDes");
        }
        else if(collision.gameObject.GetComponent<M_Bullet>()!=null){
            return;
        }
        else
        {
            _speed = 0;
            _damage = 0;
            anim.Play("BulletDes");   
        }
    }

    public void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
