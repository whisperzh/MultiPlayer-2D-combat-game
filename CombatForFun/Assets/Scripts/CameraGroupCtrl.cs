using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraGroupCtrl : MonoBehaviour
{
    private CinemachineTargetGroup cgroup;
    private List<Player_Controller> playerList;
    // Start is called before the first frame update
    void Start()
    {
        cgroup = GetComponent<CinemachineTargetGroup>();
        playerList = FindObjectOfType<Player_Input>().attachedPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Player_Controller player in playerList)
        {
            if(player.gameObject.activeSelf == false)
            {
                cgroup.m_Targets[playerList.IndexOf(player)].weight = 0;
            }
            else
            {
                cgroup.m_Targets[playerList.IndexOf(player)].weight = 1f;
            }
        }
    }
}
