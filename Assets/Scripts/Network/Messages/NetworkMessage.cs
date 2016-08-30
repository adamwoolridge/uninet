using UnityEngine;
using System.Collections;
using System.IO;

public class NetworkMessage
{
    public NetworkMessageType MessageType;

    BinaryWriter writer;
    BinaryReader reader;
    MemoryStream stream;

    public NetworkMessage(NetworkMessageType type)
    {        
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);

        MessageType = type;
        writer.Write((byte)MessageType);
    }

    public NetworkMessage(byte[] bytes)
    {
        stream = new MemoryStream(bytes);
        reader = new BinaryReader(stream);
        MessageType = (NetworkMessageType)reader.ReadByte();                           
    }

    public byte[] GetData()
    {        
        return stream.ToArray();
    }

    // Write functions
    public void Write(uint i)
    {
        writer.Write(i);
    }

    public void Write(Vector3 v)
    {
        writer.Write(v.x); writer.Write(v.y); writer.Write(v.z);
    }

    public void Write(Quaternion q)
    {
        writer.Write(q.x); writer.Write(q.y); writer.Write(q.z); writer.Write(q.w);
    }

    public void Write(string s)
    {
        writer.Write(s);
    }

    // Read functions
    public uint ReadUInt()
    {
        return reader.ReadUInt32();
    }

    public Vector3 ReadVector3()
    {        
        return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public Quaternion ReadQuaternion()
    {
        return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public string ReadString()
    {
        return reader.ReadString();
    }
}
