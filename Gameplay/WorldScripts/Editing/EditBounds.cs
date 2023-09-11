using Silk.NET.Maths;

namespace FainCraft.Gameplay.WorldScripts.Editing;
internal struct EditBounds
{
    public Vector3D<int> Min;
    public Vector3D<int> Max;

    public static bool IsOverlapping(EditBounds a, EditBounds b)
    {
        return
        !(
            a.Min.X > b.Max.X ||
            a.Min.Y > b.Max.Y ||
            a.Min.Z > b.Max.Z ||
            a.Max.X < b.Min.X ||
            a.Max.Y < b.Min.Y ||
            a.Max.Z < b.Min.Z
        );
    }
}
