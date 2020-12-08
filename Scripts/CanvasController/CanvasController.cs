﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public static bool HexInfoDisplayed = false;

    public GameObject HexInfo;

    public int currentPlanetDisplayed;

    public int currentUnitDisplayed;

    public static bool wasInteractedWith = false;
    
    void Start(){
        currentPlanetDisplayed = 0;
        currentUnitDisplayed = 0;
    }

    public static bool checkWasInteracted(){
        if(wasInteractedWith){
            wasInteractedWith = false;
            return true;
        } else {
            return false;
        }
    }
    
    public void DisplayHexInfo(){
        HexInfo.SetActive(true);
        HexInfoDisplayed = true;
    }

    public void GetRidOfHexInfo(){
        HexInfo.SetActive(false);
        HexInfoDisplayed = false;
    }

    public void IncrementPlanetIndex(){
        currentPlanetDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void DecrementPlanetIndex(){
        currentPlanetDisplayed--;
        if(currentPlanetDisplayed < 0){
            currentPlanetDisplayed = MainController.displayingHex.GetPlanetsLength() - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void IncrementShipIndex(){
        currentUnitDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void DecrementShipIndex(){
        currentUnitDisplayed--;
        if(currentUnitDisplayed < 0){
            currentUnitDisplayed = MainController.displayingHex.ShipsOnHex.Count - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void DisplayHex(Hex clickedHex){
        UpdateHexInfo(clickedHex);
        UpdateEmpireInfo(clickedHex);
        UpdateUnitInfo(clickedHex);
        DisableOrEnableButtons(clickedHex);
        MainController.DisableInteractions();
    }

    private void UpdateUnitInfo(Hex hex){
        ChangeShipDisplayText(hex);
        ChangeShipDisplayLetter(hex);
        UpdateShipList(hex);
        UpdateMoveButton(hex);
    }

    private void UpdateEmpireInfo(Hex hex){
        ChangeEmpireDisplayColor(hex);
        ChangeEmpireDisplayText(hex);
    }

    private void UpdateHexInfo(Hex clickedHex){

        ChangePlanetDisplayedColor(clickedHex.GetPlanetColor(currentPlanetDisplayed));
        ShowPlanetInfoText(clickedHex);
        UpdatePlanetScrollText(clickedHex);
        CheckIfColonizable(clickedHex);

    }

    private void CheckIfColonizable(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(4).gameObject;

        Button button = gameObject3.GetComponent<Button>();
        if(hex is SystemHex){
            SystemHex sys = (SystemHex)hex;
            if(!sys.planets[currentPlanetDisplayed%sys.planets.Length].Colonized && sys.CheckForColonyShip()){
                button.interactable = true;
                return;
            }
        }
        button.interactable = false;
    }

    private void UpdatePlanetScrollText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(3).gameObject;

        Text text = gameObject3.GetComponent<Text>();
        if(hex.GetType() == "System"){
            text.text = "" + (currentPlanetDisplayed%hex.GetPlanetsLength()+1) + "/" + hex.GetPlanetsLength();
        } else {
            text.text = "";
        }
        

        
    }

    private void ChangePlanetDisplayedColor(Color color){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;


        Image image = gameObject2.GetComponent<Image>();
        image.color = color;

    }

    private void ShowPlanetInfoText(Hex hex){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        
        Text text = gameObject2.GetComponentInChildren<Text>();

        text.text = hex.GetPlanetString(currentPlanetDisplayed);

    }

    private void ChangeEmpireDisplayColor(Hex hex){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(1).gameObject;

        Image image = gameObject2.GetComponent<Image>();
        if(hex.ControllingEmpire != null){
            image.color = hex.ControllingEmpire.empireColor;
        } else {
            image.color = new Color(0,0,0,0);
        }

    }

    private void ChangeEmpireDisplayText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(1).gameObject;
        
        Text text = gameObject2.GetComponentInChildren<Text>();

        if(hex.ControllingEmpire != null){
            text.text = hex.ControllingEmpire.GetText();
        } else {
            text.text = Empire.GetDefaultText();
        }
    }

    private void ChangeShipDisplayText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;



        Text text = gameObject3.GetComponent<Text>();

        text.text = hex.GetShipInfo(currentUnitDisplayed);
    }

    private void ChangeShipDisplayLetter(Hex hex){
        
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        text.text = hex.GetShipLetter(currentUnitDisplayed);

    }

    private void UpdateShipList(Hex hex){
        
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(1).gameObject;

        Text text = gameObject3.GetComponent<Text>();
        if(hex.GetShipCount() != 0){
            text.text = "" + (currentUnitDisplayed%hex.GetShipCount()+1) + "/" + hex.GetShipCount();
        } else {
            text.text = "";
        }
    }

    private void DisableOrEnableButtons(Hex hex){
        bool b = hex.GetType() == "System" && hex.GetPlanetsLength() > 1;
        DisableOrEnablePlanetRightButton(hex, b);
        DisableOrEnablePlanetLeftButton(hex, b);

        b = hex.GetShipCount() != 0;
        DisableOrEnableUnitRightButton(hex, b);
        DisableOrEnableUnitLeftButton(hex, b);
    }

    private void DisableOrEnablePlanetRightButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(1).gameObject;

        gameObject3.SetActive(b);
    }

    private void DisableOrEnablePlanetLeftButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(2).gameObject;

        gameObject3.SetActive(b);
    }

    private void DisableOrEnableUnitRightButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(2).gameObject;

        gameObject3.SetActive(b);
    }

    private void DisableOrEnableUnitLeftButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(3).gameObject;

        gameObject3.SetActive(b);
    }

    private void UpdateMoveButton(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(4).gameObject;

        gameObject3.SetActive(hex.ShipsOnHex.Count >= 1);

    }

    /*********************************************************************Interacting With Ships**********************************************************************************/
    public void MoveShip(){
        Ship ship = MainController.displayingHex.ShipsOnHex[currentUnitDisplayed % MainController.displayingHex.ShipsOnHex.Count];
        MainController.RequestMovement(ship);
        MainController.EnableInteractions();
    }

    public void ColonizePlanet(){
        SystemHex sys = (SystemHex)MainController.displayingHex;
        if(sys != null){
            MainController.Colonize(sys, currentPlanetDisplayed);
        } else {
            Debug.LogError("Error! You have asked to colonize a planet on a tile that isnt a system! Is the button working properly?");
        }
    }

}