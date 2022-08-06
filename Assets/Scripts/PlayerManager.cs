using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ME
{
    public class PlayerManager : MonoBehaviour
    {

        InputHandler inputHandler;
        Animator anime;
        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            anime = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            inputHandler.isInteracting = anime.GetBool("isInteracting");
            inputHandler.rollFlag = false;
        }
    }
}
