namespace Castle.Components.Validator
{
	/// <summary>
	/// Defines the entry point for validation.
	/// </summary>
	public interface IValidatorRunner
	{
		/// <summary>
		/// Determines whether the specified instance is valid.
		/// <para>
		/// All validators are run.
		/// </para>
		/// </summary>
		/// <param name="objectInstance">The object instance to be validated (cannot be null).</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified obj is valid; otherwise, <see langword="false"/>.
		/// </returns>
		bool IsValid(object objectInstance);

		/// <summary>
		/// Determines whether the specified instance is valid.
		/// <para>
		/// All validators are run for the specified <see cref="RunWhen"/> phase.
		/// </para>
		/// </summary>
		/// <param name="objectInstance">The object instance to be validated (cannot be null).</param>
		/// <param name="runWhen">Restrict the set returned to the phase specified</param>
		/// <returns>
		/// <see langword="true"/> if the specified instance is valid; otherwise, <see langword="false"/>.
		/// </returns>
		bool IsValid(object objectInstance, RunWhen runWhen);

		/// <summary>
		/// Checks whether an error summary exists for this instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// <see langword="true"/> if and only if an error summary exists. See <see cref="GetErrorSummary"/>
		/// for detailed conditions.
		/// </returns>
		bool HasErrors(object instance);

		/// <summary>
		/// Gets the error list per instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// The error summary for the instance or <see langword="null"/> if the instance
		/// was either valid or has not been validated before.
		/// </returns>
		ErrorSummary GetErrorSummary(object instance);
	}
}