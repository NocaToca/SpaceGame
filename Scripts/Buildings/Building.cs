using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    //Our base resources in buildings, along with the position of where the building is
   public Resources cost;
   public static Resources buildingCost;
   public HexCoordinates pos;

    //returns the building cost
   public Resources GetBuildingCost(){
       return buildingCost;
   }

}
