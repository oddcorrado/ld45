using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandActivate : Command
{
    [SerializeField]
    private GameObject toActivate;
    [SerializeField]
    private float duration = 0.1f;

    IEnumerator activateTimeout()
    {
        toActivate.SetActive(true);
        yield return new WaitForSeconds(duration);
        toActivate.SetActive(false);
    }

    private void Update()
    {
        if (!Check()) return;

        StopAllCoroutines();
        StartCoroutine(activateTimeout());
    }
}