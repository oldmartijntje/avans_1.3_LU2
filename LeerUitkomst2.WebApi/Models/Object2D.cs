namespace LeerUitkomst2.WebApi.Models
{
    public class TemplateObject2D
    {
        public int PrefabId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float RotationZ { get; set; }
        public int SortingLayer { get; set; }
        public int EnvironmentId { get; set; }
    }

    public class Object2D: TemplateObject2D
    {
        public int Id { get; set; }
    }
}
