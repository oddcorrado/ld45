﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyCharacter : MonoBehaviour
{
    [SerializeField]
    private float _energeticValue = 2;
    public float EnergeticValue
    {
        get { return _energeticValue; }
    }

    [SerializeField]
    private int _maximumEnergy = 2;
    public int MaximumEnergy
    {
        get { return _maximumEnergy; }
    }

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private AiMovement _aiMovement;
    private string _currentLayerName;

    public bool IsMoving
    {
        get
        {
            return _rigidbody.velocity.x != 0;
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _aiMovement = GetComponent<AiMovement>();
    }

    private void Update()
    {
        HandleLayer();
    }

    private void HandleLayer()
    {
        if (IsMoving)
        {
            AnimMovement(_rigidbody.velocity.x);
        }
    }

    private void AnimMovement(float direction)
    {
        

        if (!_aiMovement.IsGrounded)
        {
            _animator.SetFloat("x", 2.5f);
            // _animator.SetTrigger("jump");
        }
        else
        {
            _animator.SetFloat("x", Mathf.Abs(direction) > 0 ? 0.5f : 0);
        }

        if (direction > 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                Mathf.Abs(transform.localScale.y),
                Mathf.Abs(transform.localScale.z));
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                Mathf.Abs(transform.localScale.y),
                Mathf.Abs(transform.localScale.z));
        }
    }
}