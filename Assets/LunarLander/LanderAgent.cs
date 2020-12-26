using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.MLAgents; 
using Unity.MLAgents.Sensors;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LanderAgent : Agent
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(3, 3);
    }

    public float startAngle = 30.0f;
    public float startHeight = 20.0f;
    
    public override void OnEpisodeBegin()
    {
        crashed = false;
        landed = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0,startHeight,0);
        // transform.localRotation = Quaternion.identity;
        transform.localEulerAngles = new Vector3(Random.Range(-startAngle, startAngle), 0, Random.Range(-startAngle, startAngle));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // observations size 8
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(transform.eulerAngles.z);
        sensor.AddObservation(transform.eulerAngles.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
        sensor.AddObservation(rb.angularVelocity.z);
        sensor.AddObservation(rb.angularVelocity.x);
    }

    public float forceMultiplier = 7;
    public float torqueMultiplier = 0.3f;
    private bool crashed = false;
    private bool landed = false;
    public float ceilingHeight = 30;
    
    
    public override void OnActionReceived(float[] vectorAction)
    {
        // actions size 6

        if (StepCount == MaxStep)
        {
            Debug.Log("Max steps reached");
            SetReward(-2.0f);
            EndEpisode();
        }

        int action = (int)vectorAction[0];

        if (action == 1)
        {
            rb.AddRelativeForce(Vector3.up * forceMultiplier);
            //Debug.Log("main engine");
        }
        else if (action == 2)
        {
            rb.AddRelativeTorque(Vector3.back * torqueMultiplier);
            //Debug.Log("left");
        }
        else if (action == 3)
        {
            rb.AddRelativeTorque(Vector3.forward * torqueMultiplier);
            //Debug.Log("right");
        }
        else if (action == 4)
        {
            rb.AddRelativeTorque(Vector3.left * torqueMultiplier);
            //Debug.Log("forward");
        }
        else if (action == 5)
        {
            rb.AddRelativeTorque(Vector3.right * torqueMultiplier);
            //Debug.Log("back");
        }

        // Debug.Log(vectorAction[0]);
        
        if (crashed)
        {
            float crashReward = (2 - math.max(crashVelocity, -12.0f)) * 0.1f; // negative reward for crashing hard
            if (math.abs(transform.localEulerAngles.z) > 30.0f)
            {
                Debug.Log("Bad crash -1");
                //Debug.Log(-1.0f + crashReward);
                SetReward(-1.0f + crashReward);
            }
            else
            {
                Debug.Log("crashed");
                SetReward(crashReward);
            }
            EndEpisode();
            
        }
        
        else if (landed)
        {
            Debug.Log("landed");
            SetReward(1.0f);
            EndEpisode();
        }

        if (transform.localPosition.y < 0 || transform.localPosition.y > ceilingHeight)
        {
            Debug.Log("Ceiling reached");
            SetReward(-2.0f);
            EndEpisode();
        }
        
    }


    
    public override void Heuristic(float[] actionsOut)
    {
        if (Input.GetButton("Jump"))
        {
            actionsOut[0] = 1;
        }
        else if (Input.GetButton("Right"))
        {
            actionsOut[0] = 2;
        }
        else if (Input.GetButton("Left"))
        {
            actionsOut[0] = 3;
        }
        else if (Input.GetButton("Up"))
        {
            actionsOut[0] = 4;
        }
        else if (Input.GetButton("Down"))
        {
            actionsOut[0] = 5;
        }
        else
        {
            actionsOut[0] = 0;
        }
    }

    private bool collision;
    private float crashVelocity;
    
    private void OnCollisionEnter(Collision other)
    {
        collision = true;
        
        float relativeVelocity = other.relativeVelocity.y;
        
        //Debug.Log("Collision " + pointVelocity);

        if (relativeVelocity > 2)
        {
            crashed = true;
            crashVelocity = relativeVelocity;
        }


    }

    private void Update()
    {
        if (collision)
        {
            elapsed += Time.deltaTime;
            if (elapsed > necessaryTime)
            {
                landed = true;
                elapsed = 0;
            }
        }
    }

    public float necessaryTime = 1.5f;
    private float elapsed;

    private void OnCollisionExit(Collision other)
    {
        collision = false;
        elapsed = 0;
    }
}
