using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    // Initialization and Resetting the Agent
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform target;
    public Transform field;
    [SerializeField] private Material blueWin;
    [SerializeField] private Material redWin;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < -3)    // fall out from the platform
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;

            this.transform.localPosition = new Vector3(Random.Range(-5,5), 1.0f, Random.Range(-5,5));
        }
        // Move the target to a new spot
        target.localPosition = new Vector3(Random.Range(-5,5), 1.0f, Random.Range(-5,5));
    }

    // Observing the Environment
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    // Taking Actions and Assigning Rewards
    public float forceMultiplier = 10;
    double reward=0;
    bool isCrash = false;

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player"))
            isCrash = true;
        else
            isCrash = false;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);
        Vector3 Center = new Vector3((float)field.localPosition.x, (float)field.localPosition.y, (float)field.localPosition.z);
        // Debug.Log(Vector3.Distance(this.transform.localPosition, Center));
        bool SelfInside = Vector3.Distance(this.transform.localPosition, Center) < 7.2f;
        bool targetInside = Vector3.Distance(target.localPosition, Center) < 7.2f;
        

        if (isCrash)  // Reached target
        {
            SetReward(1.0f);
        }
        if (!targetInside & SelfInside)
        {
            SetReward(50.0f);
            if (this.transform.name == "blueCube")
            {
                floorMeshRenderer.material = blueWin;
            }
            if ((this.transform.name == "redCube"))
            {
                floorMeshRenderer.material = redWin;
            }
            EndEpisode();
        }
        if (!SelfInside & targetInside)
        {
            SetReward(-50.0f);
            EndEpisode();
        }

        if (target.localPosition.y < -3 | this.transform.localPosition.y < -3)     // fell off platform
        {
            EndEpisode();
        }
        else
        {
            SetReward(-0.00001f);
        }
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
    

}
