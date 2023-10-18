using FishNet.Object.Prediction;
using Unity.VisualScripting;
using UnityEngine;

enum Abilities
{
    First,
    Second,
    Third,
    Ultimate
}

public struct MoveData : IReplicateData
{
    public float XAxis;
    public float ZAxis;
    private uint _tick;

    public MoveData(float xAxis, float zAxis)
    {
        XAxis = xAxis;
        ZAxis = zAxis;
        _tick = default;
}

    public void Dispose() {}
    public uint GetTick() =>_tick ;
    public void SetTick(uint value) => _tick = value;
}

public struct ReconcileMoveData : IReconcileData
{
    public Vector3 Position;
    public Quaternion Rotation;
    private uint _tick;

    public ReconcileMoveData(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
        _tick = default;
    }

    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}
