using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class SM_Manager : MonoBehaviour
{   
    public event EventHandler OnCompleteGame;

    [Space]
    public PostData[] postData;

    [System.Serializable]
    public struct PostData
    {
        public string name;
        public PostSO post;       // Reference to the ScriptableObject containing post details
        public bool isLiked;      // whether the post is liked
        public bool isShared;     // Whether the post is shared
        public bool isFlagged;    // WhEther the post is flagged (reported)
    }

    [Range(1, 99)] private float playerCurrentScore; //calculate score based on the interaction the player has with the posrs
    public bool hasCompletedGame;

    [Header("User Interface")]
    [SerializeField] private TextMeshPro hintTextGO; 

   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
