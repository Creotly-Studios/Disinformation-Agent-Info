using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class SecureSaveUtility
{
    // Keys live in SaveSecurityKeys.cs — add that file to .gitignore.
    private static byte[] Key32 => SaveSecurityKeys.Key32;
    private static byte[] IV16 => SaveSecurityKeys.IV16;

    // ── Compression ────────────────────────────────────────────────────────── 

    public static byte[] CompressData(byte[] data)
    {
        using MemoryStream ms = new();
        using (GZipStream gzip = new(ms, CompressionMode.Compress))
        {
            gzip.Write(data, 0, data.Length);
        }
        return ms.ToArray();
    }

    public static byte[] DecompressData(byte[] data)
    {
        using MemoryStream ms = new(data);
        using GZipStream gzip = new(ms, CompressionMode.Decompress);
        using MemoryStream output = new();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    // ── Encryption ───────────────────────────────────────────────────────────

    public static byte[] EncryptData(byte[] data)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key32;
        aes.IV = IV16;

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    public static byte[] DecryptData(byte[] data)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key32;
        aes.IV = IV16;

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    // ── File I/O ─────────────────────────────────────────────────────────────

    public static void SaveToFile(string path, byte[] data)
    {
        byte[] compressed = CompressData(data);
        byte[] encrypted = EncryptData(compressed);
        File.WriteAllBytes(path, encrypted);
    }

    public static byte[] LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Save file not found: {path}");

        byte[] encrypted = File.ReadAllBytes(path);
        byte[] decrypted = DecryptData(encrypted);
        return DecompressData(decrypted);
    }
}

// ─────────────────────────────────────────────────────────────────────────────

public static class SaveSerializer
{
    private const int FILE_VERSION = 1;
    private const string FILE_HEADER = "AGENT INFO";

    /// <summary>Serializes SavedData into a custom binary format.</summary>
    public static byte[] SerializeData(SavedData data)
    {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms, Encoding.UTF8);

        writer.Write(FILE_HEADER);
        writer.Write(FILE_VERSION);

        // ── File metadata ────────────────────────────────────────────────────
        writer.Write(data.fileName);
        writer.Write(data.modifiedDate);
        writer.Write(data.isAutoSaveFile);

        // ── Quest data ───────────────────────────────────────────────────────
        writer.Write(data.sceneIndex);
        writer.Write(data.currentLevel);
        writer.Write(data.questDataList.Count);
        foreach (SerializableQuestData quest in data.questDataList)
        {
            writer.Write(quest.questName);
            writer.Write(quest.completedObjectives);
            writer.Write(quest.objectiveProgressvalue.Count);
            foreach (int value in quest.objectiveProgressvalue)
                writer.Write(value);
        }

        // ── Scene saveables ──────────────────────────────────────────────────
        writer.Write(data.saveableAssets.Count);
        foreach (ObjectSaveData asset in data.saveableAssets)
            asset.WriteToSavedData(writer);

        return ms.ToArray();
    }

    public static SavedData Deserialize(byte[] data)
    {
        using MemoryStream ms = new(data);
        using BinaryReader reader = new(ms, Encoding.UTF8);

        string header = reader.ReadString();
        if (header != FILE_HEADER)
            throw new Exception("Invalid save file format.");

        int fileVersion = reader.ReadInt32(); // reserved for future migration

        // ── File metadata ────────────────────────────────────────────────────
        string fileName = reader.ReadString();
        string modifiedDate = reader.ReadString();
        bool isAutoSave = reader.ReadBoolean();

        // ── Quest data ───────────────────────────────────────────────────────
        int sceneIndex = reader.ReadInt32();
        int levelIndex = reader.ReadInt32();
        int questCount = reader.ReadInt32();
        List<SerializableQuestData> questList = new(questCount);
        for (int i = 0; i < questCount; i++)
        {
            string questName = reader.ReadString();
            int completedObj = reader.ReadInt32();
            int progressValueCount = reader.ReadInt32();
            List<int> progressValues = new(progressValueCount);
            for (int x = 0; x < progressValueCount; x++)
                progressValues.Add(reader.ReadInt32());
            questList.Add(new SerializableQuestData(questName, completedObj, progressValues));
        }

        // ── Scene saveables ──────────────────────────────────────────────────
        int savedAssetsCount = reader.ReadInt32();
        List<ObjectSaveData> loadedAssets = new(savedAssetsCount);
        for (int i = 0; i < savedAssetsCount; i++)
        {
            long posBefore = ms.Position;

            // Peek type before constructing the right subclass.
            string typeId = reader.ReadString();
            string _persistentId = reader.ReadString(); // consumed; rewind reloads it
            ms.Position = posBefore;

            ObjectSaveData assetData = string.Equals(typeId, "Character", StringComparison.OrdinalIgnoreCase)
                ? new CharacterSaveData()
                : new ObjectSaveData();

            assetData.ReadFromSavedData(reader);
            loadedAssets.Add(assetData);
        }

        // ── Assemble ─────────────────────────────────────────────────────────
        SavedData newData = new(sceneIndex, levelIndex, fileName, modifiedDate, isAutoSave);
        newData.questDataList.AddRange(questList);
        newData.saveableAssets.AddRange(loadedAssets);
        return newData;
    }
}