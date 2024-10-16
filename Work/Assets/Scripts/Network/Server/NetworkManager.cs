using NetLibrary;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager instance { get; private set; } = null;

        public string InternetProtocol = "192.168.0.38";
        public int Port = 8000;
        private static NetLibrary.Network netWork = null;
        private Task SyncTask = null;
        private CancellationTokenSource SyncTaskTokenSource = null;
        private CancellationToken syncTaskToken;


        public Dictionary<IPEndPoint, EndUser> EndUsers = new Dictionary<IPEndPoint, EndUser>();
        public Dictionary<int, NetViewer> NetObjects = new Dictionary<int, NetViewer>();

        static int NetObjectID = 0;

        public int AllocNetObjectID()
        {
            // GUID 가 필요하지만 일단 이렇게 하자.
            NetObjectID++;
            return NetObjectID;
        }

        private void Awake()
        {
            //NetLibrary.DefineFlag.LogEnable = false;
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
            EndUsers.Clear();
            NetObjects.Clear();
            SyncTask = Task.Run(async () =>
            {
                Debug.Log("Server SyncTask Start");
                while (!syncTaskToken.IsCancellationRequested)
                {
                    var (success, user) = await netWork?.WaitSyncRequest(-1);
                    if (success)
                    {
                        if (!EndUsers.TryGetValue(user.RemoteEndPoint, out var _))
                        {
                            EndUsers.Add(user.RemoteEndPoint, user);
                            Debug.Log($"New User! {user.RemoteEndPoint} : {user.SyncID}");
                        }
                        else
                        {
                            Debug.Log($"Old User.. {user.RemoteEndPoint} : {user.SyncID}");
                        }
                    }
                }
                Debug.Log("Server SyncTask End");
            });
        }


        private void Update()
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
}