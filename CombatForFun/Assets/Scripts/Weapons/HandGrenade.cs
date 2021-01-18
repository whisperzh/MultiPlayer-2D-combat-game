using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrenade : MonoBehaviour
{
    public LayerMask DamageMask;
    public float MaxDamage;//爆心伤害
    public float ExplodeRadius;
    public float ExplosionForce=1;
    public float Speed;
    public float WaitForSecondsToStop;
    public float WaitForSecondsToExplode;
    public float H_R_lapis;

    private bool _isThrowOut = false;
    private float _timeToStop;
    private bool _isWaitForExp = false;
    private float _timeToExp;

    [Header("Shader 部分")]
    [Range(0,5)]
    public float OutlineWidth;
    public SpriteRenderer ExpRadiusDisplay;
    public Shader ExpRadiusShader;
    private Material ExpMat;

    public Rigidbody2D _rigidbody2d;
    [HideInInspector]
    public bool Activated;

    public AudioClip AC_ExpAudio;

    private void Start()
    {
        Activated = false;
        _rigidbody2d = GetComponent<Rigidbody2D>();

        ExpMat = new Material(ExpRadiusShader);
        ExpMat.SetFloat("_OutlineWidth", OutlineWidth);
        ExpMat.SetFloat("_OuterRadius", 0);
        ExpMat.SetFloat("_InnerRadius", 0);


        ExpRadiusDisplay.material = ExpMat;
        ExpRadiusDisplay.transform.localScale = new Vector2(1, 1) * ExplodeRadius * 2;

        _timeToStop = WaitForSecondsToStop;
        _timeToExp = WaitForSecondsToExplode;
    }

    private void Update()
    {
        
        if (_isThrowOut)
        {
            if(_timeToStop > 0)
            {
                _timeToStop -= Time.deltaTime;

                ExpMat.SetFloat("_OuterRadius", Mathf.SmoothStep(0, 1, (WaitForSecondsToStop - _timeToStop) / WaitForSecondsToStop));
            }
        }

        if (_isWaitForExp)
        {
            if (_timeToExp > 0)
            {
                _timeToExp -= Time.deltaTime;

                ExpMat.SetFloat("_InnerRadius", Mathf.SmoothStep(0, 1, (WaitForSecondsToExplode - _timeToExp) / WaitForSecondsToExplode));
            }
        }
    }

    public void Throw(Vector2 Dir){
        Activated = true;
        transform.parent = null;
        gameObject.SetActive(true);
        Vector2 newDir =  Dir;
        newDir = newDir.normalized * Speed;
        _rigidbody2d.velocity += newDir;
        _rigidbody2d.angularVelocity =550f;

        _isThrowOut = true;
        Invoke("Stop", WaitForSecondsToStop);
        
    }


    public void Stop(){
        _rigidbody2d.velocity = new Vector2(0, 0);
        _rigidbody2d.angularVelocity = 0;

        _isWaitForExp = true;
        Invoke("Explode", WaitForSecondsToExplode);
    }

    public void Explode() {

        CameraShake.Instance.ShakeCamera(2f, 0.4f);
        ExpMat.SetFloat("_OuterRadius", 0);

        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.GetComponent<Animator>().Play("GrenadaExp");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ExplodeRadius,DamageMask);
        foreach (Collider2D c in colliders)
        {  
            if (c.GetComponent<Player_Controller>() != null)
            {
                Vector2 dir = c.transform.position-transform.position;
                float dist = Vector2.Distance(c.transform.position, transform.position);
                dir = new Vector2(ExplodeRadius /( dir.x+1), ExplodeRadius / (dir.y+1))*ExplosionForce;
                c.GetComponent<Player_Controller>().TemporarilyCannotMove(H_R_lapis);
                c.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
                if (dist >= 0 && dist <= ExplodeRadius) {
                    // float hurt = (MaxDamage + 1) / (dist + 1) - 1;
                    float hurt = -MaxDamage/(ExplodeRadius* ExplodeRadius)*(dist * dist)+MaxDamage;
                    if (hurt > 0)
                        c.GetComponent<Player_Controller>().Hurt(hurt);
                }
            }
        }
    }

    public void PlayExpAudio()
    {
        GetComponent<AudioSource>().PlayOneShot(AC_ExpAudio);
    }

    public void DestroySelf()
    {
        Destroy(gameObject, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ExplodeRadius);
    }
}
