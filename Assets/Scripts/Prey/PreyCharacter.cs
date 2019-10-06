using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyCharacter : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody;
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
    }

    private void Update()
    {
        Debug.Log("velocity" + _rigidbody.velocity);
        HandleLayer();
    }

    private void HandleLayer()
    {
        if (IsMoving)
        {
            ActivateLayers("MoveLayer");
            AnimMovement(_rigidbody.velocity.x);
        }
        else
        {
            ActivateLayers("IdleLayer");
        }
    }

    private void AnimMovement(float direction)
    {
        _animator.SetFloat("x", Mathf.Abs(direction));

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

    private void ActivateLayers(string layerName)
    {
        if (layerName == _currentLayerName) return;

        _currentLayerName = layerName;

        for (int i = 0; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(_animator.GetLayerIndex(layerName), 1);
    }
}