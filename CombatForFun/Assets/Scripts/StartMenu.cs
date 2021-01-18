using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using System;

public class StartMenu : MonoBehaviour
{
    public SpriteRenderer[] _spr;
    public String JumptoSceneName;
    float x = 0;
    // Start is called before the first frame update
    void Start()
    {
        InputManager.OnActiveDeviceChanged += OnActiveDeviceChanged;
    }

    private void OnActiveDeviceChanged(InputDevice obj)
    {
        if (obj.Action1.IsPressed)
            SceneManager.LoadScene(JumptoSceneName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        foreach (SpriteRenderer s in _spr)
        {

            x+=Time.deltaTime;
            float a = Mathf.Abs(Mathf.Sin(x));
            s.color = new Color(s.color.r, s.color.g, s.color.b, a);
        }
    }
    


}
