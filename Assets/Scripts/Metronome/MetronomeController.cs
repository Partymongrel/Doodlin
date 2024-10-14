using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetronomeController : MonoBehaviour
{ 
    
    //Singleton pattern
    public static MetronomeController Instance;

    //Variables used for calculating the bpm of the music
    private double curBPM = 140.0F;
    public int timeSigTop = 4, timeSigBottom = 4;
    private int beatCounter = 0;
    private int loopLengthInBeats;

    [Header("Metronome Settings")]
    [Tooltip("Set if you want to hear the metronome or not.")]
    public bool muteMetronome = true;

    [Tooltip("Set loud the metronome is.")]
    [Range(0, 1)] public float gain = 0.5F;

    //Settings for setting the amplitude of the audio stream manually
    private double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;

    //Tracking the beat
    private int curBeat;
    private bool beatRunning = true;

    /// <summary>
    /// Implement singleton pattern and assign the main AudioSource
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one AudioManager!" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    /// <summary>
    /// Initialize the first music track and beat management
    /// </summary>
    void Start()
    {

        //Initialize beat tracking
        curBeat = timeSigTop;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;

    }

    /// <summary>
    /// Monitors the output of the audio source, using dspTime to calculate when to play the next clips
    /// </summary>
    /// <param name="data">An array of floats comprising the audio data. </param>
    /// <param name="channels">An int that stores the number of channels of audio data passed to this delegate. </param>
    void OnAudioFilterRead(float[] data, int channels) //beat detection and when to play logic
    {
        if (!beatRunning)
            return;

        double samplesPerTick = sampleRate * 60.0F / curBPM * 4.0F / timeSigBottom;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;

        while (n < dataLen)
        {
            if (!muteMetronome) //metronome stuff
            {
                float x = gain * amp * Mathf.Sin(phase);
                int i = 0;
                while (i < channels)
                {
                    data[n * channels + i] += x;
                    i++;
                }
            }

            while (sample + n >= nextTick)
            {

                nextTick += samplesPerTick;
                amp = 1.0F;

                //Increment metronome count
                if (++curBeat > timeSigTop)
                {
                    curBeat = 1;
                    amp *= 2.0F;
                }

                //increment beatCounter count and check if we should play new clips
                if (++beatCounter > loopLengthInBeats)
                {
                    beatCounter = 1;
                }

                Debug.Log("Beat: " + curBeat + "/" + timeSigTop + "\n" +
                          "Beat Counter = " + beatCounter);

            }
            if (!muteMetronome)
            {
                phase += amp * 0.3F;
                amp *= 0.993F;
            }
            n++;
        }
    }
}
