using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech{
    List<Tech> prereq = new List<Tech>();

    public float science;
    public string name;

    public Tech(string name, float science, List<Tech> prereq){
        this.name = name;
        this.science = science;
        this.prereq = prereq;
    }

    public void AddToQueue(){
        GameMode.AddToTechQueue(this, Board.GetPlayerEmpire());
        CanvasController.Clear();
    }

    public static Tech GetRSF(){
        return new Tech("Revolutionized Space Flight", 10.0f, new List<Tech>());
    }

    public static Tech GetShipyards(){
        List<Tech> prereq = new List<Tech>();
        prereq.Add(GetRSF());
        return new Tech("Shipyards", 15.0f, prereq);
    }

    public static List<Tech> GetAllTechs(){
        List<Tech> techs = new List<Tech>();

        //Allows the building of starports
        Tech rsf = GetRSF();
        techs.Add(rsf);

        Tech shipyards = GetShipyards();
        techs.Add(shipyards);

        return techs;
    }

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
