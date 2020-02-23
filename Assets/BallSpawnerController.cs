using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TensorFlow;

[System.Serializable]
class Prediction
{
	public float result;
}

public class BallSpawnerController : MonoBehaviour
{
	public Transform TransformGoal;
	public Transform TransformAim;
	public GameObject PrefabBall;

	[Range(0, 10)]
	public float maxVariance;
    public int frozengraphid=1;
    public string[] frozengraphfiles = { "frozen.pb", "frozen2.pb" };
	private float test = 1f;

	private TFGraph graph;
	private TFSession session;
	
	void Start ()
	{	
		File.WriteAllText("successful_shots.csv", "");

        TextAsset graphModel;
        frozengraphid = 0;
        if (frozengraphid == 0)
        {
            Debug.Log("frozengraphid=0");
            graphModel = Resources.Load(frozengraphfiles[0]) as TextAsset;
        }
        else
        {
            Debug.Log("frozengraphid=1");
            graphModel = Resources.Load(frozengraphfiles[1]) as TextAsset;

        }
        
        graph = new TFGraph ();
		graph.Import (graphModel.bytes);
		
		session = new TFSession (graph);
		
		StartCoroutine(DoShoot());
	}

	private void Update()
	{
		TransformAim.LookAt(TransformGoal);
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

			var closeness = Math.Min(10f, dist) / 10f;

            float force;
            //force = GetForceRandomly(dist);
            //force = GetForceFromTensorFlow(dist);
            force = GetForceFromMagicFormula(dist);

            var ball = Instantiate(PrefabBall, transform.position, Quaternion.identity);
			var bc = ball.GetComponent<BallController>();
			bc.Force = new Vector3(
				dir.x * arch * closeness,
				force,//* (1f / closeness) Optional: Uncomment this to experiment with artificial shot arcs!
				dir.y * arch * closeness
			);
			bc.Distance = dist;
            Debug.Log("dist: " + dist + "force " + bc.Force);
            yield return new WaitForSeconds(20.05f);
			 MoveToRandomDistance();
		}
	}

	float GetForceFromTensorFlow(float distance)
	{
		var runner = session.GetRunner ();
        Debug.Log("getforcefromtf: " + runner);
        if (frozengraphid == 0)
        {
            Debug.Log("frozengraphid=0 addinput shot_input");
            runner.AddInput(
            graph["shots_input"][0],
            new float[1, 1] { { distance } });
        }
        else {
            Debug.Log("frozengraphid=1 addinput hidden_input");


            runner.AddInput(
                graph["hidden_input"][0],
                new float[1, 1] { { distance } });
        }
        runner.Fetch (graph ["shots/BiasAdd"] [0]);
		float[,] recurrent_tensor = runner.Run () [0].GetValue () as float[,];
		var force = recurrent_tensor[0, 0] / 10;
		Debug.Log(String.Format("{0}, {1}", distance, force));
		return force;
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

	void MoveToRandomDistance()
	{
		var newPosition = new Vector3(TransformGoal.position.x + Random.Range(2.5f, 23f), transform.parent.position.y, TransformGoal.position.z);
		transform.parent.position = newPosition;
	}
}
