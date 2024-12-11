using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.DialogueSystem
{
    public class Movement2D : MonoBehaviour
    {
        float horizontal;
        Rigidbody2D rb2D;

        private void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }

        private void FixedUpdate()
        {
            rb2D.linearVelocity = new Vector2(horizontal * 10f, rb2D.linearVelocity.y);
        }
    }
}