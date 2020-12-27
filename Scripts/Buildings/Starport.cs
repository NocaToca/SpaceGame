using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starport : Building
{
    //We want to override but use the same name as our building cost
    new public static Resources buildingCost = new Resources(10.0f, 20.0f);

    public List<Ship> ShipQueue = new List<Ship>();

    public float percentTowardsStarport = 0.5f;

    float productionAmount = 0.0f;

    public Starport(){
        Initialize();
    }

    public Starport(HexCoordinates pos){
        Initialize();
        this.pos = pos;
    }

    private void Initialize(){
        cost = new Resources();
        cost.SetGold(-1.0f);
        cost.SetProduction(2.0f);
        name = "Starport";
        singleBuildingCost = buildingCost;
        TechPrereq.Add(Tech.GetRSF());
    }

    public void SetPos(HexCoordinates pos){
        this.pos = pos;
    }

    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new Starport());
   }

   public virtual Starport Base(){
       return new Starport();
   }

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
