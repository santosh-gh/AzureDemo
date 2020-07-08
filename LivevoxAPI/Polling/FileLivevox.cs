using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LivevoxAPI.Polling
{
	class FileLivevox : IPollerConnect
	{
		StreamReader _input = null;
		bool _onlyReturnChanges = false;
		string _last = null;

		public FileLivevox(string path)
			: this(path, false) { }
		public FileLivevox(string path, bool onlyReturnChanges)
		{
			_input = new StreamReader(path);
			_onlyReturnChanges = onlyReturnChanges;
		}

		public void ChangeToReady()
		{
			// no implementation
		}

		public string GetStatus()
		{
			if (_input == null) return null;

			string status;

			while (true)
			{
				if ((status = _input.ReadLine()) == null)
				{
					_input.Dispose();
					_input = null;
					return null;
				}

				if (_onlyReturnChanges && status == _last) continue;
				_last = status;
				return status;
			}
		}

		public void RetrieveCall()
		{
			// no implementation
		}
	}
}
