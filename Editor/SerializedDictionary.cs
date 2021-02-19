
using UnityEngine;
using System.Collections.Generic;

namespace AssetNote
{
	public abstract class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			m_Keys.Clear();
			m_Values.Clear();
			
			foreach( var item in this)
			{
				m_Keys.Add( item.Key);
				m_Values.Add( item.Value);
			}
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();
			
			for( int i0 = 0; i0 < m_Keys.Count; i0++)
			{
				this[ m_Keys[ i0]] = m_Values[ i0];
			}
		}
		
		[SerializeField, HideInInspector]
		List<TKey> m_Keys = new List<TKey>();
		[SerializeField, HideInInspector]
		List<TValue> m_Values = new List<TValue>();
	}
}
