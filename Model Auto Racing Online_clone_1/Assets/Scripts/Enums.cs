
using UnityEngine;

public enum SurfaceTypes
{
    All,
    Road,
    OffRoad,
    Drift
}

public enum Scale 
{
    All,
    [InspectorName("1:6")]
    Six = 6,
    [InspectorName("1:8")]
    Eight = 8,
    [InspectorName("1:10")]
    Ten = 10
}