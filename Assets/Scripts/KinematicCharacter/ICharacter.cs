using UnityEngine;

public interface ICharacter {
    public float GravityScale { get; set; }
    public Vector3 Forward { get; }
    
    public void Move(Vector3 velocity);
    public void Look(Vector3 direction);
    public void Jump(float jumpForce);
}
