using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class PathfindingAgent : MonoBehaviour
	{
		//[SerializeField] private Vector3 _target;
		[SerializeField] private Transform _target;

		private Vector3 _pathTarget;
		
		private Pathfinder _pathfinder;
		private List<PathfindingNode> _path;
		
		void Awake()
		{
			_pathfinder = FindObjectOfType<Pathfinder>();
		}
		
		void Start ()
		{
			
		}
		
		void Update () {
			if (_pathTarget != _target.position)
			{
				_pathTarget = _target.position;
				_path = _pathfinder.FindPath(transform.position, _pathTarget);
			}
		}

		void OnDrawGizmosSelected()
		{
			if (_path != null)
			{
				Gizmos.color = Color.white;
				foreach (var node in _path)
				{
					Gizmos.DrawLine(node.worldPosition, node.parent.worldPosition);
					node.DrawGizmos(0.25f, true);
				}
			}
		}
	}
}