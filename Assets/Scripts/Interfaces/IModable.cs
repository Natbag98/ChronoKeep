using UnityEngine;

interface IModable {
    public void AddMod(Mod mod, bool allow_duplicate=false);
    public void RemoveMod(Mod mod);
}
