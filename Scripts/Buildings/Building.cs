using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
   public Resources cost;
   public static Resources buildingCost;
   public HexCoordinates pos;

   public Resources GetBuildingCost(){
       return buildingCost;
   }

}
