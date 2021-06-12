using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MLAgents;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System;
public class RollerAgent : Agent
{
    Rigidbody rBody;
    //BehaviorParameters bp;
    public Transform TransformGoal;
    public Transform TransformAim;
    public GameObject PrefabBall;
    GameObject currball;
    [Range(0, 10)]
    public float maxVariance;
    List<GameObject> balls; 
    // Start is called before the first frame update
    void Start()
    {
        balls = new List<GameObject>();
        rBody = GetComponent<Rigidbody>();
        //var newPosition = new Vector3(TransformGoal.position.x + Random.Range(2.5f, 23f), transform.parent.position.y, TransformGoal.position.z);
        rBody = this.GetComponentInParent<Rigidbody>();
        //transform.parent.position = newPosition;
        //bp = this.GetComponent<BehaviorParameters>();
       // Debug.Log("behaviorparameters "+ bp);
        Debug.Log("if nnmodel is empty, huristic() defined in agent is used to generate output action");
        //StartCoroutine(DoShoot());

    }
    public Transform Target;
    public override void OnEpisodeBegin()
    {
        Debug.Log("agent reset");
        if (rBody.transform.position.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            //this.rBody.velocity = Vector3.zero;
            //this.transform.position = new Vector3(0, 0.5f, 0);
            //rBody.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        //Target.position = new Vector3(Random.value * 8 - 4,
          //                            0.5f,
            //                         TransformGoal.position.z);// Random.value * 8 - 4);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.position);
        sensor.AddObservation(this.transform.position);

        // agent position
        //AddVectorObs(transform.position.x);
        // Agent velocity
        //AddVectorObs(rBody.velocity.x);
        //AddVectorObs(rBody.velocity.z);
    }
    public float speed = 10;
    float[] curraction=new float[2];
    
    //---------3/4/2020 tf-jam RollerAgent training status -----------------------------------

    //training cmd works, traing logic still under studying...
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Rigidbody playerbody;
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
      
  
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);
        playerbody = this.GetComponentInParent<Rigidbody>();

        Debug.Log("parent body: " + playerbody + " pos " + playerbody.transform.position);
        Debug.Log("rbody: " + rBody + " pos " + rBody.transform.position);
        Debug.Log("this pos " + this.transform.position);

        playerbody.AddForce(controlSignal*speed);

        //rBody.AddForce(controlSignal * speed);
        curraction = vectorAction;
        //DoShoot();
        Debug.Log("rolleragent::agentaction " + vectorAction[0] + " " + vectorAction[1] + " dist " + distanceToTarget + " pos " + this.transform.position);
        bool hasreward=false;
        if (distanceToTarget < 1.42f)
        {
            Debug.Log("close to target reward");
            SetReward(1.0f);
            EndEpisode();
        }
            // Reached target
         foreach (GameObject pball in balls)
        {
            if (!(pball == null)){
                //Debug.Log("check ball score" + pball.GetComponent<BrickController>().ballid);
                if (pball.GetComponent<BrickController>().hasBeenScored)
                //if (distanceToTarget < 1.42f)
                {
                    SetReward(1.0f);
                    Debug.Log("reward 1 earned for ballid " + pball.GetComponent<BrickController>().ballid);
                    pball.GetComponent<BrickController>().hasBeenScored = false; //ball score should only count once
                    //Done();
                    hasreward = true;
                }
                if (pball.GetComponent<BrickController>().dist_target < 4)
                {
                    Debug.Log("reward .1 earned for hit board " + pball.GetComponent<BrickController>().dist_target);
                    hasreward = true;
                    SetReward(1f);
                }
                
            }
        }
        //reset if has reward
        if (hasreward) EndEpisode();
        
        // reset after 5 shots
        //if (BrickController.ShotCount % 5 == 0) {
          //  Debug.Log("5 shots reset");
           // Done();
        //}
        
        // Fell off platform
        if (rBody.transform.position.y < 0)
        {
            Debug.Log("fall off reset");
            EndEpisode();
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
            float[] xyforce = new float[2];
             force = GetForceRandomly(dist);
            //force = GetForceFromTensorFlow(dist);
            // force = GetForceFromMagicFormula(dist);
            //force = curraction[0];
            var ball = Instantiate(PrefabBall, transform.position, Quaternion.identity);
            currball = ball;
            balls.Add(ball);
            var bc = ball.GetComponent<BrickController>();
            //Debug.Log("rolleragent calling" + bc);
            xyforce = goodmagic();
            bc.Force = new Vector3(
                //xyforce[0],
                10*curraction[0],// xyforce[0], //dir.x * arch * closeness,
                //xyforce[1]+0.3f,
                30*curraction[1],//xyforce[1], //* (1f / closeness) Optional: Uncomment this to experiment with artificial shot arcs!
                0//dir.y * arch * closeness
            );
            bc.Distance = dist;
            Debug.Log("dist: " + dist + " force " + bc.Force + " xyforce " + xyforce[0] +", "+xyforce[1]);
            yield return new WaitForSeconds(1.05f);
            //yield return new WaitForSeconds(1);
            //MoveToRandomDistance();
        }
    }

    float GetForceRandomly(float distance)
    {
        return Random.Range(0f, 1f);
    }
    float GetForceFromMagicFormula(float distance)
    {
        var variance = Random.Range(1f, maxVariance);
        return (0.125f) + (0.0317f * distance * variance);
    }
    float[] goodmagic()
    {
        var gv2 = new Vector2(
       TransformGoal.position.x,
       TransformGoal.position.z);
        var gv3 = new Vector3(
                TransformGoal.position.x,
                TransformGoal.position.y, TransformGoal.position.z);

        var tv2 = new Vector2(
            transform.position.x, transform.position.z);
        var tv3 = new Vector3(
                    transform.position.x, transform.position.y, transform.position.z);

        var dir = (gv2 - tv2).normalized;
        var dist = (gv2 - tv2).magnitude;
        var vx = 5;
        var xyforce = new float[2];
        var deltay = gv3.y - tv3.y;
        Debug.Log("deltay: " + deltay + "," + "gv3.y " + gv3.y + ", tv3.y " + tv3.y);
        var t = dist / vx;
        var vy = (deltay + 5 * Mathf.Pow(t, 2)) / t;
        Debug.Log("vs, deltay, t, vy " + vx + "," + deltay + "," + t + "," + vy);
        Vector3 newvel = new Vector3(
        vx, //m_ForceX, //dir.x ,
        vy, //m_ForceY,  //dist*m_ForceY,dist*10/(2*dir.x)
        dir.y
        );
        xyforce[0] = vx;
        xyforce[1] = vy;
        return xyforce;
    }
    void MoveToRandomDistance()
    {
        var newPosition = new Vector3(TransformGoal.position.x + Random.Range(-20f, -5.5f), transform.parent.position.y, TransformGoal.position.z);
        transform.parent.position = newPosition;
    }
    public override void Heuristic(float[] action)
    {
        //this is called if the nn model field is empty in unity editor. the nn model field is in "behavior paramete" component

        // var action = new float[2];
  //      action[0] = 101; //test if huristic is called
  //      action[1] = 202;

       action[0] = Input.GetAxis("Horizontal"); //test with joystick controller
       action[1] = Input.GetAxis("Vertical");
        // return action;
    }
}
