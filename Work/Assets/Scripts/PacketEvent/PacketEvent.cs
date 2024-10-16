using System;
using System.Numerics;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.PacketEvent
{
    public class Event_Packet
    {
        EventDefine Event;
        public Event_Packet(EventDefine e)
        {
            Event = e;
        }
        public virtual byte[] GetBytes()
        {
            return null;
        }
    }
    public enum EventDefine : byte
    {
        Login,
        ChangeTurn,
        SetHp,
        SetPlayer,
        InstantiatePrefab,


        PlayerSyncTransform,
        MoveInput,
        LookInput,
        JumpInput,
        ThrowInput,
        SprintInput,
        ChargeInput,
    }
    public class Event_Login : Event_Packet
    {
        public EventDefine Event;
        public Event_Login()
            : base(EventDefine.Login)
        {
        }

        public static void GetDecode(Memory<byte> bytes)
        {

        }
        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;

            byte[] bytes = new byte[ProtocolSize];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.Login;
            return bytes;
        }
    }
    public class Event_SetPlayer : Event_Packet
    {
        public EventDefine Event;
        int player1ID; 
        int player2ID;


        public Event_SetPlayer(int player1ID, int player2ID)
            : base(EventDefine.SetPlayer)
        {
            this.player1ID = player1ID;
            this.player2ID = player2ID;
        }

        public static (int p1ID, int p2ID) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;

            int p1ID = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            int p2ID = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            return (p1ID, p2ID);
        }
        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int offset = 0;
            byte[] bytes = new byte[ProtocolSize + NetID_Length+ NetID_Length];
            bytes[offset] = (byte)EventDefine.SetPlayer;
            offset += ProtocolSize;
            BitConverter.GetBytes(player1ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            BitConverter.GetBytes(player2ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            return bytes;
        }
    }


    public class Event_SetHp : Event_Packet
    {
        public EventDefine Event;
        int NetID;
        float HP;

        public Event_SetHp(int id, float hp)
            : base(EventDefine.SetHp)
        {
            HP = hp;
            NetID = id;
        }

        public static (int id, float hp) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int HP_Length = 4;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;

            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;
            float hp = BitConverter.ToSingle(bytes.Slice(offset, NetID_Length).Span);
            offset += HP_Length;
            return (id,hp);
        }
        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int HP_Length = 4;

            int offset = 0;
            byte[] bytes = new byte[ProtocolSize + NetID_Length+ HP_Length];
            bytes[offset] = (byte)EventDefine.SetHp;
            offset += ProtocolSize;
            BitConverter.GetBytes(NetID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            BitConverter.GetBytes(HP).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            return bytes;
        }
    }
    public class Event_ChangeTurn : Event_Packet
    {
        public EventDefine Event;
        int TurnObjectNetID;

        public Event_ChangeTurn(int id)
            : base(EventDefine.ChangeTurn)
        {
            TurnObjectNetID  = id;
        }

        public static int GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;

            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            return id;
        }
        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int offset = 0;
            byte[] bytes = new byte[ProtocolSize+ NetID_Length];
            bytes[offset] = (byte)EventDefine.ChangeTurn;
            offset += ProtocolSize;
            BitConverter.GetBytes(TurnObjectNetID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            return bytes;
        }
    }

    public class Event_TansformSync : Event_Packet
    {
        public EventDefine Event;
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Quaternion;
        public int ID;
        public Event_TansformSync(int id, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn)
            : base(EventDefine.PlayerSyncTransform)
        {
            ID = id;
            Position = pos;
            Quaternion = Qtn;
        }

        public static (int ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion qtn) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            // Position 역직렬화
            int PosX_Length = 4;
            int PosY_Length = 4;
            int PosZ_Length = 4;

            float posX = BitConverter.ToSingle(bytes.Slice(offset, PosX_Length).Span);
            offset += PosX_Length;
            float posY = BitConverter.ToSingle(bytes.Slice(offset, PosY_Length).Span);
            offset += PosY_Length;
            float posZ = BitConverter.ToSingle(bytes.Slice(offset, PosZ_Length).Span);
            offset += PosZ_Length;
            UnityEngine.Vector3 position = new UnityEngine.Vector3(posX, posY, posZ);

            // Quaternion 역직렬화
            int QtnX_Length = 4;
            int QtnY_Length = 4;
            int QtnZ_Length = 4;
            int QtnW_Length = 4;

            float qtnX = BitConverter.ToSingle(bytes.Slice(offset, QtnX_Length).Span);
            offset += QtnX_Length;
            float qtnY = BitConverter.ToSingle(bytes.Slice(offset, QtnY_Length).Span);
            offset += QtnY_Length;
            float qtnZ = BitConverter.ToSingle(bytes.Slice(offset, QtnZ_Length).Span);
            offset += QtnZ_Length;
            float qtnW = BitConverter.ToSingle(bytes.Slice(offset, QtnW_Length).Span);
            offset += QtnW_Length;
            UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion(qtnX, qtnY, qtnZ, qtnW);
            // 결과를 반환
            return (id, position, quaternion);
        }
        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int PosX_Length = 4;
            int PosY_Length = 4;
            int PosZ_Length = 4;

            int QtnX_Length = 4;
            int QtnY_Length = 4;
            int QtnZ_Length = 4;
            int QtnW_Length = 4;

            byte[] bytes = new byte[ProtocolSize + NetID_Length + PosX_Length + PosY_Length + PosZ_Length +
                QtnX_Length + QtnY_Length + QtnZ_Length + QtnW_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.PlayerSyncTransform;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;

            BitConverter.GetBytes(Position.x).CopyTo(bytes.AsMemory(offset, PosX_Length));
            offset += PosX_Length;
            BitConverter.GetBytes(Position.y).CopyTo(bytes.AsMemory(offset, PosY_Length));
            offset += PosY_Length;
            BitConverter.GetBytes(Position.z).CopyTo(bytes.AsMemory(offset, PosZ_Length));
            offset += PosZ_Length;

            BitConverter.GetBytes(Quaternion.x).CopyTo(bytes.AsMemory(offset, QtnX_Length));
            offset += QtnX_Length;
            BitConverter.GetBytes(Quaternion.y).CopyTo(bytes.AsMemory(offset, QtnY_Length));
            offset += QtnY_Length;
            BitConverter.GetBytes(Quaternion.z).CopyTo(bytes.AsMemory(offset, QtnZ_Length));
            offset += QtnZ_Length;
            BitConverter.GetBytes(Quaternion.w).CopyTo(bytes.AsMemory(offset, QtnW_Length));
            offset += QtnW_Length;

            return bytes;
        }
    }
    public class Event_chargeInput : Event_Packet
    {
        public int ID;
        public bool charge;
        //public bool throwShoot;
        public Event_chargeInput(int id, bool charge)
           : base(EventDefine.ChargeInput)
        {
            this.ID = id;
            this.charge = charge;
        }
        public static (int ID, bool Throw) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;
            // Position 역직렬화
            int bool_Length = 1;
            bool charge = NetLibrary.Utils.Serializer.ToValue(bytes, offset, bool_Length) != 0;
            return (id, charge);
        }

        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int bool_Length = 1;

            byte[] bytes = new byte[ProtocolSize + NetID_Length + bool_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.ChargeInput;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            NetLibrary.Utils.Serializer.ToByte(charge ? 1 : 0, bool_Length).CopyTo(bytes.AsMemory().Slice(offset, bool_Length));
            return bytes;
        }
    }


    public class Event_ThrowInput : Event_Packet
    {
        public int ID;
        public bool Throw;
        public Event_ThrowInput(int id, bool Throw)
           : base(EventDefine.ThrowInput)
        {
            this.ID = id;
            this.Throw = Throw;
        }
        public static (int ID, bool Throw) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;
            // Position 역직렬화
            int bool_Length = 1;
            bool Throw = NetLibrary.Utils.Serializer.ToValue(bytes, offset, bool_Length) != 0;
            return (id, Throw);
        }

        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int bool_Length = 1;

            byte[] bytes = new byte[ProtocolSize + NetID_Length + bool_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.ThrowInput;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            NetLibrary.Utils.Serializer.ToByte(Throw ? 1 : 0, bool_Length).CopyTo(bytes.AsMemory().Slice(offset, bool_Length));
            return bytes;
        }
    }

    public class Event_sprintInput : Event_Packet
    {
        public int ID;
        public bool sprint;
        public Event_sprintInput(int id, bool sprint)
           : base(EventDefine.SprintInput)
        {
            this.ID = id;
            this.sprint = sprint;
        }
        public static (int ID, bool sprint) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;
            // Position 역직렬화
            int bool_Length = 1;
            bool jump = NetLibrary.Utils.Serializer.ToValue(bytes, offset, bool_Length) != 0;
            return (id, jump);
        }

        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int bool_Length = 1;

            byte[] bytes = new byte[ProtocolSize + NetID_Length + bool_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.SprintInput;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            NetLibrary.Utils.Serializer.ToByte(sprint ? 1 : 0, bool_Length).CopyTo(bytes.AsMemory().Slice(offset, bool_Length));
            return bytes;
        }
    }
    public class Event_JumpInput : Event_Packet
    {
        public int ID;
        public bool jump;
        public Event_JumpInput(int id, bool jump)
           : base(EventDefine.JumpInput)
        {
            this.ID = id;
            this.jump = jump;
        }
        public static (int ID, bool jump) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;
            // Position 역직렬화
            int bool_Length = 1;
            bool jump = NetLibrary.Utils.Serializer.ToValue(bytes, offset, bool_Length) != 0;
            return (id, jump);
        }

        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;
            int bool_Length = 1;

            byte[] bytes = new byte[ProtocolSize + NetID_Length + bool_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.JumpInput;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            NetLibrary.Utils.Serializer.ToByte(jump ? 1 : 0, bool_Length).CopyTo(bytes.AsMemory().Slice(offset, bool_Length));
            return bytes;
        }
    }

    public class Event_lookInput : Event_Packet
    {
        public int ID;
        public UnityEngine.Vector2 look;
        public Event_lookInput(int id, UnityEngine.Vector2 move)
           : base(EventDefine.LookInput)
        {
            this.ID = id;
            this.look = move;
        }
        public static (int ID, UnityEngine.Vector2 look) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            // Position 역직렬화
            int X_Length = 4;
            int Y_Length = 4;

            float x = BitConverter.ToSingle(bytes.Slice(offset, X_Length).Span);
            offset += X_Length;
            float y = BitConverter.ToSingle(bytes.Slice(offset, Y_Length).Span);
            offset += Y_Length;
            return (id, new UnityEngine.Vector2(x, y));
        }

        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int X_Length = 4;
            int Y_Length = 4;
            byte[] bytes = new byte[ProtocolSize + NetID_Length + X_Length + Y_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.LookInput;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            BitConverter.GetBytes(look.x).CopyTo(bytes.AsMemory(offset, X_Length));
            offset += X_Length;
            BitConverter.GetBytes(look.y).CopyTo(bytes.AsMemory(offset, Y_Length));
            offset += Y_Length;
            return bytes;
        }

    }

    public class Event_MoveInput : Event_Packet
    {
        public int ID;
        public UnityEngine.Vector2 move;
        public Event_MoveInput(int id, UnityEngine.Vector2 move)
           : base(EventDefine.MoveInput)
        {
            this.ID = id;
            this.move = move;
        }
        public static (int ID, UnityEngine.Vector2 move) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            // Position 역직렬화
            int X_Length = 4;
            int Y_Length = 4;

            float x = BitConverter.ToSingle(bytes.Slice(offset, X_Length).Span);
            offset += X_Length;
            float y = BitConverter.ToSingle(bytes.Slice(offset, Y_Length).Span);
            offset += Y_Length;
            return (id, new UnityEngine.Vector2(x, y));
        }

        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int X_Length = 4;
            int Y_Length = 4;
            byte[] bytes = new byte[ProtocolSize + NetID_Length + X_Length + Y_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.MoveInput;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;
            BitConverter.GetBytes(move.x).CopyTo(bytes.AsMemory(offset, X_Length));
            offset += X_Length;
            BitConverter.GetBytes(move.y).CopyTo(bytes.AsMemory(offset, Y_Length));
            offset += Y_Length;
            return bytes;
        }

    }


    public class Event_InstantiatePrefab : Event_Packet
    {
        public EventDefine Event;
        public UnityEngine.Vector3 Position;
        public UnityEngine.Quaternion Quaternion;
        public string Prefab;
        public int ID;
        public bool IsMine;
        public Event_InstantiatePrefab(int id, UnityEngine.Vector3 pos, UnityEngine.Quaternion Qtn, bool IsMine, string prefabName)
            : base(EventDefine.InstantiatePrefab)
        {
            ID = id;
            Position = pos;
            Quaternion = Qtn;
            Prefab = prefabName;
            this.IsMine = IsMine;
        }

        public static (int ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion qtn, bool isMine,string prefabName) GetDecode(Memory<byte> bytes)
        {
            int ProtocolSize = 1;
            int offset = 0;
            EventDefine eventDefine = (EventDefine)bytes.Span[offset];
            offset += ProtocolSize;
            int NetID_Length = 4;
            int id = BitConverter.ToInt32(bytes.Slice(offset, NetID_Length).Span);
            offset += NetID_Length;

            // Position 역직렬화
            int PosX_Length = 4;
            int PosY_Length = 4;
            int PosZ_Length = 4;

            float posX = BitConverter.ToSingle(bytes.Slice(offset, PosX_Length).Span);
            offset += PosX_Length;
            float posY = BitConverter.ToSingle(bytes.Slice(offset, PosY_Length).Span);
            offset += PosY_Length;
            float posZ = BitConverter.ToSingle(bytes.Slice(offset, PosZ_Length).Span);
            offset += PosZ_Length;
            UnityEngine.Vector3 position = new UnityEngine.Vector3(posX, posY, posZ);

            // Quaternion 역직렬화
            int QtnX_Length = 4;
            int QtnY_Length = 4;
            int QtnZ_Length = 4;
            int QtnW_Length = 4;

            float qtnX = BitConverter.ToSingle(bytes.Slice(offset, QtnX_Length).Span);
            offset += QtnX_Length;
            float qtnY = BitConverter.ToSingle(bytes.Slice(offset, QtnY_Length).Span);
            offset += QtnY_Length;
            float qtnZ = BitConverter.ToSingle(bytes.Slice(offset, QtnZ_Length).Span);
            offset += QtnZ_Length;
            float qtnW = BitConverter.ToSingle(bytes.Slice(offset, QtnW_Length).Span);
            offset += QtnW_Length;
            UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion(qtnX, qtnY, qtnZ, qtnW);


            int IsMineLength = 1;
            bool Mine = NetLibrary.Utils.Serializer.ToValue(bytes, offset, IsMineLength) != 0;

            offset += IsMineLength;
            int remainingBytes = bytes.Length - offset;

            string prefabName = System.Text.Encoding.UTF8.GetString(bytes.Slice(offset, remainingBytes).Span);




            // 결과를 반환
            return (id, position, quaternion, Mine, prefabName);
        }
        public override byte[] GetBytes()
        {
            int ProtocolSize = 1;
            int NetID_Length = 4;

            int PosX_Length = 4;
            int PosY_Length = 4;
            int PosZ_Length = 4;

            int QtnX_Length = 4;
            int QtnY_Length = 4;
            int QtnZ_Length = 4;
            int QtnW_Length = 4;

            int IsMineLength = 1;

            int Name_Length = Prefab.Length;
            byte[] bytes = new byte[ProtocolSize + NetID_Length + PosX_Length + PosY_Length + PosZ_Length +
                QtnX_Length + QtnY_Length + QtnZ_Length + QtnW_Length + IsMineLength+Name_Length];

            int offset = 0;
            bytes[offset] = (byte)EventDefine.InstantiatePrefab;
            offset += ProtocolSize;
            BitConverter.GetBytes(ID).CopyTo(bytes.AsMemory(offset, NetID_Length));
            offset += NetID_Length;

            BitConverter.GetBytes(Position.x).CopyTo(bytes.AsMemory(offset, PosX_Length));
            offset += PosX_Length;
            BitConverter.GetBytes(Position.y).CopyTo(bytes.AsMemory(offset, PosY_Length));
            offset += PosY_Length;
            BitConverter.GetBytes(Position.z).CopyTo(bytes.AsMemory(offset, PosZ_Length));
            offset += PosZ_Length;

            BitConverter.GetBytes(Quaternion.x).CopyTo(bytes.AsMemory(offset, QtnX_Length));
            offset += QtnX_Length;
            BitConverter.GetBytes(Quaternion.y).CopyTo(bytes.AsMemory(offset, QtnY_Length));
            offset += QtnY_Length;
            BitConverter.GetBytes(Quaternion.z).CopyTo(bytes.AsMemory(offset, QtnZ_Length));
            offset += QtnZ_Length;
            BitConverter.GetBytes(Quaternion.w).CopyTo(bytes.AsMemory(offset, QtnW_Length));
            offset += QtnW_Length;
            NetLibrary.Utils.Serializer.ToByte(IsMine ? 1 : 0, IsMineLength).CopyTo(bytes.AsMemory().Slice(offset, IsMineLength));
            offset += IsMineLength;
            Encoding.UTF8.GetBytes(Prefab).CopyTo(bytes.AsMemory(offset, Name_Length));
            return bytes;
        }
    }
}
