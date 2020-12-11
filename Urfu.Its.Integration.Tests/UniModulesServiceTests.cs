using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Moq;
using Moq.Protected;

namespace Urfu.Its.Integration
{
    [TestClass()]
    public class UniModulesServiceTests
    {

        [TestMethod]
        public void GetModulesForDirectionTest()
        {
            var modulesService = MockUniModulesService(@"..\..\TestData\Uni\UniModules.json");
            var modules = modulesService.GetModulesForDirection("uuid");

            Assert.IsNotNull(modules);
            Assert.AreEqual(2, modules.Count);

            // check ModuletDto
            var dto = modules[0];
            Assert.AreEqual("pstcim18ggl5g0000kcqpb8o0otmieds", dto.uuid);
            Assert.AreEqual("Техносферная безопасность", dto.title);
            Assert.AreEqual(1104347, dto.number);
            Assert.AreEqual("Техносферная безопасность", dto.shortTitle);
            Assert.AreEqual("Институт «Фундаментального образования»", dto.coordinator);
            Assert.AreEqual("Безопасность жизнедеятельности", dto.type);
            Assert.AreEqual("", dto.competence);
            Assert.AreEqual(2, dto.testUnits);
            Assert.AreEqual(5, dto.priority);
            Assert.AreEqual("Согласовано", dto.state);
            Assert.IsNull(dto.approvedDate);
            Assert.AreEqual("", dto.comment);
            Assert.AreEqual("http://study.urfu.ru/view/aid/1104347/1104347.pdf", dto.file);
            Assert.IsTrue(new[] { "43.03.03", "39.03.03", "43.03.01" }.SequenceEqual(dto.specialities));
            Assert.IsNotNull(dto.disciplines);
            Assert.AreEqual(2, dto.disciplines.Length);

            // check DisciplineDto
            var discipline1 = dto.disciplines[0];
            Assert.AreEqual("epmsdepstcim18ggl5g0000kcqpqidtublb70s;pstcim18hc2jg0000l84p4r2vq7p4c9c;", discipline1.uid);
            Assert.AreEqual("pstcim18ggl5g0000kcqpqidtublb70s", discipline1.discipline);
            Assert.AreEqual("Техносферная безопасность", discipline1.title);
            Assert.IsNull(discipline1.number);
            Assert.AreEqual("Контроль (Базовая часть)", discipline1.section);
            Assert.AreEqual(0, discipline1.testUnits);
            Assert.AreEqual("", discipline1.file);

            var discipline2 = dto.disciplines[1];
            Assert.AreEqual("epmsdepstcim18ggl5g0000kcqppt162bqa7i0;pstcim18ggl5g0000kb68kv7qmm8k154;", discipline2.uid);
            Assert.AreEqual("pstcim18ggl5g0000kcqppt162bqa7i0", discipline2.discipline);
            Assert.AreEqual("Безопасность жизнедеятельности", discipline2.title);
            Assert.AreEqual(1104356, discipline2.number);
            Assert.AreEqual("Базовая часть", discipline2.section);
            Assert.AreEqual(2, discipline2.testUnits);
            Assert.AreEqual("http://study.urfu.ru/view/aid/1104356/1104356.pdf", discipline2.file);
        }

        [TestMethod()]
        public void GetPlansForDirectionTest()
        {
            var uniModulesService = MockUniModulesService(@"..\..\TestData\Uni\UniPlans.json");
            var plans = uniModulesService.GetPlansForDirection("uuid");

            Assert.IsNotNull(plans);

            var plan = plans[0];

            // Check PlanDto
            Assert.AreEqual("unplan18hc2jg0000l7f2lden5jof2u0", plan.eduplanUUID);
            Assert.AreEqual(6091, plan.eduplanNumber);
            Assert.AreEqual("undifa18ggl5g0000jvf3dcvhljmbtgg", plan.faculty);
            Assert.AreEqual("Очная", plan.familirizationType);
            Assert.AreEqual("Традиционная", plan.familirizationTech);
            Assert.AreEqual("Полный срок", plan.familirizationCondition);
            Assert.AreEqual("Бакалавр", plan.qualification);
            Assert.AreEqual("unplvp18hc2jg0000l7f2m3dco1kki4g", plan.versionUUID);
            Assert.AreEqual("№ 1  (очн)", plan.versionTitle);
            Assert.AreEqual(1, plan.versionNumber);
            Assert.AreEqual(false, plan.versionActive);
            Assert.AreEqual("Утверждено", plan.versionStatus);
            Assert.AreEqual("unpled18hc2jg0000l866mkg5l0qs54k", plan.disciplineUUID);
            Assert.AreEqual(null, plan.additionalUUID);
            Assert.AreEqual("Физическая культура", plan.disciplineTitle);
            Assert.AreEqual("pstcim18ggl5g0000kbocd3pnsoh2pkc", plan.catalogDisciplineUUID);
            Assert.AreEqual("pstcim18ggl5g0000kbocbunatvkfjk0", plan.moduleUUID);

            // Check controls
            Assert.AreEqual(1, plan.controls.Count);
            var controls = plan.controls[0];
            Assert.AreEqual(1, controls.Count);
            Assert.IsTrue(new[] {6}.SequenceEqual(controls["Зачет"]));

            // Check loads
            Assert.IsTrue(new[] { "Практические занятия" }.SequenceEqual(plan.loads));

            // Check terms
            Assert.IsTrue(new[] {6}.SequenceEqual(plan.terms));

            // Check testUnitsByTerm
            var testUnits = plan.testUnitsByTerm;
            Assert.AreEqual(1, testUnits.Count);
            Assert.AreEqual(2, testUnits["6"]);
        }

        private static UniModulesService MockUniModulesService(string filename)
        {
            var mock = new Mock<UniModulesService>("url", "user", "pass");
            mock.Protected()
                .Setup<Stream>("OpenRead", ItExpr.IsAny<string>())
                .Returns(() => File.OpenRead(filename));

            var modulesService = mock.Object;
            return modulesService;
        }

    }

}