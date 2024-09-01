using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal struct CapsuleCastConfig {
    public Vector3 p1;
    public Vector3 p2;
    public float radius;
}

[RequireComponent(typeof(CapsuleCollider))]
public class KinematicCharacter : MonoBehaviour, ICharacter {
    public float angularDamping;
    public float velocityDamping;

    [Header("Slope")] 
    public float maxSlopeAngle = 45f;
    public float maxStepHeight = 0.2f;

    [Header("Ground Check")] 
    public float startHeight;
    public float castLength;
    public float castRadius;
    public LayerMask groundMask;

    [Header("Collision")] 
    public int maxSweeps = 3;
    public float skinWidth = 0.1f;

    private bool _isGrounded;
    private float _verticalVelocity;
    private Vector3 _groundNormal;
    private DampedAngle _yaw;
    private DampedVector _groundVelocity;
    private CapsuleCastConfig _capsuleCastConfig;

    public float GravityScale { get; set; }
    public Vector3 CurrentGroundVelocity => _groundVelocity.Current;
    public Vector3 Forward => transform.forward;

    public void Jump(float force) {
        if (_isGrounded) _verticalVelocity = force;
    }
    
    public void Move(Vector3 velocity) {
        _groundVelocity.Target = velocity;
    }

    public void Look(Vector3 forward) {
        _yaw.Target = Quaternion.LookRotation(forward, Vector3.up).eulerAngles.y;
    }

    private void Awake() {
        _yaw = new DampedAngle(transform.rotation.eulerAngles.y);

        _capsuleCastConfig = CreateCapsuleCastConfig(GetComponent<CapsuleCollider>(), skinWidth);
    }

    private void FixedUpdate() {
        _isGrounded = CalculateGrounded();
        _verticalVelocity += Physics.gravity.y * GravityScale * Time.fixedDeltaTime;

        Vector3 currentGroundVelocity = _groundVelocity.Tick(velocityDamping, Time.fixedDeltaTime);
        Vector3 moveVelocity = new Vector3(currentGroundVelocity.x, _verticalVelocity, currentGroundVelocity.z) * Time.fixedDeltaTime;
        moveVelocity = GetSlideVelocity(transform.position, moveVelocity, 0);
        
        float previousHeight = transform.position.y;
        transform.position += moveVelocity;
        _verticalVelocity = (transform.position.y - previousHeight) / Time.fixedDeltaTime;
        
        transform.rotation = Quaternion.Euler(0, _yaw.Tick(angularDamping, Time.fixedDeltaTime), 0);
    }

    private bool CalculateGrounded() {
        Ray ray = new Ray(transform.position + Vector3.up * startHeight, Vector3.down);
        
        bool grounded = Physics.SphereCast(ray, castRadius, out RaycastHit hit, castLength, groundMask);
        _groundNormal = grounded ? hit.normal : Vector3.up;
        
        return grounded;
    }

    private Vector3 GetSlideVelocity(Vector3 start, Vector3 velocity, int level) {
        if (level >= maxSweeps) return Vector3.zero;
        
        Vector3 dir = velocity.normalized;
        
        if (Physics.CapsuleCast(
                start + _capsuleCastConfig.p1,
                start + _capsuleCastConfig.p2,
                _capsuleCastConfig.radius,
                dir,
                out RaycastHit hit,
                velocity.magnitude + skinWidth
        )) {
            float travelDistance = hit.distance - skinWidth;
            
            Vector3 travelVelocity = travelDistance <= skinWidth ? Vector3.zero : dir * (hit.distance - skinWidth);
            Vector3 remainingVelocity = velocity - travelVelocity;
            Vector3 slideVelocity = Vector3.ProjectOnPlane(remainingVelocity, hit.normal);

            return travelVelocity + GetSlideVelocity(start + travelVelocity, slideVelocity, level + 1);
        }

        return velocity;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 start = transform.position + Vector3.up * startHeight;
        Gizmos.DrawWireSphere(start, castRadius);
        Gizmos.DrawWireSphere(start + Vector3.down * castLength, castRadius);
        
        Gizmos.color = Color.yellow;
        CapsuleCastConfig config = CreateCapsuleCastConfig(GetComponent<CapsuleCollider>(), skinWidth);
        Gizmos.DrawSphere(transform.position + config.p1, config.radius);
        Gizmos.DrawSphere(transform.position + config.p2, config.radius);
    }
    
    private static CapsuleCastConfig CreateCapsuleCastConfig(CapsuleCollider collider, float skinWidth) {
        float halfHeight = (collider.height * 0.25f) - skinWidth;
        Vector3 center = collider.center;
        
        return new CapsuleCastConfig() {
            p1 = center + (Vector3.down * halfHeight),
            p2 = center + (Vector3.up * halfHeight),
            radius = collider.radius - skinWidth
        };
    }
}
