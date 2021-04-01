using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class dealing with all of our technologies
public class Tech{
    List<Tech> prereq = new List<Tech>();

    //how much sciene is required for the tech
    public float science;

    //The name of the tech
    public string name;

    //Initializing the techs
    public Tech(string name, float science, List<Tech> prereq){
        this.name = name;
        this.science = science;
        this.prereq = prereq;
    }

    //Adds a tech to queue
    public void AddToQueue(){
        GameMode.AddToTechQueue(this, Board.GetPlayerEmpire());
        CanvasController.buttonPress = true;
        CanvasController.Clear();
    }

    //Our revolutionary Space flight tech
    public static Tech GetRSF(){
        return new Tech("Revolutionized Space Flight", 10.0f, new List<Tech>());
    }

    //Our ship yard tech
    public static Tech GetShipyards(){
        List<Tech> prereq = new List<Tech>();
        prereq.Add(GetRSF());
        return new Tech("Shipyards", 15.0f, prereq);
    }

    //Returns all of the techs in the game
    public static List<Tech> GetAllTechs(){
        List<Tech> techs = new List<Tech>();

        //Allows the building of starports
        Tech rsf = GetRSF();
        techs.Add(rsf);

        Tech shipyards = GetShipyards();
        techs.Add(shipyards);

        return techs;
    }

    //Filters the available techs based off a list of acquired techs
    public static List<Tech> FilterTechs(List<Tech> acquiredTechs){
        List<Tech> alltTechs = GetAllTechs();
        List<string> names = new List<string>();
        for(int i = 0; i < acquiredTechs.Count; i++){
            names.Add(acquiredTechs[i].name);
        }
        for(int i = 0; i < alltTechs.Count; i++){
            if(names.Contains(alltTechs[i].name)){
                //names.Remove(alltTechs[i].name);
                alltTechs.Remove(alltTechs[i]);
                i--;
            } else {
                foreach(Tech prereq in alltTechs[i].prereq){
                    if(!names.Contains(prereq.name)){
                        names.Remove(alltTechs[i].name);
                        alltTechs.Remove(alltTechs[i]);
                        i--;
                        break;
                    }
                }
            }
            
        }
        return alltTechs;
    }
}
