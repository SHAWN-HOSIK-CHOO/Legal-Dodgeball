using Assets.Scripts.PacketEvent;
using NetLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UIVirtualButton;

namespace Server
{
    public class GameManager : MonoBehaviour
    {
        public List<GameObject> prefabLists = new List<GameObject>();
        private Dictionary<string, GameObject> prefabDicts = new Dictionary<string, GameObject>();

        public static GameManager instance { get; private set; } = null;

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
        }
        void Start()
        {
            for (int i = 0; i < prefabLists.Count; i++)
            {
                prefabDicts.TryAdd(prefabLists[i].name, prefabLists[i]);
            }
        }
        public Vector2 GetRandomPosition()
        {
            float x = UnityEngine.Random.Range(-5f, 5f);
            float y = UnityEngine.Random.Range(-5f, 5f);
            return new Vector2(x, y);
        }

        public void SendUser(EndUser user, Event_Packet e)
        {
            user.DefferedSend(e.GetBytes());
        }


        public void SendAllUser(Event_Packet e)
        {
            foreach (var kv in NetworkManager.instance.EndUsers)
            {
                kv.Value.DefferedSend(e.GetBytes());
                kv.Value.Dispatch();
            }
        }

        public void SendAllExceptUser(Event_Packet e, EndUser uesr)
        {
            foreach (var kv in NetworkManager.instance.EndUsers)
            {
                if (kv.Value != uesr)
                {
                    kv.Value.DefferedSend(e.GetBytes());
                    kv.Value.Dispatch();
                }
            }
        }

        void ProcEvent(Memory<byte> e, EndUser user)
        {
            EventDefine type = (EventDefine)e.Span[0];
            switch (type)
            {
                case EventDefine.Login:
                    {
                        // 유니티는 싱글 스레드 이기 때문에 세션을 만들기 보다는
                        // 인스턴스용 서버를 따로 두는게 맞는 설계 인듯.

                        //하나의 인스턴스라고 가정하고 Login시 새 유저가 들어 왔음으로 
                        // 임의의 위치에 인스턴스를 생성해준다.
                        Vector2 RandomPos = GetRandomPosition();
                        Vector3 NewPos = new Vector3(RandomPos.x, 5, RandomPos.y);
                        GameObject prefab = prefabDicts["SPlayerGroup"];
                        GameObject NetObject = Instantiate(prefab, NewPos, Quaternion.identity);
                        int NetID = NetworkManager.instance.AllocNetObjectID();
                        var view = NetObject.GetComponent<NetViewer>();
                        view.NetID = NetID;
                        view.user = user;
                        view.IsMine = false;
                        view.prefabName = "SPlayerGroup";
                        NetworkManager.instance.NetObjects.Add(NetID, view);

                        // 기존에 존재하는 넷오브젝트를 해당 유저에게 제공해야 한다.
                        foreach (var kv in NetworkManager.instance.NetObjects)
                        {
                            NetViewer Oldview = kv.Value;
                            if (Oldview != view)
                            {
                                var InstantiateEvent = new Event_InstantiatePrefab(kv.Key, Oldview.transform.position, Oldview.transform.rotation, false, Oldview.prefabName);
                                SendUser(user, InstantiateEvent);
                            }
                        }
                        // 새로운 프리팹을 모두에게 제공한다.
                        foreach (var kv in NetworkManager.instance.EndUsers)
                        {
                            Event_InstantiatePrefab NetEvent;
                            if (kv.Value != user)
                            {
                                NetEvent = new Event_InstantiatePrefab(NetID, NewPos, Quaternion.identity, false, "SPlayerGroup");
                            }
                            else
                            {
                                NetEvent = new Event_InstantiatePrefab(NetID, NewPos, Quaternion.identity, true, "CPlayerGroup");
                            }
                            kv.Value.DefferedSend(NetEvent.GetBytes());
                        }
                        Debug.Log($"SendAllUser Event_InstantiatePrefab NetID {NetID}");
                    }
                    break;
                case EventDefine.JumpInput:
                    {
                        (int id, bool jump) = Event_JumpInput.GetDecode(e);
                        var JumpEvent = new Event_JumpInput(id, jump);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().JumpInput(jump);
                        }
                        SendAllExceptUser(JumpEvent, user);
                    }
                    break;
                case EventDefine.SprintInput:
                    {
                        (int id, bool sprint) = Event_sprintInput.GetDecode(e);
                        var SprintEvent = new Event_sprintInput(id, sprint);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().SprintInput(sprint);
                            SendAllExceptUser(SprintEvent, user);
                        }
                    }
                    break;
                case EventDefine.LookInput:
                    {
                        (int id, UnityEngine.Vector2 look) = Event_lookInput.GetDecode(e);
                        var LookEvent = new Event_lookInput(id, look);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().LookInput(look);
                            SendAllExceptUser(LookEvent, user);
                        }
                    }
                    break;

                case EventDefine.MoveInput:
                    {
                        (int id, UnityEngine.Vector2 move) = Event_MoveInput.GetDecode(e);
                        var moveEvent = new Event_lookInput(id, move);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().MoveInput(move);
                            SendAllExceptUser(moveEvent, user);
                        }
                    }
                    break;

                case EventDefine.PlayerSyncTransform:
                    {
                        (int id, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn) = Event_TansformSync.GetDecode(e);
                        var TansformSyncEvent = new Event_lookInput(id, pos);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            var controller = unityobj.GetComponent<CharacterController>();
                            controller.enabled = false;
                            unityobj.transform.position = Vector3.Lerp(unityobj.transform.position, pos, Mathf.Clamp01(3 * Time.deltaTime));
                            unityobj.CinemachineCameraTarget.transform.rotation = Qtn;
                            controller.enabled = true;
                            SendAllExceptUser(TansformSyncEvent, user);
                        }
                    }
                    break;
            }
        }

        void Update()
        {
            foreach (var kv in NetworkManager.instance.EndUsers)
            {
                while (kv.Value.PacketCompleteQueue.TryDequeue(out var e))
                {
                    ProcEvent(e, kv.Value);
                }
                kv.Value.Dispatch();
            }
        }
    }
}