using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour {

    AudioSource _audioSource;
    public static float[] _samplesLeft = new float[512];
    public static float[] _samplesRight = new float[512];

    public static float[] _samplesBuffer = new float[512];
    private float[] _samplesBufferDecrease = new float[512];
    public static float[] _freqBand = new float[8];
    public static float[] _bandBuffer = new float[8];
    private float[] _bandBufferDecrease = new float[8];

    private float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    public static float _amplitude, _amplitudeBuffer;
    private float _amplitudeMax;
    public float _audioProfile;

    // Use this for initialization
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        //Set the maaximum level for the bandss
        AudioProfile(_audioProfile);
    }

    //Set the maximum band amplitudes
    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        SamplesBuffer();
        GetAmplitude();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samplesLeft, 0, FFTWindow.Blackman);
        _audioSource.GetSpectrumData(_samplesRight, 1, FFTWindow.Blackman);
    }

    //Separate raw 512 samples into 8 separate frequency bands
    void MakeFrequencyBands()
    {
        /*
         * 22050 / 512 = 43hz per sample
         * 20 - 60 Hz
         * 60 - 250 Hz
         * 500 - 2000 Hz
         * 2000 - 4000 Hz
         * 4000 - 6000 Hz
         * 6000 - 200000 Hz
         * 
         * 0 - 2 samples = 86Hz
         * 1 - 4 samples = 172Hz (87Hz - 258Hz)
         * 2 - 8 samples = 344Hz (259Hz - 602Hz)
         * 3 - 16 samples = 688 (603 - 1290Hz)
         * 4 - 32 samples = 1376Hz (1291 - 2666)
         * 5 - 64 samples = 2752Hz (2667 - 5418)
         * 6 - 128 samples = 5504Hz (5419 - 10922)
         * 7 - 256 samples = 11008Hz (10923 - 21930)
         * 
         */
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            //calculate the number of samples for this band
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            float average = 0;

            //add the last two bands (otherwise only 510 bands are accounted for.
            if (i == 7)
            {
                sampleCount += 2;
            }

            //For all the samples on this band, average out the total amplitude
            for (int j = 0; j < sampleCount; j++)
            {
                average += _samplesLeft[count] + _samplesRight[count] * (count + 1);
                count++;
            }
            average /= count;

            _freqBand[i] = average * 10;
        }


    }


    //
    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetAmplitude()
    {
        float _currentAmplitude = 0;
        float _currentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _currentAmplitude += _audioBand[i];
            _currentAmplitudeBuffer += _audioBandBuffer[i];
        }

        if (_currentAmplitude > _amplitudeMax)
        {
            _amplitudeMax = _currentAmplitude;
        }

        _amplitude = _currentAmplitude / _amplitudeMax;
        _amplitudeBuffer = _currentAmplitudeBuffer / _amplitudeMax;
    }

    void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBand[i];
                _bandBufferDecrease[i] = 0.005f;
            }

            if (_freqBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bandBufferDecrease[i];
                _bandBufferDecrease[i] *= 1.2f;
            }
        }
    }

    void SamplesBuffer()
    {
        for (int i = 0; i < 512; i++)
        {
            if (_samplesLeft[i] > _samplesBuffer[i])
            {
                _samplesBuffer[i] = _samplesLeft[i];
                _samplesBufferDecrease[i] = 0.005f;
            }

            if (_samplesLeft[i] < _samplesBuffer[i])
            {
                _samplesBuffer[i] -= _samplesBufferDecrease[i];
                _samplesBufferDecrease[i] *= 1.2f;
            }
        }
    }
}
