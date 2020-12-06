using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyInit : MonoBehaviour
{
    public int galRad = 110;

    public int coreRad = 20;

    public float angularOffset = .0004f;
    public float coreEdgeEcc = .85f;
    public float galEdgeEcc = .95f;
    public int orbitalVelCore = 20;
    public int orbitalVelGal = 30;
    public int numStars = 15000;
    public bool darkMatter = true;
    public int perturbations = 200;
    public int amplitudePert = 40;
    public int dustRenderSize = 24;
    public int HIIRenderSize = 10;
    public float dustTempBase = 9000.0f;
    public int steps = 10000;
    public float velocityScaling = 100.0f;
    public int colourOffset = 0;
    public float starBaseTemp = 3000.0f;
    public int numHII = 100;
    //public float alphaValue = 1.0f;

    [HideInInspector]
    public int numDust;
    [HideInInspector]
    public float radFarFeild;

    Galaxy gal;
    // Start is called before the first frame update
    void Start()
    {
        //First we need to get the galaxy object assigned to the same game object
        gal = GetComponent<Galaxy>();

        //Abort the whole process if so
        if(gal == null){
            Debug.Log("Please assign the galaxy script to the same object with the galaxy init script");
            return;
        }

        //Setting up ratios used in the galaxy
        radFarFeild = galRad * 2.0f;
        numDust = (int)(numStars / 3);

        //Setting the galaxy generator to have these properties
        gal.galProp = this;

        //Creation call
        gal.Initiate();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
