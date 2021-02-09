using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pops
{
    //Pops currently arent implemented though will be next. The current idea for them is to have independent resources for the type of pop, then being able to assign them to improve a planet's resource
    //production
    public Resource popResource;
    public string name;
    public Resource assignedResource;

    public List<PopModifiers> popMods = new List<PopModifiers>();

    public Pops(){
        name = "Joe Glyptodon";
    }
    public Pops(string name){
        this.name = name;
    }

    public void SetResource(Resource resource){
        assignedResource = resource;
    }

}

public class PopModifiers{

}