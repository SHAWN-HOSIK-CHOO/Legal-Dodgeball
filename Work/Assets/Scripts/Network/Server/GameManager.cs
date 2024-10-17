using Assets.Scripts.PacketEvent;
using NetLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Server
{
    public class GameManager : Assets.Scripts.Network.GameManager
    {
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
            DefineFlag.LogEnable = false;
            switch (type)
            {
                case EventDefine.Login:
                    {
                        Debug.Log("Receive Login");
                        // ����Ƽ�� �̱� ������ �̱� ������ ������ ����� ���ٴ�
                        // �ν��Ͻ��� ������ ���� �δ°� �´� ���� �ε�.

                        //�ϳ��� �ν��Ͻ���� �����ϰ� Login�� �� ������ ��� �������� 
                        // ������ �����ϴ� �ݿ�����Ʈ�� �ش� �������� �����ؾ� �Ѵ�.
                        foreach (var kv in NetworkManager.instance.NetObjects)
                        {
                            NetViewer NetView = kv.Value;
                            var InstantiateEvent = new Event_InstantiatePrefab(kv.Key, NetView.transform.position, NetView.transform.rotation, user == NetView.user, NetView.prefabName);
                            SendUser(user, InstantiateEvent);
                        }

                        // ������ ��ġ�� �ν��Ͻ��� �������ش�.
                        Vector2 RandomPos = GetRandomPosition();
                        Vector3 NewPos = new Vector3(RandomPos.x, 5, RandomPos.y);
                        GameObject prefab = prefabDicts["SPlayerGroup"];
                        GameObject NetObject = Instantiate(prefab, NewPos, Quaternion.identity);
                        var view = NetObject.GetComponentInChildren<NetViewer>();
                        int NetID = NetworkManager.instance.AllocNetObjectID();
                        view.NetID = NetID;
                        view.user = user;
                        view.IsMine = false;
                        view.prefabName = "SPlayerGroup";
                        NetworkManager.instance.NetObjects.Add(NetID, view);

                        // ���ο� �������� ��ο��� �����Ѵ�.
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


                        if (GManager.Instance.Players[0] == null)
                        {
                            GManager.Instance.Players[0] = view.gameObject;
                            Debug.Log($"SET PLAYER 1  NetID {view.NetID}");

                        }
                        else if (GManager.Instance.Players[1] == null)
                        {
                            GManager.Instance.Players[1] = view.gameObject;
                            Debug.Log($"SET PLAYER 2  NetID {view.NetID}");
                        }

                        // �÷��̾ �����Ѵ�.
                        if (GManager.Instance.Players[0] == null || GManager.Instance.Players[1] == null) return;
                        GManager.Instance.TurnPlayer = GManager.Instance.Players[0];

                        var SetPlayer_Event = new Event_SetPlayer(GManager.Instance.Players[0].GetComponent<NetViewer>().NetID,
                            GManager.Instance.Players[1].GetComponent<NetViewer>().NetID);
                        SendAllUser(SetPlayer_Event);
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
                case EventDefine.ChargeInput:
                    {
                        (int id, bool charge) = Event_chargeInput.GetDecode(e);
                        var ChargeEvent = new Event_chargeInput(id, charge);
                        if (NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Player unityobj = obj.GetComponent<Player>();
                            unityobj.GetComponent<NetPlayerInput>().ChargeInput(charge);
                            SendAllExceptUser(ChargeEvent, user);
                        }
                    }
                    break;
                case EventDefine.ThrowInput:
                    {
                        (int id, bool Throw) = Event_ThrowInput.GetDecode(e);
                        var ThrowEvent = new Event_ThrowInput(id, Throw);
                        if (Throw && NetworkManager.instance.NetObjects.TryGetValue(id, out var obj))
                        {
                            Debug.Log($"Someone try to Throw");


                            if (GManager.Instance.TurnPlayer != obj.gameObject) return;

                            if (obj.gameObject == GManager.Instance.Players[0])
                            {
                                Debug.Log($"Player1 Turn!");
                            }
                            else if (obj.gameObject == GManager.Instance.Players[1])
                            {
                                Debug.Log($"Player2 Turn!");
                            }

                            Assets.Scripts.Network.Player unityobj = obj.GetComponent<Assets.Scripts.Network.Player>();
                            unityobj.GetComponent<NetPlayerInput>().ThrowInput(Throw);
                            unityobj.GetComponent<ThrowBallController>().AttackShootAction();
                            SendAllUser(ThrowEvent);
                            GManager.Instance.ChangeTurn();
                        }
                    }
                    break;
                case EventDefine.MoveInput:
                    {
                        (int id, UnityEngine.Vector2 move) = Event_MoveInput.GetDecode(e);
                        var moveEvent = new Event_MoveInput(id, move);
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
                        var TansformSyncEvent = new Event_TansformSync(id, pos , Qtn);
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

            //�� ��ȣ�� �����ش�.


        }
    }
}