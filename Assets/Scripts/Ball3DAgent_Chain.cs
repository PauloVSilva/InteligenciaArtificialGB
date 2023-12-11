using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.VisualScripting;

public class Ball3DAgent_Chain : Agent
{
    [Header("Specific to Ball3D")]
    //public GameObject ball;
    public GameObject[] balls;
    [Tooltip("Whether to use vector observation. This option should be checked " +
        "in 3DBall scene, and unchecked in Visual3DBall scene. ")]
    public bool useVecObs;
    //Rigidbody m_BallRb;
    List<Rigidbody> m_ballsRb = new();
    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        //m_BallRb = ball.GetComponent<Rigidbody>();

        foreach (GameObject ball in balls)
        {
            m_ballsRb.Add(ball.GetComponent<Rigidbody>());
        }

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVecObs)
        {
            sensor.AddObservation(gameObject.transform.rotation.z);
            sensor.AddObservation(gameObject.transform.rotation.x);

            //sensor.AddObservation(ball.transform.position - gameObject.transform.position);
            //sensor.AddObservation(m_BallRb.velocity);

            foreach (GameObject ball in balls)
            {
                sensor.AddObservation(ball.transform.position - gameObject.transform.position);
            }

            foreach (Rigidbody m_BallRb in m_ballsRb)
            {
                sensor.AddObservation(m_BallRb.velocity);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionZ = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        var actionX = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
            (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
        }

        if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
            (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
        }

        foreach (GameObject ball in balls)
        {
            if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
            Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
            {
                SetReward(-1f);
                EndEpisode();
            }
            else
            {
                SetReward(0.1f);
            }
        }

        /*
        if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
            Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
        */
    }

    public override void OnEpisodeBegin()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        //m_BallRb.velocity = new Vector3(0f, 0f, 0f);
        //ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
        //    + gameObject.transform.position;

        foreach (Rigidbody m_BallRb in m_ballsRb)
        {
            m_BallRb.velocity = new Vector3(0f, 0f, 0f);
        }

        foreach (GameObject ball in balls)
        {
            ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
                + gameObject.transform.position;
        }


        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public void SetBall()
    {
        //Set the attributes of the ball by fetching the information from the academy
        //m_BallRb.mass = m_ResetParams.GetWithDefault("mass", 1.0f);
        //var scale = m_ResetParams.GetWithDefault("scale", 1.0f);
        //ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetBall();
    }
}