
using UnityEngine;

internal struct DampedVector {
    private Vector3 _velocity;

    public Vector3 Current { get; private set; }
    public Vector3 Target { get; set; }
    
    public DampedVector(Vector3 start) {
        Current = start;
        Target = start;
        _velocity = Vector3.zero;
    }

    public Vector3 Tick(float damping, float dt) {
        Current = Vector3.SmoothDamp(Current, Target, ref _velocity, damping, Mathf.Infinity, dt);
        return Current;
    }
}

internal struct DampedAngle {
    private float _velocity;
    
    public float Current { get; private set; }
    public float Target { get; set; }

    public DampedAngle(float start) {
        Current = start;
        Target = start;

        _velocity = 0;
    }

    public float Tick(float damping, float dt) {
        Current = Mathf.SmoothDampAngle(Current, Target, ref _velocity, damping, Mathf.Infinity, dt);
        return Current;
    }
}