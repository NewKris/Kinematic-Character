using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour {
    public Transform target;

    private void LateUpdate() {
        transform.position = target.position;
    }
}
