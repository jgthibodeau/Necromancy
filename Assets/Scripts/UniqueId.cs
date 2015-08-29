using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Placeholder for UniqueIdDrawer script
public class UniqueIdentifierAttribute : PropertyAttribute {}

public class UniqueId : MonoBehaviour {
	[UniqueIdentifier]
	public string uid;
}

//using UnityEngine;
//
//public class UniqueIdentifierAttribute : PropertyAttribute {}
//
//public class UniqueId : MonoBehaviour
//{
////	[SerializeField]
//	[UniqueIdentifier]
//	public string uniqueId;
//	
//	private System.Guid _guid;
//	public System.Guid guid
//	{
//		get
//		{	
//			//TODO check if field is empty and set it if it is
//			if ( _guid == System.Guid.Empty || System.String.IsNullOrEmpty( uniqueId ) )
//			{
//				_guid = new System.Guid( uniqueId );
//			}
//			return _guid;
//		}
//	}
//	
//	public void Generate()
//	{
//		_guid = System.Guid.NewGuid();
//		uniqueId = guid.ToString();
//	}
//}