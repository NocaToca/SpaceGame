using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//The starport is one of the most complex buildings. It is required to build ships on a planet and splits the production of the planet to build the ships.
//It will be able to be upgraded by building other buildings to improve it
public class Starport : Building
{
    //We want to override but use the same name as our building cost
    new public static Resource buildingCost = new Resource(10.0f, 20.0f);

    public List<Ship> ShipQueue = new List<Ship>(); //What ships are in our queue

    //How much production is going towards the starport 
    public float percentTowardsStarport = 0.5f;

    //The current amount of production we have produced towards the current ship we are making
    float productionAmount = 0.0f;

    public Starport(){
        Initialize();
    }

    public Starport(HexCoordinates pos){
        Initialize();
        this.pos = pos;
    }

    //To build a starport, you'd need the tech "Revolutionized Spaceflight", so we need to make sure we add that
    private void Initialize(){
        cost = new Resource();
        cost.SetGold(-1.0f);
        cost.SetProduction(2.0f);
        name = "Starport";
        singleBuildingCost = buildingCost;
        TechPrereq.Add(Tech.GetRSF());
    }

    //If we didnt set it in our construction, this is done here
    public void SetPos(HexCoordinates pos){
        this.pos = pos;
    }

    //The build function for our Starport
    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new Starport());
    }

    //Returning the base structure for our building
    new public virtual Starport Base(){
        return new Starport();
    }

    //Handling our build queue much like we handle it with planets
    public void BuildQueue(float production){
        
        productionAmount += production * percentTowardsStarport;
        if(ShipQueue.Count != 0){
            if(productionAmount == ShipQueue[0].GetCost().Production){
                productionAmount = 0;
                //CreateShip
                Board.AddShip(ShipQueue[0], Board.GetEmpireThatControlsHex(Board.GetHexFromHexCoords(pos)));
                ShipQueue.Remove(ShipQueue[0]);
            } else
            if (productionAmount > ShipQueue[0].GetCost().Production){
                productionAmount -= ShipQueue[0].GetCost().Production;
                //CreateShip
                Board.AddShip(ShipQueue[0], Board.GetEmpireThatControlsHex(Board.GetHexFromHexCoords(pos)));
                ShipQueue.Remove(ShipQueue[0]);
            }
        } else {
            productionAmount = 0;
        }
        //Debug.Log(productionAmount);
    }

    //Adds a ship to queue
    public void AddToQueue(Ship ship){
        ShipQueue.Add(ship);
    }
    //Removes a ship from queue
    public void RemoveFromQueue(Ship ship){
        ShipQueue.Remove(ship);
    }
}
