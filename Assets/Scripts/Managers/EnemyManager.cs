using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	private readonly List<Enemy> _enemies = new List<Enemy>();
	private readonly Dictionary<Guid, Enemy> _cache = new Dictionary<Guid, Enemy>();

	public void AddEnemy(Enemy enemy)
	{
		_enemies.Add(enemy);
		_cache.Add(enemy.Id, enemy);
	}

	public void RemoveEnemy(Enemy enemy)
	{
		_enemies.Remove(enemy);
		_cache.Remove(enemy.Id);
	}

	public Enemy GetEnemyForLock(Vector3 position, Quaternion rotation, float maxDist = 10)
	{
		var index = -1;
		var angle = 360f;

		for (var i = 0; i < _enemies.Count; i++)
		{
			var dir = _enemies[i].transform.position - position;
			var deltaAngle = Quaternion.Angle(rotation, Quaternion.LookRotation(dir.normalized));
			if (dir.magnitude < maxDist && deltaAngle < angle)
			{
				angle = deltaAngle;
				index = i;
			}
		}

		return index == -1 ? null : _enemies[index];
	}
}