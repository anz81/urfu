using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urfu.Its.Integration;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Web.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void shouldGetDirectionsXml()
        {
            var list = new UniRestService().GetDirectionsXml();
            Assert.AreNotEqual(list.Count,0);
            var direction = list.First();
            Assert.IsNotNull(direction.ID);
            Assert.IsNotNull(direction.CODE);
            Assert.IsNotNull(direction.NAME);
            Assert.IsNotNull(direction.QUALIFICATION);
            Assert.IsNotNull(direction.STANDART_VPO);
            Assert.IsNotNull(direction.UGN_ID);
        }

        [TestMethod]
        public void shouldGetGroupsXml()
        {
            var list = new UniRestService().GetGroupsXml();
            Assert.AreNotEqual(list.Count,0);
            var direction = list.First();
            Assert.IsNotNull(direction.Id);
            Assert.IsNotNull(direction.Name);
        }

        [TestMethod]
        public void shouldGetStudentsXml()
        {
            var list = new UniRestService().GetStudentsXml();
            var ints = list.Where(s=>s.IsInternational).ToList();
            var tars = list.Where(s=>s.IsTarget).ToList();

            Assert.AreNotEqual(list.Count,0);
            Assert.AreNotEqual(ints.Count, 0);
            Assert.AreNotEqual(tars.Count, 0);
            var direction = list.First();
            Assert.IsNotNull(direction.Id);
            Assert.IsNotNull(direction.Status);
        }

        [TestMethod]
        public void shouldGetPersonsXml()
        {
            var list = new UniRestService().GetPersonsXml();
            Assert.AreNotEqual(list.Count,0);
            var direction = list.First();
            Assert.IsNotNull(direction.Id);
            Assert.IsNotNull(direction.Name);
        }

        [TestMethod]
        public void shouldGetModules()
        {
            var list = UniModulesService.Create().GetModulesForDirection("uncass18ggl5g0000k7gv9aa38ljht48");
            Assert.AreNotEqual(list.Count, 0);
            var direction = list.First();
            Assert.IsNotNull(direction.file);
            Assert.IsNotNull(direction.uuid);
            Assert.IsNotNull(direction.type);
            Assert.IsNotNull(direction.state);
            Assert.IsNotNull(direction.specialities);
            Assert.IsNotNull(direction.disciplines);
        }


        [TestMethod]
        public void shouldGetPlans()
        {
            var list = UniModulesService.Create().GetPlansForDirection("uncass18ggl5g0000k7gv9aa38ljht48");
            Assert.AreNotEqual(list.Count, 0);
            var direction = list.First();
            Assert.IsNotNull(direction.disciplineTitle);
            Assert.IsNotNull(direction.eduplanNumber);
            Assert.IsNotNull(direction.terms);
            Assert.IsNotNull(direction.loads);
            Assert.IsNotNull(direction.controls);
        }


        [TestMethod]
        public void shouldGetDirections()
        {
            var list = new UniRestService().GetDirections();
            Assert.AreNotEqual(list.Count, 0);
            var direction = list.First();
            Assert.IsNotNull(direction.uid);
            Assert.IsNotNull(direction.title);
            Assert.IsNotNull(direction.standard);
            Assert.IsNotNull(direction.okso);
            Assert.IsNotNull(direction.ugnTitle);
        }

        [TestMethod]
        public void shouldGetRating()
        {
            //var text = File.ReadAllText("c:\\1.txt",Encoding.Default);
            //var deserialize = new XmlSerializer(typeof (RatingDto)).Deserialize(new StringReader(text));
            var list = new BrsService().GetRatings(2014,2, 1, true);
            Assert.AreNotEqual(list.Count, 0);
            var dto = list.First();
            Assert.IsNotNull(dto.id);
            Assert.IsTrue(dto.rate>0);
        }

        [TestMethod]
        public void shouldGetSelection()
        {
            //var text = File.ReadAllText("c:\\1.txt",Encoding.Default);
            //var deserialize = new XmlSerializer(typeof (RatingDto)).Deserialize(new StringReader(text));
            var list = new LksService().GetSelection();
            Assert.AreNotEqual(list.Count, 0);
            var dto = list.First();
            Assert.IsNotNull(dto.studentPersonId);
            Assert.IsTrue(dto.variants.Count>0);
        }

        [TestMethod]
        public void shouldGetTeachers()
        {
            var list = new TeacherService().GetTeachers().ToList();
            Assert.AreNotEqual(list.Count, 0);
            var dto = list.First();
            Assert.IsNotNull(dto.firstName);
            Assert.IsNotNull(dto.pkey);
        }
        [TestMethod]
        public void shouldGetEntrantRating()
        {
            var list = new EntrantsService().GetEntrantsRating();
            Assert.AreNotEqual(list.Count, 0);
            var dto = list.First();
            Assert.IsNotNull(dto.student);
            Assert.IsNotNull(dto.entrant);
        }
        [TestMethod]
        public void shouldGetDivisions()
        {
            var list = new UniDivisionsService().GetDivisions();
            Assert.AreNotEqual(list.Count, 0);
            var dto = list.First();
            Assert.IsNotNull(dto.Value.uuid);
            Assert.IsNotNull(dto.Value.shortTitle);
        }

        [TestMethod]
        public void shouldGetApploads()
        {
            var list = new ApploadService().GetApploads(2015,1,true);
            Assert.AreNotEqual(list.Count, 0);
            var dto = list.First();
            Assert.IsNotNull(dto.uuid);
        }

        [TestMethod]
        public void shouldGetStudentPlanPairs()
        {
            var list = new UniRestService().GetStudentPlans();
            Assert.AreNotEqual(list.Count, 0);
            var direction = list.First();
            Assert.IsNotNull(direction.StudentId);
        }
    }
}
