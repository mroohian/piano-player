using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Midi;
using System.IO;

public class PlayMidi : MonoBehaviour {
    public string midiFileName;
    public float startTime;
    public float tempo;

    private MidiInputSystem midiInputSystem;
    private Dictionary<long, List<NoteOnEvent>> midiData;
    private long[] notePoints;
    private int ticksPerBeat;
    private float tempoUnit;
    private long lastPlayedNote;

    // Use this for initialization
    void Start () {
        midiInputSystem = gameObject.GetComponent<MidiInputSystem>();

        // disable if not  midi input system is found.
        if (midiInputSystem == null)
        {
            Debug.Log("Could not find Midi input system.");

            gameObject.SetActive(false);

            return;
        }

        MidiFile midiFile = null;

        try
        {
            var midiAsset = Resources.Load<TextAsset>(midiFileName);
            Stream s = new MemoryStream(midiAsset.bytes);
            BinaryReader br = new BinaryReader(s);

            var tempFile = Path.GetTempPath() + System.Guid.NewGuid().ToString() + ".mid";

            byte[] buffer = new byte[16 * 1024];
            using (FileStream fs = new FileStream(tempFile, FileMode.Create))
            {
                int read;
                while ((read = br.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, read);
                }
            }

            midiFile = new MidiFile(tempFile);

            //midiFile = new MidiFile(filename);
        } catch(System.Exception)
        {
            Debug.Log("Could not read midi file.");

            gameObject.SetActive(false);

            return;
        }

        ticksPerBeat = midiFile.DeltaTicksPerQuarterNote;
        tempoUnit = 60.0f / tempo;

        midiData = new Dictionary<long, List<NoteOnEvent>>();

        long currentTick = 0;
        var currentTickNotes = new List<NoteOnEvent>();

        foreach (MidiEvent noteEvent in midiFile.Events.SelectMany(e => e).OrderBy(e => e.AbsoluteTime))
        {
            if (noteEvent.CommandCode != MidiCommandCode.NoteOn)
            {
                continue;
            }

            var noteOnEvent = noteEvent as NoteOnEvent;

            if (noteOnEvent == null)
            {
                continue;
            }

            if (currentTick == noteOnEvent.AbsoluteTime)
            {
                currentTickNotes.Add(noteOnEvent);
            } else
            {
                // process and empty the list
                midiData[currentTick] = currentTickNotes;
                currentTickNotes = new List<NoteOnEvent>();

                // Debug message
                //float time = tempoUnit * currentTick / ticksPerBeat;
                //Debug.Log(time + " " + string.Join(", ", currentTickNotes.Select(e => e.NoteName).ToArray()));

                // add the new note
                currentTick = noteOnEvent.AbsoluteTime;                
                currentTickNotes.Add(noteOnEvent);
            }
        }

        notePoints = midiData.Keys.OrderBy(e => e).ToArray();

        ResetPlayBack();
    }

    private void ResetPlayBack()
    {
        // TODO: implement playback from middle of the song.
        lastPlayedNote = -1;
    }

    public void SendNotes()
    {
        //foreach (var noteOnEvent1 in currentTickNotes)
        //{
        //    midiInputSystem.EnqeueMidiEvent(noteOnEvent1);
        //}
    }

    void FixedUpdate()
    {
        if (Time.time < startTime)
        {
            return;
        }

        // song finished
        if (lastPlayedNote >= notePoints.LongLength - 1)
        {
            startTime = Time.time + 2f;
            ResetPlayBack();
            return;
        }

        // current time
        long deltaTimeMs = (long)((Time.time - startTime) * 1000);        

        // current tick
        long currentTick = (long)(ticksPerBeat * 0.001 * deltaTimeMs / tempoUnit);

        // tick range
        var nextNoteToPlay = lastPlayedNote + 1;

        while (nextNoteToPlay < notePoints.LongLength && notePoints[nextNoteToPlay] <= currentTick)
        {
            var currentTickNotes = midiData[notePoints[nextNoteToPlay]];

            foreach (var noteOnEvent in currentTickNotes)
            {
                midiInputSystem.EnqeueMidiEvent(noteOnEvent);

                if (noteOnEvent.OffEvent == null)
                {
                    noteOnEvent.OffEvent = new NoteOnEvent(0, noteOnEvent.Channel, noteOnEvent.NoteNumber, 0, 0);
                }

                var duration = 0.9f * noteOnEvent.OffEvent.DeltaTime * tempoUnit / ticksPerBeat;

                var coroutine = ReleaseNote(noteOnEvent.OffEvent, duration);
                StartCoroutine(coroutine);
            }

            // Debug message
            // float deltaTime = deltaTimeMs / 1000.0f;
            // Debug.Log(deltaTime + " (" + currentTick + ") [" + notePoints[nextNoteToPlay] + "] " + string.Join(", ", currentTickNotes.Select(e => e.NoteName).ToArray()));

            nextNoteToPlay++;
        }

        // update last note played
        lastPlayedNote = nextNoteToPlay - 1;
    }

    private IEnumerator ReleaseNote(NoteEvent noteOnEvent, float duration)
    {
        yield return new WaitForSeconds(duration);
        midiInputSystem.EnqeueMidiEvent(noteOnEvent);
    }
}
