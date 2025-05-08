using Godot;

public partial class SpecCam : Camera3D
{
    [Export] public float MoveSpeed = 5.0f; // Movement speed of the camera
    [Export] public float MouseSensitivity = 0.01f; // Mouse sensitivity for rotation (radians per pixel)

    private Vector2 _mouseDelta = Vector2.Zero; // Stores the relative mouse movement

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // If this camera is set as current in the editor, capture the mouse
        if (IsCurrent())
        {
            //Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    // Called when an input event occurs.
    public override void _Input(InputEvent @event)
    {

        // Toggle camera state on "debug" action
        if (@event.IsActionPressed("debug"))
        {
            GD.Print("Debug action pressed!"); // Check if action is detected
            if (IsCurrent())
            {
                // If this is the current camera, make it non-current and release the mouse
                ClearCurrent(); 
                Input.MouseMode = Input.MouseModeEnum.Visible;
                GD.Print("SpecCam Deactivated. MouseMode: " + Input.MouseMode);
            }
            else
            {
                // If this is not the current camera, make it current and capture the mouse
                MakeCurrent();
                Input.MouseMode = Input.MouseModeEnum.Captured;
                GD.Print("SpecCam Activated. MouseMode: " + Input.MouseMode);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // GD.Print($"IsCurrent: {IsCurrent()}, MouseMode: {Input.MouseMode}, MouseDelta: {_mouseDelta}"); // Uncomment for continuous status

        // Only process if this camera is the current one
        if (!IsCurrent())
        {
            _mouseDelta = Vector2.Zero; // Reset mouse delta if not active
            return;
        }

        // Camera rotation based on mouse movement
        if (_mouseDelta.LengthSquared() > 0)
        {
            RotateY(-_mouseDelta.X * MouseSensitivity); // Yaw (left/right)
            RotateObjectLocal(Vector3.Right, -_mouseDelta.Y * MouseSensitivity); // Pitch (up/down)

            // Clamp vertical rotation (pitch) to prevent flipping
            Vector3 currentRotation = RotationDegrees;
            currentRotation.X = Mathf.Clamp(currentRotation.X, -89.9f, 89.9f);
            RotationDegrees = currentRotation;

            _mouseDelta = Vector2.Zero; // Reset mouse delta after applying rotation
        }

        // Camera movement based on input actions
        Vector3 inputDir = Vector3.Zero;
        if (Input.IsActionPressed("Forward")) // Define in Input Map (e.g., W)
            inputDir.Z -= 1;
        if (Input.IsActionPressed("Backward")) // Define in Input Map (e.g., S)
            inputDir.Z += 1;
        if (Input.IsActionPressed("Left")) // Define in Input Map (e.g., A)
            inputDir.X -= 1;
        if (Input.IsActionPressed("Right")) // Define in Input Map (e.g., D)
            inputDir.X += 1;
        if (Input.IsActionPressed("Up")) // Define in Input Map (e.g., Space or E)
            inputDir.Y += 1;
        if (Input.IsActionPressed("Down")) // Define in Input Map (e.g., Shift or Q)
            inputDir.Y -= 1;
		if(Input.IsActionPressed("E")) // Define in Input Map (e.g., Space)
			RotationDegrees -= new Vector3(0, 0.4f, 0);
		if(Input.IsActionPressed("A")) // Define in Input Map (e.g, Shift)
			RotationDegrees += new Vector3(0, 0.4f, 0);
        if (inputDir != Vector3.Zero)
        {
            inputDir = inputDir.Normalized(); // Normalize for consistent speed in all directions
            // Translate the camera relative to its own orientation
            GlobalTranslate(Basis.X * inputDir.X * MoveSpeed * (float)delta);
            GlobalTranslate(Basis.Y * inputDir.Y * MoveSpeed * (float)delta);
            GlobalTranslate(Basis.Z * inputDir.Z * MoveSpeed * (float)delta);
        }
    }
}
