using System;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace AutoFlash
{
    public class StructureGeneration
    {
        public void CreateStructures(StructureSet structureSet, string ptvBreastId, string ptvSCVId, string ptvAxillaId, string ptvIMNId,
            string laterality, double anteriorMargin, double lateralMargin, double outerMargin100, double innerMargin100, double outerMargin50, double innerMargin50,
            double lungOptMargin, string lungId)
        {
            structureSet.Patient.BeginModifications();
            Structure ptvBreast = structureSet.Structures.Where(structure => structure.Id == ptvBreastId).FirstOrDefault();
            Structure ptvSCV = structureSet.Structures.Where(structure => structure.Id == ptvSCVId).FirstOrDefault();
            Structure ptvAxilla = structureSet.Structures.Where(structure => structure.Id == ptvAxillaId).FirstOrDefault();
            Structure ptvIMN = structureSet.Structures.Where(structure => structure.Id == ptvIMNId).FirstOrDefault();
            Structure lung = structureSet.Structures.Where(structure => structure.Id == lungId).FirstOrDefault();

            Structure ptvOpt;
            Structure ptvExp;
            Structure ptvOptExp;
            Structure CTHU0;
            Structure ring100;
            Structure ring50;
            Structure lungOpt;
            Structure bodyCrop5mm;
            Structure bodyCrop2mm;
            Structure ptvMarginInner100;
            Structure ptvMarginInner50;
            Structure bodyCrop5mmPTV;
            Structure lungMarginInner;
            Structure body;

            string ptvOptId = "PTV_opt";
            string ptvExpId = "PTV_exp";
            string ptvOptExpId = "PTV_opt+exp";
            string CTHU0Id = "CT HU = 0";
            string ring100Id = "Ring_100%";
            string ring50Id = "Ring_50%";
            string lungOptId = "Lung_opt";
            string bodyCrop5mmId = "Spacer 5mm";
            string bodyCrop2mmId = "Spacer 2mm";
            string ptvMarginInner100Id = "Spacer 100%";
            string ptvMarginInner50Id = "Spacer 50%";
            string bodyCrop5mmPTVId = "Spacer in PTV";
            string lungMarginInnerId = "Spacer Lung";
            
            if (structureSet.Structures.Any(x => x.Id == "BODY"))  //reset body to default parameters
            {
                body = structureSet.Structures.Where(x => x.Id == "BODY").FirstOrDefault();
                structureSet.RemoveStructure(body);
            }
            body = structureSet.CreateAndSearchBody(structureSet.GetDefaultSearchBodyParameters());

            ptvOpt = FindStructure(structureSet, ptvOptId);
            ptvExp = FindStructure(structureSet, ptvExpId);
            ptvOptExp = FindStructure(structureSet, ptvOptExpId);
            CTHU0 = FindStructure(structureSet, CTHU0Id);
            ring100 = FindStructure(structureSet, ring100Id);
            ring50 = FindStructure(structureSet, ring50Id);
            lungOpt = FindStructure(structureSet, lungOptId);
            bodyCrop5mm = FindStructure(structureSet, bodyCrop5mmId);
            bodyCrop2mm = FindStructure(structureSet, bodyCrop2mmId);
            ptvMarginInner100 = FindStructure(structureSet, ptvMarginInner100Id);
            ptvMarginInner50 = FindStructure(structureSet, ptvMarginInner50Id);
            bodyCrop5mmPTV = FindStructure(structureSet, bodyCrop5mmPTVId);
            lungMarginInner = FindStructure(structureSet, lungMarginInnerId);

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
            CTHU0.SetAssignedHU(0);
            CTHU0.SegmentVolume = CTHU0.Sub(body);
            ptvExp.SegmentVolume = CTHU0.Or(ptvBreast);
            //ptvExp.ConvertToHighResolution();
            body.SegmentVolume = body.Or(CTHU0);
            //bodyCrop2mm.SegmentVolume = body.Margin(-2);
            //bodyCrop2mm.SegmentVolume = body.Sub(bodyCrop2mm);
            //ptvExp.SegmentVolume = ptvExp.Sub(bodyCrop2mm);
            if (ptvSCV != null)
                ptvOpt.SegmentVolume = ptvOpt.Or(ptvSCV);
            if (ptvAxilla != null)
                ptvOpt.SegmentVolume = ptvOpt.Or(ptvAxilla);
            if (ptvIMN != null)
                ptvOpt.SegmentVolume = ptvOpt.Or(ptvIMN);

            ptvOptExp.SegmentVolume = ptvOpt.Or(ptvExp);
            ptvMarginInner100.SegmentVolume = ptvOptExp.Margin(-1 * innerMargin100);
            ptvMarginInner50.SegmentVolume = ptvOptExp.Margin(-1 * innerMargin50);
            ring100.SegmentVolume = ptvOptExp.Margin(outerMargin100);
            ring50.SegmentVolume = ptvOptExp.Margin(outerMargin50);
            ring100.SegmentVolume = ring100.Sub(ptvMarginInner100);           
            ring50.SegmentVolume = ring50.Sub(ptvMarginInner50);
            ring100.SegmentVolume = ring100.And(body); // remove outside body
            ring50.SegmentVolume = ring50.And(body);

            if (lung != null)
            {              
                lungMarginInner.SegmentVolume = ptvBreast.Margin(lungOptMargin);
                lungOpt.SegmentVolume = lung.Sub(lungMarginInner);
            }         

            structureSet.RemoveStructure(bodyCrop2mm);
            structureSet.RemoveStructure(bodyCrop5mm);
            structureSet.RemoveStructure(ptvMarginInner100);
            structureSet.RemoveStructure(ptvMarginInner50);
            structureSet.RemoveStructure(bodyCrop5mmPTV);
            structureSet.RemoveStructure(lungMarginInner);
            structureSet.RemoveStructure(ptvOptExp);
        }

        public Structure FindStructure(StructureSet structureSet, string id)
        {
            if (structureSet.Structures.Any(x => x.Id.ToUpper() == id.ToUpper()))
                return structureSet.Structures.Where(x => x.Id.ToUpper() == id.ToUpper()).FirstOrDefault();
            else
            {
                var structure = structureSet.AddStructure("CONTROL", id);  //doesnt exist yet
                return structure;
            }                             
        }
    }
}
