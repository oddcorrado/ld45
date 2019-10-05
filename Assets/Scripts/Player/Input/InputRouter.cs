using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRouter : MonoBehaviour {
    public enum Key { A, B, C, J, UP, DOWN, LEFT, RIGHT, HORIZONTAL}
    public float X { get; set; }
    public float Y { get; set; }
    public bool A { get; set; }
    public bool B { get; set; }
    public bool C { get; set; }
    public bool J { get; set; }
    public bool ForceMoveX { get; set; }
    [SerializeField]
    private int playerId;
    public int PlayerId { get { return playerId; } set { playerId = value; } }
    private float prevX;
    public bool NeedsReset { get; set; }


    public bool Check(Key[] keys)
    {
        foreach(var key in keys)
        {
            switch (key)
            {
                case Key.A:
                    if (!A) return false;
                    break;
                case Key.B:
                    if (!B) return false;
                    break;
                case Key.C:
                    if (!C) return false;
                    break;
                case Key.J:
                    if (!A) return false;
                    break;
                case Key.UP:
                    if (Y <= 0.9f) return false;
                    break;
                case Key.DOWN:
                    if (Y >= -0.9f) return false;
                    break;
                case Key.LEFT:
                    if (X >= -0.9f) return false;
                    break;
                case Key.RIGHT:
                    if (X <= 0.9f) return false;
                    break;
                case Key.HORIZONTAL:
                    if (Mathf.Abs(X) <= Mathf.Epsilon) return false;
                    break;
                default:
                    return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update ()
    {
         /* Debug.Log(Input.GetKey(KeyCode.Joystick1Button0)
                  + " " + Input.GetKey(KeyCode.Joystick1Button0)
                  + " " + Input.GetKey(KeyCode.Joystick1Button1)
                  + " " + Input.GetKey(KeyCode.Joystick1Button2)
                  + " " + Input.GetKey(KeyCode.Joystick1Button3)
                  + " " + Input.GetKey(KeyCode.Joystick1Button4)
                  + " " + Input.GetKey(KeyCode.Joystick1Button5)
                  + " " + Input.GetKey(KeyCode.Joystick1Button6)
                  + " " + Input.GetKey(KeyCode.Joystick1Button7)
                  + " " + Input.GetKey(KeyCode.Joystick1Button8)
                  + " " + Input.GetKey(KeyCode.Joystick1Button9)
                  + " " + Input.GetKey(KeyCode.Joystick1Button10)
                  + " " + Input.GetKey(KeyCode.Joystick1Button11)
                  + " " + Input.GetKey(KeyCode.Joystick1Button12)
                  + " " + Input.GetKey(KeyCode.Joystick1Button13)
                  + " " + Input.GetKey(KeyCode.Joystick1Button14)
                  + " " + Input.GetKey(KeyCode.Joystick1Button15)
                  + " " + Input.GetKey(KeyCode.Joystick1Button16)
                  + " " + Input.GetKey(KeyCode.Joystick1Button17)
                  + " " + Input.GetKey(KeyCode.Joystick1Button18)
                  + " " + Input.GetKey(KeyCode.Joystick1Button19)
                 ); */
        X = Input.GetAxis("J" + PlayerId + "H") + Input.GetAxis("K" + PlayerId + "H");
        if (ForceMoveX && Mathf.Abs(X) < Mathf.Epsilon) X = prevX;
        prevX = X;
        Y = Input.GetAxis("J" + PlayerId + "V") + Input.GetAxis("K" + PlayerId + "V");
        switch(PlayerId) {
            case 1:
                A = Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.G);
                B = Input.GetKey(KeyCode.Joystick1Button3) || Input.GetKey(KeyCode.H);
                C = Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.J);
                J = Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.Space);
                break;
            case 2:
                A = Input.GetKey(KeyCode.Joystick2Button0) || Input.GetKey(KeyCode.V);
                B = Input.GetKey(KeyCode.Joystick2Button3) || Input.GetKey(KeyCode.B);
                C = Input.GetKey(KeyCode.Joystick2Button2) || Input.GetKey(KeyCode.N);
                J = Input.GetKey(KeyCode.Joystick2Button1) || Input.GetKey(KeyCode.Space);
                break;        
            case 3:
                A = Input.GetKeyDown(KeyCode.Joystick3Button0);
                B = Input.GetKeyDown(KeyCode.Joystick3Button1);
                C = Input.GetKeyDown(KeyCode.Joystick3Button2);
                J = Input.GetKeyDown(KeyCode.Joystick3Button3);
                break;   
            case 4:
                A = Input.GetKeyDown(KeyCode.Joystick4Button0);
                B = Input.GetKeyDown(KeyCode.Joystick4Button1);
                C = Input.GetKeyDown(KeyCode.Joystick4Button2);
                J = Input.GetKeyDown(KeyCode.Joystick4Button3);
                break;   
        }
        //Debug.Log(X + "/" + Y + "/" + A + "/" + B + "/" + C + "/");
    }
}
