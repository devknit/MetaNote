
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AssetNote
{
	static class MetaExporter
	{
		[MenuItem( "Assets/Assets/Export Meta Property", validate=true)]
		static bool Validate()
		{
			return (Selection.assetGUIDs?.Length ?? 0) > 0;
		}
		[MenuItem( "Assets/Assets/Export Meta Property", validate=false)]
		static void Export()
		{
			if( (Selection.assetGUIDs?.Length ?? 0) > 0)
			{
				string exportPath = EditorUtility.SaveFilePanelInProject( "Create assets table", "MetaProperty", "md", string.Empty);
				if( string.IsNullOrEmpty( exportPath) == false)
				{
					var assetPaths = new List<string>();
					
					for( int i0 = 0; i0 < Selection.assetGUIDs.Length; ++i0)
					{
						string assetPath = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ i0]);
						if( (assetPath?.Contains( "Assets/") ?? false) != false)
						{
							assetPaths.Add( assetPath);
						}
					}
					assetPaths.Sort();
					
					var table = new Dictionary<string, string[]>
					{
						{ kAssetPathKey, new string[ Selection.assetGUIDs.Length] },
					};
					for( int i0 = 0; i0 < assetPaths.Count; ++i0)
					{
						string assetPath = assetPaths[ i0];
						AssetImporter importer = AssetImporter.GetAtPath( assetPath);
						
						if( importer != null)
						{
							table[ kAssetPathKey][ i0] = Path.GetFileName( assetPath);
							
							if( string.IsNullOrEmpty( importer.userData) == false)
							{
								UserData userData = JsonUtility.FromJson<UserData>( importer.userData);
								
								foreach( var item in userData)
								{
									if( table.ContainsKey( item.Key) == false)
									{
										table.Add( item.Key, new string[ Selection.assetGUIDs.Length]);
									}
									table[ item.Key][ i0] = item.Value;
								}
							}
						}
					}
					string[] list;
					
					if( table.TryGetValue( string.Empty, out list) != false)
					{
						table.Remove( string.Empty);
						var sort = new Dictionary<string, string[]>( table);
						
						for( int i0 = 0; i0 < list.Length; ++i0)
						{
							if( string.IsNullOrWhiteSpace( list[ i0]) == false)
							{
								sort.Add( string.Empty, list);
								break;
							}
						}
						table = sort;
					}
					var builder = new StringBuilder();
					
					foreach( var key in table.Keys)
					{
						string caption = key switch
						{
							kAssetPathKey => "FileName",
							"" => "Note",
							_ => key
						};
						builder.Append( $"| {caption} ");
					}
					builder.AppendLine( "| ");
					
					foreach( var key in table.Keys)
					{
						builder.Append( $"| --- ");
					}
					builder.AppendLine( "| ");
					
					for( int i0 = 0; i0 < Selection.assetGUIDs.Length; ++i0)
					{
						foreach( var value in table.Values)
						{
							string str = value[ i0]?.Replace( "\r\n", "<br>").Replace( "\n", "<br>") ?? string.Empty;
							builder.Append( $"| {str} ");
						}
						builder.AppendLine( "| ");
					}
					using( var stream = new StreamWriter( exportPath))
					{
						stream.Write( builder.ToString());
					}
					AssetDatabase.Refresh();
				}
			}
		}
		
		const string kAssetPathKey = "0000000000000000000000000000000000000000";
	}
}
