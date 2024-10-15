using NetLibrary;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEditor.PackageManager;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance { get; private set; } = null;

    public string RemoteIP = "192.168.0.38";
    public int RemotePort = 8000;
    public string LocalIP = "192.168.0.38";
    public int LocalPort = 0;
    

    private static NetLibrary.Network netWork = null;
    public ConcurrentQueue<EndUser> NewSyncUsers = new ConcurrentQueue<EndUser>();

    private async void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);

        IPEndPoint ServerAddress = new IPEndPoint(IPAddress.Parse(RemoteIP).MapToIPv6(), RemotePort);
        IPEndPoint LoaclAddress = new IPEndPoint(IPAddress.Parse(LocalIP).MapToIPv6(), LocalPort);
        NetLibrary.Network client = new NetLibrary.Network(LoaclAddress, 255);
        bool Success = client.CreateEndUser(ServerAddress, SessionType.RUDP, out var user);

        bool MightBeSuccess = await user.SyncEndUser(1000);
        if(!MightBeSuccess)
        {
            Debug.Log("ΩÃ≈© Ω«∆–");
        }
        else
        {
            NewSyncUsers.Enqueue(user);
        }
    }

    private void OnDisable()
    {
        netWork?.Dispose();
        Destroy(this);
    }
}
