using UnityEngine;
using UnityEngine.InputSystem;

// ai use: written with claude (anthropic) to move the game onto the input system package. owner: cthullu96
//
// every control in the game lives here, made with the input system package
// so the keys only need changing in one place and a gamepad works too
// scripts just ask an action here if its held or was pressed this frame
public static class GameInput
{
    // player
    public static readonly InputAction Move;
    public static readonly InputAction Sprint;
    public static readonly InputAction Jump;
    public static readonly InputAction Attack;

    // camera
    public static readonly InputAction RotateCamera;   // hold to spin the camera
    public static readonly InputAction Look;           // mouse movement while spinning
    public static readonly InputAction Zoom;           // scroll wheel

    // weapons
    public static readonly InputAction Weapon1;
    public static readonly InputAction Weapon2;
    public static readonly InputAction Reload;

    // abilities, picked in the hub's inventory
    public static readonly InputAction AbilityQ;
    public static readonly InputAction AbilityE;

    // food and drink slots, keys 4 to 9 then 0
    public static readonly InputAction[] UseSlot;

    // menus and other keys
    public static readonly InputAction Interact;
    public static readonly InputAction Inventory;
    public static readonly InputAction Achievements;
    public static readonly InputAction HideUI;
    public static readonly InputAction Menu;

    static GameInput()
    {
        // wasd, arrow keys or the left stick
        Move = new InputAction("Move", InputActionType.Value);
        Move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        Move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
        Move.AddBinding("<Gamepad>/leftStick");

        Sprint = Button("Sprint", "<Keyboard>/leftShift", "<Gamepad>/leftStickPress");
        Jump = Button("Jump", "<Keyboard>/space", "<Gamepad>/buttonSouth");
        Attack = Button("Attack", "<Mouse>/leftButton", "<Keyboard>/x", "<Gamepad>/rightTrigger");

        RotateCamera = Button("RotateCamera", "<Mouse>/rightButton");
        Look = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
        Zoom = new InputAction("Zoom", InputActionType.Value, "<Mouse>/scroll");

        Weapon1 = Button("Weapon1", "<Keyboard>/1");
        Weapon2 = Button("Weapon2", "<Keyboard>/2");
        Reload = Button("Reload", "<Keyboard>/r", "<Gamepad>/buttonEast");

        AbilityQ = Button("AbilityQ", "<Keyboard>/q", "<Gamepad>/leftShoulder");
        AbilityE = Button("AbilityE", "<Keyboard>/e", "<Gamepad>/rightShoulder");

        string[] slotKeys = { "4", "5", "6", "7", "8", "9", "0" };
        UseSlot = new InputAction[slotKeys.Length];
        for (int i = 0; i < slotKeys.Length; i++)
            UseSlot[i] = Button("UseSlot" + slotKeys[i], "<Keyboard>/" + slotKeys[i]);

        Interact = Button("Interact", "<Keyboard>/e", "<Gamepad>/buttonWest");
        Inventory = Button("Inventory", "<Keyboard>/i", "<Gamepad>/buttonNorth");
        Achievements = Button("Achievements", "<Keyboard>/j");
        HideUI = Button("HideUI", "<Keyboard>/h");
        Menu = Button("Menu", "<Keyboard>/escape", "<Gamepad>/start");

        // actions do nothing until theyre switched on
        Move.Enable();
        Look.Enable();
        Zoom.Enable();
        foreach (InputAction a in new[] { Sprint, Jump, Attack, RotateCamera, Weapon1, Weapon2, Reload, AbilityQ, AbilityE,
                                          Interact, Inventory, Achievements, HideUI, Menu })
            a.Enable();
        foreach (InputAction a in UseSlot)
            a.Enable();
    }

    // where the mouse is on screen, used for aiming
    public static Vector2 PointerPosition
    {
        get { return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero; }
    }

    // a button with one or more keys
    static InputAction Button(string name, params string[] bindings)
    {
        InputAction action = new InputAction(name, InputActionType.Button);
        foreach (string b in bindings)
            action.AddBinding(b);
        return action;
    }
}
