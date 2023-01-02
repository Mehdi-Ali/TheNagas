using UnityEngine;

enum Abilities
{
    First,
    Second,
    Third,
    Ultimate
}

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
