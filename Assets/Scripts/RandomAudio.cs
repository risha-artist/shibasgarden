using System.Collections.Generic;
using UnityEngine;

public class RandomAudio : MonoBehaviour {
    [SerializeField]
    private List<AudioSource> _audioSources;

    public void Play() {
        _audioSources[Random.Range(0, _audioSources.Count)].Play();
    }
}