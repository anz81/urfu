using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Autofac;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;

namespace Urfu.Its.VersionedDocs.Documents.Module
{
    public class ModuleWorkingProgramFgosVoService : ModuleWorkingProgramServiceBase<ModuleWorkingProgramFgosVoSchemaModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVersionedDocumentService _documentService;

        public ModuleWorkingProgramFgosVoService(ApplicationDbContext db, IVersionedDocumentSchemaService schemaService,
            IVersionedDocumentService documentService,
            IVersionedDocumentModelDescriptorFactory<ModuleWorkingProgramFgosVoSchemaModel> descriptorFactory,
            ILifetimeScope scope, IVersionedDocumentDescriptorService descriptorService, IPrincipal user) 
            : base(db, schemaService, descriptorFactory, scope, descriptorService, user)
        {
            _db = db;
            _documentService = documentService;
        }

        protected override string GetStandard()
        {
            return StandardNames.FgosVo;
        }

        protected override IReadOnlyDictionary<string, object> GetDefaultBlockValues(string linkedEntityId)
        {
            var module = _db.UniModules().First(m => m.uuid == linkedEntityId);

            var institutes = _db.Divisions.Where(d =>
                d.typeCode == "institute" || d.typeCode == "faculty" || d.typeCode == "branch").ToList();
            var coordinator = module.coordinator;
            var instituteEntity = institutes.FirstOrDefault(i => string.Equals(i.typeTitle + " «" + i.title + "»",
                coordinator, StringComparison.InvariantCultureIgnoreCase));
            
            var institute = new InstituteInfo
            {
                Id = instituteEntity?.uuid,
                Name = instituteEntity?.title
            };

            return new Dictionary<string, object>
            {
                {nameof(ModuleWorkingProgramFgosVoSchemaModel.Institute), institute}
            };
        }

        public override IEnumerable<WorkingProgramSection> GetSections()
        {
            yield return new WorkingProgramSection(null, "ТИТУЛЬНЫЙ ЛИСТ")
            {
                Sections = new []
                {
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Institute), "1. Институт"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Directions), "2. Направления"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Profiles), "3. Образовательная программа"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Authors), "4. Рабочая программа модуля составлена авторами"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Head), "5. Руководитель модуля"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.EduProgramHead), "6. Руководитель образовательной программы"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Council), "7. Рекомендовано учебно-методическим советом института"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Direction), "8. Согласовано: Дирекция образовательных программ"), 
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.RequisitesOrders), "9. Реквизиты приказа Минобрнауки РФ об утверждении ФГОС ВО"), 
                }
            };
            //yield return new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Fdps), "ПАРАМЕТРЫ ТАБЛИЦ");

            yield return new WorkingProgramSection(null, "1. ОБЩАЯ ХАРАКТЕРИСТИКА МОДУЛЯ")
            {
                Sections = new[]
                {
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.Annotation), "1.2. Аннотация")
                },
            };

            yield return new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.ModuleStructures), "2. СТРУКТУРА МОДУЛЯ И РАСПРЕДЕЛЕНИЕ УЧЕБНОГО ВРЕМЕНИ ПО ДИСЦИПЛИНАМ");

            yield return new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.DisciplineSequence), "3. ПОСЛЕДОВАТЕЛЬНОСТЬ ОСВОЕНИЯ ДИСЦИПЛИН В МОДУЛЕ");

            yield return new WorkingProgramSection(null, "4. ПЛАНИРУЕМЫЕ РЕЗУЛЬТАТЫ ОСВОЕНИЯ МОДУЛЯ")
            {
                Sections = new[]
                {
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.PlannedResults), "4.1. Планируемые результаты освоения модуля и составляющие их компетенции"),
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.DisciplineCompetences), "4.2. Распределение формирования компетенций по дисциплинам модуля"),
                }
            };

            yield return new WorkingProgramSection(null, "5. ПРОМЕЖУТОЧНАЯ АТТЕСТАЦИЯ ПО МОДУЛЮ")
            {
                Sections = new[]
                {
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.ModuleIntermediateCertificationForms), "5.2. Форма промежуточной аттестации по модулю"),
                }
            };

            yield return new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.ChangesList), "6. ЛИСТ РЕГИСТРАЦИИ ИЗМЕНЕНИЙ В РАБОЧЕЙ ПРОГРАММЕ МОДУЛЯ");

            yield return new WorkingProgramSection(null, "ПРИЛОЖЕНИЕ 1. 5.3. ФОНД ОЦЕНОЧНЫХ СРЕДСТВ ДЛЯ ПРОВЕДЕНИЯ ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО МОДУЛЮ")
            {
                Sections = new[]
                {
                    new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.ControlEventsEstimationCriterias), "5.3.1. ОБЩИЕ КРИТЕРИИ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО МОДУЛЮ"), 
                    new WorkingProgramSection(null, "5.3.2. ОЦЕНОЧНЫЕ СРЕДСТВА ДЛЯ ПРОВЕДЕНИЯ ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ ПО МОДУЛЮ")
                    {
                        Sections = new[]
                        {
                            new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.IntegratedExamQuestions), "5.3.2.1. Перечень примерных вопросов для интегрированного экзамена по модулю"), 
                            new WorkingProgramSection(nameof(ModuleWorkingProgramFgosVoSchemaModel.ModuleProjectThemes), "5.3.2.2. Перечень примерных тем итоговых проектов по модулю"), 
                        }
                    }
                }
            };
        }

        public override VersionedDocumentTemplate GetDocumentTemplate()
        {
            var documentType = GetDocumentType();
            return _db.VersionedDocumentTemplates.Where(t => t.DocumentType == documentType).OrderByDescending(t => t.Version).FirstOrDefault();
        }

        public override DocumentPartViewModel GetNavigationViewModel(VersionedDocument document)
        {
            var wp = _db.ModuleWorkingPrograms.Find(document.Id);
            return new ModuleWorkingProgramViewModel(wp, _documentService)
            {
                AllowEdit = IsInEditableState(document)
            };
        }

        public override void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document)
        {
            builder.RegisterInstance(document);            
            var wp = _db.ModuleWorkingPrograms.Find(document.Id);
            builder.RegisterInstance(wp);
            builder.RegisterInstance(wp.Module);
        }

        protected override VersionedDocumentType GetDocumentType()
        {
            return VersionedDocumentType.ModuleWorkingProgram;
        }
    }
}