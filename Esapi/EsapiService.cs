using System.Linq;
using System.Threading.Tasks;
using EsapiEssentials.Plugin;
using VMS.TPS.Common.Model.API;
using System;

namespace AutoFlash
{
    public class EsapiService : EsapiServiceBase<PluginScriptContext>, IEsapiService
    {
        private readonly StructureGeneration _planGeneration;

        public EsapiService(PluginScriptContext context) : base(context)
        {
            _planGeneration = new StructureGeneration();
        }

        public Task<StructureSetViewModel[]> GetStructureSetsAsync() =>
            RunAsync(context =>
            {

                return context.Patient.StructureSets.Where(x => x.Id.Contains("CBCT") != true)
                .Select(x => new StructureSetViewModel
                {
                    CreationDate = x.HistoryDateTime,
                    ImageId = x.Image.Id,
                    StructureSetId = x.Id,
                    StructureSetIdWithCreationDate = x.Id + " - " + x.HistoryDateTime.ToString(),
                    CanModify = Helpers.CheckStructureSet(context.Patient, x).Item1,
                    Status = Helpers.CheckStructureSet(context.Patient, x).Item2
                })
                .ToArray();
            });

        public Task<StructureViewModel[]> GetStructureIdsAsync(string structureSet, string keyword) =>
            RunAsync(context =>
            {
                var array = context.Patient.StructureSets?
                        .FirstOrDefault(x => x.Id == structureSet)
                        .Structures.Where(x => x.Id.ToUpper().Contains(keyword.ToUpper()) == true).Select(x => new StructureViewModel
                        {
                            StructureId = x.Id,
                            StructureVolume = x.Volume,
                            CanModify = Helpers.CheckStructure(x),
                            IsHighRes = x.IsHighResolution
                        })
                        .ToArray();
                return array;
            });

        public Task AddStructuresAsync(string selectedStructureSetId, string ptvBreastId, string ptvSCVId, string ptvAxillaId, string ptvIMNId,
            string laterality, double anteriorMargin, double lateralMargin, double outerMargin100, double innerMargin100, double outerMargin50, double innerMargin50,
            double lungOptMargin) =>
            RunAsync(context => AddStructures(context.Patient, selectedStructureSetId, ptvBreastId, ptvSCVId, ptvAxillaId, ptvIMNId, laterality, anteriorMargin, lateralMargin,
                outerMargin100, innerMargin100, outerMargin50, innerMargin50, lungOptMargin));

        public void AddStructures(Patient patient, string selectedStructureSetId, string ptvBreastId, string ptvSCVId, string ptvAxillaId, string ptvIMNId, string laterality,
            double anteriorMargin, double lateralMargin, double outerMargin100, double innerMargin100, double outerMargin50, double innerMargin50, double lungOptMargin)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == selectedStructureSetId);
            _planGeneration.CreateStructures(structureSet, ptvBreastId, ptvSCVId, ptvAxillaId,ptvIMNId, laterality, anteriorMargin, lateralMargin, outerMargin100, innerMargin100,
                outerMargin50, innerMargin50, lungOptMargin);           
        }
    }
}
