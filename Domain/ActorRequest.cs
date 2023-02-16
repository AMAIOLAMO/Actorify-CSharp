using System.Diagnostics.Contracts;

namespace CxActorify;

/// <summary>
///     Represents a request to an actor which requires casting
/// </summary>
public readonly struct ActorRequest
{
	public ActorRequest( IRequestMessage message ) =>
		Message = message;

	public IRequestMessage Message { get; }
}
public static class ActorRequestConversionExtensions
{
	/// <summary>
	///     Wraps this request message into <see cref="ActorRequest" />
	/// </summary>
	[Pure]
	public static ActorRequest ToRequest( this IRequestMessage requestMessage ) =>
		new( requestMessage );
}
