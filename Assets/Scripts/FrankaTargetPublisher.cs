using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public class FrankaTargetPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/unity/target_pose";
    
    [Header("Referencias de la Escena")]
    [Tooltip("El GameObject que representa el objetivo al que queremos ir")]
    public GameObject targetCube;
    
    [Tooltip("El GameObject que es la base del robot (fr3_link0)")]
    public Transform robotBase;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

    void Update()
    {
        // Al pulsar Espacio, mandamos la petición a MoveIt
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendTargetPose();
        }
    }

    private void SendTargetPose()
    {
        // 1. Calculamos la posición y rotación del cubo VISTAS DESDE la base del robot
        Vector3 posicionLocal = robotBase.InverseTransformPoint(targetCube.transform.position);
        Quaternion rotacionLocal = Quaternion.Inverse(robotBase.rotation) * targetCube.transform.rotation;

        // 2. Construimos el mensaje convirtiendo a las coordenadas de ROS (FLU)
        PoseStampedMsg targetPoseMsg = new PoseStampedMsg
        {
            header = new HeaderMsg
            {
                frame_id = "fr3_link0" // Le confirmamos a MoveIt que son coordenadas de la base
            },
            pose = new PoseMsg
            {
                position = posicionLocal.To<FLU>(),
                orientation = rotacionLocal.To<FLU>()
            }
        };

        // 3. Publicamos
        ros.Publish(topicName, targetPoseMsg);
        Debug.Log($"Pose LOCAL enviada a MoveIt: X={posicionLocal.x:F3}, Y={posicionLocal.y:F3}, Z={posicionLocal.z:F3}");
    }
}
