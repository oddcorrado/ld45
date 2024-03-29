﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobEat : MonoBehaviour
{
    private GameObject _gameObjectProgessBar;
    private BlobProgressBarEat _progressBarUi;
    private PlayerMovementGroundSticky _playerMovement;
    private GameObject _soundController;
    private AudioController _audioEat;
    private Dictionary<string, int> _preys = new Dictionary<string, int>();
    public Dictionary<string, int> Preys
    {
        get { return _preys; }
    }

    [System.Serializable]
    public class PreFood
    {
        public string name;
        public int count;
    }

    [SerializeField] private PreFood[] preFoods;
    private void Awake()
    {
        _playerMovement = transform.GetComponent<PlayerMovementGroundSticky>();
        _gameObjectProgessBar = GameObject.Find("BlobUI");
        _progressBarUi = _gameObjectProgessBar.transform.Find("ProgressBar").GetComponent<BlobProgressBarEat>();

        _soundController = GameObject.Find("SoundController");
        if (_soundController != null)
            _audioEat = _soundController.transform.GetComponent<AudioController>();

        foreach (var preFood in preFoods)
        {
            Preys.Add(preFood.name, preFood.count);
        }

        Debug.Log("audio : " + _audioEat);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(IsEdible(collision.transform.tag))
        {
            Eat(collision.transform);
        }
    }

    private void Eat(Transform prey)
    {
        if (_playerMovement.SizeMul < prey.GetComponent<PreyStats>().SizeMulMin) return;
        
        _audioEat.Eat.Play();

        SetPrey(prey.GetComponent<PreyStats>().Name);
        _progressBarUi.Value += prey.GetComponent<PreyCharacter>().EnergeticValue;
        _progressBarUi.ValueProgressBarMax += prey.GetComponent<PreyCharacter>().MaximumEnergy;

        

        Destroy(prey.gameObject);
    }

    private bool IsEdible(string prey)
    {
        if(prey == "Prey") return true;
        return false;
    }

    private void SetPrey(string prey)
    {
        if(_preys.ContainsKey(prey))
        {
            foreach(KeyValuePair<string, int> currentPrey in _preys)
            {
                if(currentPrey.Key == prey)
                {
                    _preys[currentPrey.Key] += 1;
                    return;
                }
            }
        } 
        else
        {
            _preys.Add(prey, 1);
        }
    }
}
