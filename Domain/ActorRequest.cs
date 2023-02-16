namespace CxUtils.ActorModel;

/// <summary>
///     Represents a request to an actor which requires casting
/// </summary>
public readonly struct ActorRequest : IActorRequest<IRequestMessage>
{
	public ActorRequest( IRequestMessage message ) =>
		Message = message;

	public IRequestMessage Message { get; }
}
