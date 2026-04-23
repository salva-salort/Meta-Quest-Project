using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
// Importante: Estos nombres pueden variar ligeramente según cómo generaste los mensajes
using RosMessageTypes.Franka;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public class FrankaStatePublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/franka_robot_state";

    [Header("Configuración")]
    public float publishFrequency = 0.1f; // 10Hz para que sea fluido
    private float timeElapsed;

    [Header("Simulación de posición (Opcional)")]
    // Si tienes un objeto en Unity que representa dónde crees que está el robot
    public GameObject endEffectorVisual;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        // Registramos el publicador con el tipo de mensaje oficial de Franka
        ros.RegisterPublisher<FrankaRobotStateMsg>(topicName);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishFrequency)
        {
            // 1. Creamos el contenedor principal del mensaje
            FrankaRobotStateMsg robotState = new FrankaRobotStateMsg();

            // 2. Rellenamos la pose del Efector Final (o_t_ee)
            // Nota: Franka usa PoseStamped, que incluye un Header y una Pose
            robotState.o_t_ee = new PoseStampedMsg();
            robotState.o_t_ee.header = new HeaderMsg();
            robotState.o_t_ee.header.frame_id = "fr3_link0"; // El origen del robot

            // Extraemos posición y rotación del objeto visual de Unity
            // Usamos .ToRos() para convertir de coordenadas Unity (Y-up) a ROS (Z-up)
            Vector3 position = endEffectorVisual.transform.position;
            Quaternion rotation = endEffectorVisual.transform.rotation;

            robotState.o_t_ee.pose = new PoseMsg
            {
                position = new PointMsg(position.x, position.y, position.z),
                orientation = new QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w)
            };

            // 3. (Opcional) Puedes marcar el modo del robot
            robotState.robot_mode = FrankaRobotStateMsg.ROBOT_MODE_MOVE;

            // 4. Publicar
            ros.Publish(topicName, robotState);

            timeElapsed = 0;
        }
    }
}