using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CharacterSelectManager : MonoBehaviour
{
    //public GameObject playerPrefab;
    public List<Player_UI> playersUI;

    const int maxPlayers = 4;


    void Start()
    {
        InputManager.OnDeviceDetached += OnDeviceDetached;
        //InputManager.OnActiveDeviceChanged += OnActiveDeviceChanged;
        for (int i = 0; i < 4; i++)
        {
            playersUI[i].ID = i;
        }
        foreach (Player_UI p in playersUI)
        {
            p.players = playersUI;
        }
    }

    void Update()
    {
        Debug.Log(InputManager.Devices.Count);
        var inputDevice = InputManager.ActiveDevice;

        for (int i = 0; i < 4; i++)
        {
            Player_Input.Devices[i] = playersUI[i].GetTheDeviceofThisCharacter();
        }
        switch (InputManager.Devices.Count) {
            case 1:
                playersUI[0].Getup();
                playersUI[1].Dormant();
                playersUI[2].Dormant();
                playersUI[3].Dormant();
                break;
            case 2:
                playersUI[1].Getup();
                playersUI[2].Dormant();
                playersUI[3].Dormant();
                break;
            case 3:
                playersUI[1].Getup();
                playersUI[2].Getup();
                playersUI[3].Dormant();
                break;
            case 4:
                playersUI[1].Getup();
                playersUI[2].Getup();
                playersUI[3].Getup();
                break;
            default:
                playersUI[0].Dormant();
                playersUI[1].Dormant();
                playersUI[2].Dormant();
                playersUI[3].Dormant();
                break;


        }

        if (JoinButtonWasPressedOnDevice(inputDevice))
        {
            if (FindPlayerUsingDevice(inputDevice) == null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (playersUI[i].Device == null)
                    {
                        playersUI[i].Device = inputDevice;
                        return;
                    }
                }
            }
        }
    }

    public void AllPlayersOff() {


    }

    public void OnePlayerOn() {


    }

    public void JumpToGameScene() {
        for (int i = 0; i < 4; i++)
        {
            Player_Input.Devices[i] = playersUI[i].GetTheDeviceofThisCharacter();
        }
        SceneManager.LoadScene("Map2");
       
    }

    //void OnActiveDeviceChanged(InputDevice inputDevice)
    //{
    //    if (inputDevice.Command.IsPressed)
    //        JumpToGameScene();
    //    foreach (Player_UI pu in players)
    //    {
    //        if (pu.Device==null)
    //        {
    //            pu.Device = inputDevice;
    //            return;
    //        }
    //    }
    //}

    void OnDeviceDetached(InputDevice inputDevice)
    {
        Debug.Log("detach");
    }

    /// <summary>
    /// 查找当前是否有玩家按下手柄上ABXY的四个按键
    /// </summary>
    /// <param name="inputDevice"></param>
    /// <returns>返回手柄上四个按键是否被按下的bool值</returns>
    bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
    {
        return inputDevice.AnyButtonIsPressed;
            //.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
    }

    /// <summary>
    /// 查找当前是否有输入参数以外的玩家正在使用手柄
    /// </summary>
    /// <param name="inputDevice"></param>
    /// <returns></returns>
    Player_UI FindPlayerUsingDevice(InputDevice inputDevice)
    {
        var playerCount = 4;
        for (var i = 0; i < playerCount; i++)
        {
            var player = playersUI[i];
            if (player.Device == inputDevice)
            {
                return player;
            }
        }
        return null;
    }
}
