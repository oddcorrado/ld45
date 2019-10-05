using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField]
    private float minZoom = 3;
    [SerializeField]
    float topY = 7.2f;
    [SerializeField]
    float topX;
    [SerializeField]
    float zoomOutSpeed = 0.02f;
    [SerializeField]
    float zoomInSpeed = 0.01f;
    [SerializeField]
    float centerSpeed = 0.03f;
    [SerializeField]
    float zoomOutSizeHorizontal = 2;
    [SerializeField]
    float zoomOutSizeVertical = 4;
    [SerializeField]
    float zoomInSize = 5;

    new private Camera camera;
    new List<float> sizes = new List<float>();
    float aspectRatio = (float)16 / (float)9;



    void Start()
    {
        topX = aspectRatio * topY;
        // Debug.Log("TopX " + topX);
        camera = GetComponent<Camera>();
    }

    private Rect GetCameraBounds()
    {
        Rect rect = new Rect();

        rect.yMin = camera.transform.position.y - camera.orthographicSize;
        rect.yMax = camera.transform.position.y + camera.orthographicSize;

        rect.xMin = camera.transform.position.x - camera.orthographicSize * aspectRatio;
        rect.xMax = camera.transform.position.x + camera.orthographicSize * aspectRatio;

        // if (rect.xMin <= 0) rect.xMin = -Mathf.Infinity;
        // if (rect.yMin <= 0) rect.yMin = -300;

        return rect;
    }

    private Rect GetPlayerBounds()
    {
        Rect rect = new Rect();

        var xMax = -Mathf.Infinity;
        var yMax = -Mathf.Infinity;
        var xMin = Mathf.Infinity;
        var yMin = Mathf.Infinity;

        foreach (var player in FightManager.Instance.Players)
        {
            if (player != null && player.gameObject != null)
            {
                xMax = Mathf.Max(xMax, player.gameObject.transform.position.x);
                yMax = Mathf.Max(yMax, player.gameObject.transform.position.y);
                xMin = Mathf.Min(xMin, player.gameObject.transform.position.x);
                yMin = Mathf.Min(yMin, player.gameObject.transform.position.y);
            }
        }

        rect.xMax = xMax;
        rect.yMax = yMax;
        rect.xMin = xMin;
        rect.yMin = yMin;

        return rect;
    }

    private bool AInB(Rect A, Rect B)
    {
        if (A.xMin < B.xMin) return false;
        if (A.yMin < B.yMin) return false;
        if (A.xMax > B.xMax) return false;
        if (A.yMax > B.yMax) return false;
        return true;
    }

    private Rect PushRect(Rect rect, float hPush, float vPush)
    {
        Rect pushRect = new Rect();

        pushRect.xMax = rect.xMax + hPush;
        pushRect.yMax = rect.yMax + vPush;
        pushRect.xMin = rect.xMin - hPush;
        pushRect.yMin = rect.yMin - vPush;
       
        return pushRect;
    }

    private void DrawRect(Rect r, Color c)
    {
        Debug.DrawLine(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), c);
        Debug.DrawLine(new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), c);
        Debug.DrawLine(new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax), c);
        Debug.DrawLine(new Vector2(r.xMin, r.yMax), new Vector2(r.xMin, r.yMin), c);
    }

    private void CameraClamp()
    {
        var size = camera.orthographicSize;
        var position = camera.transform.position;

        size = Mathf.Min(size, topY);
        if (position.y < size) position.y = size;
        if (position.x < size * aspectRatio) position.x = size * aspectRatio;
        if (position.y + size > topY * 2) position.y = topY * 2 - size;
        if (position.x + size * aspectRatio > topX * 2) position.x = topX * 2 - size * aspectRatio;

        camera.transform.position = position;
        camera.orthographicSize = size;
    }

    void Update()
    {
        if (FightManager.Instance.Players.Length <= 0) return;
        var playerRect = GetPlayerBounds();
        var cameraRect = GetCameraBounds();

        var zoomOut = PushRect(cameraRect, -zoomOutSizeHorizontal, -zoomOutSizeVertical);
        var zoomIn = PushRect(cameraRect, -zoomInSize, -zoomInSize);

        bool botLock = cameraRect.yMin <= zoomOutSizeVertical * 0.1f;
        bool topLock = cameraRect.yMax >= topY * 2 - zoomOutSizeVertical * 0.1f;
        if (botLock)
        {
            zoomOut.yMin = 0;
            zoomIn.yMin = 0;
        }
        if (topLock)
        {
            zoomOut.yMax = topY * 2;
            zoomIn.yMax = topY * 2;
        }

        bool leftLock = cameraRect.xMin <= zoomOutSizeHorizontal * 0.1f;
        bool rightLock = cameraRect.xMax >= topX * 2 - zoomOutSizeHorizontal * 0.1f;
                                  
        if (leftLock)
        {
            zoomOut.xMin = 0;
            zoomIn.xMin = 0;
        }
        if (rightLock)
        {
            zoomOut.xMax = topX * 2;
            zoomIn.xMax = topX * 2; 
        }


        DrawRect(cameraRect, Color.blue);
        DrawRect(zoomOut, Color.red);
        DrawRect(zoomIn, Color.magenta);
        DrawRect(playerRect, Color.green);


        if(AInB(playerRect, zoomOut) == false)
        {
            // Debug.Break();
            var hsize = Mathf.Max(zoomOut.xMin - playerRect.xMin, playerRect.xMax - zoomOut.xMax) / aspectRatio;
            var vsize = Mathf.Max(zoomOut.yMin - playerRect.yMin, playerRect.yMax - zoomOut.yMax);
            var size = Mathf.Max(hsize, vsize);
            // camera.orthographicSize = (1 - zoomOutSpeed) * camera.orthographicSize + zoomOutSpeed * size;
            if(size > 0.3f)
                camera.orthographicSize += zoomOutSpeed * Mathf.Round(size / 0.3f);
            else
                camera.orthographicSize += zoomOutSpeed;
        }
        else
        {
            bool hJitterLock = rightLock && leftLock && playerRect.width > topX * 1.8f;
            bool vJitterLock = topLock && botLock && playerRect.height > topY * 1f;
            if (AInB(playerRect, zoomIn) && !hJitterLock && !vJitterLock)
                camera.orthographicSize -= zoomInSpeed;
        }
        var center = new Vector3(playerRect.center.x, playerRect.center.y, camera.transform.position.z);
        var position = center * centerSpeed + (1 - centerSpeed) * camera.transform.position;
        camera.transform.position = position;


        CameraClamp();
    }

	void UpdateOld()
    {
        //
        float topY = 7.2f;
        float topX = aspectRatio * topY;

        if (FightManager.Instance.Players.Length <= 0) return;

        Vector3 pos = Vector3.zero;

        var playerRect = GetPlayerBounds();
        pos = new Vector3(playerRect.x, playerRect.y);

        float size = Mathf.Min(topY, Mathf.Max(minZoom, 0.7f * Mathf.Max(
            playerRect.yMax - playerRect.yMin, (playerRect.xMax - playerRect.xMin) / aspectRatio)));


        var deltaSize = size - camera.orthographicSize;
        // var deltaPos = new Vector3(pos.x, pos.y, -100) - transform.position;

        sizes.Add(size);

        if(deltaSize > 0)
        {
            camera.orthographicSize = size;

            pos.x = Mathf.Clamp(pos.x, -topX + aspectRatio * size, topX - aspectRatio * size);
            pos.y = Mathf.Clamp(pos.y, -topY + size, topY - size);
             transform.position = new Vector3(pos.x, pos.y, transform.position.z);
           // var deltaPos = pos - transform.position;
            // transform.position += Mathf.Min(0.1f, deltaPos.magnitude) * deltaPos.normalized;
        }
        else 
        {

            // transform.position += deltaPos;
            if(deltaSize < -0.5f)
            {
                deltaSize = Mathf.Max(-0.01f, deltaSize);
                camera.orthographicSize += deltaSize;
            }
            pos.x = Mathf.Clamp(pos.x, -topX + aspectRatio * camera.orthographicSize, topX - aspectRatio * camera.orthographicSize);
            pos.y = Mathf.Clamp(pos.y, -topY + camera.orthographicSize, topY - camera.orthographicSize);
            pos.z = transform.position.z;
            //if((transform.position - pos).magnitude > 0.1f)
            {
                // var deltaPos = pos - transform.position;
                // transform.position += Mathf.Min(0.1f, deltaPos.magnitude) * deltaPos.normalized;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }

        }
    }
}
