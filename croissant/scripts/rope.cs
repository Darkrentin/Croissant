using Godot;
using System;
using System.Collections.Generic;

public partial class rope : Node2D
{

    private int _segments = 10;
    public List<RigidBody2D> Segment = new List<RigidBody2D>();

    PackedScene ropeSegmentScene;

    [Export] public StaticBody2D Anchor;
    [Export] public int Segments {
        get => _segments;
        set {
            _segments = value;
            SegmentValueChanged(value);
        }
    }

    public override void _Ready()
    {
        ropeSegmentScene = GD.Load<PackedScene>("res://scenes/rope_segment.tscn");

        Segment.Add(GetNode<RigidBody2D>("Anchor"));
        for(int i = 0; i<_segments; i++)
        {
            addSegment();
        }
        
    }

    private void SegmentValueChanged(int value)
    {
        GD.Print("New value: " + value);
    }

    public void addSegment()
    {
        if(Segment.Count <= 0)
        {
            RigidBody2D segment = (RigidBody2D)ropeSegmentScene.Instantiate<RigidBody2D>();
            Segment.Add(segment);
            AddChild(segment);
        }
        else
        {
            RigidBody2D lastSegment = Segment[Segment.Count - 1];
            PinJoint2D lastPinJoint = lastSegment.GetNode<PinJoint2D>("PinJoint2D");
            RigidBody2D segment = (RigidBody2D)ropeSegmentScene.Instantiate<RigidBody2D>();
            AddChild(segment);
            segment.GlobalPosition = lastPinJoint.GlobalPosition;
            lastPinJoint.NodeB = segment.GetPath();

            Segment.Add(segment);
        }
    }
}
