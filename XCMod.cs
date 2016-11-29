using UnityEngine;
using System.Collections;
using System.IO;

namespace UnityEditor.XCodeEditor
{
	public class XCMod
	{
		private Hashtable _datastore = new Hashtable();
		private ArrayList _libs = null;

		public string name { get; private set; }

		public string path { get; private set; }

		public string group
		{
			get
			{
				return fetch("group");
			}
		}

		private T fetch<T>(string key) where T:new()
		{
			if(_datastore != null && _datastore.Contains(key))
			{
				return (T)_datastore[key];
			}
			return new T();
		}

		private string fetch(string key)
		{
			if(_datastore != null && _datastore.Contains(key))
			{
				return (string)_datastore[key];
			}
			return string.Empty;
		}

		public ArrayList patches
		{
			get
			{
				return fetch<ArrayList>("patches");
			}
		}

		public ArrayList libs
		{
			get
			{
				if(_libs == null)
				{
					var list = fetch<ArrayList>("libs");

					_libs = new ArrayList(list.Count);
					foreach(string fileRef in list)
					{
						Debug.Log("Adding to Libs: " + fileRef);
						_libs.Add(new XCModFile(fileRef));
					}
				}

				return _libs;
			}
		}

		public ArrayList frameworks
		{
			get
			{
				return fetch<ArrayList>("frameworks");
			}
		}

		public ArrayList headerpaths
		{
			get
			{
				return fetch<ArrayList>("headerpaths");
			}
		}

		public ArrayList files
		{
			get
			{
				return fetch<ArrayList>("files");
			}
		}

		public ArrayList folders
		{
			get
			{
				return fetch<ArrayList>("folders");
			}
		}

		public ArrayList excludes
		{
			get
			{
				return fetch<ArrayList>("excludes");
			}
		}

		public ArrayList compiler_flags
		{
			get
			{
				return fetch<ArrayList>("compiler_flags");
			}
		}

		public ArrayList linker_flags
		{
			get
			{
				return fetch<ArrayList>("linker_flags");
			}
		}

		public ArrayList embed_binaries
		{
			get
			{
				return fetch<ArrayList>("embed_binaries");
			}
		}

		public Hashtable plist
		{
			get
			{
				return fetch<Hashtable>("plist");
			}
		}

		public XCMod(string filename)
		{	
			FileInfo projectFileInfo = new FileInfo(filename);
			if(!projectFileInfo.Exists)
			{
				Debug.LogWarning("File does not exist.");
			}
			
			name = System.IO.Path.GetFileNameWithoutExtension(filename);
			path = System.IO.Path.GetDirectoryName(filename);
			
			string contents = projectFileInfo.OpenText().ReadToEnd();
			Debug.Log(contents);
			_datastore = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(contents);
			if(_datastore == null || _datastore.Count == 0)
			{
				Debug.Log(contents);
				throw new UnityException("Parse error in file " + System.IO.Path.GetFileName(filename) + "! Check for typos such as unbalanced quotation marks, etc.");
			}
		}
	}

	public class XCModFile
	{
		public string filePath { get; private set; }

		public bool isWeak { get; private set; }

		public XCModFile(string inputString)
		{
			isWeak = false;
			
			if(inputString.Contains(":"))
			{
				string[] parts = inputString.Split(':');
				filePath = parts[0];
				isWeak = (parts[1].CompareTo("weak") == 0);	
			}
			else
			{
				filePath = inputString;
			}
		}
	}
}
