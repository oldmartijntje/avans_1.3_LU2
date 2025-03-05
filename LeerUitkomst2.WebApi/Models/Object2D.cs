namespace LeerUitkomst2.WebApi.Models
{
    public class Object2DTemplate
    {
        public int PrefabId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float RotationZ { get; set; }
        public int SortingLayer { get; set; }
        public int EnvironmentId { get; set; }

        public void FixRotation()
        {
            this.RotationZ = this.RotationZ % 360;
        }
    }

    public class Object2D: Object2DTemplate
    {
        public int Id { get; set; }
    }
}
