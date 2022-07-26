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
            string laterality, double anteriorMargin, double lateralMargin, double outerMargin100, double innerMargin100, double outerMargin50, double innerMargin50,
            double lungOptMargin)
        {
            structureSet.Patient.BeginModifications();
            Structure ptvBreast = structureSet.Structures.Where(structure => structure.Id == ptvBreastId).FirstOrDefault();
            Structure ptvSCV = structureSet.Structures.Where(structure => structure.Id == ptvSCVId).FirstOrDefault();
            Structure ptvAxilla = structureSet.Structures.Where(structure => structure.Id == ptvAxillaId).FirstOrDefault();
            Structure ptvIMN = structureSet.Structures.Where(structure => structure.Id == ptvIMNId).FirstOrDefault();

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
            if (structureSet.Structures.Any(x => x.Id == ptvMarginInner100Id))
                ptvMarginInner100 = structureSet.Structures.Where(x => x.Id == ptvMarginInner100Id).FirstOrDefault();
            else
                ptvMarginInner100 = structureSet.AddStructure("CONTROL", ptvMarginInner100Id);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ptvMarginInner50Id))
                ptvMarginInner50 = structureSet.Structures.Where(x => x.Id == ptvMarginInner50Id).FirstOrDefault();
            else
                ptvMarginInner50 = structureSet.AddStructure("CONTROL", ptvMarginInner50Id);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == bodyCrop5mmPTVId))
                bodyCrop5mmPTV = structureSet.Structures.Where(x => x.Id == bodyCrop5mmPTVId).FirstOrDefault();
            else
                bodyCrop5mmPTV = structureSet.AddStructure("CONTROL", bodyCrop5mmPTVId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == lungMarginInnerId))
                lungMarginInner = structureSet.Structures.Where(x => x.Id == lungMarginInnerId).FirstOrDefault();
            else
                lungMarginInner = structureSet.AddStructure("CONTROL", lungMarginInnerId);  //doesnt exist yet
           
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
            ptvMarginInner100.SegmentVolume = ptvOptExp.Margin(-1 * innerMargin100);
            ptvMarginInner50.SegmentVolume = ptvOptExp.Margin(-1 * innerMargin50);
            ring100.SegmentVolume = ptvOptExp.Margin(outerMargin100);
            ring50.SegmentVolume = ptvOptExp.Margin(outerMargin50);
            ring100.SegmentVolume = ring100.Sub(ptvMarginInner100);           
            ring50.SegmentVolume = ring50.Sub(ptvMarginInner50);
            ring100.SegmentVolume = ring100.And(body); // remove outside body
            ring50.SegmentVolume = ring50.And(body);

            try // if lung right or left exists
            {
                if (laterality == "Left")
                {
                    Structure lungL = structureSet.Structures.Where(structure => structure.Id == "Lung_L").FirstOrDefault();
                    lungMarginInner.SegmentVolume = ptvBreast.Margin(lungOptMargin);
                    lungOpt.SegmentVolume = lungL.Sub(lungMarginInner);
                }
                if (laterality == "Right")
                {
                    Structure lungR = structureSet.Structures.Where(structure => structure.Id == "Lung_R").FirstOrDefault();
                    lungMarginInner.SegmentVolume = ptvBreast.Margin(lungOptMargin);
                    lungOpt.SegmentVolume = lungR.Sub(lungMarginInner);
                }              
            }
            catch
            {
                structureSet.RemoveStructure(lungOpt);
            }
            structureSet.RemoveStructure(bodyCrop2mm);
            structureSet.RemoveStructure(bodyCrop5mm);
            structureSet.RemoveStructure(ptvMarginInner100);
            structureSet.RemoveStructure(ptvMarginInner50);
            structureSet.RemoveStructure(bodyCrop5mmPTV);
            structureSet.RemoveStructure(lungMarginInner);
            structureSet.RemoveStructure(ptvOptExp);
        }
    }
}
