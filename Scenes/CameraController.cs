using Godot;
using System;

public partial class CameraController : Node3D
{
	public enum Direction
	{
		North = 0,
		East = 90,
		South = 180,
		West = 270,
	}

	private Direction _facing;
	public Direction Facing { get { return _facing; } set {
            if (_facing == value) return; //don't do anything if they're the same
            Direction old = _facing;
			_facing = value;
			UpdateRotation(old); 
		} }


	private Tween RotTween;

	[Export] Node3D TargetNode;
	[Export] float FollowSpeed = 1;
    [Export(PropertyHint.Flags, "X:1,Y:2,Z:4")]
    public int FreezeMovement { get; set; } = 0;

    [Export] private float TurnTime = 1.0f;

	private Camera3D _camera;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        //lerp position
        if (IsInstanceValid(TargetNode))
        {
            //get the position
            Vector3 targetPos = TargetNode.GlobalPosition;

            //set each of the things...
            if (FreezeMovement % 2 != 0) //X
            {
                targetPos.X = GlobalPosition.X;
            }
            if ((FreezeMovement >> 1) % 2 != 0) //Y
            {
                targetPos.Y = GlobalPosition.Y;
            }
            if ((FreezeMovement >> 2) % 2 != 0) //Z
            {
                targetPos.Z = GlobalPosition.Z;
            }

            GlobalPosition = GlobalPosition.Lerp(targetPos, FollowSpeed * (float)delta);
        }


    }

    public void UpdateRotation(Direction oldFacing)
	{
		if(!IsInstanceValid(RotTween))
		{
			RotTween = CreateTween();
		}
		else 
		{
            if (RotTween.IsValid())
			{
                RotTween.Kill();
            }
            RotTween = CreateTween();
        }

		float diff = (((float)_facing) - GlobalRotationDegrees.Y + 360) % 360;
		float dist = diff;
		float time = TurnTime;
        //CCW - do nothing, no fixing needed

        //GD.Print($"{_facing} from {oldFacing}: {diff}");

		//Don't fix 180 degree turns...
		bool allowFix = true;
		if(((_facing == Direction.North || _facing == Direction.South) && (oldFacing == Direction.South || oldFacing == Direction.North))
		 || ((_facing == Direction.East || _facing == Direction.West)  && (oldFacing == Direction.West  || oldFacing == Direction.East))
			) {
			allowFix = false;
		}

        if (diff > 180 && allowFix)
		{
			//CW -> fix from 270 to -90, or similar
			diff = diff - 360;
			dist = diff * -1;
        }

        //GD.Print($"FIXED => {_facing} from {oldFacing}: {diff}");

        RotTween.TweenProperty(this, "global_rotation_degrees", new Vector3(0, GlobalRotationDegrees.Y + diff, 0), TurnTime * dist / 90)
                .From(new Vector3(0, GlobalRotationDegrees.Y, 0));

		//RotTween.Finished += () => { while (GlobalRotationDegrees.Y < 0) { GlobalRotationDegrees += new Vector3(0, 360f, 0); } };



    }

	public void RotLeft()
	{
		switch(_facing)
		{
			case Direction.North:
				Facing = Direction.East;
				break;
            case Direction.East:
                Facing = Direction.South;
                break;
            case Direction.South:
                Facing = Direction.West;
                break;
            case Direction.West:
                Facing = Direction.North;
                break;
        }
	}

	public void RotRight()
	{
        switch (_facing)
        {
            case Direction.North:
                Facing = Direction.West;
                break;
            case Direction.West:
                Facing = Direction.South;
                break;
            case Direction.South:
                Facing = Direction.East;
                break;
            case Direction.East:
                Facing = Direction.North;
                break;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("CameraLeft"))
		{
			RotLeft();
		}
		else if (@event.IsActionPressed("CameraRight"))
		{
			RotRight();
		}
    }


   
}
