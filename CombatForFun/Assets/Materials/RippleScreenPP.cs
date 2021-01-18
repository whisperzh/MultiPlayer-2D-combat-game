using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleScreenPP : PostProcessBase
{
    public Shader RippleScreenShader;
    private Material RippleScreenMaterial;
    public Material material
    {
        get
        {
            RippleScreenMaterial = CheckShaderAndCreateMatrerial(RippleScreenShader, RippleScreenMaterial);
            return RippleScreenMaterial;
        }
    }

    public Vector2 pos;

    public float MaxAmount = 50f;
    [Range(0, 1)]
    public float Friction = 0.9f;
    private float Amount = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            this.Amount = this.MaxAmount;
            Vector3 pos = Input.mousePosition;
            material.SetFloat("_CenterX", pos.x);
            material.SetFloat("_CenterY", pos.y);
        }
        material.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture des)
    {

        if (material != null)
        {
            Graphics.Blit(src, des, material, 0);
        }
        else
            Graphics.Blit(src, des);
    }
}
