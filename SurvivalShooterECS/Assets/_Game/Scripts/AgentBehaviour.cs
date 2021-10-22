using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum AgentState {
    IDLE,
    RESTING,
    WALKING,
    EATING,
    HUNTING,
    SCARING,
    DEAD
}

public class AgentBehaviour : MonoBehaviour {

    public float maxSpeed;
    public float maxForce;

    protected Vector2 velocity;
    protected Vector2 acceleration;

    public float _health;
    public float health {
        get { return _health; }
        set {
            _health = Mathf.Clamp(value, 0, 100);
            if (_health <= 0)
                state = AgentState.DEAD;
        }
    }

    public AgentState _state;

    public AgentState state {
        get { return _state; }
        set {
            _state = value;
            switch (_state) {
                case AgentState.DEAD:
                    GetComponent<SpriteRenderer>().color = Color.red;
                    break;
            }
        }
    }

    protected virtual void Start() {

    }

    protected virtual bool FixedUpdate() {

        if (state == AgentState.DEAD) {
            if (velocity != Vector2.zero) {
                StopMove();
                UpdateMove();
            }

            return false;
        }

        return true;
    }

    public void UpdateMove() {
        AvoidLimits(1);
        velocity += acceleration;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3) velocity * Time.fixedDeltaTime;
        acceleration = acceleration.SetMagnitude(0);
        if (velocity.magnitude > 0.1f)
            transform.rotation = Miscellaneous.RotateToDirection(transform.position, transform.position - (Vector3) velocity);
    }

    void ApplyForce(Vector2 force) {
        acceleration += force;
    }

    public void Seek(Vector2 target) {
        Vector2 desired = target - (Vector2) transform.position;
        desired = desired.SetMagnitude(maxSpeed);
        Vector2 steer = desired - velocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);
        ApplyForce(steer);
    }

    public void Flee(Vector2 target) {
        Vector2 desired = (Vector2) transform.position - target;
        desired = desired.SetMagnitude(maxSpeed);
        Vector2 steer = desired - velocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);
        ApplyForce(steer);
    }

    public void Arrive(Vector2 target) {
        Vector2 desired = target - (Vector2) transform.position;

        if (desired.magnitude < 3)
            desired = desired.SetMagnitude(Miscellaneous.Map(desired.magnitude, 0, 3, 0, maxSpeed));
        else
            desired = desired.SetMagnitude(maxSpeed);
        Vector2 steer = desired - velocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);
        ApplyForce(steer);
    }

    float angle;

    public void Wander() {
        if (velocity == Vector2.zero)
            velocity = transform.up;
        float angleVelocity = Mathf.Atan2(velocity.y, velocity.x);
        angle += Random.Range(-0.5f, 0.5f);
        float x = Mathf.Cos(angle + angleVelocity) * 1.5f;
        float y = Mathf.Sin(angle + angleVelocity) * 1.5f;

        Vector2 v = transform.position + (Vector3) (velocity.normalized * 4);
        Seek(new Vector2(v.x + x, v.y + y));
    }

    public void AvoidLimits(float distance) {

        Vector2 desired = (Vector2) transform.position;

        float halfCameraWidth = Miscellaneous.HalfWidthCamera(CameraControllerBase.instance.camera);

        if (transform.position.x < -(halfCameraWidth - distance)) {
            desired = new Vector2(maxSpeed, velocity.y);
            Vector2 steer = desired - velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce * 2);
            ApplyForce(steer);
        }

        if (transform.position.x > halfCameraWidth - distance) {
            desired = new Vector2(-maxSpeed, velocity.y);
            Vector2 steer = desired - velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce * 2);
            ApplyForce(steer);
        }

        if (transform.position.y < -(halfCameraWidth - distance)) {
            desired = new Vector2(velocity.x, maxSpeed);
            Vector2 steer = desired - velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce * 2);
            ApplyForce(steer);
        }

        if (transform.position.y > halfCameraWidth - distance) {
            desired = new Vector2(velocity.x, -maxSpeed);
            Vector2 steer = desired - velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce * 2);
            ApplyForce(steer);
        }
    }

    public void StopMove() {
        ApplyForce(Vector2.ClampMagnitude(-velocity, maxForce * 2.5f));
    }

    public void Separate(AgentBehaviour[] abList) {
        Vector2 sum = Vector2.zero;
        int count = 0;
        foreach (var ab in abList) {
            if (ab == this || ab.state == AgentState.DEAD) continue;
            float d = Vector2.Distance(transform.position, ab.transform.position);
            if (d > 0 && d < 2) {
                Vector2 diff = transform.position - ab.transform.position;
                sum += diff.normalized;
                ++count;
            }
        }

        if (count > 0) {
            sum /= count;
            sum = sum.SetMagnitude(maxSpeed);
            Vector2 steer = sum - velocity;
            steer = Vector2.ClampMagnitude(steer, maxForce*2);
            ApplyForce(steer);
        }
    }
}