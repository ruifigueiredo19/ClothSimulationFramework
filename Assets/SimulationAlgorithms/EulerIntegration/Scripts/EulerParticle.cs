using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerParticle : BaseParticle {
    public Vector3 velocity = Vector3.zero;

    public override void TimeStep() {
        if (!isFixed) {
            // Update Velocity And Position
            Vector3 deltaVelocity = accumulatedForce / mass * Time.deltaTime;
            velocity += deltaVelocity;
            // Symplectic Euler: Update velocity, and use v_(t+1) to update velocity. More stable
            Position += velocity * Time.deltaTime;

        }

        // Reset the accumulated force at the end of any frame, regardless of fixed or not
        accumulatedForce = Vector3.zero;
    }



    public EulerParticle() : base() { }
    public EulerParticle(Vector3 position) : base(position) { }
    public EulerParticle(Vector3 position, Vector3 velocity) : base(position) {
        this.velocity = velocity;
    }
    public EulerParticle(Vector3 position, float mass) : base(position, mass) {  }
    public EulerParticle(Vector3 position, Vector3 velocity, float mass) : base(position, mass) {
        this.velocity = velocity;
    }
    public EulerParticle(Vector3 position, bool isFixed) : base(position, isFixed) { }
    public EulerParticle(Vector3 position, Vector3 velocity, bool isFixed) : base(position, isFixed) {
        this.velocity = velocity;
    }
    public EulerParticle(Vector3 position, float mass, bool isFixed) : base(position, mass, isFixed) { }
    public EulerParticle(Vector3 position, Vector3 velocity, float mass, bool isFixed) : base(position, mass, isFixed) {
        this.velocity = velocity;
    }
}