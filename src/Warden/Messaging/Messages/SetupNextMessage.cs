namespace Warden.Messaging.Messages;

public sealed record SetupNextMessage(Type ViewType, int StepIndex);
