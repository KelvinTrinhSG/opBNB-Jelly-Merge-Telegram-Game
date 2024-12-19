using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class AudioFix : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            AudioController.CreateAudioListener();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

