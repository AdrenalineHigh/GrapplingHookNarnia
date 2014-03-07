using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicDelay : MonoBehaviour {
    void Start() {
        audio.PlayDelayed(3);
    }
}