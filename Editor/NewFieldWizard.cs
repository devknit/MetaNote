
using UnityEngine;
using UnityEditor;

namespace AssetNote
{
	public sealed class NewFieldWizard : EditorWindow
	{
		public static void Open( System.Action<string> callback)
		{
			var wizard = GetWindow<NewFieldWizard>();
			wizard.titleContent = new GUIContent( "Create new field");
			wizard.minSize = new Vector2( 256, 96);
			wizard.maxSize = new Vector2( 256, 96);
			wizard.maximized = false;
			wizard.m_Callback = callback;
			wizard.ShowModal();
		}
		void OnGUI()
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			++EditorGUI.indentLevel;
			EditorGUILayout.LabelField( "New field name:");
			Rect rect = EditorGUILayout.GetControlRect();
			rect.xMax -= 16;
			m_NewFieldName = EditorGUI.TextField( rect, m_NewFieldName);
			--EditorGUI.indentLevel;
			
			rect = EditorGUILayout.GetControlRect();
			rect.xMax -= 16;
			
			if( rect.width > 46)
			{
				rect.xMin += rect.width - 46;
			}
			EditorGUI.BeginDisabledGroup( string.IsNullOrEmpty( m_NewFieldName));
			{
				if( GUI.Button( rect, "Add") != false)
				{
					m_Callback?.Invoke( m_NewFieldName);
					Close();
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		
		[SerializeField]
		string m_NewFieldName = default;
		[System.NonSerialized]
		System.Action<string> m_Callback;
	}
}
