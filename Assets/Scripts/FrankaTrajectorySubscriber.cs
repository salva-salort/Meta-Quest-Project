using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Trajectory;
using System.Collections;
using System.Collections.Generic;

public class FrankaTrajectorySubscriber : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/unity/planned_path";

    [System.Serializable]
    public class JointConfig
    {
        public string rosJointName; 
        
        [Tooltip("¡Cambiado! Arrastra aquí la pieza que tiene el ArticulationBody")]
        public ArticulationBody motorFisico; 
    }

    public List<JointConfig> robotJoints;
    private Coroutine currentAnimation;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointTrajectoryMsg>(topicName, TrajectoryCallback);
        Debug.Log($"[Franka] Suscrito a {topicName}. Usando físicas (ArticulationBody).");
    }

    void TrajectoryCallback(JointTrajectoryMsg trajectoryMsg)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimateRobot(trajectoryMsg));
    }

    IEnumerator AnimateRobot(JointTrajectoryMsg trajectory)
    {
        float startTime = Time.time;
        
        foreach (var point in trajectory.points)
        {
            float targetTime = (float)point.time_from_start.sec + (float)point.time_from_start.nanosec / 1e9f;

            while (Time.time - startTime < targetTime)
            {
                yield return null; 
            }

            for (int i = 0; i < trajectory.joint_names.Length; i++)
            {
                string jointName = trajectory.joint_names[i];
                float angleInRadians = (float)point.positions[i];
                float angleInDegrees = angleInRadians * Mathf.Rad2Deg; 

                // Buscamos el motor en nuestra lista
                JointConfig jc = robotJoints.Find(j => j.rosJointName == jointName);
                if (jc != null && jc.motorFisico != null)
                {
                    // MAGIA: En lugar de rotar la malla, le decimos al motor interno a dónde ir
                    var drive = jc.motorFisico.xDrive;
                    drive.target = angleInDegrees;
                    jc.motorFisico.xDrive = drive;
                }
            }
        }
    }
}