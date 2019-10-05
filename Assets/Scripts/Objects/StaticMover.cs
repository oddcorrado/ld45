using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMover : MonoBehaviour {
    public float beat;
    public float rotSpeed = 0;
    public float rotOffset = 0;
    public float rotAmp = 0;
    public AnimationCurve rotWaveform = new AnimationCurve(
        new Keyframe(0, 0, 0, 0, 0, 0),
        new Keyframe(1, 1, 0, 0, 0, 0));

    public float posSpeed = 0;
    public float posOffset = 0;
    public Vector3 posAmp = Vector3.zero;
    public AnimationCurve posWaveform = new AnimationCurve(
        new Keyframe(0, 0, 0, 0, 0, 0),
        new Keyframe(0.5f, 1, 0, 0, 0, 0),
        new Keyframe(1, 0, 0, 0, 0, 0));

    public float sizeSpeed = 0;
    public float sizeOffset = 0;
    public Vector3 sizeAmp = new Vector3(1, 1, 1);
    public AnimationCurve sizeWaveform = new AnimationCurve(
        new Keyframe(0, 0, 0, 0, 0, 0),
        new Keyframe(0.5f, 1, 0, 0, 0, 0),
        new Keyframe(1, 0, 0, 0, 0, 0));
    
    public float blinkSpeed = 0;
    public float blinkOffset = 0;
    public AnimationCurve blinkWaveform = new AnimationCurve(
        new Keyframe(0, 0, 0, 0, 0, 0),
        new Keyframe(0.5f, 1, 0, 0, 0, 0),
        new Keyframe(1, 0, 0, 0, 0, 0));

    private Vector3 posStart;
    private Vector3 sizeStart;
    private float rotStart;

    private Collider2D collider2d;
    public SpriteRenderer[] spriteRenderers;

    private bool doSize = true;
    public bool DoSize { get { return doSize; } set { doSize = value; }}


    float GetRatio(AnimationCurve curve, float ratio)
    {
        return (curve.Evaluate(ratio % 1f));
    }

	// Use this for initialization
	void Start () 
    {
        collider2d = GetComponent<Collider2D>();
        // spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Mod());

	}

    IEnumerator Mod()
    {
        rotStart = transform.localRotation.eulerAngles.z;
        posStart = transform.localPosition;
        sizeStart = transform.localScale;
        while (true)
        {
            // if ((float)AudioHelm.AudioHelmClock.GetGlobalBeatTime() > beat)
            {
                if(rotSpeed > 0)
                {
                    float rotTime = rotOffset + 1 / rotSpeed * 2 * Time.time;
                    float rotValue = rotStart + rotAmp * GetRatio(rotWaveform, rotTime);
                    transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotValue));
                }

                if (posSpeed > 0)
                {
                    float posTime = posOffset + 1 / posSpeed * 2 * Time.time;
                    Vector3 posValue = posStart + posAmp * GetRatio(posWaveform, posTime);
                    transform.localPosition = posValue;
                }

                if (sizeSpeed > 0 && DoSize)
                {
                    float sizeTime = sizeOffset + 1 / sizeSpeed * 2 * Time.time;
                    Vector3 sizeValue = sizeStart + sizeAmp * GetRatio(sizeWaveform, sizeTime);
                    transform.localScale = sizeValue;
                }

                if (blinkSpeed > 0)
                {
                    float blinkTime = blinkOffset + 1 / blinkSpeed * 2 * Time.time;
                    bool blinkValue = GetRatio(blinkWaveform, blinkTime) > 0.5f;
                    foreach (var sr in spriteRenderers)
                        sr.color = new Color(1, 1, 1, blinkValue ? 0.3f : 1);
                    if(collider2d != null) collider2d.isTrigger = blinkValue;
                }
            }

            yield return new WaitForSeconds(0.016f);
        }
    }
}
