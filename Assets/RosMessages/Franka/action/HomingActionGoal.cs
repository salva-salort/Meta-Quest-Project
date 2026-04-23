using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.Franka
{
    public class HomingActionGoal : ActionGoal<HomingGoal>
    {
        public const string k_RosMessageName = "franka_msgs/HomingActionGoal";
        public override string RosMessageName => k_RosMessageName;


        public HomingActionGoal() : base()
        {
            this.goal = new HomingGoal();
        }

        public HomingActionGoal(HeaderMsg header, GoalIDMsg goal_id, HomingGoal goal) : base(header, goal_id)
        {
            this.goal = goal;
        }
        public static HomingActionGoal Deserialize(MessageDeserializer deserializer) => new HomingActionGoal(deserializer);

        HomingActionGoal(MessageDeserializer deserializer) : base(deserializer)
        {
            this.goal = HomingGoal.Deserialize(deserializer);
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
