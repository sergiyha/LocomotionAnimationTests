using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public Guid Id { get; protected set; }

	[SerializeField] private EnemyManager _enemyManager;
	public Vector3 LockPointOffset;

	private readonly LockPoint _lockPoint = new LockPoint();

	private void Awake()
	{
		Id = System.Guid.NewGuid();
		_enemyManager.AddEnemy(this);
		_lockPoint.UpdatePosition(transform.position + LockPointOffset);
	}

	private void Update()
	{
		_lockPoint.UpdatePosition(transform.position + LockPointOffset);
	}

	public LockPoint GetLockPoint(Vector3 position)
	{
		return _lockPoint;
	}
}