using UnityEngine;
using System.Collections;

public class SoundManagerScript : MonoBehaviour {

    public GameObject blip1;
    public GameObject blip2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play (string which)
    {
        if (which == "blip1")
        {
            blip1.GetComponent<AudioSource>().Play();
        }
        if (which == "blip2")
        {
            blip2.GetComponent<AudioSource>().Play();
        }
    }
}
