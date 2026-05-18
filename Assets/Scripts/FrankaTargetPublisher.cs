using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

public class FrankaTargetPublisher : MonoBehaviour
{
    ROSConnection ros;
    
    [Header("Configuración ROS 2")]
    public string topicName = "/unity/target_pose";
    
    [Header("Referencias de la Escena")]
    public GameObject targetCube;
    public Transform robotBase;

    [Header("Orientación de la Pinza")]
    [Tooltip("Grados (Euler) para la postura de la pinza. X=180 suele mirar hacia abajo.")]
    public Vector3 rotacionPinza = new Vector3(180f, 0f, 0f);

    [Header("Seguimiento de Manos (Meta Quest)")]
    [Tooltip("Arrastra aquí el objeto RightHand")]
    public OVRHand manoDerecha;
    [Tooltip("Arrastra aquí el objeto LeftHand")]
    public OVRHand manoIzquierda;

    // Variables de memoria separadas para cada mano
    private bool estabaPellizcandoDerecha = false;
    private bool estabaPellizcandoIzquierda = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);

        // Búsqueda automática mejorada si se te olvida poner las manos en el Inspector
        if (manoDerecha == null || manoIzquierda == null)
        {
            OVRHand[] manos = Object.FindObjectsByType<OVRHand>(FindObjectsSortMode.None);
            foreach (OVRHand mano in manos)
            {
                if (mano.gameObject.name.Contains("Right") && manoDerecha == null)
                    manoDerecha = mano;
                else if (mano.gameObject.name.Contains("Left") && manoIzquierda == null)
                    manoIzquierda = mano;
            }
        }
    }

    void Update()
    {
        bool pellizcoDerecha = false;
        bool pellizcoIzquierda = false;

        // 1. Leemos el sensor de la mano derecha
        if (manoDerecha != null && manoDerecha.IsTracked)
        {
            pellizcoDerecha = manoDerecha.GetFingerIsPinching(OVRHand.HandFinger.Index);
        }

        // 2. Leemos el sensor de la mano izquierda
        if (manoIzquierda != null && manoIzquierda.IsTracked)
        {
            pellizcoIzquierda = manoIzquierda.GetFingerIsPinching(OVRHand.HandFinger.Index);
        }

        // 3. Disparamos si la Derecha acaba de pellizcar
        if (pellizcoDerecha && !estabaPellizcandoDerecha)
        {
            SendTargetPose("Mano Derecha");
        }

        // 4. Disparamos si la Izquierda acaba de pellizcar
        if (pellizcoIzquierda && !estabaPellizcandoIzquierda)
        {
            SendTargetPose("Mano Izquierda");
        }

        // Actualizamos la memoria para el siguiente fotograma
        estabaPellizcandoDerecha = pellizcoDerecha;
        estabaPellizcandoIzquierda = pellizcoIzquierda;

        // Disparador de teclado para cuando pruebes en el PC sin gafas
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendTargetPose("Teclado (Espacio)");
        }
    }

    private void SendTargetPose(string origen)
    {
        // Posición del cubo respecto a la base
        Vector3 posicionLocal = robotBase.InverseTransformPoint(targetCube.transform.position);
        
        // Usamos los grados que has puesto en el Inspector de Unity
        Quaternion rotacionLocal = Quaternion.Euler(rotacionPinza);
        //Quaternion rotacionLocal = Quaternion.Inverse(robotBase.rotation) * targetCube.transform.rotation;
        PoseStampedMsg targetPoseMsg = new PoseStampedMsg
        {
            header = new HeaderMsg
            {
                frame_id = "fr3_link0"
            },
            pose = new PoseMsg
            {
                position = posicionLocal.To<FLU>(),
                orientation = rotacionLocal.To<FLU>()
            }
        };

        ros.Publish(topicName, targetPoseMsg);
        Debug.Log($"[{origen}] Pose enviada. Pos: X={posicionLocal.x:F2}, Y={posicionLocal.y:F2}, Z={posicionLocal.z:F2}");
    }
}