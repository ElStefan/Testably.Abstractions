﻿namespace Testably.Abstractions.Testing.TimeSystem;

/// <summary>
///     The timer strategy.
/// </summary>
public class TimerStrategy : ITimerStrategy
{
	/// <summary>
	///     The default time strategy uses <see cref="TimerMode.StartImmediately" />.
	/// </summary>
	public static ITimerStrategy Default { get; }
		= new TimerStrategy(TimerMode.StartImmediately);

	/// <inheritdoc cref="ITimerStrategy.Mode"/>
	public TimerMode Mode { get; }

	/// <inheritdoc cref="ITimerStrategy.SwallowExceptions"/>
	public bool SwallowExceptions { get; }

	/// <summary>
	///     Initializes a new instance of <see cref="TimerStrategy" />.
	/// </summary>
	/// <param name="mode">The timer mode.</param>
	/// <param name="swallowExceptions">Flag, indicating if exceptions should be swallowed.</param>
	public TimerStrategy(
		TimerMode mode = TimerMode.StartImmediately,
		bool swallowExceptions = false)
	{
		Mode = mode;
		SwallowExceptions = swallowExceptions;
	}
}