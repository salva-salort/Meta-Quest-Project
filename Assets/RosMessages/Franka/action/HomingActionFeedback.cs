using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.Franka
{
    public class HomingActionFeedback : ActionFeedback<HomingFeedback>
    {
        public const string k_RosMessageName = "franka_msgs/HomingActionFeedback";
        public override string RosMessageName => k_RosMessageName;


        public HomingActionFeedback() : base()
        {
            this.feedback = new HomingFeedback();
        }

        public HomingActionFeedback(HeaderMsg header, GoalStatusMsg status, HomingFeedback feedback) : base(header, status)
        {
            this.feedback = feedback;
        }
        public static HomingActionFeedback Deserialize(MessageDeserializer deserializer) => new HomingActionFeedback(deserializer);

        HomingActionFeedback(MessageDeserializer deserializer) : base(deserializer)
        {
            this.feedback = HomingFeedback.Deserialize(deserializer);
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
