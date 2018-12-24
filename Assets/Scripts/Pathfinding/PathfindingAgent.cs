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
				foreach (var node in _path)
				{
					node.DrawGizmos(0.5f, true);
				}
			}
		}
	}
}