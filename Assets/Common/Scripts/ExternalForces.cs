using UnityEngine;

public class ExternalForce {
    public bool enabled;

    public virtual Vector3 GetForce() {
        // This function does nothing, just a placeholder to be overridden
        return default;
    }
}



[System.Serializable]
public class WindExternalForce : ExternalForce {
    public float strength = 5f;
    public float frequency = 0.2f;
    public Vector3 direction = Vector3.right;

    public override Vector3 GetForce() {
        float cos_value = Mathf.Cos(Mathf.PI * frequency * Time.unscaledTime);
        return strength * (1 - cos_value * cos_value) * direction.normalized;
        //return strength / 2 * (1 - Mathf.Cos(2 * Mathf.PI * frequency * Time.unscaledTime)) * Vector3.right;
    }

}

[System.Serializable]
public class GravityExternalForce : ExternalForce {
    public float gravity = 9.8f;

    public override Vector3 GetForce() {
        // We assume that the gravity is pointing down
        return Vector3.down * gravity;
    }

}

