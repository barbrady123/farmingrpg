using System;
using UnityEngine;

[Serializable]
public class Vector3Serializable
{
    public float X;

    public float Y;

    public float Z;

    public Vector3Serializable() { }

    public Vector3Serializable(float x, float y, float z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector3Serializable(Vector3 vector)
    {
        this.X = vector.x;
        this.Y = vector.y;
        this.Z = vector.z;
    }

    public Vector3 ToVector3() => new Vector3(this.X, this.Y, this.Z);
}
