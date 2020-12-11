using UnityEngine;

[System.Serializable]
public class EulerConstants {
    public float springDampingConstant = 3f;
    public float airDampingConstant = 3f;
    public bool enableTearing = false;
    public float maxTearingRatio = 3f;
}