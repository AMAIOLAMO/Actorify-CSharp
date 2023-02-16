namespace CxUtils.ActorModel;

[AttributeUsage( AttributeTargets.Method )]
public class HandleRequestAttribute : Attribute
{
	public HandleRequestAttribute( Type handlingType ) =>
		HandlingType = handlingType;

	public Type HandlingType { get; }
}

[AttributeUsage( AttributeTargets.Method )]
public class HandleDefaultRequest : Attribute
{
}
