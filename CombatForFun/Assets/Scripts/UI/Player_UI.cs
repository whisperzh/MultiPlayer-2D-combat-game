using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
public class Player_UI : MonoBehaviour
{
    public Image[] mImage;
    public Color SelectColor;
    public Animator PlayerSpawn;
    [HideInInspector] public Text SelectInfo;
    [HideInInspector] public InputDevice Device { get; set; }
    [HideInInspector] public int whoChoseme = -1;
    [HideInInspector] public int ID;
    [HideInInspector] public int NowChoosing = 0;
    [HideInInspector] public bool Locked = false;
    [HideInInspector] public bool playerHaveChosen = false;
    [HideInInspector] public List<Player_UI> players;
    private bool _needClearState = false;
    private bool _buttonPressed = false;

    void Start()
    {
        whoChoseme = -1;
        SelectInfo = GetComponentInChildren<Text>();
        SelectInfo.text = "NULL";
        ImageOff();
        ImageOn(0);
        
    }

    void Update()
    {  
        if (Device!=null)
        {
            if (Device.Action1.IsPressed)
            {
                if (!players[NowChoosing].Locked)//进入选择s
                {
                    playerHaveChosen = true;
                    players[NowChoosing].PlayerSpawn.SetTrigger("Open");
                    players[ID].mImage[NowChoosing].color = players[NowChoosing].SelectColor;
                    players[NowChoosing].Locked = true;
                    players[NowChoosing].ShowLockInfo(ID);
                }
            }
            else if (Device.Action2.IsPressed)//取消选择
            {
                if (playerHaveChosen)//进入选择
                {
                    playerHaveChosen = false;
                    players[NowChoosing].PlayerSpawn.SetTrigger("Close");
                    players[ID].mImage[NowChoosing].color = Color.white;
                    players[NowChoosing].Locked = false;
                    players[NowChoosing].ShowLockInfo(-1);
                }
            }
            else
            {
                if (!playerHaveChosen)
                {
                    if (Device.DPadLeft.WasReleased || Device.DPadRight.WasReleased)
                        _buttonPressed = false;
                    if (Device.DPadLeft.IsPressed && !_buttonPressed)
                    {
                        _buttonPressed = true;
                        do
                        {
                            NowChoosing -= 1;
                            NowChoosing = (NowChoosing + 4) % 4;
                        } while (players[NowChoosing].Locked);
               
                        Choose();
                    }
                    else if (Device.DPadRight.IsPressed && !_buttonPressed)
                    {
                        _buttonPressed = true;
                        do
                        {
                            NowChoosing += 1;
                            NowChoosing = NowChoosing % 4;
                        } while (players[NowChoosing].Locked);
                        Choose();
                    }
                }
            }
        }
    }

    public void ShowLockInfo(int seq)
    {
        if (seq != -1)
        {
            SelectInfo.text = "P" + (seq+1).ToString();
            whoChoseme = seq;
        }
        else
        {
            whoChoseme = -1;
            SelectInfo.text = "NULL";
        }
    }

    public void Choose() {
        ImageOff();
        ImageOn(NowChoosing);
    }

    public void ImageOn(int seq)
    {
        mImage[seq].enabled = true;
    }

    public void ImageOff()
    {
        foreach (Image i in mImage)
        {
            i.enabled = false;
        }
    }

    public InputDevice GetTheDeviceofThisCharacter() {

        if (whoChoseme != -1)
        {
            return players[whoChoseme].Device;
        }
        else return null;
    }

    public void Dormant() {
        //Player_Input._remainPlayers -= 1;
        ImageOff();
    }

    public void Getup() {
        //Player_Input._remainPlayers += 1;
        ImageOff();
        ImageOn(NowChoosing);
    }
}