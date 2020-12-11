using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity.Spatial;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Documents.Discipline.Loaders
{
    public class DisciplineScopesLoader : ObjectBlockContentLoader<IEnumerable<DisciplineScopeInfo>>
    {
        private readonly ApplicationDbContext _db;
        private readonly JObject _loadedDocumentData;

        public DisciplineScopesLoader(ApplicationDbContext db, JObject loadedDocumentData)
        {
            _db = db;
            _loadedDocumentData = loadedDocumentData;
        }

        protected override IEnumerable<DisciplineScopeInfo> LoadAnyContent(JToken blockContent)
        {
            var fdps = _loadedDocumentData["Fdps"];
            if (fdps == null)
                yield break;

            var disciplineName = _loadedDocumentData[nameof(DisciplineWorkingProgramFgosVoSchemaModel.Name)].ToString();

            // TODO BlockDataHelper.GetActualMergedData
            var disciplineScopes = GetActualDisciplineScopes(fdps, (JArray)blockContent).ToList();

            //var oldDisciplineScopes = disciplineScopes.Where(item => !actualFdpIds.Contains(item[nameof(DisciplineScopeInfo.FdpId)].Value<string>())).ToList();

            var fakeDisciplineIds = fdps
                .Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId)].Value<string>()).ToList();

            var actualDisciplinePlans = PreparePlanDisciplineIdsForDiscipline(fakeDisciplineIds, disciplineName, _db);
            var disciplineIds = actualDisciplinePlans.Select(p=>p.Value.disciplineUUID).ToList();

            var planAdditionalList = _db.PlanAdditionals.Where(a => disciplineIds.Contains(a.disciplineUUID)).ToList();

            var plans = _db.Plans.Where(p => disciplineIds.Contains(p.disciplineUUID)).ToList()
                .ToDictionary(p => p.disciplineUUID, p => p);
            var eduPlanIds = plans.Select(p => p.Value.eduplanUUID).ToList();
            var weeksData = _db.PlanTermWeeks.Where(w => eduPlanIds.Contains(w.eduplanUUID)).ToList();
            var data = new Dictionary<string, DisciplineScopeInfo>();
            var planAdditionals = new Dictionary<string, PlanAdditional>();
            var fdpIds = new Dictionary<string, string>();
            
            foreach (var disciplineScope in disciplineScopes)
            {
                var fdp = fdps.First(f =>
                    f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>() ==
                    disciplineScope.FdpId);

                var fakeDisciplineId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.DisciplineId)].Value<string>();
                var disciplineId = actualDisciplinePlans[fakeDisciplineId].disciplineUUID;
                var pa = planAdditionalList.FirstOrDefault(a=> a.disciplineUUID == disciplineId);

                fdpIds.Add(disciplineId, fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>());
                planAdditionals.Add(disciplineId, pa);
                data.Add(disciplineId, disciplineScope);                                                           
            }

            var semestersData = GetSemestersData(planAdditionals, plans,weeksData, 8);
            
            foreach (var disciplineId in disciplineIds)
            {
                var startSemester = semestersData[disciplineId].Item1;
                
                var disciplineScope = data[disciplineId];
                disciplineScope.FdpId = fdpIds[disciplineId];
                disciplineScope.StartSemester = startSemester ?? 1;

                if (startSemester == null)
                {
                    yield return disciplineScope;
                    continue;
                }

                var tlist = semestersData[disciplineId].Item2;
                var disciplineScopeData = data[disciplineId];
                var plan = plans[disciplineId];
                var planAdditional = planAdditionals[disciplineId];                
                
                var audit = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.allaudit,
                    DisciplineContactTime = planAdditional.allaudit,
                    Semesters = tlist.Select(t=>t?.tl).ToList()
                };
                disciplineScope.Auditory = audit;
                
                var lec = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.lecture,
                    DisciplineContactTime = planAdditional.contactLecture,
                    Semesters = tlist.Select(t => t?.tllec).ToList()
                };
                disciplineScope.Lections = lec;

                var prac = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.practice,
                    DisciplineContactTime = planAdditional.contactPractice,
                    Semesters = tlist.Select(t => t?.tlprac).ToList()
                };
                disciplineScope.Practices = prac;

                var labs = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.labs,
                    DisciplineContactTime = planAdditional.contactLabs,
                    Semesters = tlist.Select(t => t?.tllab).ToList()
                };
                disciplineScope.Labs = labs;

                var self = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.self,
                    DisciplineContactTime = planAdditional.contactSelf,
                    Semesters = disciplineScopeData.SelfWork.Semesters?.ToArray() ?? new decimal?[8]
                };
                disciplineScope.SelfWork = self;

                var controls = GetControls(startSemester.Value, tlist, plan);
                var intermediate = new PlanAdditionalIntermediateItemInfo
                {
                    DisciplineTime = string.Join(", ", planAdditional.controls.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(PrepareKmName)),
                    DisciplineContactTime = planAdditional.contactControl,
                    Semesters = controls
                };
                disciplineScope.Intermediate = intermediate;

                var totalTime = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.allload,
                    DisciplineContactTime = planAdditional.contactSelf,
                    Semesters = self.Semesters.Select((s,i)=>
                    {
                        var s1 = audit.Semesters.ElementAt(i);
                        var s2 = self.Semesters.ElementAt(i);
                        if (s1 == null)
                            return s2;
                        if (s2 == null)
                            return s1;
                        return s1 + s2;                        
                    }).ToArray()
                };
                disciplineScope.TotalAmountTime = totalTime;

                var testUnits = GetTestUnits(startSemester.Value, tlist, plan);
                var totalUnits = new PlanAdditionalItemInfo
                {
                    DisciplineTime = planAdditional.gosLoadInTestUnits,
                    DisciplineContactTime = null,
                    Semesters = testUnits.ToArray()
                };
                disciplineScope.TotalAmountUnits = totalUnits;

                yield return disciplineScope;
            }            
        }

        private IReadOnlyDictionary<string,Plan> PreparePlanDisciplineIdsForDiscipline(List<string> disciplineIds, string disciplineName,
            ApplicationDbContext db)
        {
            var dic = new Dictionary<string, Plan>();
            var plans = db.Plans.Where(p => disciplineIds.Contains(p.disciplineUUID)).ToList();
            foreach (var fakePlan in plans)
            {
                var disciplinePlan = db.Plans
                    .FirstOrDefault(p => p.disciplineTitle == disciplineName
                                         && p.versionUUID == fakePlan.versionUUID
                                         && p.eduplanNumber == fakePlan.eduplanNumber
                                         && p.moduleUUID == fakePlan.moduleUUID
                                         && p.directionId == fakePlan.directionId
                                         && p.familirizationType == fakePlan.familirizationType);
                dic.Add(fakePlan.disciplineUUID, disciplinePlan);                
            }

            return dic;
        }

        [Obsolete("Нужно использовать BlockDataHelper.GetActualMergedData")]
        private static IEnumerable<DisciplineScopeInfo> GetActualDisciplineScopes(JToken fdps, JArray disciplineScopes)
        {
            var actualFdpIds = fdps.Select(fdp => fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>());
            var oldFdpIds = disciplineScopes.Select(item => item[nameof(DisciplineScopeInfo.FdpId)].Value<string>());
            var existingDisciplineScopes = disciplineScopes
                .Where(item => actualFdpIds.Contains(item[nameof(DisciplineScopeInfo.FdpId)].Value<string>()))
                .Select(item=>JsonConvert.DeserializeObject<DisciplineScopeInfo>(item.ToString())).ToList();
            var newFdps = fdps.Where(fdp =>
                !oldFdpIds.Contains(fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>())).ToList();

            var newDisciplineScopes = newFdps.Select(f => new DisciplineScopeInfo
            {
                FdpId = f[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>(),
            });

            var actualDisciplineScopes = existingDisciplineScopes.Concat(newDisciplineScopes).ToList();

            foreach (var fdp in fdps)
            {
                var fdpId = fdp[nameof(FamilirizationTypeDirectionPlanInfo.ItemId)].Value<string>();
                var ds = actualDisciplineScopes.FirstOrDefault(s => s.FdpId == fdpId);
                yield return ds;
            }
        }

        private IEnumerable<decimal?> GetTestUnits(int startSemester, tlInfo[] tlist, Plan plan)
        {
            var dic = new Dictionary<int, int>();
            var testUnitsByTerm = JObject.Parse(plan.testUnitsByTerm);
            foreach (var property in testUnitsByTerm.Properties())
            {
                var semester = Convert.ToInt32(property.Name);
                var units = property.Value.Value<int>();
                dic.Add(semester, units);
            }

            Debug.Assert(dic.Keys.Min() >= startSemester);

            return Enumerable.Range(startSemester, tlist.Length).Select(i =>
            {
                if (dic.TryGetValue(i, out var v))
                    return v;
                return (decimal?) null;
            });
        }

        private string[] GetControls(int startSemester, tlInfo[] tlist, Plan plan)
        {
            var dic = new Dictionary<int, List<string>>();
            var controls = JArray.Parse(plan.controls);
            foreach (JObject control in controls)
            {
                foreach (var property in control.Properties())
                {
                    var name = property.Name;
                    var semesters = (JArray)property.Value;
                    foreach (var semesterItem in semesters)
                    {
                        var semester = semesterItem.Value<int>();
                        if (!dic.TryGetValue(semester, out var kms))
                        {
                            kms = new List<string>();
                            dic.Add(semester, kms);
                        }
                        if (!kms.Contains(name))
                            kms.Add(name);
                    }
                }
            }

            Debug.Assert(dic.Keys.Min() >= startSemester);

            var result = Enumerable.Range(startSemester, tlist.Length).Select(i =>
            {
                if (dic.TryGetValue(i, out var v))
                    return string.Join(", ", v.Select(PrepareKmName));
                return (string)null;
            });

            /*var result = tlist.Select(_ => (string)null).ToList();
            for (int i = 1; i < tlist.Length+1; i++)
            {
                if (dic.TryGetValue(i, out var kms))
                {
                    result[i] = string.Join(", ", kms.Select(PrepareKmName));
                }
            }*/

            return result.ToArray();
        }

        private string PrepareKmName(string longName)
        {
            switch (longName)
            {
                case "Экзамен":
                    return "Э";
                case "Зачет":
                    return "З";
                case "Интегрированная оценка":
                    return "ИО";
                case "Курсовая работа":
                    return "КР";
                case "Курсовой проект":
                    return "КП";
                case "Проект по модулю":
                    return "ПМ";
                case "Междисциплинарный курсовой проект":
                    return "МКП";
                case "Зачет-Проект по модулю":
                    return "ЗПМ";
                case "Экзамен накопительный/Письменный экзамен":
                    return "ЭН/ПЭ";
                default:
                    Debug.Fail($"Тип КМ {longName} не обработан.");
                    throw new NotImplementedException($"Тип КМ {longName} не обработан.");
            }
        }

        private static IReadOnlyDictionary<string, Tuple<int?, tlInfo[]>> GetSemestersData(Dictionary<string, PlanAdditional> planAdditionals, Dictionary<string, Plan> plans, List<PlanTermWeek> weeksData, int maxSemestersCount)
        {
            var dic = new Dictionary<string, Tuple<int?, tlInfo[]>>();
            foreach (var pair in planAdditionals)
            {
                var planAdditional = pair.Value;
                if (planAdditional == null)
                {
                    dic.Add(pair.Key, Tuple.Create((int?)null, new tlInfo[maxSemestersCount]));
                    continue;
                }

                var plan = plans[pair.Key];

                var terms = JArray.Parse(plan.terms).Select(t => t.Value<int>()).OrderBy(s => s).ToList();
                var startSemester2 = terms.FirstOrDefault();

                var weeks = weeksData.Where(w => w.eduplanUUID == plan.eduplanUUID)
                    .ToDictionary(w => w.Term, w => w.WeeksCount);

                var tlist = new List<tlInfo>
                {
                    ToSemesterHours(1, planAdditional.tl1, weeks, terms),
                    ToSemesterHours(2, planAdditional.tl2, weeks, terms),
                    ToSemesterHours(3, planAdditional.tl3, weeks, terms),
                    ToSemesterHours(4, planAdditional.tl4, weeks, terms),
                    ToSemesterHours(5, planAdditional.tl5, weeks, terms),
                    ToSemesterHours(6, planAdditional.tl6, weeks, terms),
                    ToSemesterHours(7, planAdditional.tl7, weeks, terms),
                    ToSemesterHours(8, planAdditional.tl8, weeks, terms),
                    ToSemesterHours(9, planAdditional.tl9, weeks, terms),
                    ToSemesterHours(10, planAdditional.tl10, weeks, terms),
                    ToSemesterHours(11, planAdditional.tl11, weeks, terms),
                    ToSemesterHours(12, planAdditional.tl12, weeks, terms),
                };

                var minIndex = tlist.FindIndex(tl => tl != null && tl.tl > 0);
                int? startSemester1 = 0;
                if (minIndex >= 0)
                    startSemester1 = minIndex + 1;

                Debug.Assert(startSemester1 == startSemester2,
                    "Семестры из данных по часам не совпадают с данными по семестрами из версии УП.");
                var startSemester = startSemester2;// Math.Min(startSemester1 ?? 0, startSemester2 ?? 0);

                var tlInfos = tlist.Skip(minIndex).Take(maxSemestersCount).ToList();
                while (tlInfos.Count < 8)
                    tlInfos.Add(null);
                dic.Add(pair.Key, Tuple.Create(startSemester == 0 ? (int?) null : startSemester, tlInfos.ToArray()));
            }

            return dic;
        }

        private static tlInfo ToSemesterHours(int semester, string weekHours, Dictionary<int, int> weeks, List<int> terms)
        {
            if (weekHours == null)
                return null;

            if (!terms.Contains(semester))
                return null;

            if (!weeks.TryGetValue(semester, out var weekCount))
                return null;

            var tlinfo = JsonConvert.DeserializeObject<tlInfo>(weekHours);
            tlinfo.tlprac *= weekCount;
            tlinfo.tllec *= weekCount;
            tlinfo.tllab *= weekCount;
            tlinfo.tl *= weekCount;
            tlinfo.tlprac = Math.Round(tlinfo.tlprac);
            tlinfo.tllec = Math.Round(tlinfo.tllec);
            tlinfo.tllab = Math.Round(tlinfo.tllab);
            tlinfo.tl = Math.Round(tlinfo.tl); 
            return tlinfo;
        }

        private class tlInfo
        {
            public decimal tllec { get; set; }
            public decimal tlprac { get; set; }
            public decimal tllab { get; set; }
            public decimal tl { get; set; }
        }        
    }

    internal class SemestersJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}