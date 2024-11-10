using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    /*
    private float soundSpeed;
    private float soundRange;
    public GameObject Sound
    {
        get { return Sound; }   // get method
        set { Sound = value; }  // set method
    }
    Vector3 pos;
    public Vector3 ScaleChange
    {
        get { return ScaleChange; }
        set { ScaleChange = value; }
    }
    public Sound(Vector3 pos, float soundSpeed, float soundRange)
    {
        this.pos = pos;
        scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
        this.soundSpeed = soundSpeed;
        this.soundRange = soundRange;
        Sound = Instantiate(GetComponent<Creator>().soundCreater, pos, Quaternion.identity);
    }

    
    public void Update()
    {
        Sound.transform.localScale += scaleChange * soundSpeed;

        // Reverse scaling if object exceeds the max range
        if (sound.transform.localScale.x > soundRange)
        {
            // Reverse the scale increment for the individual object
            scaleChange = -scaleChange;
        }

        // Destroy the sound object if it shrinks below a certain size
        else
        {
            Destroy(sound);
        }
    }
    */
}
