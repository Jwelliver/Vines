using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public struct ControlTextByPlatform
{
    // struct to easily preset and use text that differs by platform (keyboard vs mobile).
    // TODO: you can flesh this out into a class that returns the name of the control based on the currentControlScheme(which you can set somwhere)
    //  todo: ...and get the binding based on the action. 

    // public static string GetBindingByAction(string actionName) {
    //     InputAction action = PlayerInput.customInput.FindAction(actionName);
    // }

    public string keyboard;
    public string mobile;

    public string GetText()
    { //TODO: Move platform check to dedicated class in order to handle complex checks like ipad on webgl
        return GameContext.PlayerSettings.useTouchScreenControls ? mobile : keyboard;
    }



}


public static class ControlText
{

    public static InputControlScheme activeControlScheme;


}




public class GameplayUIController : MonoBehaviour
{
    //maintain references to all text and UI components used during gameplay
    // handle enabling/disabling UI components as needed (e.g. mobile-only UI and when a UI should be active/interactable)
    // handle setting up text and UI elements per the platform (e.g. "Press R" vs "Tap")

    /*
        List:
            - You died menu
                - provide enable/disable
                - set text based on platform
                - 
            - fps Text
                - enable/disable (FPS script will handle update)
            - new record text
                - 
            - "amulet"/player text
                - handle 

            - pause menu
                - 

    */

    // FPS text
    public static bool fpsTextEnabled; // just going to handle the enabled value here and let the script contain the text obj



}
