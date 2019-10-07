using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private GameObject _blob;
    private AnimationManager _blobMovement;
    private bool _isFalling = false;
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

        if (_blobMovement.Movement == "air")
            _isFalling = true;
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

        if(_blobMovement.Movement == "idle")
        {
            _walk.Stop();
            _walk.loop = false;            
        }

        if(_blobMovement.Movement == "run" && !_isFalling)
        {
            if(!_walk.isPlaying)
            {
                _walk.Play();
                _walk.loop = true;
            }
        }

        if (_blobMovement.Movement == "air")
        {            
            if (!_jump.isPlaying && !_isFalling)
            { 
                _walk.Stop();
                _land.Stop();
                _jump.Play();
                _isFalling = true;
            }
        } 
        else
        {
            if (_isFalling)
            {
                if (!_land.isPlaying)
                {
                    _walk.Stop();
                    _land.Stop();
                    _jump.Stop();
                    _land.Play();
                    _isFalling = false;
                }
            }
        }
    }
}