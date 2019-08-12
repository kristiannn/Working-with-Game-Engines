using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerUtilityScript : MonoBehaviour
{

    string typeName = "lel";
    string gameName = "leel";
    HostData[] hostList;
    string ipAddress;

    public GameObject voxelChunkPrefab;
    public GameObject networkFPCPrefab;
    public GameObject menuButtons;
    public InputField ipAdrInput;

    void Start()
    {
        MasterServer.ipAddress = ipAddress;
    }

    public void StartServer()
    {
        if (!Network.isServer && !Network.isClient)
        {
            Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
            MasterServer.RegisterHost(typeName, gameName);
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initialized");

        Network.Instantiate(voxelChunkPrefab, Vector3.zero, Quaternion.identity, 0);

        menuButtons.SetActive(false);
        Debug.Log("Here");
    }

    public void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostList = MasterServer.PollHostList();

            foreach (HostData hd in hostList)
            {
                if (hd.gameName == gameName)
                {
                    Network.Connect(hd);
                }
            }
        }
    }

    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
        Network.Instantiate(networkFPCPrefab, new Vector3(8, 8, 8), Quaternion.identity, 0);
        menuButtons.SetActive(false);
    }
}