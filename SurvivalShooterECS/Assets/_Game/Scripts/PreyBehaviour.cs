using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PreyBehaviour : AgentBehaviour {

    HunterBehavior hunter;

    bool resting;

    float timeFlee;
    bool _fleeing;
    bool fleeing {
        get { return _fleeing; }
        set {
            _fleeing = value;
            if (_fleeing)
                timeFlee = Time.time;
        }
    }

    protected override void Start() {
        base.Start();
        health = 100;
        hunter = FindObjectOfType<HunterBehavior>();
        Wander();
    }

    protected override bool FixedUpdate() {
        if (!base.FixedUpdate())
            return false;

        if (!fleeing && state != AgentState.RESTING && health < 50) {
            state = AgentState.RESTING;
        }

        if (state == AgentState.RESTING && health < 100 && velocity == Vector2.zero) {
            health += Time.fixedDeltaTime * 3;
        } else {
            if (health >= 100) {
                state = AgentState.IDLE;
            }
            health -= Time.fixedDeltaTime * 4;
        }

        if (state == AgentState.DEAD) return false;

        if (fleeing || hunter && hunter.state != AgentState.DEAD && Vector2.Distance(transform.position, hunter.transform.position) < 5) {
            state = AgentState.WALKING;
            Flee(hunter.transform.position);
            health -= Time.fixedDeltaTime * 3;
        } else if (state != AgentState.RESTING) {
            state = AgentState.WALKING;
            Wander();
        } else {
            StopMove();
        }

        if (state != AgentState.RESTING)
            Separate(FindObjectsOfType<PreyBehaviour>());

        UpdateMove();

        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, health / 100f);

        if (Time.time - timeFlee > 8.5f)
            fleeing = false;

        return true;
    }
}