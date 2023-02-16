using System.Collections.Concurrent;
using System.Reflection;

namespace CxActorify;

/// <summary>
///     A Concrete implementation of a request actor that handles execution of messages concurrently
/// </summary>
public abstract class Actor : IRequestActor<IRequestMessage, ActorRequest>, IDisposable
{
	public void Dispose()
	{
		_disposed = true;

		_requestCollection.Dispose();
	}

	public void Request( ActorRequest request )
	{
		_requestCollection.Add( request );

		TryStartRequestExecutionWorker();
	}

	public void Request( IRequestMessage message ) =>
		Request( message.ToRequest() );

	public Task RequestAsync( ActorRequest request ) =>
		Task.Run( () => Request( request ) );

	public Task RequestAsync( IRequestMessage message ) =>
		RequestAsync( message.ToRequest() );

	/// <summary>
	///     Blocks the current thread and waits all the requests to be finished
	/// </summary>
	public void WaitAllRequestsFinished()
	{
		if ( _requestExecutionThread is { IsAlive: true } )
			_requestExecutionThread.Join();
	}

	public int RequestCount => _requestCollection.Count;

	readonly BlockingCollection<ActorRequest> _requestCollection = new();

	readonly object _threadLock = new();

	// TODO: change this into async, and return Task when request finished
	Thread? _requestExecutionThread;

	bool _disposed;

	void TryStartRequestExecutionWorker()
	{
		lock ( _threadLock )
		{
			// is not null & IsAlive property = true
			if ( _requestExecutionThread is { IsAlive: true } )
				return;
			// else

			_requestExecutionThread = new Thread( RequestExecutionWorker );
			_requestExecutionThread.Start();
		}
	}

	void RequestExecutionWorker()
	{
		while ( !( _disposed || _requestCollection.Count == 0 ) )
		{
			ActorRequest request = _requestCollection.Take();
			ParseRequest( request );
		}
	}

	void ParseRequest( ActorRequest request )
	{
		// TODO: cache methods
		MethodInfo[] methods = GetType().GetMethods( BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic );

		foreach ( MethodInfo method in methods )
		{
			IEnumerable<HandleRequestAttribute> attributes = method.GetCustomAttributes<HandleRequestAttribute>( true );

			foreach ( HandleRequestAttribute handleRequestAttribute in attributes )
			{
				if ( request.Message.GetType() != handleRequestAttribute.HandlingType )
					continue;

				ParameterInfo[] methodParameters = method.GetParameters();

				if ( methodParameters.Length != 1 )
					throw new ArgumentException( $"Method {method.Name} on instance {this} expected 1 parameter, but found {methodParameters.Length}" );
				// else

				method.Invoke( this, new[] { request.Message } );

				return;
			}

			// else 
			// TODO: HANDLE DEFAULT HERE
		}
	}
}
