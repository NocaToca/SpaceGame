using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributionFunction : MonoBehaviour
{
    float maxIntensity;
    float bulge;
    float discScaleLength;
    float bulgeRad;
    float startInt;
    float endInt;
    int points;

    List<float> M1;
    List<float> X1;
    List<float> Y1;

    List<float> M2;
    List<float> X2;
    List<float> Y2;

    public void setUp(float maxIntensity, float bulge, float discScaleLength, float bulgeRad, float startInt, float endInt, int points){

        this.maxIntensity = maxIntensity;
        this.bulge = bulge;
        this.discScaleLength = discScaleLength;
        this.bulgeRad = bulgeRad;
        this.startInt = startInt;
        this.endInt = endInt;
        this.points = points;

        BuildDF(points);

    }

    //The build here is to set up how the stars are distibuted based on the intesity of the brightness of a galaxy
    private void BuildDF(int points){
        float h = (endInt - startInt)/points;
        float x = 0, y = 0;

        M1 = new List<float>();
        X1 = new List<float>();
        Y1 = new List<float>();

        M2 = new List<float>();
        X2 = new List<float>();
        Y2 = new List<float>();

        M1.Clear();
        X1.Clear();
        Y1.Clear();

        M2.Clear();
        X2.Clear();
        Y2.Clear();

        X1.Add(0.0f);
        Y1.Add(0.0f);
        M1.Add(0.0f);
        for(int i = 0; i<points; i+=2){

            x = (i+2) * h;
            y += (h/3.0f) * (Intensity(startInt + i * h) + 4 * (Intensity(startInt + (i + 1) * h) + Intensity(startInt + (i + 2) * h)));
            
            M1.Insert(M1.Count, (y - Y1[Y1.Count-1]/(2*h)));
            X1.Insert(X1.Count, x);
            Y1.Insert(Y1.Count, y);

        }

        M1.RemoveAt(0);
        M1.Insert(M1.Count, 0.0f);

        if(M1.Count != X1.Count || M1.Count != Y1.Count){
            Debug.Log("Arrays in DistributionFunction have mis-matched sizes!");

        }

        for(int i = 0; i < Y1.Count; ++i){
            Y1[i] /= Y1[Y1.Count-1];
            M1[i] /= Y1[Y1.Count-1];
        }

        X2.Add(0.0f);
        Y2.Add(0.0f);
        M2.Add(0.0f);

        float p;
        h = 1.0f/points;

        for(int i=1, k = 0; i<points; ++i){
            p = (float)i * h;

            for(; Y1[k+1] <= p; ++k){

            }
            y = X1[k] + (p - Y1[k])/M1[k];

            M2.Insert(M2.Count, (y-Y2[Y2.Count-1])/h);
            X2.Insert(X2.Count, p);
            Y2.Insert(Y2.Count, y);

        }

        M2.RemoveAt(0);
        M2.Insert(M2.Count, 0.0f);

        if(M2.Count != X2.Count || M2.Count != Y2.Count){
            Debug.Log("Arrays in DistributionFunction have mis-matched sizes!");

        }

    }

    private float Intensity(float x){
        return (x < bulgeRad) ? IntensityBulge(x, maxIntensity, bulge) : 
        IntensityDisc(x-bulgeRad, IntensityBulge(bulgeRad, maxIntensity, bulge), discScaleLength);
    }

    //The two main equations for the intesity of a galaxy
    private float IntensityBulge(float r, float i, float b){

        float val = i * Mathf.Exp(-b*Mathf.Pow(r, .25f));
        return val;

    }

    private float IntensityDisc(float r, float i, float s){
        
        float val= i * Mathf.Exp(-r/s);
        return val;

    }

    //Once we have a radius we can use what we calculated to place the star
    public float ValFromProb(float rad){

        float h = 1.0f / (Y2.Count - 1);

        int i = (int)(rad/h);
        float r = rad - i*h;


        return (Y2[i] + M2[i] * r);

    }
}
