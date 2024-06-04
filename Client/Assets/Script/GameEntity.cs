using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proto;

public class GameEntity : MonoBehaviour
{
	public int entityId;
	public Vector3 position;
	public Vector3 direction;
	public bool isMine;
	// Start is called before the first frame update
	void Start()
	{
		//开启协程，每秒10次，向服务器上传hero的属性
		StartCoroutine(SyncRequest());
	}

	
	//向服务器发送同步请求
	IEnumerator SyncRequest()
	{
		while (true)
		{
			SpaceEntitySyncRequest req = new SpaceEntitySyncRequest();
			//req.EntitySync.Entity
			//Position,Direction,entityID
			// req.EntitySync

			yield return new WaitForSeconds(0.1f);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!isMine) {
			this.transform.position = new Vector3(position.x, position.y, position.z);
			this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);	
		}
		else {
			// 主角
			this.position = transform.position;
			var q = transform.rotation;
			this.direction = new Vector3(q.x, q.y, q.z);
			// 发送同步消息给服务器
			
		}
	}

	/// <summary>
	/// 设置GameEntity的位置和坐标
	/// </summary>
	/// <param name="nEntity"></param>
	public void SetData(NEntity nEntity, bool toTransform = false)
	{
		this.entityId = nEntity.Id;
		var p = nEntity.Position;
		var d = nEntity.Direction;
		this.position = new Vector3(p.X , p.Y , p.Z );
		this.direction = new Vector3(d.X , d.Y , d.Z );
		position *= 0.001f;
		direction *= 0.001f;

		this.transform.position = position;
		this.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);	
	}
}