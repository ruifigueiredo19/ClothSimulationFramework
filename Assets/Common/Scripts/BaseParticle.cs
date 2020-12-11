using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseParticle {
    protected Vector3 _position = Vector3.zero;
    public float mass = 1f;
    public bool isFixed = false;
    public Vector3 accumulatedForce = Vector3.zero;
    public int index = -1;  // not required for simulation, just useful

    public Vector3 Position {
        get {
            return _position;
        }
        set {
            if (isFixed) {
                Debug.LogWarning("Trying to set position on a fixed particle.");
                return;
            }

            _position = value;
        }
    }



    public void AddForce(Vector3 force) {
        accumulatedForce += force;
    }

    public void ResetAccumulatedForce() {
        accumulatedForce = Vector3.zero;
    }


    public virtual void MoveParticle(Vector3 newPosition) {
        if (!isFixed) {
            _position = newPosition;
        }
    }

    public virtual void OffsetPosition(Vector3 offset) {
        if (!isFixed) {
            _position += offset;
        }
    }

    public virtual void ForceMoveParticle(Vector3 newPosition) {
        _position = newPosition;
    }


    public virtual void TimeStep() {
        // This function does nothing, just a placeholder to be overridden

        // Reset the accumulated force at the end of any frame, regardless of fixed or not
        accumulatedForce = Vector3.zero;
    }


    public BaseParticle() {
    }

    public BaseParticle(Vector3 position) {
        _position = position;
    }

    public BaseParticle(Vector3 position, float mass) {
        _position = position;
        this.mass = mass;
    }

    public BaseParticle(Vector3 position, bool isFixed) {
        _position = position;
        this.isFixed = isFixed;
    }


    public BaseParticle(Vector3 position, float mass, bool isFixed) {
        _position = position;
        this.mass = mass;
        this.isFixed = isFixed;
    }
}



