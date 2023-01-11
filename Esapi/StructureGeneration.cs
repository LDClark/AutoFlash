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

            //reset body to default parameters
            if (structureSet.Structures.Any(x => x.Id == "BODY"))
            {
                body = structureSet.Structures.Where(x => x.Id == "BODY").FirstOrDefault();
                structureSet.RemoveStructure(body);
            }
            body = structureSet.CreateAndSearchBody(structureSet.GetDefaultSearchBodyParameters());

            //planning structures
            Structure ptvOpt = FindStructure(structureSet, "PTV_opt");
            Structure ptvExp = FindStructure(structureSet, "PTV_exp");
            Structure CTHU0 = FindStructure(structureSet, "CT HU = 0");
            Structure ring100 = FindStructure(structureSet, "Ring_100%");
            Structure ring50 = FindStructure(structureSet, "Ring_50%");
            Structure lungOpt = FindStructure(structureSet, "Lung_opt");

            //intermediate structures
            Structure ptvOptExp = FindStructure(structureSet, "PTV_opt+exp");
            Structure ptvExpSpacer = FindStructure(structureSet, "Spacer PTV_exp");  //fill in the holes in PTV_exp

            if (laterality == "Left")
            {
                CTHU0.SegmentVolume = ptvBreast.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 0, anteriorMargin, 0, lateralMargin, 0, 0));
                CTHU0.SegmentVolume = CTHU0.Sub(body); // remove CTHU0 portion that is inside body
                ptvExpSpacer.SegmentVolume = CTHU0.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 3, 0, 3, 0, 0, 0));
            }
            if (laterality == "Right")
            {
                CTHU0.SegmentVolume = ptvBreast.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, anteriorMargin, 0, lateralMargin, 0, 0, 0));
                CTHU0.SegmentVolume = CTHU0.Sub(body); // remove CTHU0 portion that is inside body
                ptvExpSpacer.SegmentVolume = CTHU0.AsymmetricMargin(new AxisAlignedMargins(StructureMarginGeometry.Outer, 0, 3, 0, 3, 0, 0));
            }

            ptvOpt = Crop(structureSet, -5, true, ptvOpt, ptvBreast, body); // crop 5mm inside body surface
            CTHU0.SetAssignedHU(0);
            
            ptvExp.SegmentVolume = CTHU0.Or(ptvBreast); // combine CTHU0 with ptvBreast
            ptvExp.SegmentVolume = ptvExp.Or(ptvExpSpacer); // combine to fill in holes between CTHU0 and ptvBreast
            body.SegmentVolume = body.Or(ptvExp); // combine body with ptvExp
            ptvExp.SegmentVolume = ptvExp.And(body);  // remove ptvExp outside body
            ptvExp = Crop(structureSet, -2, true, ptvExp, ptvExp, body);  // crop 2mm inside body surface

            // combine other ptvs with ptvOpt
            if (ptvSCV != null)
                ptvOpt.SegmentVolume = ptvOpt.Or(ptvSCV);
            if (ptvAxilla != null)
                ptvOpt.SegmentVolume = ptvOpt.Or(ptvAxilla);
            if (ptvIMN != null)
                ptvOpt.SegmentVolume = ptvOpt.Or(ptvIMN);

            ptvOptExp.SegmentVolume = ptvOpt.Or(ptvExp);  //combine ptvOpt and ptvExp to create ptvOptExp
            ring100 = ExtractWall(structureSet, outerMargin100, innerMargin100, ring100, ptvOptExp);
            ring100.SegmentVolume = ring100.And(body); // remove outside body
            ring50 = ExtractWall(structureSet, outerMargin50, innerMargin50, ring50, ptvOptExp);
            ring50.SegmentVolume = ring50.And(body); // remove outside body

            if (lung != null)
                lungOpt = Crop(structureSet, lungOptMargin, false, lungOpt, lung, ptvOptExp);

            //remove intermediate structures
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

        public Structure Crop(StructureSet structureSet, double cropMargin, bool removeOutside, Structure targetStructure, Structure structureToCrop, Structure structureToCropFrom)
        {
            Structure spacer = FindStructure(structureSet, "Spacer");
            spacer.SegmentVolume = LargeMargin(structureToCropFrom.SegmentVolume, cropMargin);

            if (removeOutside)
                spacer.SegmentVolume = structureToCropFrom.Sub(spacer);

            targetStructure.SegmentVolume = structureToCrop.Sub(spacer);
            structureSet.RemoveStructure(spacer);
            return targetStructure;
        }

        public Structure ExtractWall(StructureSet structureSet, double outerWallMargin, double innerWallMargin, Structure targetStructure, Structure structureToExtractFrom)
        {
            Structure spacer = FindStructure(structureSet, "Spacer");
            targetStructure.SegmentVolume = LargeMargin(structureToExtractFrom.SegmentVolume, outerWallMargin);
            spacer.SegmentVolume = LargeMargin(structureToExtractFrom.SegmentVolume, -1 * innerWallMargin);
            targetStructure.SegmentVolume = targetStructure.Sub(spacer);
            structureSet.RemoveStructure(spacer);
            return targetStructure;
        }

        public static SegmentVolume LargeMargin(SegmentVolume base_segment, double base_margin)
        {
            if (base_margin != 0)
            {
                double mmLeft;
                if (Math.Abs(base_margin) < 50)
                    return base_segment.Margin(base_margin);
                else
                    mmLeft = base_margin;
                SegmentVolume targetLeft = base_segment;
                while (mmLeft > 50)
                {
                    mmLeft -= 50;
                    targetLeft = targetLeft.Margin(50);
                }
                SegmentVolume result = targetLeft.Margin(mmLeft);
                return result;
            }
            else
                return base_segment;
        }
    }
}
