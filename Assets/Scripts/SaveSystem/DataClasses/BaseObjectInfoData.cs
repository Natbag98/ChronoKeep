using UnityEngine;

public class BaseObjectInfoData {
    public Vector2Int location;
    public string baseObject;
    public string faction;

    public BaseObjectInfoData(Game.BaseObjectInfo baseObjectInfo) {
        location = baseObjectInfo.location;
        baseObject = baseObjectInfo.base_object.name;
        faction = baseObjectInfo.faction.Name;
    }
}
