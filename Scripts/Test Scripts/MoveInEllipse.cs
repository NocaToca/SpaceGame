using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInEllipse : MonoBehaviour
{
    public GameObject sphere;
    public float Velocity = 4;

    public float a = 5;
    public float b = 4;
    public float theta = 0;

    float alpha;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float beta = theta * Mathf.Deg2Rad;

        float cosAlpha = Mathf.Cos(alpha);
        float sinAlpha = Mathf.Sin(alpha);
        float cosBeta = Mathf.Cos(beta);
        float sinBeta = Mathf.Sin(beta);

        alpha += Velocity / 100.0f;
        float x = (a * cosAlpha * cosBeta - b * sinAlpha * sinBeta);
        float y = (a * cosAlpha * sinBeta + b * sinAlpha * cosBeta);

        sphere.transform.position = new Vector3(x, y, 0.0f);

    }
}
