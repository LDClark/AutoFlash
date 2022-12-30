using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace AutoFlash
{
    public static class Helpers
    {
        private static Course GetCourse(Patient patient, string courseId) =>
            patient?.Courses?.FirstOrDefault(x => x.Id == courseId);

        public static Structure GetStructure(PlanningItem plan, string structureId) =>
            plan?.StructureSet?.Structures?.FirstOrDefault(x => x.Id == structureId);
        public static bool CheckStructure(Structure structure)
        {
            if (structure.ApprovalHistory.FirstOrDefault().ApprovalStatus == VMS.TPS.Common.Model.Types.StructureApprovalStatus.Approved)
                return false;
            else
                return true;
        }

        public static Tuple<bool, string> CheckStructureSet(Patient patient, StructureSet structureSet)
        {
            var status = String.Empty;

            bool canModifyStructureSet = false;
            foreach (Course course in patient.Courses)
            {
                foreach (PlanSetup planSetup in course.PlanSetups)
                {
                    if (planSetup.StructureSet == structureSet)
                    {
                        if (course.CompletedDateTime == null) //course is active and can modify ss
                            canModifyStructureSet = true;
                        else
                        {
                            canModifyStructureSet = false;
                            status = string.Format("Course {0} containing linked plans is completed.  Change course status.", course.Id);
                        }
                        if (planSetup.IsDoseValid)  //dose is calculated and will not let you modify body or AssignedHU for structures
                        {
                            canModifyStructureSet = false;
                            status = string.Format("Dose is calculated for this structure set.  Clear dose from plan {0}, " +
                                "or create a new structure set if already treated.", planSetup.Id);
                        }
                            
                    }
                    else
                    {
                        canModifyStructureSet = true; //no plans present
                    }
                }
            }
            return Tuple.Create(canModifyStructureSet, status);
        }

        public static Course AddCourse(Patient patient, string courseId)
        {
            patient.BeginModifications();
            try
            {
                var res = patient.Courses.Where(c => c.Id == courseId);
                if (res.Any())
                {
                    var oldCourse = res.Single();
                    patient.RemoveCourse(oldCourse);
                }
                var course = patient.AddCourse();
                course.Id = courseId;
                return course;
            }
            catch
            {
                var course = patient.AddCourse();
                course.Id = courseId;
                return course;
            }
        }

        public static ExternalPlanSetup FindPlanSetup(Patient patient, string courseId, string planSetupId)
        {
            var plans = new List<PlanSetup>();
            foreach (var c in patient.Courses)
            {
                if (c.Id == courseId)
                {
                    var temp = c.PlanSetups.Where(p => p.Id == planSetupId);
                    plans.AddRange(temp);
                }
            }
            return plans.First() as ExternalPlanSetup;
        }

        public static void RemoveStructures(StructureSet structureSet, List<string> structureIDs)
        {
            try
            {
                foreach (var id in structureIDs)
                {
                    if (structureSet.Structures.Any(st => st.Id == id))
                    {
                        Structure structure = structureSet.Structures.Single(x => x.Id == id);
                        Structure body = structureSet.Structures.Single(x => x.Id == "BODY");
                        structure.SegmentVolume = structure.Sub(body);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
