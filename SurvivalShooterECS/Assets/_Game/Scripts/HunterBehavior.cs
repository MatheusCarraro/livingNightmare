using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class HunterBehavior : AgentBehaviour {

    public TextMeshProUGUI healthT, energyT, stateT;

    public float _energy;
    public float energy {
        get { return _energy; }
        set {
            _energy = Mathf.Clamp(value, 0, 100);
        }
    }

    public float necessity;

    PreyBehaviour foodTarget, preyToHunt, preyToScare;

    int currentActionID = 0;
    float changeActionTime, delay;

    protected override void Start() {
        base.Start();
        InitializeStatus();
    }

    protected override bool FixedUpdate() {
        if (state == AgentState.DEAD)
            stateT.text = "Dead...";
        healthT.text = "Health: " + health.ToString("F1");
        energyT.text = "Energy: " + energy.ToString("F1");
        if (!base.FixedUpdate())
            return false;

        FIS.SetInput("HEALTH", health);
        FIS.SetInput("ENERGY", energy);
        necessity = FIS.Evaluate("NECESSITY");

        //Arrive(Miscellaneous.MouseToWorldPosition2D(CameraControllerBase.instance.camera, Input.mousePosition));
        // if (Input.GetKey(KeyCode.Space)) {
        //     Scare();
        //     // foodTarget = PreyNear(GetPreysDead());
        //     // if (foodTarget)
        //     //     Eat();
        //     // else
        //     //     Hunt();
        // } else {
        //     Rest();
        // }

        CheckActionTime();

        switch (currentActionID) {
            case 0:
                Rest();
                break;
            case 1:
                Hunt();
                break;
            case 2:
                Scare();
                break;
        }

        UpdateHungerStatus();
        UpdateMove();

        return true;
    }

    public void InitializeStatus() {
        health = energy = 100;
        state = AgentState.IDLE;
        delay = 1f;
        changeActionTime = Time.time;
        currentActionID = Miscellaneous.MapToInt(necessity, 0, 100, 3);
    }

    void CheckActionTime() {
        if (Time.time - changeActionTime > delay) {
            currentActionID = Miscellaneous.MapToInt(necessity, 0, 100, 3);
            changeActionTime = Time.time;
        }
    }

    void UpdateHungerStatus() {

        if (state == AgentState.RESTING) {
            energy += Time.fixedDeltaTime * 2;
            health -= Time.fixedDeltaTime / 2;
        } else {
            if (state == AgentState.HUNTING)
                energy -= Time.fixedDeltaTime;

            energy -= Time.fixedDeltaTime;
            health -= Time.fixedDeltaTime;
        }

        if (energy <= 50) {
            health -= Time.fixedDeltaTime / 2;
        }
    }

    void Rest() {
        stateT.text = "Resting...";
        StopMove();
        state = AgentState.RESTING;
        energy += Time.fixedDeltaTime * 3;
    }

    void Eat() {
        // if (!foodTarget) {
        //     // foodTarget = PreyNear(GetPreysDead());
        //     // stateT.text = "Walking...";
        //     // Wander();
        //     // state = AgentState.WALKING;
        // } else 
        if (state != AgentState.EATING) {
            if (Vector2.Distance(transform.position, foodTarget.transform.position) > 0.5f) {
                stateT.text = "Hunting...";
                Arrive(foodTarget.transform.position);
                state = AgentState.WALKING;
            } else {
                if (state != AgentState.EATING) {
                    state = AgentState.EATING;
                    StartCoroutine(EatFood());
                }
            }
        } else {
            stateT.text = "Eating...";
            StopMove();
        }
    }

    IEnumerator EatFood() {
        yield return new WaitForSeconds(1f);
        if (foodTarget) {
            Destroy(foodTarget.gameObject);
            health += 10;
        }
        state = AgentState.IDLE;
    }

    void Hunt() {
        foodTarget = PreyNear(GetPreysDead());
        if (!foodTarget) {
            stateT.text = "Hunting...";
            if (!preyToHunt)
                preyToHunt = PreyNear(new List<PreyBehaviour>(FindObjectsOfType<PreyBehaviour>()));

            if (!preyToHunt && state != AgentState.HUNTING) {
                Wander();
                state = AgentState.WALKING;
            } else if (preyToHunt.state != AgentState.DEAD) {
                if (state != AgentState.HUNTING) {
                    state = AgentState.HUNTING;
                }
                Arrive(preyToHunt.transform.position);
            } else {
                state = AgentState.IDLE;
                preyToHunt = null;
            }
        } else {
            Eat();
        }
    }

    void Scare() {
        stateT.text = "Scaring...";
        if (!preyToScare)
            preyToScare = PreyNear(GetPreysResting());

        if (!preyToScare && state != AgentState.SCARING) {
            Wander();
            state = AgentState.WALKING;
        } else {
            if (state != AgentState.SCARING) {
                state = AgentState.SCARING;
                StartCoroutine(ScarePrey());
            }
            Arrive(preyToScare.transform.position);
        }
    }

    IEnumerator ScarePrey() {
        yield return new WaitForSeconds(3f);
        //yield return new WaitUntil(() => preyToScare && preyToScare.state != AgentState.RESTING);
        preyToScare = null;
        state = AgentState.IDLE;
    }

    List<PreyBehaviour> GetPreysDead() {
        PreyBehaviour[] preys = FindObjectsOfType<PreyBehaviour>();
        List<PreyBehaviour> preysDead = new List<PreyBehaviour>();
        foreach (var prey in preys)
            if (prey.state == AgentState.DEAD)
                preysDead.Add(prey);

        return preysDead;
    }

    List<PreyBehaviour> GetPreysResting() {
        PreyBehaviour[] preys = FindObjectsOfType<PreyBehaviour>();
        List<PreyBehaviour> preysResting = new List<PreyBehaviour>();
        foreach (var prey in preys)
            if (prey.state == AgentState.RESTING)
                preysResting.Add(prey);

        return preysResting;
    }

    PreyBehaviour PreyNear(List<PreyBehaviour> preys) {
        float distance = Mathf.Infinity;
        PreyBehaviour preyTarget = null;
        foreach (var prey in preys) {
            if (Vector2.Distance(transform.position, prey.transform.position) < distance) {
                distance = Vector2.Distance(transform.position, prey.transform.position);
                preyTarget = prey;
            }
        }

        return preyTarget;
    }
}