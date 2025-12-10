using System.IO;
using UnityEngine;

public interface ISaveable
{
    public ObjectSaveData GetSaveData();
    public void UpdateSavedData();
    public void ReloadDataFromSavedFile(ObjectSaveData saveData);
}

[System.Serializable]
public class ObjectSaveData
{
    public string name;
    public string type_ID;
    public string persistent_ID;
    protected float[] objectPosArray;
    protected float[] objectRotArray;

    public bool SwitchStatus { get; private set; }
    public Vector3 ObjectPosition { get; protected set; }
    public Quaternion ObjectRotation { get; protected set; }

    public ObjectSaveData()
    {
        type_ID = "Object";
        objectPosArray = new float[3];
        objectRotArray = new float[4];
        persistent_ID = System.Guid.NewGuid().ToString();
    }

    public ObjectSaveData(ObjectSaveData data)
    {
        SwitchStatus = data.SwitchStatus;
        ObjectPosition = data.ObjectPosition;
        ObjectRotation = data.ObjectRotation;
    }

    public virtual void WriteToSavedData(BinaryWriter binaryWriter)
    {
        binaryWriter.Write(type_ID);
        binaryWriter.Write(persistent_ID);

        binaryWriter.Write(name);
        foreach (float value in objectPosArray)
        {
            binaryWriter.Write(value);
        }
        foreach(float value in objectRotArray)
        {
            binaryWriter.Write(value);
        }
        binaryWriter.Write(SwitchStatus);
    }

    public virtual void ReadFromSavedData(BinaryReader binaryReader)
    {
        type_ID = binaryReader.ReadString();
        persistent_ID = binaryReader.ReadString();

        name = binaryReader.ReadString();
        ObjectPosition = new(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        ObjectRotation = new(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        SwitchStatus = binaryReader.ReadBoolean();
    }

    public void UpdateSaveData(Vector3 pos, Quaternion rot, bool status)
    {
        SwitchStatus = status;
        objectPosArray = new float[] { pos.x, pos.y, pos.z };
        objectRotArray = new float[] { rot.x, rot.y, rot.z, rot.w };
    }
}

[System.Serializable]
public class CharacterSaveData: ObjectSaveData
{
    public int coinAmount;
    public int healthCount;

    public CharacterSaveData()
    {
        type_ID = "Character";
        objectPosArray = new float[3];
        objectRotArray = new float[4];
        persistent_ID = System.Guid.NewGuid().ToString();
    }

    public CharacterSaveData(CharacterSaveData data)
    {
        coinAmount = data.coinAmount;
        healthCount = data.healthCount;
        ObjectPosition = data.ObjectPosition;
        ObjectRotation = data.ObjectRotation;
    }

    public override void WriteToSavedData(BinaryWriter binaryWriter)
    {
        base.WriteToSavedData(binaryWriter);
        binaryWriter.Write(coinAmount);
        binaryWriter.Write(healthCount);
    }

    public override void ReadFromSavedData(BinaryReader binaryReader)
    {
        base.ReadFromSavedData(binaryReader);
        coinAmount = binaryReader.ReadInt32();
        healthCount = binaryReader.ReadInt32();
    }

    public void UpdateSaveData(int coin, int health, Vector3 pos, Quaternion rot)
    {
        coinAmount = coin;
        healthCount = health;
        UpdateSaveData(pos, rot, false);
    }
}