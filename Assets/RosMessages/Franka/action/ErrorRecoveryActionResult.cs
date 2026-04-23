using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.Franka
{
    public class ErrorRecoveryActionResult : ActionResult<ErrorRecoveryResult>
    {
        public const string k_RosMessageName = "franka_msgs/ErrorRecoveryActionResult";
        public override string RosMessageName => k_RosMessageName;


        public ErrorRecoveryActionResult() : base()
        {
            this.result = new ErrorRecoveryResult();
        }

        public ErrorRecoveryActionResult(HeaderMsg header, GoalStatusMsg status, ErrorRecoveryResult result) : base(header, status)
        {
            this.result = result;
        }
        public static ErrorRecoveryActionResult Deserialize(MessageDeserializer deserializer) => new ErrorRecoveryActionResult(deserializer);

        ErrorRecoveryActionResult(MessageDeserializer deserializer) : base(deserializer)
        {
            this.result = ErrorRecoveryResult.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.status);
            serializer.Write(this.result);
        }


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
