using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;


namespace RosMessageTypes.Franka
{
    public class ErrorRecoveryAction : Action<ErrorRecoveryActionGoal, ErrorRecoveryActionResult, ErrorRecoveryActionFeedback, ErrorRecoveryGoal, ErrorRecoveryResult, ErrorRecoveryFeedback>
    {
        public const string k_RosMessageName = "franka_msgs/ErrorRecoveryAction";
        public override string RosMessageName => k_RosMessageName;


        public ErrorRecoveryAction() : base()
        {
            this.action_goal = new ErrorRecoveryActionGoal();
            this.action_result = new ErrorRecoveryActionResult();
            this.action_feedback = new ErrorRecoveryActionFeedback();
        }

        public static ErrorRecoveryAction Deserialize(MessageDeserializer deserializer) => new ErrorRecoveryAction(deserializer);

        ErrorRecoveryAction(MessageDeserializer deserializer)
        {
            this.action_goal = ErrorRecoveryActionGoal.Deserialize(deserializer);
            this.action_result = ErrorRecoveryActionResult.Deserialize(deserializer);
            this.action_feedback = ErrorRecoveryActionFeedback.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.action_goal);
            serializer.Write(this.action_result);
            serializer.Write(this.action_feedback);
        }

    }
}
