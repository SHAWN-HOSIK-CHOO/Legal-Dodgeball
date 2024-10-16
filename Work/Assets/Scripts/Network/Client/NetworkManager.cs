using Assets.Scripts.PacketEvent;
using NetLibrary;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
namespace Client
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager instance { get; private set; } = null;

        public string RemoteIP = "192.168.0.38";
        public int RemotePort = 8000;
        public string LocalIP = "192.168.0.38";
        public int LocalPort = 0;


        public EndUser EndUser;
        public Dictionary<int, NetViewer> NetObjects = new Dictionary<int, NetViewer>();

        private static NetLibrary.Network netWork = null;

        private async void Awake()
        {
            // 비동기 함수의 생명주기에 주의할것
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
            if (!MightBeSuccess)
            {
                Debug.Log("싱크 실패");
                EndUser = null;
            }
            else
            {
                Debug.Log($"New Sync{user.SyncID}");
                EndUser = user;

                var e = new Event_Login();
                EndUser.DefferedSend(e.GetBytes());
            }
        }

        private void OnDisable()
        {
            netWork?.Dispose();
            Destroy(this);
        }
    }
}