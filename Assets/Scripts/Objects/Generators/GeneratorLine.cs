using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorLine : PlayerObject
{
    [SerializeField]
    private PooledBullet prefab = null;
    [SerializeField]
    private int count = 10;
    [SerializeField]
    private float period = 0.1f;
    [SerializeField]
    private float xRange = 20;
    [SerializeField]
    private float yOffset = 10;
    [SerializeField]
    private float vxRange = 5;
    [SerializeField]
    private float dieTimeout = 2;
    [SerializeField]
    private ParticleSystem px;

    private SpriteRenderer bg;
    enum State { IN, NORMAL, OUT}
    private State state;

    IEnumerator SwarmTimer()
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(period);
            var go = prefab.Get<PooledBullet>(true);
            go.transform.position = transform.position + new Vector3(Random.Range(-xRange, xRange), yOffset, -1);
            go.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-vxRange, vxRange), -10);
            go.GetComponent<PlayerObject>().PlayerId = PlayerId;
        }
        state = State.OUT;
        if (px) px.Stop();
        yield return new WaitForSeconds(dieTimeout);

        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(SwarmTimer());
        bg = GetComponent<SpriteRenderer>();
        state = State.IN;
        bg.color = new Color(1, 1, 1, 0);
    }

    void Update()
	{
        Debug.Log(state + " " + bg.color.a);
        float a = bg.color.a;

        if (state == State.IN) a = Mathf.Min(1, a + 0.03f);
        if (state == State.OUT) a = Mathf.Max(0, a - 0.03f);

        bg.color = new Color(1, 1, 1, a);
	}
}
