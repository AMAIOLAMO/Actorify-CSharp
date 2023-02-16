namespace CxUtils.ActorModel;

/// <summary>
///     Implements an actor which should allow requesting of messages
/// </summary>
public interface IRequestActor<TMessage, in TActorRequest> where TActorRequest : IActorRequest<TMessage>
{
	/// <summary>
	///     Requests an actor with the given <paramref name="request" />
	/// </summary>
	void Request( TActorRequest request );
}
