using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.MLAgents; 
using Unity.MLAgents.Sensors;
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

    public float startAngle = 15.0f;
    public float startHeight = 10.0f;
    
    public override void OnEpisodeBegin()
    {
        crashed = false;
        landed = false;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0,startHeight,0);
        // transform.localRotation = Quaternion.identity;
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(-startAngle, startAngle));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // observations size 5
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(transform.eulerAngles.z);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.angularVelocity.z);
    }

    public float forceMultiplier = 5;
    public float torqueMultiplier = 1;
    private bool crashed = false;
    private bool landed = false;
    public float ceilingHeight = 30;
    
    
    public override void OnActionReceived(float[] vectorAction)
    {
        // actions size 4

        int action = (int)vectorAction[0];

        if (action == 1)
        {
            rb.AddRelativeForce(Vector3.up * forceMultiplier);
        }
        else if (action == 2)
        {
            rb.AddRelativeTorque(Vector3.back * torqueMultiplier);
        }
        else if (action == 3)
        {
            rb.AddRelativeTorque(Vector3.forward * torqueMultiplier);
        }

        // Debug.Log(vectorAction[0]);
        
        if (crashed)
        {
            if (math.abs(transform.localEulerAngles.z) > 30.0f)
            {
                Debug.Log("Bad crash -1");
                SetReward(-1.0f);
            }
            Debug.Log("crashed");
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
        else
        {
            actionsOut[0] = 0;
        }
    }

    private bool collision;
    
    private void OnCollisionEnter(Collision other)
    {
        collision = true;
        
        float relativeVelocity = other.relativeVelocity.y;
        
        //Debug.Log("Collision " + pointVelocity);

        if (relativeVelocity > 2)
        {
            crashed = true;
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
