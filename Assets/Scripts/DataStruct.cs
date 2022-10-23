using UnityEngine;


public struct MoveData
{
    public float XAxis;
    public float ZAxis;

    public MoveData(float xAxis, float zAxis)
    {
        XAxis = xAxis;
        ZAxis = zAxis;
    }
}

public struct ReconcileMoveData
{
    public Vector3 Position;
    public Quaternion Rotation;

    public ReconcileMoveData(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public struct AbilityData
{
    public float XAxis;
    public float ZAxis;

    public AbilityData(float xAxis, float zAxis)
    {
        XAxis = xAxis;
        ZAxis = zAxis;
    }
}