using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.Franka
{
    public class HomingActionResult : ActionResult<HomingResult>
    {
        public const string k_RosMessageName = "franka_msgs/HomingActionResult";
        public override string RosMessageName => k_RosMessageName;


        public HomingActionResult() : base()
        {
            this.result = new HomingResult();
        }

        public HomingActionResult(HeaderMsg header, GoalStatusMsg status, HomingResult result) : base(header, status)
        {
            this.result = result;
        }
        public static HomingActionResult Deserialize(MessageDeserializer deserializer) => new HomingActionResult(deserializer);

        HomingActionResult(MessageDeserializer deserializer) : base(deserializer)
        {
            this.result = HomingResult.Deserialize(deserializer);
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
