using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.Franka
{
    public class ErrorRecoveryActionFeedback : ActionFeedback<ErrorRecoveryFeedback>
    {
        public const string k_RosMessageName = "franka_msgs/ErrorRecoveryActionFeedback";
        public override string RosMessageName => k_RosMessageName;


        public ErrorRecoveryActionFeedback() : base()
        {
            this.feedback = new ErrorRecoveryFeedback();
        }

        public ErrorRecoveryActionFeedback(HeaderMsg header, GoalStatusMsg status, ErrorRecoveryFeedback feedback) : base(header, status)
        {
            this.feedback = feedback;
        }
        public static ErrorRecoveryActionFeedback Deserialize(MessageDeserializer deserializer) => new ErrorRecoveryActionFeedback(deserializer);

        ErrorRecoveryActionFeedback(MessageDeserializer deserializer) : base(deserializer)
        {
            this.feedback = ErrorRecoveryFeedback.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.status);
            serializer.Write(this.feedback);
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
