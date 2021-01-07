using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;


public class Dog : Agent
{
    private Spawner spawner;
    private Rigidbody body;
    public float speed = 10;
    public float rotationSpeed = 350;
    bool ballInMouth;
    public GameObject tBall;
    public GameObject player;


    public void Update()
    {
        // fell();
    }

    public override void Initialize()
    {
        base.Initialize();
        body = GetComponent<Rigidbody>();
        spawner = GetComponentInParent<Spawner>();
        

    }

    public override void OnEpisodeBegin()
    {
        spawner.ClearEnvironment();
        spawner.SpawnBall();
        tBall.SetActive(false);
        ballInMouth = false;
        transform.localPosition = new Vector3(1.733055f, 1.3f, -17.78904f);
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;
    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0f;
        actionsOut[1] = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) // Moving fwd
        {
            actionsOut[0] = 2f;

        }
        else if (Input.GetKey(KeyCode.DownArrow)) // Turning left
        {
            actionsOut[0] = 1f;

        }
        else if (Input.GetKey(KeyCode.LeftArrow)) // Turning left
        {
            actionsOut[1] = 1f;

        }
        else if (Input.GetKey(KeyCode.RightArrow)) // Turning right
        {
            actionsOut[1] = 2f;

        }
    }


    //code van Meneer Dhaese bij Obelix.cs - MLAgents - VR Experience github
    public override void OnActionReceived(float[] vectorAction)
    {      
        //bij stilstaan afstraffen, nog niet zeker of dit nodig is
        if (vectorAction[0] == 0)
        {

            AddReward(-0.001f);
            return;
        }

        if (vectorAction[0] != 0)
        {
            Vector3 translation = transform.forward * speed * (vectorAction[0] * 2 - 3) * Time.deltaTime;
            transform.Translate(translation, Space.World);
        }

        if (vectorAction[1] != 0)
        {
            float rotation = rotationSpeed * (vectorAction[1] * 2 - 3) * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }

    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("tennisball") && !ballInMouth)
        {
            //load material of dog with ball in mouth
            // collision.gameObject.GetComponent<Renderer>().material = 
            ballInMouth = true;
            tBall.SetActive(true);
            spawner.ClearEnvironment();
            // add reward for getting ball
            AddReward(0.5f);

        }

        if (collision.gameObject.CompareTag("Player") && ballInMouth)
        {
            
            ballInMouth = false;

            //add reward for returning ball to player
            AddReward(1f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Player") && !ballInMouth)
        {
            
            //ballInMouth = false;
            AddReward(-0.5f);

        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ballInMouth);


    }

    public void fell()
    {
        if (GameObject.Find("Dog").transform.position.y < 0)
        {
            
            AddReward(-1f);
            ballInMouth = false;
            EndEpisode();
        }
        
        
    }

}
