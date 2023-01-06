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

            Structure body;
            
            if (structureSet.Structures.Any(x => x.Id == "BODY"))  //reset body to default parameters
            {
                body = structureSet.Structures.Where(x => x.Id == "BODY").FirstOrDefault();
                structureSet.RemoveStructure(body);
            }
            body = structureSet.CreateAndSearchBody(structureSet.GetDefaultSearchBodyParameters());

            Structure ptvOpt = FindStructure(structureSet, "PTV_opt");
            Structure ptvExp = FindStructure(structureSet, "PTV_exp");
            Structure ptvOptExp = FindStructure(structureSet, "PTV_opt+exp");
            Structure CTHU0 = FindStructure(structureSet, "CT HU = 0");
            Structure ring100 = FindStructure(structureSet, "Ring_100%");
            Structure ring50 = FindStructure(structureSet, "Ring_50%");
            Structure lungOpt = FindStructure(structureSet, "Lung_opt");
            Structure bodyCrop5mm = FindStructure(structureSet, "Spacer 5mm");
            Structure bodyCrop2mm = FindStructure(structureSet, "Spacer 2mm");
            Structure ptvMarginInner100 = FindStructure(structureSet, "Spacer 100%");
            Structure ptvMarginInner50 = FindStructure(structureSet, "Spacer 50%");
            Structure bodyCrop5mmPTV = FindStructure(structureSet, "Spacer in PTV");
            Structure lungMarginInner = FindStructure(structureSet, "Spacer Lung"); 
            Structure ptvExpSpacer = FindStructure(structureSet, "Spacer PTV_exp");  //fill in the holes in PTV_exp

            bodyCrop5mm.SegmentVolume = body.Margin(-5);
            bodyCrop5mm.SegmentVolume = body.Sub(bodyCrop5mm);
            bodyCrop2mm.SegmentVolume = body.Margin(-2);
            bodyCrop2mm.SegmentVolume = body.Sub(bodyCrop2mm);

            ptvOpt.SegmentVolume = ptvBreast.Sub(bodyCrop5mm);     
            if (laterality == "Left")
            {
                CTHU0.SegmentVolume = ptvBreast.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 0, anteriorMargin, 0, lateralMargin, 0, 0));
                ptvExpSpacer.SegmentVolume = CTHU0.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 3, 0, 3, 0, 0, 0));
            }           
            if (laterality == "Right")
            {
                CTHU0.SegmentVolume = ptvBreast.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, anteriorMargin, 0, lateralMargin, 0, 0, 0));
                ptvExpSpacer.SegmentVolume = CTHU0.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 0, 3, 0, 3, 0, 0));
            }
                
            CTHU0.SetAssignedHU(0);
            CTHU0.SegmentVolume = CTHU0.Sub(body);
            ptvExp.SegmentVolume = CTHU0.Or(ptvBreast);
            ptvExp.SegmentVolume = ptvExp.Or(ptvExpSpacer);
            body.SegmentVolume = body.Or(CTHU0);
            
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
            structureSet.RemoveStructure(ptvExpSpacer);
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
