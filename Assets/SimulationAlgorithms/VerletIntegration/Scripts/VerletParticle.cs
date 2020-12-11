using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletParticle : BaseParticle {
    public Vector3 oldPosition;

    public void TimeStep(float damping) {
        if (!isFixed) {
            Vector3 initialPosition = _position;
            Vector3 acceleration = accumulatedForce / mass;
            _position += (_position - oldPosition) * (1f - damping) + acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime;
            
            // Save old position
            oldPosition = initialPosition;
        }

        // Reset the accumulated force at the end of any frame, regardless of fixed or not
        accumulatedForce = Vector3.zero;
    }


    public VerletParticle() : base () { }
    public VerletParticle(Vector3 position) : base(position) {
        oldPosition = position;
    }
    public VerletParticle(Vector3 position, Vector3 oldPosition) : base(position) {
        this.oldPosition = oldPosition;
    }
    public VerletParticle(Vector3 position, float mass) : base(position, mass) {
        oldPosition = position;
    }
    public VerletParticle(Vector3 position, Vector3 oldPosition, float mass) : base(position, mass) {
        this.oldPosition = oldPosition;
    }
    public VerletParticle(Vector3 position, bool isFixed) : base (position, isFixed) {
        oldPosition = position;
    }
    public VerletParticle(Vector3 position, Vector3 oldPosition, bool isFixed) : base(position, isFixed) {
        this.oldPosition = oldPosition;
    }
    public VerletParticle(Vector3 position, float mass, bool isFixed) : base (position, mass, isFixed) {
        oldPosition = position;
    }
    public VerletParticle(Vector3 position, Vector3 oldPosition, float mass, bool isFixed) : base(position, mass, isFixed) {
        this.oldPosition = oldPosition;
    }
}
