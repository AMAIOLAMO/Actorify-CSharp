using CxUtils.ActorModel;

public interface IActorRequest<out TMessage>
{
	TMessage Message { get; }
}
