using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public Data[] levelsData;
    
    [System.Serializable]
    public struct Data
    {
        public string name;
        public int levelIndex;
        [Range(001, 999)] public int levelLoadCode;
    }
}

