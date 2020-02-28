using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Random = UnityEngine.Random;
using System;
public class RollerAgent : Agent
{
    Rigidbody rBody;
    BehaviorParameters bp;
    public Transform TransformGoal;
    public Transform TransformAim;
    public GameObject PrefabBall;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        //var newPosition = new Vector3(TransformGoal.position.x + Random.Range(2.5f, 23f), transform.parent.position.y, TransformGoal.position.z);
        rBody = this.GetComponentInParent<Rigidbody>();
        //transform.parent.position = newPosition;
        bp = this.GetComponent<BehaviorParameters>();
        Debug.Log("behaviorparameters "+ bp);
        Debug.Log("if nnmodel is empty, huristic() defined in agent is used to generate output action");
        StartCoroutine(DoShoot());

    }
    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.position.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.position = new Vector3(Random.value * 8 - 4,
                                      0.5f,
                                      Random.value * 8 - 4);
    }
    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.position);
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }
    public float speed = 10;
    float[] curraction=new float[2];
    public override void AgentAction(float[] vectorAction)
    {
        // Actions, size = 2
        
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
        Debug.Log("agentaction " + vectorAction[0] + " " + vectorAction[1]);
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);
        //DoShoot(vectorAction);
        curraction = vectorAction;
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            Done();
        }

        // Fell off platform
        if (this.transform.position.y < 0)
        {
            Done();
        }

    }
    IEnumerator DoShoot()
    {
        while (true)
        {
            //			yield return new WaitUntil(() => !Input.GetButton("Jump"));
            //			yield return new WaitUntil(() => Input.GetButton("Jump"));

            var gv2 = new Vector2(
                TransformGoal.position.x,
                TransformGoal.position.z);

            var tv2 = new Vector2(
                transform.position.x, transform.position.z);

            var dir = (gv2 - tv2).normalized;
            var dist = (gv2 - tv2).magnitude;
            var arch = 0.5f;

            var closeness = Math.Min(6f, dist) / 6f;

            float force;
            // force = GetForceRandomly(dist);
            //force = GetForceFromTensorFlow(dist);
            // force = GetForceFromMagicFormula(dist);
            force = curraction[0];
            var ball = Instantiate(PrefabBall, transform.position, Quaternion.identity);
            var bc = ball.GetComponent<BrickController>();
            Debug.Log("ballspawnercontroller_2 calling" + bc);
            bc.Force = new Vector3(
                dir.x * arch * closeness,
                force,//* (1f / closeness) Optional: Uncomment this to experiment with artificial shot arcs!
                dir.y * arch * closeness
            );
            bc.Distance = dist;
            Debug.Log("dist: " + dist + "force " + bc.Force);
            //     yield return new WaitForSeconds(.05f);
            yield return new WaitForSeconds(1);
            MoveToRandomDistance();
        }
    }
    void MoveToRandomDistance()
    {
        var newPosition = new Vector3(TransformGoal.position.x + Random.Range(2.5f, 23f), transform.parent.position.y, TransformGoal.position.z);
        transform.parent.position = newPosition;
    }
    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        
        return action;
    }
}
