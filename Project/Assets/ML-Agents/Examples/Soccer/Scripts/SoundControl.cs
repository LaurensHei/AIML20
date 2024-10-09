using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{

    public AudioSource source;
    public AudioClip ballWall;

    void ballWallSound()
    {
        source.clip = ballWall;
        source.Play();
    }
    
}
