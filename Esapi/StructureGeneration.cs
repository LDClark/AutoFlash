using System;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace AutoFlashIMRT
{
    public class StructureGeneration
    {
        public void CreateStructures(StructureSet structureSet, string ptvBreastId, string ptvSCVId, string ptvAxillaId, string ptvIMNId,
            string laterality, double anteriorMargin, double lateralMargin, double outerMargin100, double innerMargin100, double outerMargin50, double innerMargin50)
        {
            structureSet.Patient.BeginModifications();
            Structure ptvBreast = structureSet.Structures.Where(structure => structure.Id == ptvBreastId).FirstOrDefault();
            Structure ptvSCV = structureSet.Structures.Where(structure => structure.Id == ptvSCVId).FirstOrDefault();
            Structure ptvAxilla = structureSet.Structures.Where(structure => structure.Id == ptvAxillaId).FirstOrDefault();
            Structure ptvIMN = structureSet.Structures.Where(structure => structure.Id == ptvIMNId).FirstOrDefault();
            Structure body = structureSet.Structures.Where(structure => structure.Id == "BODY").FirstOrDefault();

            Structure ptvOpt;
            Structure ptvExp;
            Structure ptvOptExp;
            Structure CTHU0;
            Structure ring100;
            Structure ring50;
            Structure lungOpt;
            Structure bodyCrop5mm;
            Structure bodyCrop2mm;

            string ptvOptId = "PTV_opt";
            string ptvExpId = "PTV_exp";
            string ptvOptExpId = "PTV_opt+exp";
            string CTHU0Id = "CT HU = 0";
            string ring100Id = "Ring_100%";
            string ring50Id = "Ring_50%";
            string lungOptId = "Lung_opt";
            string bodyCrop5mmId = "Spacer 5mm";
            string bodyCrop2mmId = "Spacer 2mm";


            if (structureSet.Structures.Any(x => x.Id == ptvOptId))
                ptvOpt = structureSet.Structures.Where(x => x.Id == ptvOptId).FirstOrDefault();
            else
                ptvOpt = structureSet.AddStructure("CONTROL", ptvOptId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ptvExpId))
                ptvExp = structureSet.Structures.Where(x => x.Id == ptvExpId).FirstOrDefault();
            else
                ptvExp = structureSet.AddStructure("CONTROL", ptvExpId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ptvOptExpId))
                ptvOptExp = structureSet.Structures.Where(x => x.Id == ptvOptExpId).FirstOrDefault();
            else
                ptvOptExp = structureSet.AddStructure("CONTROL", ptvOptExpId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == CTHU0Id))
                CTHU0 = structureSet.Structures.Where(x => x.Id == CTHU0Id).FirstOrDefault();
            else
                CTHU0 = structureSet.AddStructure("CONTROL", CTHU0Id);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ring100Id))
                ring100 = structureSet.Structures.Where(x => x.Id == ring100Id).FirstOrDefault();
            else
                ring100 = structureSet.AddStructure("CONTROL", ring100Id);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ring50Id))
                ring50 = structureSet.Structures.Where(x => x.Id == ring50Id).FirstOrDefault();
            else
                ring50 = structureSet.AddStructure("CONTROL", ring50Id);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == lungOptId))
                lungOpt = structureSet.Structures.Where(x => x.Id == lungOptId).FirstOrDefault();
            else
                lungOpt = structureSet.AddStructure("CONTROL", lungOptId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == bodyCrop5mmId))
                bodyCrop5mm = structureSet.Structures.Where(x => x.Id == bodyCrop5mmId).FirstOrDefault();
            else
                bodyCrop5mm = structureSet.AddStructure("CONTROL", bodyCrop5mmId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == bodyCrop2mmId))
                bodyCrop2mm = structureSet.Structures.Where(x => x.Id == bodyCrop2mmId).FirstOrDefault();
            else
                bodyCrop2mm = structureSet.AddStructure("CONTROL", bodyCrop2mmId);  //doesnt exist yet

            bodyCrop5mm.SegmentVolume = body.Margin(-5);
            bodyCrop5mm.SegmentVolume = body.Sub(bodyCrop5mm);
            bodyCrop2mm.SegmentVolume = body.Margin(-2);
            bodyCrop2mm.SegmentVolume = body.Sub(bodyCrop2mm);

            ptvOpt.SegmentVolume = ptvBreast.Sub(bodyCrop5mm);     
            if (laterality == "Left")
                CTHU0.SegmentVolume = ptvBreast.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 0, anteriorMargin, 0, lateralMargin, 0, 0));
            if (laterality == "Right")
                CTHU0.SegmentVolume = ptvBreast.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, lateralMargin, anteriorMargin, 0, 0, 0, 0));
            //CTHU0.ConvertToHighResolution();
            //CTHU0.SetAssignedHU(0);
            CTHU0.SegmentVolume = CTHU0.Sub(body);
            ptvExp.SegmentVolume = CTHU0.Or(ptvBreast);
            //ptvExp.ConvertToHighResolution();
            //body.And(CTHU0);
            //ptvExp.SegmentVolume = ptvExp.Sub(bodyCrop2mm);  //fix ptv exp holes with a spacer margin
            if (ptvSCV != null)
            {
                if (ptvSCV.IsEmpty != true)
                    ptvOpt.SegmentVolume = ptvOpt.Or(ptvSCV);
            }
            if (ptvAxilla != null)
            {
                if (ptvAxilla.IsEmpty != true)
                    ptvOpt.SegmentVolume = ptvOpt.Or(ptvAxilla);
            }
            if (ptvIMN != null)
            {
                if (ptvIMN.IsEmpty != true)
                    ptvOpt.SegmentVolume = ptvOpt.Or(ptvIMN);
            }
            ptvOptExp.SegmentVolume = ptvOpt.Or(ptvExp);
            structureSet.RemoveStructure(bodyCrop2mm);
            structureSet.RemoveStructure(bodyCrop5mm);
        }
    }
}
