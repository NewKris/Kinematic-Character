using System;
using UnityEngine;

public class KinematicCharacterV2 : MonoBehaviour, ICharacter {
    public float velocityDamping = 0.1f;
    public float angularDamping = 0.1f;

    private float _verticalVelocity;
    private DampedAngle _yaw;
    private DampedVector _moveVelocity;
    private Rigidbody _rigidbody;
    
    public float GravityScale { get; set; }
    public Vector3 Forward => transform.forward;
    
    public void Move(Vector3 velocity) {
        _moveVelocity.Target = velocity;
    }

    public void Look(Vector3 direction) {
        _yaw.Target = Quaternion.LookRotation(direction).eulerAngles.y;
    }

    public void Jump(float jumpForce) {
        _verticalVelocity = jumpForce;
    }

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 previousPosition = _rigidbody.position;
        
        _verticalVelocity += Physics.gravity.y * GravityScale * Time.fixedDeltaTime;
        Vector3 velocity = _moveVelocity.Tick(velocityDamping, Time.fixedDeltaTime);
        velocity.y = _verticalVelocity;

        transform.position += velocity * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0, _yaw.Tick(angularDamping, Time.fixedDeltaTime), 0);
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        //_rigidbody.MovePosition(_rigidbody.position + velocity * Time.fixedDeltaTime);
        //_rigidbody.rotation = Quaternion.Euler(0, _yaw.Tick(angularDamping, Time.fixedDeltaTime), 0);
        
        _verticalVelocity = (_rigidbody.position.y - previousPosition.y) / Time.fixedDeltaTime;
    }
}
