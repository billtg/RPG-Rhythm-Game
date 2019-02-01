using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplitudeVis : MonoBehaviour
{
    public float _startScale, _scaleMultiplier;
    public bool _useBuffer;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (Visualizer._amplitudeBuffer * _scaleMultiplier) + _startScale, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, (Visualizer._amplitude * _scaleMultiplier) + _startScale, transform.localScale.z);
        }
    }
}
