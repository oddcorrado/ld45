using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private GameObject _blob;
    private AnimationManager _blobMovement;
    private AudioSource _walk;
    public AudioSource Walk
    {
        get { return _walk; }
    }

    private AudioSource _jump;
    public AudioSource Jump
    {
        get { return _jump; }
    }

    private AudioSource _eat;
    public AudioSource Eat
    {
        get { return _eat; }
    }

    private AudioSource _land;
    public AudioSource Land
    {
        get { return _land; }
    }

    private void Start()
    {
        _blob = GameObject.Find("BlobSticky");
        _blobMovement = _blob.GetComponent<AnimationManager>();
    }

    private void Awake()
    {
        _walk = transform.Find("Walk").GetComponent<AudioSource>();
        _jump = transform.Find("Jump").GetComponent<AudioSource>();
        _eat = transform.Find("Eat").GetComponent<AudioSource>();
        _land = transform.Find("Land").GetComponent<AudioSource>();
    }

    private void Update()
    {
        Debug.Log("movement : " + _blobMovement.Movement);

        if(_blobMovement.Movement == "run")
        {
            if(!_walk.isPlaying)
            {
                _walk.Play();
                _walk.loop = true;
            }
        }

        if(_blobMovement.Movement == "idle")
        {
            _walk.Stop();
            _walk.loop = false;
        }

        if (_blobMovement.Movement == "air")
        {
            if(!_jump.isPlaying)
            {
                _jump.Play();
            }
        }
    }
}