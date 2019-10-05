using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TintManager : MonoBehaviour
{
    void Start()
    {
    }

    public void Tint(Color color, float duration)
    {
    }

    public void Black(Color color, float duration)
    {
    }
}

#if SPINE
using Spine.Unity;
using UnityEngine;

public class TintManager : MonoBehaviour
{
    private Coroutine tintCoroutine;
    private Coroutine blackCoroutine;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = transform?.Find("Spine").GetComponent<MeshRenderer>();
    }

    IEnumerator Tinter(Color color, float duration)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(duration);

        for(float ratio = 0; ratio < 1; ratio += 0.1f)
        {
            var c = Color.Lerp(color, Color.white, ratio);
            yield return new WaitForSeconds(0.016f);
            mpb.SetColor("_Color", c);
            meshRenderer.SetPropertyBlock(mpb);
        }
        mpb.SetColor("_Color", Color.white);
        meshRenderer.SetPropertyBlock(mpb);
    }

    public void Tint(Color color, float duration)
    {
        if (tintCoroutine != null) StopCoroutine(tintCoroutine);
        tintCoroutine = StartCoroutine(Tinter(color, duration));
    }

    IEnumerator Blacker(Color color, float duration)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Black", color);
        meshRenderer.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(duration);

        for (float ratio = 0; ratio < 1; ratio += 0.25f)
        {
            var c = Color.Lerp(color, Color.black, ratio);
            yield return new WaitForSeconds(0.016f);
            mpb.SetColor("_Black", c);
            meshRenderer.SetPropertyBlock(mpb);
        }
        mpb.SetColor("_Black", Color.black);
        meshRenderer.SetPropertyBlock(mpb);
    }

    public void Black(Color color, float duration)
    {
        if (blackCoroutine != null) StopCoroutine(blackCoroutine);
        blackCoroutine = StartCoroutine(Blacker(color, duration));
    }
}
#endif