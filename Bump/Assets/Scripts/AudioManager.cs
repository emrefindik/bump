using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField]
    private AudioClip _winJingle;
    public float WinJingleLength
    {
        get { return _winJingle.length; }
    }

    [SerializeField]
    private AudioClip _spit;

    [SerializeField]
    private AudioClip _bite;

    [SerializeField]
    private AudioClip _denied;

	// Use this for initialization
	void Start () {
        instance = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void playWinJingle()
    {
        GetComponent<AudioSource>().PlayOneShot(_winJingle);
    }

    public void playSpit()
    {
        GetComponent<AudioSource>().PlayOneShot(_spit);
    }

    public void playBite()
    {
        GetComponent<AudioSource>().PlayOneShot(_bite);
    }

    public void playDenied()
    {
        GetComponent<AudioSource>().PlayOneShot(_denied);
    }
}
