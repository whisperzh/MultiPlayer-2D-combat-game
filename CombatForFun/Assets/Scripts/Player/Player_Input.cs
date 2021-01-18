using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Input: MonoBehaviour
{
    //public GameObject playerPrefab;
    public List<Player_Controller> attachedPlayers;
    public static InputDevice[] Devices=new InputDevice[4];
    public static int _remainPlayers = 0;
    public Vector3[] playerOriginalPos;
    public Animator WinSlogan;
    public LayerMask cullingMask;

    const int maxPlayers = 4;

    void Start()
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            playerOriginalPos[i] = attachedPlayers[i].transform.position;
        }
        SetInputToPlayer();   
    }


    void Update()
    {
        if (IfOktoJumpBackToSelectMenu())
            SceneManager.LoadScene("SelectingCharacter");
        SetInputToPlayer();

        var inputDevice = InputManager.ActiveDevice;

        if (JoinButtonWasPressedOnDevice(inputDevice))
        {
            if (FindPlayerUsingDevice(inputDevice) == null)
            {
                HelpDeviceFindPlayer(inputDevice);
            }
        }
    }

    public bool IfOktoJumpBackToSelectMenu() {
        if (_remainPlayers==1)
            foreach(var i in Devices)
                if(i!=null&&i.Command.IsPressed)
                    return true;
        return false;
    }

    public void SetInputToPlayer()
    {
        InputManager.OnDeviceDetached += OnDeviceDetached;
        for (int i = 0; i < 4; i++)
        {
            if (Devices[i] == null)
            {
                attachedPlayers[i].ResetPlayerSprite();
                attachedPlayers[i].gameObject.SetActive(false);
                attachedPlayers[i].transform.position = playerOriginalPos[i];
            }
            else
            {
                attachedPlayers[i].gameObject.SetActive(true);
                attachedPlayers[i].device = Devices[i];
                attachedPlayers[i].weaponControl.device = Devices[i];
                attachedPlayers[i].memoryDevice = Devices[i];
            }
        }
    }

    /// <summary>
    /// 查找当前是否有玩家按下手柄上ABXY的四个按键
    /// </summary>
    /// <param name="inputDevice"></param>
    /// <returns>返回手柄上四个按键是否被按下的bool值</returns>
    bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
    {
        return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
    }

    /// <summary>
    /// 查找当前是否有输入参数以外的玩家正在使用手柄
    /// </summary>
    /// <param name="inputDevice"></param>
    /// <returns></returns>
    Player_Controller FindPlayerUsingDevice(InputDevice inputDevice)
    {
        var playerCount = attachedPlayers.Count;
        for (var i = 0; i < playerCount; i++)
        {
            var player = attachedPlayers[i];
            if (player.device == inputDevice)
            {
                return player;
            }
        }
        return null;
    }

    void OnDeviceDetached(InputDevice inputDevice)
    {  
        RemovePlayer(inputDevice);
    }

    void HelpDeviceFindPlayer(InputDevice inputDevice) {
        foreach (Player_Controller p in attachedPlayers)
        {
#if false
            if (p.DeviceserialNumber == inputDevice.GUID)
            {
                p.Device = inputDevice;
                return;
            }

#else 
            if (p.memoryDevice == inputDevice)
            {
                p.device = inputDevice;
                p.weaponControl.device = inputDevice;
                return;
            }
#endif
        }
        for (int i = 0; i < 4; i++)
        {
#if false
            if (attachedPlayers[i].Device == null)
            {
                attachedPlayers[i].Device = inputDevice;
                attachedPlayers[i].DeviceserialNumber = inputDevice.GUID;
                return;
            }
#else
            if (attachedPlayers[i].device == null)
            {
                attachedPlayers[i].device = inputDevice;
                attachedPlayers[i].weaponControl.device = inputDevice;
                attachedPlayers[i].memoryDevice = inputDevice;
                return;
            }
#endif
        }
        
    }


    void RemovePlayer(InputDevice player)
    {
        foreach (Player_Controller p in attachedPlayers)
        {
#if false
            if (p.DeviceserialNumber == player.GUID)
            {
                p.Device = null;
                return;
            }
#else
            if (p.memoryDevice == player)
            {
                p.device = null;
                p.weaponControl.device = null;
                return;
            }
#endif
        }

    }

    void OnGUI()
    {
        const float h = 22.0f;
        var y = 10.0f;

        GUI.Label(new Rect(10, y, 300, y + h), "Active players: " + attachedPlayers.Count + "/" + maxPlayers);
        y += h;

        if (attachedPlayers.Count < maxPlayers)
        {
            GUI.Label(new Rect(10, y, 300, y + h), "Press a button to join!");
            y += h;
        }
    }

    public  void PlayerDeclaredDearth(InputDevice inputDevice)
    {
        _remainPlayers = 0;
        for (int i = 0; i < 4; i++)
        {
            if (Devices[i] == inputDevice)
            {
                Devices[i] = null;
                break;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (Devices[i]!=null)
            {
                _remainPlayers++;
            }
        }
        if (_remainPlayers == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Devices[i] != null)
                {
                    WinSlogan.gameObject.SetActive(true);
                    WinSlogan.SetTrigger("p"+(i+1).ToString());
                    //winner is devices[i]
                    Camera.main.backgroundColor = new Color(0, 0, 0, 1);
                    Camera.main.cullingMask = cullingMask;

                }
            }
            //游戏结束
        }
    }

}
