using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed;
    public float jumpHeight = 2;
    public float jumpTime = 0.7f;

    private float _jumpForce;
    private ICharacter _kinematicCharacter;

    private void Awake() {
        _kinematicCharacter = GetComponent<ICharacter>();

        float t = jumpTime * 0.5f;
        _jumpForce = (2 * jumpHeight) / t;

        float gravity = (-2 * jumpHeight) / (t * t);
        _kinematicCharacter.GravityScale = gravity / Physics.gravity.y;
    }

    private void Update() {
        Vector3 movement = new Vector3(
            x: Input.GetAxisRaw("Horizontal"),
            y: 0,
            z: Input.GetAxisRaw("Vertical")
        ).normalized * moveSpeed;
        
        _kinematicCharacter.Move(movement);

        Vector3 lookDirection = movement == Vector3.zero
            ? _kinematicCharacter.Forward
            : movement;
        _kinematicCharacter.Look(lookDirection.normalized);
        
        if (Input.GetKeyDown(KeyCode.Space)) _kinematicCharacter.Jump(_jumpForce);
    }
}
