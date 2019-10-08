using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Final : MonoBehaviour
{
    private PlayerMovementGroundSticky playerMovementGroundSticky;
    private Coroutine finalCoroutine;
    private Rigidbody2D body;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject stars;
    [SerializeField] private GameObject planet;
    [SerializeField] private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementGroundSticky = GetComponent<PlayerMovementGroundSticky>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerMovementGroundSticky.SizeMul > 50 && finalCoroutine == null)
        {
            finalCoroutine = StartCoroutine(FinalSequence());
        }
    }

    IEnumerator FinalSequence()
    {
        playerMovementGroundSticky.GetComponent<InputRouter>().PlayerId = 1 ;
        playerMovementGroundSticky.GetComponent<Collider2D>().isTrigger = true;

        cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);

        // grow and shake
        int count = 0;
        while (count++ < 50)
        {
            body.velocity = new Vector3(0, 0, 0);
            transform.position += 2 * playerMovementGroundSticky.SizeMul
                * new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
            // playerMovementGroundSticky.SizeMul *= 1.1f;
            yield return new WaitForSeconds(0.01f);
        }



        body.velocity = new Vector3(0, 0, 0);
        playerMovementGroundSticky.GravityMul = 0;

        count = 0;
        // fly to the stars
        while(transform.position.y < 1000)
        {
            cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, cam.transform.position.z);
            body.velocity = new Vector3(0, 2000, 0);
            yield return new WaitForSeconds(0.01f);
        }

        stars.SetActive(true);
        // fly to the stars
        count = 0;
        while (count++ < 200)
        {
            body.velocity = new Vector3(0, 2000, 0);
            yield return new WaitForSeconds(0.01f);
        }

        panel.SetActive(true);
        // rotate camera
        while (cam.transform.rotation.eulerAngles.z < 150)
        {
            body.velocity = new Vector3(0, 2000, 0);
            var rot = cam.transform.rotation.eulerAngles;
            cam.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + 1);
            yield return new WaitForSeconds(0.01f);
        }

        count = 0;
        // rotate to planet
        while (count++ < 200)
        {
            planet.transform.position += new Vector3(-1, +1, 0);
            yield return new WaitForSeconds(0.01f);
        }


    }
}
