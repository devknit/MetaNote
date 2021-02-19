
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AssetNote
{
	[InitializeOnLoad]
	static class MetaNoteGUI
	{
		static MetaNoteGUI()
		{
			Editor.finishedDefaultHeaderGUI += OnInspectorGUI;
		}
		static void OnInspectorGUI( Editor editor)
		{
			if( editor.targets.Length == 1)
			{
				string assetPath = AssetDatabase.GetAssetPath( editor.target);
				if( (assetPath?.Contains( "Assets/") ?? false) != false)
				{
					AssetImporter importer = AssetImporter.GetAtPath( assetPath);
					bool foldout, plus, save, info = false;
					UserData userData = null;
					
					if( bool.TryParse( EditorUserSettings.GetConfigValue( "MetaNote.Foldout") , out foldout) == false)
					{
						foldout = false;
					}
					if( string.IsNullOrEmpty( importer.userData) == false)
					{
						userData = JsonUtility.FromJson<UserData>( importer.userData);
					}
					if( (userData?.Count ?? 0) == 0)
					{
						userData = new UserData
						{ 
							{ string.Empty, string.Empty }
						};
					}
					foreach( var item in userData)
					{
						if( string.IsNullOrEmpty( item.Value) == false)
						{
							info = true;
							break;
						}
					}
					bool result = Foldout( foldout, "Note", info, out plus, out save);
					if( foldout != result)
					{
						EditorUserSettings.SetConfigValue( "MetaNote.Foldout", result.ToString());
						foldout = result;
					}
					if( foldout != false)
					{
						++EditorGUI.indentLevel;
						OnNoteGUI( importer, userData, plus, save);
						--EditorGUI.indentLevel;
					}
				}
			}
		}
		static bool Foldout( bool foldout, string title, bool info, out bool plus, out bool save)
		{
			plus = false;
			save = false;
			
			var style = new GUIStyle( "PreToolbar");
			style.contentOffset = new Vector2( 20f, 0f);
			
			if( info == false)
			{
				style.fontStyle = FontStyle.Normal;
			}
			var rect = GUILayoutUtility.GetRect( 16f, 22f, style);
			rect.xMax += 4;
			GUI.Box( rect, title, style);
			rect.xMax -= 4;
			
			var ev = Event.current;
			
			var toggleRect = new Rect( rect.x + 6f, rect.y + 4f, 13f, 13f);
			if( ev.type == EventType.Repaint)
			{
				EditorStyles.foldout.Draw( toggleRect, false, false, foldout, false);
			}
			Rect foldoutRect = rect;
			
			if( foldout != false)
			{
				foldoutRect.xMax -= 26 * 2;
			}
			if( ev.type == EventType.MouseDown && foldoutRect.Contains( ev.mousePosition) != false)
			{
				foldout = !foldout;
				ev.Use();
			}
			if( foldout != false)
			{
				var buttonRect = rect;
				
				buttonRect.y += 1;
				
				if( buttonRect.width > 26)
				{
					buttonRect.xMin += buttonRect.width - 26;
				}
				if( GUI.Button( buttonRect, EditorGUIUtility.TrIconContent( "Toolbar Plus"), "toolbarbutton") != false)
				{
					plus = true;
				}
				buttonRect.x -= 26;
				
				if( GUI.Button( buttonRect, EditorGUIUtility.TrIconContent( "SaveAs"), "toolbarbutton") != false)
				{
					save = true;
				}
			}
			return foldout;
		}
		static void OnNoteGUI( AssetImporter importer, UserData userData, bool plus, bool save)
		{
			string comment = userData[ string.Empty];
			var userDatas = userData.ToArray();
			bool saveAndReimport = false;
			
			foreach( var item in userDatas)
			{
				if( item.Key != string.Empty)
				{
					EditorGUI.BeginChangeCheck();
					Rect itemRect = EditorGUILayout.GetControlRect();
					Rect textFieldRect = itemRect;
					textFieldRect.xMax -= 30;
					userData[ item.Key] = EditorGUI.DelayedTextField( textFieldRect, item.Key, item.Value);
					if( EditorGUI.EndChangeCheck() != false)
					{
						importer.userData = JsonUtility.ToJson( userData, false);
						saveAndReimport = true;
					}
					Rect buttonRect = itemRect;
					buttonRect.xMin += textFieldRect.width + 4;
					
					if( GUI.Button( buttonRect, EditorGUIUtility.TrIconContent( "Toolbar Minus"), "RL FooterButton") != false)
					{
						if( userData.ContainsKey( item.Key) != false)
						{
							userData.Remove( item.Key);
							importer.userData = JsonUtility.ToJson( userData, false);
							saveAndReimport = true;
						}
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			s_ScrollPos = EditorGUILayout.BeginScrollView( s_ScrollPos,GUILayout.Height( 80));
			userData[ string.Empty] = EditorGUILayout.TextArea( comment, GUILayout.ExpandHeight( true));
			EditorGUILayout.EndScrollView();
			if( EditorGUI.EndChangeCheck() != false)
			{
				importer.userData = JsonUtility.ToJson( userData, false);
			}
			if( plus != false)
			{
				NewFieldWizard.Open( (newFieldName) =>
				{
					if( string.IsNullOrEmpty( newFieldName) == false)
					{
						if( userData.ContainsKey( newFieldName) == false)
						{
							userData[ newFieldName] = string.Empty;
							importer.userData = JsonUtility.ToJson( userData, false);
							saveAndReimport = true;			
						}
					}
				});
			}
			if( save != false)
			{
				saveAndReimport = true;
			}
			if( saveAndReimport != false)
			{
				if( userData.Count == 1)
				{
					if( string.IsNullOrEmpty( userData[ string.Empty]) != false)
					{
						importer.userData = string.Empty;
					}
				}
				importer.SaveAndReimport();
			}
		}
		
		static Vector2 s_ScrollPos = default;
	}
}
