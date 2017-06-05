using System.Collections.Generic;
using UnityEngine;

public class KeySound : MonoBehaviour {
    public int pianoIndex;

    private static readonly Dictionary<string, int> noteRootPosMap;
    private AudioSource audioSource;

    static KeySound() {
        noteRootPosMap = new Dictionary<string, int>() {
            { "C", 1 },
            { "C#", 2 },
            { "D", 3 },
            { "D#", 4 },
            { "E", 5 },
            { "F", 6 },
            { "F#", 7 },
            { "G", 8 },
            { "G#", 9 },
            { "A", 10 },
            { "A#", 11 },
            { "B", 12 }
        };
    }

    void Start()
    {
        var note = gameObject.name;

        var noteRoot = note.Substring(0, note.Length - 1);
        var noteRootPos = noteRootPosMap[noteRoot];

        var octaveStr = note.Substring(note.Length - 1);
        var octave = int.Parse(octaveStr);

        pianoIndex = octave * 12 + noteRootPos - 9 /* piano start key offset*/;

        audioSource = gameObject.GetComponent<AudioSource>();
    }    

    public void Play()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
        //AudioSystem.PlayNote(note);
    }

    public void Pause()
    {
        if (audioSource != null)
        {
            // Causes interupted voice
            //audioSource.Stop();
        }

        //AudioSystem.Stop();
    }
}
