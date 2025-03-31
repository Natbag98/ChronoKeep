using UnityEngine;

public class BaseObjectInfoData {
    public Vector2IntData location;
    public string baseObject;
    public string faction;

    public BaseObjectInfoData(Game.BaseObjectInfo baseObjectInfo) {
        location = new(baseObjectInfo.location);
        baseObject = baseObjectInfo.base_object.name;
        faction = baseObjectInfo.faction.Name;
    }

    public BaseObjectInfoData() {}
}
