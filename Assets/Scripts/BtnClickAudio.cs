using UnityEngine;
using System.Collections;

public class BtnClickAudio : MonoBehaviour {
	
    public void btnClickAudioPlay()
    {
        GetComponent<AudioSource>().Play();
    }
}
