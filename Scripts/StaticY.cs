﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticY : MonoBehaviour
{
    public float y;
    // Start is called before the first frame update
    void Awake(){
        y = transform.position.y;
    }

    // Update is called once per frame
    void Update(){
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

    }
}
