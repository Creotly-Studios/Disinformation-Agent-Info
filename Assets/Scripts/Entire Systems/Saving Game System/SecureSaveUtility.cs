using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class SecureSaveUtility
{
    private static readonly byte[] SecretKey16 = Encoding.UTF8.GetBytes("A2B4C6D8E0F1G3H5"); //16 Byte Key
    private static readonly byte[] SecretKey32 = Encoding.UTF8.GetBytes("3F5B2A8C7D1E9F0G4H6J8K9L2M1N3PQX"); //32 Byte Key

    public static byte[] CompressData(byte[] data)
    {
        using (MemoryStream ms = new())
        {
            using(GZipStream gzip = new(ms, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }

    public static byte[] DecompressData(byte[] data)
    {
        using MemoryStream ms = new(data);
        using GZipStream gzip = new(ms, CompressionMode.Decompress);
        using MemoryStream output = new();

        gzip.CopyTo(output);
        return output.ToArray();
    }

    public static byte[] EncryptData(byte[] data)
    {
        using(Aes aes = Aes.Create())
        {
            aes.Key = SecretKey32;
            aes.IV = SecretKey16;

            using MemoryStream ms = new();
            {
                using(CryptoStream cryptoStream = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
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

            using (MemoryStream ms = new())
            {
                using (CryptoStream cryptoStream = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
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
        using (MemoryStream ms = new())
        {
            using(BinaryWriter writer = new(ms, Encoding.UTF8))
            {
                writer.Write(FILE_HEADER);
                writer.Write(FILE_VERSION);

                #region Data File

                writer.Write(data.fileName);
                writer.Write(data.modifiedDate);
                writer.Write(data.isAutoSaveFile);

                #endregion

                #region Quest Data
                writer.Write(data.sceneIndex);
                writer.Write(data.currentLevel);
                writer.Write(data.questDataList.Count);
                foreach(SerializableQuestData quest in data.questDataList)
                {
                    writer.Write(quest.questName);
                    writer.Write(quest.completedObjectives);
                    writer.Write(quest.objectiveProgressvalue.Count);

                    foreach(int value in quest.objectiveProgressvalue)
                    {
                        writer.Write(value);
                    }
                }
                #endregion

                #region Scene Instances
                writer.Write(data.killedEnemiesIndex.Count);
                foreach(int value in data.killedEnemiesIndex)
                {
                    writer.Write(value);
                }

                writer.Write(data.saveableAssets.Count);
                foreach(var asset in data.saveableAssets)
                {
                    asset.WriteToSavedData(writer);
                }
                #endregion
                return ms.ToArray();
            }
        }
    }

    public static SavedData Deserialize(byte[] data)
    {
        using MemoryStream ms = new(data);
        using BinaryReader reader = new(ms, Encoding.UTF8);
        string header = reader.ReadString();
        if (header != FILE_HEADER)
        {
            throw new Exception("Invalid Save File Format");
        }
        int fileVersion = reader.ReadInt32();

        #region Data File
        string fileName = reader.ReadString();
        string modifiedDate = reader.ReadString();
        bool isAutoSave = reader.ReadBoolean();
        #endregion

        #region Quest Data
        int sceneIndex = reader.ReadInt32();
        int levelIndex = reader.ReadInt32();
        int questCount = reader.ReadInt32();
        List<SerializableQuestData> questList = new();
        for (int i = 0; i < questCount; i++)
        {
            string questname = reader.ReadString();
            int completedObjCount = reader.ReadInt32();
            int progressValueCount = reader.ReadInt32();

            List<int> objectProgressiveValues = new();
            for (int x = 0; x < progressValueCount; x++)
            {
                int progressValue = reader.ReadInt32();
                objectProgressiveValues.Add(progressValue);
            }
            SerializableQuestData newQuestData = new(questname, completedObjCount, objectProgressiveValues);
            questList.Add(newQuestData);
        }
        #endregion

        #region Scene Instances
        int killedEnemiesCount = reader.ReadInt32();
        int[] killedEnemies = new int[killedEnemiesCount];
        for (int i = 0; i < killedEnemiesCount; i++)
        {
            killedEnemies[i] = reader.ReadInt32();
        }

        int savedAssetsCount = reader.ReadInt32();
        List<ObjectSaveData> loadedAssets = new(savedAssetsCount);
        for (int i = 0; i < savedAssetsCount; i++)
        {
            // Create a temporary ObjectSaveData and let it read its data (it reads TypeId & PersistentId first)
            // We must peek the type to construct the correct subclass. Because ReadFromSavedData reads the type
            // internally, we will read TypeId & PersistentId manually, then create appropriate typed instance
            long posBefore = ms.Position;

            // Peek the TypeId & PersistentId strings (we must read them in same order written)
            string typeId = reader.ReadString();
            string persistentId = reader.ReadString();

            // Rewind back to beginning of this asset block so we can let the typed instance read everything
            ms.Position = posBefore;
            ObjectSaveData assetData;
            if (string.Equals(typeId, "Character", StringComparison.OrdinalIgnoreCase))
            {
                assetData = new CharacterSaveData();
            }
            else
            {
                assetData = new ObjectSaveData();
            }
            assetData.ReadFromSavedData(reader);
            loadedAssets.Add(assetData);
        }
        #endregion

        // Create SavedData and populate the fields you expect
        SavedData newData = new(sceneIndex, levelIndex, fileName, modifiedDate, isAutoSave);
        newData.questDataList.Clear();
        newData.questDataList.AddRange(questList);

        // Attach loaded scene instance data
        // IMPORTANT: ensure SavedData has a List<ObjectSaveData> saveableAssets property
        newData.saveableAssets.Clear();
        newData.saveableAssets.AddRange(loadedAssets);
        newData.killedEnemiesIndex.Clear();
        newData.killedEnemiesIndex.AddRange(killedEnemies);
        return newData;
    }
}
