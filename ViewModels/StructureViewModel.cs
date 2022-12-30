namespace AutoFlash
{
    public class StructureViewModel
    {
        public string StructureId { get; set; }
        public double StructureVolume { get; set; }
        public bool CanModify { get; set; }
        public bool IsHighRes { get; set; }
        public bool IsSelected { get; set; }
    }
}
