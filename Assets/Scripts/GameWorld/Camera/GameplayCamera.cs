﻿using UnityEngine;
using System.Collections;

public class GameplayCamera : MonoBehaviour {

	private bool  _isDraging;
	private Rect  _initialCameraRect;
	private float _minWidth;
	private float _dragDistance;

	public GameWorld _gameWorld;

	public float _dampTime;
	public float _levelWidth;

	void Start()
	{
		_dragDistance = 1f;
		_initialCameraRect = CalculateCameraRect();
		_minWidth = _initialCameraRect.width;
	}
	
	void Update()
	{
		// Updating camera zoom
		CalculateOrthographicSize();

		if(!_isDraging)
		{
			Bird target = _gameWorld.GetCurrentBird();

			Vector3 cameraNextPos = Vector3.zero;
			cameraNextPos.x = LeftBound();

			if(target && target.OutOfSlingShot && target.IsInFrontOfSlingshot())
			{
				cameraNextPos.x = RightBound()/2f;
			}
			else
			{
				if(_dragDistance < 0f)
					
					cameraNextPos.x = RightBound();
				else
					cameraNextPos.x = LeftBound();
			}

			FollowTarget(cameraNextPos);
		}

		_isDraging = false;
	}

	void FollowTarget(Vector3 targetPosition)
	{
		Vector3 cameraPos = transform.position;
		cameraPos.x = Mathf.Lerp(cameraPos.x, targetPosition.x, _dampTime * Time.deltaTime);
		transform.position = cameraPos;
	}

	public float LeftBound()
	{
		return _initialCameraRect.width/2f - _levelWidth/2f;
	}

	public float RightBound()
	{
		return _levelWidth/2f - _initialCameraRect.width/2f;
	}
	
	public void DragCamera(Vector3 dragDistance)
	{
		Bird target = _gameWorld.GetCurrentBird();

		if(target && target.IsFlying())
			return;

		_isDraging = true;

		if(dragDistance.magnitude > 0f)
			_dragDistance = dragDistance.x;

		Vector3 cameraPos = transform.position;
		cameraPos.x = Mathf.Lerp(cameraPos.x, cameraPos.x - dragDistance.x, _dampTime * Time.deltaTime);
		cameraPos.x = Mathf.Clamp(cameraPos.x, LeftBound(), RightBound());
		transform.position = cameraPos;
	}

	public void ZoomCamera(float zoomFactor)
	{
		_initialCameraRect.width += zoomFactor;
		_initialCameraRect.width = Mathf.Clamp(_initialCameraRect.width,  _minWidth, _levelWidth);
	}

	public Rect CalculateCameraRect()
	{
		float height = 2f * camera.orthographicSize;
		float width = height * camera.aspect;	

		return new Rect(transform.position.x - width/2f, transform.position.y - height/2f, width, height);
	}

	public void CalculateOrthographicSize()
	{
		float orthographicSize = camera.orthographicSize;
		Vector3 topRight = new Vector3(_initialCameraRect.x + _initialCameraRect.width, _initialCameraRect.y, 0f);
		Vector3 topRightAsViewport = camera.WorldToViewportPoint(topRight);
		
		if (topRightAsViewport.x >= topRightAsViewport.y)

			orthographicSize = Mathf.Abs(_initialCameraRect.width) / camera.aspect / 2f;
		else
			orthographicSize = Mathf.Abs(_initialCameraRect.height) / 2f;
		
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, orthographicSize, _dampTime * Time.deltaTime);
	}
}
