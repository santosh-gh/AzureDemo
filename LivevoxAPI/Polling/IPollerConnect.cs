using System;
using System.Collections.Generic;
using System.Text;

namespace LivevoxAPI.Polling
{
	/// <summary>
	/// Represents an object that can retrieve the agent status and 
	/// perform some utility functions
	/// </summary>
	interface IPollerConnect
	{
		/// <summary>
		/// Retrieves the latest agent status
		/// </summary>
		/// <returns>Returns the agent status as an array of AgentStatus structs</returns>
		string GetStatus();
		/// <summary>
		/// Changes the agent's status to ready
		/// </summary>
		void ChangeToReady();
		/// <summary>
		/// Retreives a call on hold.
		/// </summary>
		void RetrieveCall();
	}
}
