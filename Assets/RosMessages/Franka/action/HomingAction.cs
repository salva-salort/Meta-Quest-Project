using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;


namespace RosMessageTypes.Franka
{
    public class HomingAction : Action<HomingActionGoal, HomingActionResult, HomingActionFeedback, HomingGoal, HomingResult, HomingFeedback>
    {
        public const string k_RosMessageName = "franka_msgs/HomingAction";
        public override string RosMessageName => k_RosMessageName;


        public HomingAction() : base()
        {
            this.action_goal = new HomingActionGoal();
            this.action_result = new HomingActionResult();
            this.action_feedback = new HomingActionFeedback();
        }

        public static HomingAction Deserialize(MessageDeserializer deserializer) => new HomingAction(deserializer);

        HomingAction(MessageDeserializer deserializer)
        {
            this.action_goal = HomingActionGoal.Deserialize(deserializer);
            this.action_result = HomingActionResult.Deserialize(deserializer);
            this.action_feedback = HomingActionFeedback.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.action_goal);
            serializer.Write(this.action_result);
            serializer.Write(this.action_feedback);
        }

    }
}
