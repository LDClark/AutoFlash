using System.Threading.Tasks;

namespace AutoFlashIMRT
{
    public interface IEsapiService
    {
        Task<StructSet[]> GetStructureSetsAsync();
        Task<Struct[]> GetStructureIdsAsync(string structureSet, string keyword);
        Task AddStructuresAsync(string selectedStructureSetId, string ptvBreastId, string ptvSCVId, string ptvAxillaId, string ptvIMNId,
            string laterality, double anteriorMargin, double lateralMargin, double outerMargin100, double innerMargin100, double outerMargin50, double innerMargin50);
    }
}
