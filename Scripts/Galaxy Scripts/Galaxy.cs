using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : MonoBehaviour
{
    //We need a particle system, particle array, and data array for each of the things we're making
    public ParticleSystem theParticleSystem;
    private ParticleSystem.Particle[] points;

    public ParticleSystem theDustParticleSystem;
    private ParticleSystem.Particle[] dustParticles;

    public ParticleSystem theHIIParticleSystem;
    private ParticleSystem.Particle[] HIIParticles;

    private GalObjectData[] stars;
    private GalObjectData[] dust;
    private GalObjectData[] HII;

    [HideInInspector]
    public GalaxyInit galProp;

    DistributionFunction df; //Function to distribute stars
    
    //Main create function that initalizes all of the dust/stars
    private void Create(){    
        //Calling each create function
        CreateStars();
        CreateDust();
        CreateHII();
    }

    //Each of the create functions run through the data we generate and then assign each particle's position and colour based on it
    private void CreateStars(){
        for (int i = 0; i < galProp.numStars; i++) {
            points[i].position = Compute(stars[i].angle, stars[i].a, stars[i].b, stars[i].theta, galProp.perturbations, galProp.amplitudePert);
            points[i].startSize = Random.Range (1f, 1f);
            points[i].remainingLifetime = 10000000.0f;
            points[i].angularVelocity = stars[i].angularVelocity;
            points[i].velocity = new Vector3(1.0f, 0.0f, 0.0f);
            Color col = ColorFromTemperature(stars[i].temp);
            //points[i].startColor = new Color(.2f + (col.r * stars[i].mag), .2f + (col.g*stars[i].mag), .2f +(col.b*stars[i].mag));
            points[i].startColor = new Color(col.r, col.g, col.b, stars[i].mag);
        }
    }

    private void CreateDust(){
        for(int i = 0; i < galProp.numDust; i++){
            dustParticles[i].position = Compute(dust[i].angle, dust[i].a, dust[i].b, dust[i].theta, galProp.perturbations, galProp.amplitudePert);
            dustParticles[i].startSize = galProp.dustRenderSize;
            dustParticles[i].remainingLifetime = 100000.0f;
            dustParticles[i].angularVelocity = dust[i].angularVelocity;
            Color col = ColorFromTemperature(dust[i].temp);
            //dustParticles[i].startColor = new Color(.2f + (col.r * dust[i].mag), .2f + (col.g*dust[i].mag), .2f +(col.b*dust[i].mag));
            dustParticles[i].startColor = new Color(col.r, col.g, col.b, dust[i].mag);
        }
    }

    private void CreateHII(){
        for(int i = 0; i < galProp.numHII; i++){
            HIIParticles[i].position = Compute(HII[i].angle, HII[i].a, HII[i].b, HII[i].theta, galProp.perturbations, galProp.amplitudePert);
            HIIParticles[i].startSize = galProp.HIIRenderSize;
            HIIParticles[i].remainingLifetime = 100000.0f;
            HIIParticles[i].angularVelocity = HII[i].angularVelocity;
            Color col = ColorFromTemperature(HII[i].temp);
            //HIIParticles[i].startColor = new Color(.2f + (col.r * dust[i].mag), .2f + (col.g*dust[i].mag), .2f +(col.b*dust[i].mag));
            HIIParticles[i].startColor = new Color(col.r, col.g, col.b, HII[i].mag);
        }
    }

    //Using a pre-made lookup array full of color values, it takes a temperature and then gets the corresponding colour
    Color ColorFromTemperature(float temp){
        int i = (int)temp;
        i /= 100;
        i += galProp.colourOffset; //If we want to shift the colors up or down we can do that here
        if(i > 120){
            Debug.Log("Need More Values!");
            i = 120;
        } else if(i < 10){
            Debug.Log("Color Values are too low!");
            i = 10;
        }
        return StarColorData.colors[i];
    }

    //This is called to upate the particles, mainly position.
    /*
    If you want to be able to interact with the galaxy live, undo all of the comments in the corresponding update(object)Pos functions
    I commented them out originally so they wouldn't kill performance, however if your computer is good it wont hamper it at all
    (You can always update particle speed live, however, to increase or decrease the movement speed of the particles)
    */
    void updateParticlePos(){

        updateStarPos();
        updateDustPos();
        updateHIIPos();

    }

    //Each update(object)Pos function needs to make sure the particle system has not been destroyed before setting the newly updated particle positions
    private void updateStarPos(){
        for (int i = 0; i < galProp.numStars; i++) {

            stars[i].theta += stars[i].angularVelocity;
            points[i].position = Compute(stars[i].angle, stars[i].a, stars[i].b, stars[i].theta, galProp.perturbations, galProp.amplitudePert);
            stars[i].angularVelocity = GetOrbitalVelocity(stars[i].a);
            
            //Color col = ColorFromTemperature(stars[i].temp);
            //points[i].startColor = new Color(.2f + (col.r * stars[i].mag), .2f + (col.g*stars[i].mag), .2f +(col.b*stars[i].mag));
            //points[i].startColor = new Color(col.r, col.g, col.b, stars[i].mag);

        }

        if (points != null) {
     
            theParticleSystem.SetParticles(points, points.Length); 
     
        }
    }

    private void updateDustPos(){
        for(int i = 0; i < galProp.numDust; i++){

            //dustParticles[i].startSize = galProp.dustRenderSize;
            dust[i].theta += dust[i].angularVelocity;
            dustParticles[i].position = Compute(dust[i].angle, dust[i].a, dust[i].b, dust[i].theta, galProp.perturbations, galProp.amplitudePert);
            dust[i].angularVelocity = GetOrbitalVelocity(dust[i].a);
            
            //Color col = ColorFromTemperature(dust[i].temp);
            //dustParticles[i].startColor = new Color(.2f + (col.r * dust[i].mag), .2f + (col.g*dust[i].mag), .2f +(col.b*dust[i].mag));
            //dustParticles[i].startColor = new Color(col.r, col.g, col.b, dust[i].mag);


        }

        if(dustParticles != null){
            theDustParticleSystem.SetParticles(dustParticles, dustParticles.Length);
        }
    }

    private void updateHIIPos(){
        for(int i = 0; i < galProp.numHII; i++){

            //HIIParticles[i].startSize = galProp.HIIRenderSize;
            HII[i].theta += HII[i].angularVelocity;
            HIIParticles[i].position = Compute(HII[i].angle, HII[i].a, HII[i].b, HII[i].theta, galProp.perturbations, galProp.amplitudePert);
            HII[i].angularVelocity = GetOrbitalVelocity(HII[i].a);
            
            //Color col = ColorFromTemperature(HII[i].temp);
            //HIIParticles[i].startColor = new Color(.2f + (col.r * dust[i].mag), .2f + (col.g*dust[i].mag), .2f +(col.b*dust[i].mag));
            //HIIParticles[i].startColor = new Color(col.r, col.g, col.b, HII[i].mag);


        }

        if(HIIParticles != null){
            theHIIParticleSystem.SetParticles(HIIParticles, HIIParticles.Length);
        }
    }

    // Update is called once per frame
    void Update () {
        updateParticlePos();
    }
     
    //The main initaite function to begin the main set up process
    public void Initiate() {
     
        //Stoping each particle system so they dont destroy our particles because they think their entitled to their own
        theParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting); 
        theDustParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        //setting up the distribution function so it can distribute thes stars
        df = new DistributionFunction();
        df.setUp(1.0f, .02f, galProp.galRad/3.0f, galProp.coreRad, 0.0f, galProp.radFarFeild, galProp.steps);

        initiateStars();
        initaiteDust();
        initaiteHII();
        Create();

        if (points != null) {
     
            theParticleSystem.SetParticles(points, points.Length); 
     
        }
        if(dustParticles != null){
            theDustParticleSystem.SetParticles(dustParticles, dustParticles.Length);
        }
        //theParticleSystem.Pause();
        
    
    }

    /*
    A very brief overview of each initiation function:
    For each star/dust/HII we:
    -Find the length of the major axis of it's orbit, then create an ellipse based off of that
    -We get the angle that the ellipse is tilted (anuglar offset)
    -Find the theta of where we are on the ellipse 
    -Find the angular velocity based on position
    -Find the temperature to determine color
    -and finally the magnitude to determine brightness
    Each function handles things a bit differently, but this is the main gist of it
    */
    private void initiateStars(){

        points = new ParticleSystem.Particle[galProp.numStars];
        stars = new GalObjectData[galProp.numStars];

        //We start out making a star at the center, core edge, and galaxy edge
        points[0].position = new Vector3(0, 0, 0);
        stars[0].a = 0.0f;
        stars[0].b = 0.0f;
        stars[0].angle = 0.0f;
        stars[0].theta = 0.0f;
        stars[0].temp = 6000.0f;
        stars[0].angularVelocity = GetOrbitalVelocity((stars[0].a + stars[0].b)/2.0f);

        //points[1].position = new Vector3(0, 0, 0);
        stars[1].a = galProp.coreRad;
        stars[1].b = galProp.coreRad * GetExcentricity(galProp.coreRad);
        stars[1].angle = GetAngularOffset(galProp.coreRad);
        stars[1].theta = 0.0f;
        stars[1].temp = 6000.0f;
        stars[1].angularVelocity = GetOrbitalVelocity((stars[0].a + stars[0].b)/2.0f);

        //points[2].position = new Vector3(0, 0, 0);
        stars[2].a = galProp.galRad;
        stars[2].b = galProp.galRad * GetExcentricity(galProp.galRad);
        stars[2].angle = GetAngularOffset(galProp.galRad);
        stars[2].theta = 0.0f;
        stars[2].temp = 6000.0f;
        stars[2].angularVelocity = GetOrbitalVelocity((stars[0].a + stars[0].b)/2.0f);

        float farRad = galProp.radFarFeild;
        farRad /= 100.0f;
        
        int min = 0;
        int max = galProp.numStars;

        for(int i = 3; i < galProp.numStars; ++i){

            float rad = df.ValFromProb(UnityEngine.Random.Range(min, max-1)/(float)max);

            stars[i].angle = GetAngularOffset(rad);
            stars[i].a = rad;
            stars[i].b = rad * GetExcentricity(rad);
            stars[i].theta = 360.0f * UnityEngine.Random.Range(min, max-1)/(float)max;
            stars[i].angularVelocity = GetOrbitalVelocity(rad);
            stars[i].temp = 6000 + (galProp.starBaseTemp * (UnityEngine.Random.Range(min, max-1)/(float)max) - galProp.starBaseTemp/2.0f);
            stars[i].mag = .3f + .2f * UnityEngine.Random.Range(min, max-1)/(float)max;
            
        }

    }

    private void initaiteDust(){
        float x,y,radius;
        int max = galProp.numDust;
        int min = 0;

        dustParticles = new ParticleSystem.Particle[galProp.numDust];
        dust = new GalObjectData[galProp.numDust]; 

        for(int i = 0; i < galProp.numDust; ++i){

            if(i % 4 == 0){

                radius = df.ValFromProb(UnityEngine.Random.Range(min, max-1)/(float)max);

            } else {
                x = 2.0f * galProp.galRad * (UnityEngine.Random.Range(min, max-1)/(float)max) - galProp.galRad;
                y = 2.0f * galProp.galRad * (UnityEngine.Random.Range(min, max-1)/(float)max) - galProp.galRad;
                radius = Mathf.Sqrt(x*x + y*y);
            }

            dust[i].a = radius;
            dust[i].b = radius * GetExcentricity(radius);
            dust[i].angle = GetAngularOffset(radius);
            dust[i].theta = 360*UnityEngine.Random.Range(min, max-1)/(float)max;
            dust[i].angularVelocity = GetOrbitalVelocity((dust[i].a + dust[i].b)/2.0f);
            
            dust[i].temp = galProp.dustTempBase + radius/4.5f;
            //dust[i].temp = galProp.dustTempBase + (galProp.dustTempBase * (UnityEngine.Random.Range(min, max-1)/(float)max)) - galProp.dustTempBase/2.0f;


            dust[i].mag = .015f + .02f * UnityEngine.Random.Range(min, max-1)/(float)max;

        }
    }

    //The function to find the temperature is mostly experimental backed up with no math. It's a bit hard to change, but I just liked the way it looked
    private void initaiteHII(){
        HII = new GalObjectData[galProp.numHII];
        HIIParticles = new ParticleSystem.Particle[galProp.numHII];

        int max = galProp.numHII;
        int min = 0;
        float x,y, radius;

        for(int i = 0; i < galProp.numHII/2; ++i){

            x = 2*(galProp.galRad) * UnityEngine.Random.Range(min, max-1)/(float)max - galProp.galRad;
            y = 2*(galProp.galRad) * UnityEngine.Random.Range(min, max-1)/(float)max - galProp.galRad;
            radius = Mathf.Sqrt(x*x + y*y);

            int k1 = 2*i;
            HII[k1].a = radius;
            HII[k1].b = radius * GetExcentricity(radius);
            HII[k1].angle = GetAngularOffset(radius);
            HII[k1].theta = 360.0f * UnityEngine.Random.Range(min, max-1)/(float)max;
            HII[k1].angularVelocity = GetOrbitalVelocity((HII[k1].a + HII[k1].b)/2.0f);
            HII[k1].temp = 2000.0f + (2000.0f * UnityEngine.Random.Range(min, max-1)/(float)max) - 1000.0f;
            HII[k1].mag = (HII[k1].a > galProp.coreRad + 5.0f  || HII[k1].a > galProp.radFarFeild - 5.0f) ? 1.0f/(1.0f + Mathf.Exp((radius - (galProp.coreRad + galProp.galRad)/2.0f) * .05f)) : 
            0.0f;


            int dist = 100;
            int k2 = 2*i + 1;
            HII[k2].a = radius + dist;
            HII[k2].b = radius * GetExcentricity(radius);
            HII[k2].angle = GetAngularOffset(radius);
            HII[k2].theta = 360.0f * UnityEngine.Random.Range(min, max-1)/(float)max;
            HII[k2].angularVelocity = GetOrbitalVelocity((HII[k1].a + HII[k1].b)/2.0f);
            HII[k2].temp = 2000.0f + (2000.0f * UnityEngine.Random.Range(min, max-1)/(float)max) - 1000.0f;
            HII[k1].mag = (HII[k2].a > galProp.coreRad + 5.0f || HII[k2].a > galProp.radFarFeild - 5.0f) ? 1.0f/(1.0f + Mathf.Exp((radius - (galProp.coreRad + galProp.galRad)/2.0f) * .05f)) : 
            0.0f;


        }
    }

    //The velocity based off of gravity and how it interacts
    float GetOrbitalVelocity(float rad){
        float kms = 0;

        kms = VelocityCurve.v(rad, galProp);

        float u = 2.0f * Mathf.PI * rad * (3.086f * Mathf.Pow(10, 13)); //Parsecs to Kilometers
        float time = u / (kms * (3.154f * Mathf.Pow(10, 7))); //Seconds in a year

        float val = 360.0f/time;
        val *= galProp.velocityScaling;

        //Debug.Log(val);
        return val;
    }

    //The excentricity is different off of where it is in the galaxy, determined by this
    float GetExcentricity(float r){
        if(r < galProp.coreRad){
            return 1 + (r/galProp.coreRad) * (galProp.coreEdgeEcc - 1);
        }
        else if(r > galProp.coreRad && r <= galProp.galRad){
            return galProp.coreEdgeEcc + (r - galProp.coreRad) / (galProp.galRad - galProp.coreRad) * (galProp.galEdgeEcc - galProp.coreEdgeEcc);
        }
        else if(r > galProp.galRad && r < galProp.radFarFeild){
            return galProp.galEdgeEcc + (r - galProp.galRad)/(galProp.radFarFeild - galProp.galRad) * (1-galProp.galEdgeEcc);
        }
        else{
            return 1;
        }
    }

    float GetAngularOffset(float r){
        return Mathf.Exp(r * galProp.angularOffset);
    }

    //Computing position based on the rotation of an ellipse and a few others
    Vector3 Compute(float angle, float a, float b, float theta, int pertN, float pertAmp){
        float alpha = angle;
        float beta = theta * Mathf.Deg2Rad;
        //float alpha = galProp.alphaValue;

        float cosAlpha = Mathf.Cos(alpha);
        float sinAlpha = Mathf.Sin(alpha);
        float cosBeta = Mathf.Cos(beta);
        float sinBeta = Mathf.Sin(beta);

        float x = (a * cosAlpha * cosBeta - b * sinAlpha * sinBeta);
        float y = (a * cosAlpha * sinBeta + b * sinAlpha * cosBeta);

        float z = 0;

        if(pertAmp > 0 && pertN > 0){
            x += (a/pertAmp) * Mathf.Sin(alpha * 2 * pertN);
            y += (a/pertAmp) * Mathf.Cos(alpha * 2 * pertN);
        }

        Vector3 pos = new Vector3(x,y,z);

        return pos;

    }

    GalaxyInit getGalProp(){
        return galProp;
    }

    

    struct VelocityCurve{
        
        public static float MS(float r, GalaxyInit galProp){
            float d = galProp.coreRad/2.0f;
            float rho_so = 1.0f;
            float rH = galProp.coreRad/2.0f;
            return rho_so*Mathf.Exp(-r/rH) * (r * r) * Mathf.PI * d;
        }

        public static double MH(float r, GalaxyInit galProp){
            float rho = .15f;
            float rC = 25.0f;
            return rho * (1/(1 + Mathf.Pow(r/rC, 2) * (4 * Mathf.PI * Mathf.Pow(r,3)/3)));
        }

        public static float v(float r, GalaxyInit galProp){
            float MZ = 100.0f;
            float G = 6.672f * Mathf.Pow(10, -11);

            return 20000 * Mathf.Sqrt((float)(G * (MH(r, galProp) + MS(r, galProp) + MZ)/r));

        }

        public static float vd(float r, GalaxyInit galProp){
            float MZ = 100.0f;
            float G = 6.672f * Mathf.Pow(10, -11);
            return 20000 * Mathf.Sqrt((G*MS(r, galProp) + MZ)/r);
        }

    }

    public struct GalObjectData{
        public float a;
        public float b;
        public float angle;
        public float theta;
        public float temp;
        public float mag;
        public float angularVelocity;
    }

}