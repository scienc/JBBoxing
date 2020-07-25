using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWorkMain : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
    }
}