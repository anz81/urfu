using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Features.Indexed;
using Newtonsoft.Json.Schema;
using TemplateEngine;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs;
using Urfu.Its.VersionedDocs.Documents.CompetencePassports;
using Urfu.Its.VersionedDocs.Documents.Discipline;
using Urfu.Its.VersionedDocs.Documents.Gia;
using Urfu.Its.VersionedDocs.Documents.Module;
using Urfu.Its.VersionedDocs.Documents.ModuleAnnotations;
using Urfu.Its.VersionedDocs.Documents.ModuleChangeList;
using Urfu.Its.VersionedDocs.Documents.Practices;
using Urfu.Its.VersionedDocs.Loggers;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs
{
    public class WorkingProgramsModule : Autofac.Module
    {
        private readonly Func<IPrincipal> _getCurrentUser;
        private readonly Func<IDictionary<string, string>> _getQueryParameters;

        public WorkingProgramsModule(Func<IPrincipal> getCurrentUser, Func<IDictionary<string,string>> getQueryParameters)
        {
            _getCurrentUser = getCurrentUser;
            _getQueryParameters = getQueryParameters;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();

            builder.RegisterType<VersionedDocumentSchemaService>().As<IVersionedDocumentSchemaService>();
            builder.RegisterType<WordDocxTemplateReportingEngine>().As<ITemplateReportingEngine>().InstancePerRequest();

            var registerUser = new Action<PreparingEventArgs>(e => e.Context.ComponentRegistry.Register(
                RegistrationBuilder.ForDelegate((c, p) => _getCurrentUser()).SingleInstance().CreateRegistration()));

            builder.RegisterGeneric(typeof(VersionedDocumentsLogger<>)).As(typeof(IObjectLogger<>));

            builder.RegisterType<VersionedDocumentService>()
                .Named<IVersionedDocumentService>("VersionedDocumentService")
                .OnPreparing(registerUser)
                .InstancePerRequest();
            builder.RegisterDecorator<IVersionedDocumentService>((c, inner)=> new DebugVersionedDocumentService(inner), "VersionedDocumentService")                
                .InstancePerRequest();

            builder.RegisterType<ModuleWorkingProgramFgosVoService>()
                .As<IWorkingProgramService<ModuleWorkingProgramFgosVoSchemaModel>>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.ModuleWorkingProgram)            
                .Keyed<IWorkingProgramService>(VersionedDocumentType.ModuleWorkingProgram);                
            builder.RegisterType<DisciplineWorkingProgramFgosVoService>()
                .WithParameter((p,c)=> p.Name == "moduleId", (p,c) =>
                {
                    // Параметр moduleId необходим для создания связи с РПМ при создании новой РПД.
                    _getQueryParameters().TryGetValue("moduleId", out var moduleId);
                    return moduleId;
                })
                .As<IWorkingProgramService<DisciplineWorkingProgramFgosVoSchemaModel>>()                
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.DisciplineWorkingProgram)
                .Keyed<IWorkingProgramService>(VersionedDocumentType.DisciplineWorkingProgram);
            builder.RegisterType<GiaWorkingProgramFgosVoService>()
                .As<IWorkingProgramService<GiaWorkingProgramFgosVoSchemaModel>>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.GiaWorkingProgram)
                .Keyed<IWorkingProgramService>(VersionedDocumentType.GiaWorkingProgram);
            builder.RegisterType<PracticesWorkingProgramFgosVoService>()
                .As<IWorkingProgramService<PracticesWorkingProgramFgosVoSchemaModel>>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.PracticesWorkingProgram)
                .Keyed<IWorkingProgramService>(VersionedDocumentType.PracticesWorkingProgram);
            builder.RegisterType<BasicCharacteristicOPService>()
                .As<IWorkingProgramService<BasicCharacteristicOPSchemaModel>>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.BasicCharacteristicOP)
                .Keyed<IWorkingProgramService>(VersionedDocumentType.BasicCharacteristicOP);
            builder.RegisterType<CompetencePassportService>()
                .As<IWorkingProgramService<CompetencePassportSchemaModel>>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.CompetencePassport)
                .Keyed<IWorkingProgramService>(VersionedDocumentType.CompetencePassport);
            builder.RegisterType<ModuleAnnotationService>()
                .As<IWorkingProgramService<ModuleAnnotationSchemaModel>>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.ModuleAnnotation)
                .Keyed<IWorkingProgramService>(VersionedDocumentType.ModuleAnnotation);
            builder.RegisterType<ModuleChangeListService>()
                .Keyed<IVersionedDocumentImplementationService>(VersionedDocumentType.ModuleChangeList);

            builder.RegisterType<LoggingVersionedDocumentInspector>()
                .As<IVersionedDocumentInspector>()
                .InstancePerLifetimeScope();

            builder.RegisterType<VersionedDocumentDescriptorService>()
                .As<IVersionedDocumentDescriptorService>();

            builder.RegisterGeneric(typeof(AllPropertiesAreBlocksDescriptorFactory<>))
                .As(typeof(IVersionedDocumentModelDescriptorFactory<>));
            
            base.Load(builder);
        }        
    }

    public class DebugVersionedDocumentService : IVersionedDocumentService
    {
        private readonly IVersionedDocumentService _documentService;

        public DebugVersionedDocumentService(IVersionedDocumentService documentService)
        {
            _documentService = documentService;
        }

        public string CreateSerializedModel(VersionedDocument document, params string[] loadBlocks)
        {
            var sw = Stopwatch.StartNew();
            var serializedModel = _documentService.CreateSerializedModel(document, loadBlocks);
            sw.Stop();
            Trace.WriteLine("CreateSerializedModel: " + sw.Elapsed);
            return serializedModel;
        }

        public object CreateProxyModel(VersionedDocument document, params string[] loadBlocks)
        {
            var sw = Stopwatch.StartNew();
            var proxyModel = _documentService.CreateProxyModel(document, loadBlocks);
            sw.Stop();
            Trace.WriteLine("CreateProxyModel: " + sw.Elapsed);
            return proxyModel;
        }

        public object CreateModel(VersionedDocument document, params string[] loadBlocks)
        {
            var sw = Stopwatch.StartNew();
            var obj = _documentService.CreateModel(document, loadBlocks);
            sw.Stop();
            Trace.WriteLine("CreateModel: " + sw.Elapsed);
            return obj;
        }

        public Stream Print(VersionedDocument document, FileFormat fileFormat)
        {
            var sw = Stopwatch.StartNew();
            var stream = _documentService.Print(document, fileFormat);
            sw.Stop();
            Trace.WriteLine("Print: " + sw.Elapsed);
            return stream;
        }

        public bool IsSchemaActual(VersionedDocument document)
        {
            var sw = Stopwatch.StartNew();
            var isSchemaActual = _documentService.IsSchemaActual(document);
            sw.Stop();
            Trace.WriteLine("IsSchemaActual: " + sw.Elapsed);
            return isSchemaActual;
        }

        public string ApplyDocumentChanges(VersionedDocument document, string serializedDocumentData, out VersionedDocumentBlockInspectionInfo[] inspections)
        {
            var sw = Stopwatch.StartNew();
            var changes = _documentService.ApplyDocumentChanges(document, serializedDocumentData, out inspections);
            sw.Stop();
            Trace.WriteLine("ApplyDocumentChanges: " + sw.Elapsed);
            return changes;
        }

        public bool ValidateBySchema(VersionedDocument document, out ValidationError[] validationErrors)
        {
            var sw = Stopwatch.StartNew();
            var isValid = _documentService.ValidateBySchema(document, out validationErrors);
            sw.Stop();
            Trace.WriteLine("ValidateBySchema: " + sw.Elapsed);
            return isValid;
        }

        public void ResaveDocument(VersionedDocument document)
        {
            var sw = Stopwatch.StartNew();
            _documentService.ResaveDocument(document);
            sw.Stop();
            Trace.WriteLine("ResaveDocument: " + sw.Elapsed);            
        }

        public MemoryStream PrintZip(VersionedDocument document, FileFormat fileFormat)
        {
            var sw = Stopwatch.StartNew();
            var stream = _documentService.PrintZip(document, fileFormat);
            sw.Stop();
            Trace.WriteLine("PrintZip: " + sw.Elapsed);
            return stream;
        }
    }
}