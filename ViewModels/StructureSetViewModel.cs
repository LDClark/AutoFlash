using System;

namespace AutoFlash
{
    public class StructureSetViewModel
    {
        public string ImageId { get; set; }
        public string StructureSetId { get; set; }
        public DateTime CreationDate { get; set; }
        public string StructureSetIdWithCreationDate { get; set; }
        public bool CanModify { get; set; }
        public string Status { get; set; }
    }
}
