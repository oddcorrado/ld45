using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputRouter))]
public class CommandShootDir : Command
{
    [Serializable]
    public class AngleRange
    {
        public float min;
        public float max;
    }
    [SerializeField]
    private PooledBullet prefab;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private AngleRange[] angleRanges;
    [SerializeField]
    private Aim aim;
    [SerializeField]
    private bool shootOnKeyDown;
    [SerializeField]
    private bool isGravityCancel = false;
    [SerializeField]
    private float angleStep = 45;
    [SerializeField]
    private int multi = 1;
    [SerializeField]
    private float multiAngle = 10;

    private Vector2 dir;
    private Rigidbody2D body;
    private PlayerMovement playerMovement;
    private Coroutine lockCoroutine;
    private float shootDate;
    private bool doCreateBullets = false;

    public bool IsAiming { get; set; }

    new void Start()
    {
        base.Start();

        foreach (var range in angleRanges)
        {
            if (range.min > range.max
                || Mathf.Abs(range.min) > 360
                || range.min < 0
                || Mathf.Abs(range.max) > 360
                || range.max < 0)
                Debug.LogError("unvalid range " + range.min + "/" + range.max);
        }
        body = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private Vector2 RestrainAngle()
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        if (angle < 0) angle += 360;

        if (angleRanges.Length == 0)
        {
            angle = (Mathf.Round(angle / angleStep)) * angleStep;
            return Quaternion.Euler(0, 0, angle) * Vector2.right;
        }



        float best = 2000;

        foreach (var range in angleRanges)
        {
            if (angle >= range.min && angle <= range.max) return dir;
            float clamp = Mathf.Clamp(angle, range.min, range.max);

            if (Mathf.Abs(angle - clamp) < Mathf.Abs(angle - best)) best = clamp;
        }

        best = (Mathf.Round(best / angleStep)) * angleStep;
        return Quaternion.Euler(0, 0, best) * Vector2.right;
    }

    void Place(PooledBullet bullet, Vector2 localDir)
    {
        bullet.transform.position = transform.position + distance * new Vector3(localDir.x, localDir.y, 0).normalized;
        /* var angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        bullet.transform.transform.rotation = Quaternion.Euler(0, 0, angle); */

        bullet.GetComponent<Rigidbody2D>().velocity = localDir * velocity;
#if SPINE
        var sb = bullet.GetComponent<SimpleBullet>();

        if(sb != null) sb.ForceVelocity = localDir * velocity;

#endif
        bullet.GetComponent<PlayerObject>().PlayerId = input.PlayerId;
    }

    void CreateBullets()
    {
        Vector3 currentDir = multi == 1 ? new Vector3(dir.x, dir.y, 0) : Quaternion.Euler(0, 0, -multiAngle * 0.5f) * dir;
        for (int i = 0; i < multi; i++)
        {
            PooledBullet bullet = prefab.Get<PooledBullet>(true);
                bullet.GetComponent<PlayerObject>().PlayerId = input.PlayerId;
            Place(bullet, currentDir);
            currentDir = Quaternion.Euler(0, 0, multiAngle / multi) * currentDir;
        }
    }

    IEnumerator GravityCancel()
    {
        playerMovement.GravityMul = 0;
        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);
        playerMovement.GravityMul = 1;
    }

    void UpdateDir()
    {
        var newDir = new Vector2(input.X, input.Y).normalized;
        if (newDir.magnitude > Mathf.Epsilon) dir = newDir;
        dir = RestrainAngle();
    }

    void Update()
	{
        if(!shootOnKeyDown && IsAiming && !CheckKeys())
        {
            if(doCreateBullets)
            {
                CreateBullets();
                doCreateBullets = false;
            }
            if (isGravityCancel) playerMovement.GravityMul = 1;
            playerMovement.IsLocked = false;
            IsAiming = false;
            if (aim != null) aim.gameObject.SetActive(false);
            return;
        }
        else if(shootOnKeyDown && IsAiming && !CheckKeys())
        {
            playerMovement.IsLocked = false;
            IsAiming = false;
            if (isGravityCancel) playerMovement.GravityMul = 1;
            if (aim != null) aim.gameObject.SetActive(false);
            return;
        }

        if( IsAiming && aim != null)
        {
            UpdateDir();
            if (isGravityCancel)
            {
                playerMovement.GravityMul = 0;
                body.velocity = Vector2.zero;
            }
            aim.SetAngle(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
        }

        if(aim != null && !IsAiming && isLoading)
        {
            UpdateDir();
            if (aim != null) aim.gameObject.SetActive(true);
            aim.SetAngle(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
            playerMovement.IsLocked = true;
            IsAiming = true;
        }

        if (!Check()) return;


        if(shootOnKeyDown)
        {
            CreateBullets();
            if(aim != null && !IsAiming)
            {
                UpdateDir();
                if (aim != null) aim.gameObject.SetActive(true);
                aim.SetAngle(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
                if(Mathf.Abs(input.X) < Mathf.Epsilon) playerMovement.IsLocked = true;
                IsAiming = true;
            }
        }
        else
        {
            UpdateDir();
            doCreateBullets = true;
            if (aim != null) aim.gameObject.SetActive(true);
            aim.SetAngle(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
            if (Mathf.Abs(input.X) < Mathf.Epsilon) playerMovement.IsLocked = true;
            IsAiming = true;
        }
	}
}
