using UnityEngine;
using NAudio.Wave;
using System.Timers;

public static class AudioSystem {
    private static WaveOut _waveOut;
    private static Timer _timer;

    public static void PlayNote(string note)
    {
        if (_waveOut == null)
        {
            var sineWaveProvider = new SineWaveProvider32();
            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            sineWaveProvider.Frequency = 1000;
            sineWaveProvider.Amplitude = 0.25f;
            _waveOut = new WaveOut();
            _waveOut.Init(sineWaveProvider);
            _waveOut.Play();

            StopTimer();

            _timer = new Timer();
            _timer.Elapsed += TimerElapsed;
            _timer.Interval = 20000;
            _timer.Start();
        }
        else
        {
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;

            StopTimer();
        }
    }

    private static void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Close();
            _timer.Dispose();
            _timer = null;
        }
    }

    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (_waveOut != null)
        {
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }
    }

    public static void Stop()
    {
        if (_waveOut != null)
        {
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }
    }
}


public class SineWaveProvider32 : WaveProvider32
{
    int sample;

    public SineWaveProvider32()
    {
        Frequency = 1000;
        Amplitude = 0.25f; // let's not hurt our ears            
    }

    public float Frequency { get; set; }
    public float Amplitude { get; set; }

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        int sampleRate = WaveFormat.SampleRate;
        for (int n = 0; n < sampleCount; n++)
        {
            buffer[n + offset] = (float)(Amplitude * Mathf.Sin((2 * Mathf.PI * sample * Frequency) / sampleRate));
            sample++;
            if (sample >= sampleRate) sample = 0;
        }
        return sampleCount;
    }
}

public abstract class WaveProvider32 : IWaveProvider
{
    private WaveFormat waveFormat;

    public WaveProvider32()
        : this(44100, 1)
    {
    }

    public WaveProvider32(int sampleRate, int channels)
    {
        SetWaveFormat(sampleRate, channels);
    }

    public void SetWaveFormat(int sampleRate, int channels)
    {
        this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        WaveBuffer waveBuffer = new WaveBuffer(buffer);
        int samplesRequired = count / 4;
        int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
        return samplesRead * 4;
    }

    public abstract int Read(float[] buffer, int offset, int sampleCount);

    public WaveFormat WaveFormat
    {
        get { return waveFormat; }
    }
}