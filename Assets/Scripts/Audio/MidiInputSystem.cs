using UnityEngine;
using NAudio.Midi;
using System.Collections.Generic;

public class MidiInputSystem : MonoBehaviour {
    private const int MAX_QUEUE_PER_UPDATE = 10;
    private object eventQueueObject = new object();
    private Queue<MidiEvent> eventQueue = new Queue<MidiEvent>();

    private MidiIn _midiIn;

    // Use this for initialization
    void Start () {
        if (_midiIn != null)
        {
            return;
        }

        try
        {
            _midiIn = new MidiIn(0);
        } catch (System.Exception)
        {
            Debug.Log("Could not read from Midi device.");
            return;
        }

        _midiIn.MessageReceived += MidiIn_MessageReceived;

        _midiIn.Start();
    }

    void Update()
    {
        lock (eventQueueObject)
        {
            int processed = 0;

            while (eventQueue.Count > 0 || processed > MAX_QUEUE_PER_UPDATE)
            {
                processed++;

                var midiEvent = eventQueue.Dequeue();

                ProcessMidiEvent(midiEvent);
            }
        }
    }

    private void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
    {
        var midiEvent = e.MidiEvent;

        if (midiEvent.CommandCode == MidiCommandCode.TimingClock)
        {
            return;
        }

        if (midiEvent.CommandCode == MidiCommandCode.AutoSensing)
        {
            return;
        }

        if (midiEvent.CommandCode == MidiCommandCode.PitchWheelChange)
        {
            return;
        }

        if (midiEvent.CommandCode == MidiCommandCode.ControlChange)
        {
            return;
        }

        EnqeueMidiEvent(e.MidiEvent);
    }

    public void EnqeueMidiEvent(MidiEvent e)
    {
        lock (eventQueueObject)
        {
            eventQueue.Enqueue(e);
        }
    }

    // Use this for initialization
    void OnDestroy()
    {
        if (_midiIn != null)
        {
            _midiIn.MessageReceived -= MidiIn_MessageReceived;
        }

        lock (eventQueueObject)
        {
            eventQueue.Clear();
        }

        if (_midiIn != null)
        {
            _midiIn.Stop();

            _midiIn.Dispose();

            _midiIn = null;
        }
    }

    private void ProcessMidiEvent(MidiEvent midiEvent) { 
        var noteOnEvent = midiEvent as NoteOnEvent;

        if (noteOnEvent != null)
        {
            var noteGameObject = GameObject.Find(noteOnEvent.NoteName);

            if (noteGameObject != null)
            {
                var noteGameObjectMidi = noteGameObject.GetComponent<KeyMidiReceiver>();

                noteGameObjectMidi.SetPressed(noteOnEvent.Velocity / 127.0f);
            }            
        }

        var noteEvent = midiEvent as NoteEvent;

        if (noteOnEvent == null && noteEvent != null)
        {
            var noteGameObject = GameObject.Find(noteEvent.NoteName);

            if (noteGameObject != null)
            {
                var noteGameObjectMidi = noteGameObject.GetComponent<KeyMidiReceiver>();

                noteGameObjectMidi.SetReleased();                
            }            
        }
    }
}
