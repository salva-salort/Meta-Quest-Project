using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.Franka
{
    public class ErrorRecoveryActionGoal : ActionGoal<ErrorRecoveryGoal>
    {
        public const string k_RosMessageName = "franka_msgs/ErrorRecoveryActionGoal";
        public override string RosMessageName => k_RosMessageName;


        public ErrorRecoveryActionGoal() : base()
        {
            this.goal = new ErrorRecoveryGoal();
        }

        public ErrorRecoveryActionGoal(HeaderMsg header, GoalIDMsg goal_id, ErrorRecoveryGoal goal) : base(header, goal_id)
        {
            this.goal = goal;
        }
        public static ErrorRecoveryActionGoal Deserialize(MessageDeserializer deserializer) => new ErrorRecoveryActionGoal(deserializer);

        ErrorRecoveryActionGoal(MessageDeserializer deserializer) : base(deserializer)
        {
            this.goal = ErrorRecoveryGoal.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.goal_id);
            serializer.Write(this.goal);
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
