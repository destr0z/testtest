﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private const string typeName = "DummyServer";
    private const string gameName = "Stupid room";

    private bool isRefreshingHostList = false;
    private HostData[] hostList;

    public GameObject playerPrefab;

    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
                }
            }
        }
    }

    private void StartServer()
    {
        Network.InitializeServer(5, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnServerInitialized()
    {
        SpawnPlayer();
    }


    void Update()
    {
        if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
        {
            isRefreshingHostList = false;
            hostList = MasterServer.PollHostList();
        }
    }

    private void RefreshHostList()
    {
        if (!isRefreshingHostList)
        {
            isRefreshingHostList = true;
            MasterServer.RequestHostList(typeName);
        }
    }


    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
        var player = Network.Instantiate(playerPrefab, Vector3.up * 5, Quaternion.identity, 0);

        //Camera.main.GetComponent<MouseAimCamera>().target = player as GameObject;
        //Camera.main.GetComponent<MouseAimCamera>().enabled = true;
		//Camera.main.GetComponent<DungeonCamera>().target = player as GameObject;
		//Camera.main.GetComponent<DungeonCamera>().enabled = true;
		Camera.main.GetComponent<FollowCamera>().target = player as GameObject;
		Camera.main.GetComponent<MouseAimCamera>().target = player as GameObject;
		Camera.main.GetComponent<FollowCamera>().enabled = true;
		//Camera.main.GetComponent<MouseAimCamera>().enabled = true;

    }
}
