using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet
{
    public List<Ship> shipsInFleet = new List<Ship>();

    public Fleet(List<Ship> ships){
        shipsInFleet = ships;
    }
    public Fleet(Ship ship){
        shipsInFleet.Add(ship);
    }

    public void AddShip(Ship ship){
        shipsInFleet.Add(ship);
    }

    public int FleetSize(){
        int size = 0;
        foreach(Ship ship in shipsInFleet){
            size += ship.space;
        }
        return size;
    }

    public float CalculateAttackDamage(){
        float damage = 0.0f;
        foreach(Ship ship in shipsInFleet){
            damage += ship.damage;
        }
        return damage;
    }
    public float CalculateDefenseDamage(){
        float damage = 0.0f;
        foreach(Ship ship in shipsInFleet){
            damage += ship.damage;
        }
        return damage;
    }

    public void DealDamage(float damage){
        damage /= shipsInFleet.Count;
        List<int> indexesToRemove = new List<int>();
        for(int i = 0; i < shipsInFleet.Count; i++){
            shipsInFleet[i].health -= damage;
            if(shipsInFleet[i].health <= 0){
                indexesToRemove.Add(i);
            }
        }
        int timesRemoved = 0;
        foreach(int i in indexesToRemove){
            Board.DestroyShip(shipsInFleet[i-timesRemoved]);
            shipsInFleet.RemoveAt(i-timesRemoved);
            timesRemoved++;
        }
    }

    public int GetMovePoints(){
        int min = shipsInFleet[0].availableMovementPoints;
        foreach(Ship ship in shipsInFleet){
            if(ship.availableMovementPoints < min){
                min = ship.availableMovementPoints;
            }
        }
        return min;
    }

    public void Move(int points){
        foreach(Ship ship in shipsInFleet){
            ship.availableMovementPoints -= points;
        }
    }
}
