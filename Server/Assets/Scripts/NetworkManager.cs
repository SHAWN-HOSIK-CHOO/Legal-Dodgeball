using NetLibrary;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance { get; private set; } = null;

    public string InternetProtocol = "192.168.0.38";
    public int Port = 8000;
    private static NetLibrary.Network netWork = null;
    private Task SyncTask = null;
    private CancellationTokenSource SyncTaskTokenSource = null;
    private CancellationToken syncTaskToken;
    public ConcurrentQueue<EndUser> NewSyncUsers = new ConcurrentQueue<EndUser>();

    static int NetObjectID = 0;

    public int AllocNetObjectID()
    {
        // GUID 가 필요하지만 일단 이렇게 하자.
        NetObjectID++;
        return NetObjectID;
    }

    private void Awake()
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
        SyncTaskTokenSource = new CancellationTokenSource();
        syncTaskToken = SyncTaskTokenSource.Token;
        netWork = new NetLibrary.Network(new IPEndPoint(IPAddress.Parse(InternetProtocol), Port), 255);
        NewSyncUsers.Clear();
        SyncTask = Task.Run(async () =>
        {
            Debug.Log("Server SyncTask Start");
            while (!syncTaskToken.IsCancellationRequested)
            {
                var (success, user) = await netWork?.WaitSyncRequest(-1);
                if (success)
                {
                    Debug.Log($"New Sync! {user.RemoteEndPoint} : {user.SyncID}");
                    NewSyncUsers.Enqueue(user);
                }
            }
            Debug.Log("Server SyncTask End");
        });
    }

    void Start()
    {

    }

    void Update()
    {

    }


    private void OnDisable()
    {
        netWork?.Dispose();
        SyncTaskTokenSource?.Cancel();
        SyncTask.Wait();
        SyncTaskTokenSource?.Dispose();
        SyncTask?.Dispose();
        Destroy(this);
    }
}
