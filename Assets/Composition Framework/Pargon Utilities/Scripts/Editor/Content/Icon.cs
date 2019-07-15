using System;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class Icon
	{
		public static readonly Icon Add = BuiltIn("Toolbar Plus");
		public static readonly Icon CustomAdd = BuiltIn("Toolbar Plus More");
		public static readonly Icon Remove = BuiltIn("Toolbar Minus");
		public static readonly Icon Inspect = BuiltIn("UnityEditor.InspectorWindow");
		public static readonly Icon Expanded = BuiltIn("IN foldout focus on");
		public static readonly Icon Collapsed = BuiltIn("IN foldout focus");
		public static readonly Icon Refresh = BuiltIn("d_preAudioLoopOff");
		public static readonly Icon Load = BuiltIn("SceneLoadIn");
		public static readonly Icon Unload = BuiltIn("SceneLoadOut");
		public static readonly Icon Close = BuiltIn("LookDevClose");
		public static readonly Icon LeftArrow = BuiltIn("tab_prev");
		public static readonly Icon RightArrow = BuiltIn("tab_next");
		public static readonly Icon Info = BuiltIn("console.infoicon");
		public static readonly Icon Warning = BuiltIn("console.warnicon");
		public static readonly Icon Error = BuiltIn("console.erroricon");
		public static readonly Icon Settings = BuiltIn("_Popup");
		public static readonly Icon View = BuiltIn("UnityEditor.LookDevView");
		public static readonly Icon SearchBarLeft = BuiltIn("toolbarsearch");
		public static readonly Icon SearchBarRightCancel = BuiltIn("toolbarsearchCancelButton");
		public static readonly Icon SearchBarRight = BuiltIn("toolbarsearchCancelButtonOff");
		public static readonly Icon Lock = BuiltIn("AssemblyLock");

		private const string _invalidIconError = "(PIIII) failed to create icon content: the built in icon {0} could not be loaded";
		private const string _invalidDataError = "(PIIID) failed to create icon content: the supplied data is not a valid base 64 string";
		private const string _invalidContentError = "(PIIIC) failed to create icon content: the supplied data could not be loaded";

		private string _name;
		private string _data;

		private Texture _content;

		public Texture Content
		{
			get
			{
				if (_content == null)
				{
					_content = EditorGUIUtility.whiteTexture; // set to something so if loading fails it only fails once

					if (!string.IsNullOrEmpty(_name))
					{
						var content = EditorGUIUtility.IconContent(_name);

						if (content != null)
							_content = content.image;
						else
							Debug.LogErrorFormat(_invalidIconError, _name);
					}
					else if (!string.IsNullOrEmpty(_data))
					{
						var content = new Texture2D(1, 1);
						var data = Convert.FromBase64String(_data);

						content.hideFlags = HideFlags.HideAndDontSave;

						if (data != null)
						{
							if (content.LoadImage(data))
								_content = content;
							else
								Debug.LogError(_invalidContentError);
						}
						else
						{
							Debug.LogError(_invalidDataError);
						}
					}
				}

				return _content;
			}
		}

		public static Icon BuiltIn(string name)
		{
			return new Icon { _name = name };
		}

		public static Icon Base64(string data)
		{
			return new Icon { _data = data };
		}
	}
}
