using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.IO.Compression;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class SecureSaveUtility
{
    private static readonly byte[] SecretKey16 = Encoding.UTF8.GetBytes("A2B4C6D8E0F1G3H5"); //16 Byte Key
    private static readonly byte[] SecretKey32 = Encoding.UTF8.GetBytes("3F5B2A8C7D1E9F0G4H6J8K9L2M1N3PQX"); //32 Byte Key

    public static byte[] CompressData(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using(GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }

    public static byte[] DecompressData(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            using(GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
            {
                using(MemoryStream output = new MemoryStream())
                {
                    gzip.CopyTo(output);
                    return output.ToArray();
                }
            }
        }
    }

    public static byte[] EncryptData(byte[] data)
    {
        using(Aes aes = Aes.Create())
        {
            aes.Key = SecretKey32;
            aes.IV = SecretKey16;

            using(MemoryStream ms = new MemoryStream())
            {
                using(CryptoStream cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
    }

    public static byte[] DecryptData(byte[] data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = SecretKey32;
            aes.IV = SecretKey16;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
    }

    public static void SaveToFile(string path, byte[] data)
    {
        byte[] compressedData = CompressData(data);
        byte[] encryptedData = EncryptData(compressedData);
        File.WriteAllBytes(path, encryptedData);
    }

    public static byte[] LoadFromFile(string path)
    {
        if(File.Exists(path) != true)
        {
            throw new FileNotFoundException($"Save File Not Found At {path}");
        }

        byte[] encryptedData = File.ReadAllBytes(path);
        byte[] decompressedData = DecryptData(encryptedData);
        return DecompressData(decompressedData);
    }
}

public static class SaveSerializer
{
    //File Info
    private const int FILE_VERSION = 1;
    private const string FILE_HEADER = "AGENT INFO";
    
    /// <summary> Serializes Save Data into a custom Binary Format, as the C# Binary Formatter is Obsolete</summary>
    public static byte[] SerializeData(SavedData data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using(BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(FILE_HEADER);
                writer.Write(FILE_VERSION);

                #region Data File

                writer.Write(data.fileName);
                writer.Write(data.modifiedDate);
                writer.Write(data.isAutoSaveFile);

                #endregion

                # region Player Data

                writer.Write(data.coinAmount);
                writer.Write(data.healthCount);

                //Player Position
                foreach(float value in data.playerPos)
                {
                    writer.Write(value);
                }
                //Player Rotation
                foreach (float value in data.playerRot)
                {
                    writer.Write(value);
                }
                #endregion

                #region Quest Data

                writer.Write(data.sceneIndex);
                writer.Write(data.questDataList.Count);
                foreach(SerializableQuestData quest in data.questDataList)
                {
                    writer.Write(quest.questName);
                    writer.Write(quest.objectiveProgressvalue.Count);

                    foreach(int value in quest.objectiveProgressvalue)
                    {
                        writer.Write(value);
                    }
                }

                #endregion

                return ms.ToArray();
            }
        }
    }

    public static SavedData Deserialize(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            using(BinaryReader reader = new BinaryReader(ms))
            {
                string header = reader.ReadString();
                if(header != FILE_HEADER)
                {
                    throw new Exception("Invalid Save File Format");
                }
                int fileVersion = reader.ReadInt32();

                #region Data File

                string fileName = reader.ReadString();
                string modifiedDate = reader.ReadString();
                bool isAutoSave = reader.ReadBoolean();

                #endregion

                # region Player Data

                int coinAmount = reader.ReadInt32();
                int healthCount = reader.ReadInt32();

                Vector3 position = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Quaternion rotation = new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                #endregion

                #region Quest Data

                int sceneIndex = reader.ReadInt32();
                int questCount = reader.ReadInt32();
                List<SerializableQuestData> questList = new();
                for(int i = 0; i < questCount; i++)
                {
                    string questname = reader.ReadString();
                    int progressValueCount = reader.ReadInt32();

                    List<int> objectProgressiveValues = new List<int>();
                    for(int x = 0; x < progressValueCount; x++)
                    {
                        int progressValue = reader.ReadInt32();
                        objectProgressiveValues.Add(progressValue);
                    }

                    SerializableQuestData newQuestData = new (questname, objectProgressiveValues);
                    questList.Add(newQuestData);
                }
                #endregion

                SavedData newData = new(sceneIndex, fileName, modifiedDate, isAutoSave)
                {
                    coinAmount = coinAmount,
                    healthCount = healthCount,
                    playerPosition = position,
                    playerRotation = rotation
                };
                newData.questDataList.Clear();
                newData.SetPlayerTransformValues();
                newData.questDataList.AddRange(questList);
                return newData;
            }
        }
    }
}
