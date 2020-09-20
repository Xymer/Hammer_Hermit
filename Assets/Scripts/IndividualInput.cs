using UnityEngine;
internal enum PlayerButton { A, B, X, Y, LeftBumper, RightBumper, LeftTrigger, RightTrigger, Start, Select, Left, Right, Up, Down }
public class IndividualInput
{
    private string horizontalAxis;
    private string verticalAxis;
    private string triggerAxis;

    private string aButton;
    private string bButton;
    private string xButton;
    private string yButton;

    private string leftBumper;
    private string rightBumper;

    private string startButton;
    private string selectButton;

    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public float Trigger { get; set; }

    private float lastHorizontalValue;
    private float lastVerticalValue;
    private float lastTriggerValue;

    private bool horizontalHeld;
    private bool verticalHeld;
    private bool triggerHeld;

    public int controllerNumber;
    public bool HasControllerAssigned { get { return controllerNumber > 0; } }
    public bool ControllerIsGamepad { get { return controllerNumber > 3; } }

    public IndividualInput()
    {

    }

    internal bool IsButtonDown(PlayerButton p_button)
    {
        if (!HasControllerAssigned)
            return false;
        bool pressedTheButton = false;
        switch (p_button)
        {
            case PlayerButton.A:
                pressedTheButton = Input.GetButtonDown(aButton);
                break;
            case PlayerButton.B:
                pressedTheButton = Input.GetButtonDown(bButton);
                break;
            case PlayerButton.X:
                pressedTheButton = Input.GetButtonDown(xButton);
                break;
            case PlayerButton.Y:
                pressedTheButton = Input.GetButtonDown(yButton);
                break;
            case PlayerButton.LeftBumper:
                pressedTheButton = Input.GetButtonDown(leftBumper);
                break;
            case PlayerButton.RightBumper:
                pressedTheButton = Input.GetButtonDown(rightBumper);
                break;
            case PlayerButton.LeftTrigger:
                pressedTheButton = (Trigger < 0 && !triggerHeld) ? true : false;
                break;
            case PlayerButton.RightTrigger:
                pressedTheButton = (Trigger > 0 && !triggerHeld) ? true : false;
                break;
            case PlayerButton.Start:
                pressedTheButton = Input.GetButtonDown(startButton);
                break;
            case PlayerButton.Select:
                pressedTheButton = Input.GetButtonDown(selectButton);
                break;
            case PlayerButton.Left:
                pressedTheButton = (Horizontal < 0 && !horizontalHeld) ? true : false;
                break;
            case PlayerButton.Right:
                pressedTheButton = (Horizontal > 0 && !horizontalHeld) ? true : false;
                break;
            case PlayerButton.Up:
                pressedTheButton = (Vertical > 0 && !verticalHeld) ? true : false;
                break;
            case PlayerButton.Down:
                pressedTheButton = (Vertical < 0 && !verticalHeld) ? true : false;
                break;
            default:
                break;
        }

        return pressedTheButton;
    }

    internal bool IsButtonHeld(PlayerButton p_button)
    {
        if (!HasControllerAssigned)
            return false;
        bool pressedTheButton = false;
        switch (p_button)
        {
            case PlayerButton.A:
                pressedTheButton = Input.GetButton(aButton);
                break;
            case PlayerButton.B:
                pressedTheButton = Input.GetButton(bButton);
                break;
            case PlayerButton.X:
                pressedTheButton = Input.GetButton(xButton);
                break;
            case PlayerButton.Y:
                pressedTheButton = Input.GetButton(yButton);
                break;
            case PlayerButton.LeftBumper:
                pressedTheButton = Input.GetButton(leftBumper);
                break;
            case PlayerButton.RightBumper:
                pressedTheButton = Input.GetButton(rightBumper);
                break;
            case PlayerButton.LeftTrigger:
                pressedTheButton = Trigger < 0;
                break;
            case PlayerButton.RightTrigger:
                pressedTheButton = Trigger > 0;
                break;
            case PlayerButton.Start:
                pressedTheButton = Input.GetButton(startButton);
                break;
            case PlayerButton.Select:
                pressedTheButton = Input.GetButton(selectButton);
                break;
            case PlayerButton.Left:
                pressedTheButton = Horizontal < 0 ? true : false;
                break;
            case PlayerButton.Right:
                pressedTheButton = Horizontal > 0 ? true : false;
                break;
            case PlayerButton.Up:
                pressedTheButton = Vertical > 0 ? true : false;
                break;
            case PlayerButton.Down:
                pressedTheButton = Vertical < 0 ? true : false;
                break;
            default:
                break;
        }

        return pressedTheButton;
    }
    internal bool IsButtonUp(PlayerButton p_button)
    {
        bool pressedTheButton = false;
        switch (p_button)
        {
            case PlayerButton.A:
                pressedTheButton = Input.GetButton(aButton);
                break;
            case PlayerButton.B:
                pressedTheButton = Input.GetButton(bButton);
                break;
            case PlayerButton.X:
                pressedTheButton = Input.GetButton(xButton);
                break;
            case PlayerButton.Y:
                pressedTheButton = Input.GetButton(yButton);
                break;
            case PlayerButton.LeftBumper:
                pressedTheButton = Input.GetButton(leftBumper);
                break;
            case PlayerButton.RightBumper:
                pressedTheButton = Input.GetButton(rightBumper);
                break;
            case PlayerButton.LeftTrigger:
                pressedTheButton = Trigger < 0;
                break;
            case PlayerButton.RightTrigger:
                pressedTheButton = Trigger > 0;
                break;
            case PlayerButton.Start:
                pressedTheButton = Input.GetButton(startButton);
                break;
            case PlayerButton.Select:
                pressedTheButton = Input.GetButton(selectButton);
                break;
            case PlayerButton.Left:
                pressedTheButton = Horizontal < 0 ? true : false;
                break;
            case PlayerButton.Right:
                pressedTheButton = Horizontal > 0 ? true : false;
                break;
            case PlayerButton.Up:
                pressedTheButton = Vertical > 0 ? true : false;
                break;
            case PlayerButton.Down:
                pressedTheButton = Vertical < 0 ? true : false;
                break;
            default:
                break;
        }

        return pressedTheButton;
    }

    internal bool AnyDirectionsPressed()
    {
        if (!horizontalHeld && Horizontal != 0)
            return true;
        else if (!verticalHeld && Vertical != 0)
            return true;
        return false;
    }

    internal bool AnyDirectionsHeld()
    {
        if (Horizontal != 0)
            return true;
        else if (Vertical != 0)
            return true;
        return false;
    }

    internal void SetControllerOption(int p_number)
    {
        controllerNumber = p_number;
        Debug.Log("Controller number " + controllerNumber);

        horizontalAxis = controllerNumber + " Horizontal";
        verticalAxis = controllerNumber + " Vertical";

        aButton = controllerNumber + " A";
        bButton = controllerNumber + " B";
        xButton = controllerNumber + " X";
        yButton = controllerNumber + " Y";

        triggerAxis = controllerNumber + " Triggers";
    }

    public void UpdateAxisValues()
    {
        if (HasControllerAssigned)
        {
            Horizontal = Input.GetAxisRaw(horizontalAxis);
            horizontalHeld = lastHorizontalValue == Horizontal;

            Trigger = Input.GetAxisRaw(triggerAxis);
            triggerHeld = lastTriggerValue == Trigger;

            lastHorizontalValue = Horizontal;
            lastVerticalValue = Vertical;
            lastTriggerValue = Trigger;
        }
    }
}
