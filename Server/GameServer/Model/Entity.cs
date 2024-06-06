using Summer;

namespace GameServer.Model;

public class Entity
{
    private int id;
    private Vector3Int position;    //位置
    private Vector3Int direction;   //方向
    private int spaceId;    //所在地图ID
    public int SpaceId
    {
        get { return spaceId; }
        set { spaceId = value; }
    }
    public int entityId { get { return id; } }
    public Vector3Int Position
    {
        get { return position; }
        set { position = value; }
    }
    public Vector3Int Direction
    {
        get { return direction; }
        set { direction = value; }
    }


    public Entity(int id,Vector3Int position,Vector3Int direction)
    {
        this.id = id;
        this.position = position;
        this.direction = direction;
    }

    public Proto.NEntity GetData()
    {
        var data = new Proto.NEntity();
        data.Id = id;
        data.Position = new Proto.NVector3() { X = position.x, Y = position.y, Z = position.z };
        data.Direction = new Proto.NVector3() { X = direction.x, Y = -direction.y, Z = -direction.z };
        return data;
    }
    public void SetEntityData(Proto.NEntity entity)
    {
        position.x = entity.Position.X;
        position.y = entity.Position.Y;
        position.z = entity.Position.Z;
        direction.x = entity.Direction.X;
        direction.y = entity.Direction.Y;
        direction.z = entity.Direction.Z;
    }
}