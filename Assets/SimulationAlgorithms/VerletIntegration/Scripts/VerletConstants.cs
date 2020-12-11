using UnityEngine;

[System.Serializable]
public class VerletConstants {
    public int constraintIterations = 3;
    [Range(0, 1)]
    public float dampingRatio = 0.01f;
    public bool enableTearing = false;
    public float maxTearingRatio = 2f;
}