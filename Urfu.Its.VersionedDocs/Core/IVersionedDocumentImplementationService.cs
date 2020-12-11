using System;
using System.Collections.Generic;
using System.Security.Principal;
using Autofac;
using Newtonsoft.Json;
using Urfu.Its.VersionedDocs.Documents.ModuleChangeList;
using Urfu.Its.VersionedDocs.ViewModels;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IChangeListService<TSchemaModel> : IChangeListService
    {

    }

    public interface IChangeListService : IVersionedDocumentImplementationService
    {
        VersionedDocument CreateDocument(int sourceId, int targetId);
    }

    public interface IWorkingProgramService<TSchemaModel> : IWorkingProgramService
    {
        
    }

    public interface IWorkingProgramService : IVersionedDocumentImplementationService
    {
        IEnumerable<WorkingProgramSection> GetSections();

        IReadOnlyDictionary<VersionedDocument, DocumentVersionInfo> GetDocumentsAndVersions(string linkedEntityId, string standard = null, int? year = null, int? planNumber = null, int? planVersionNumber = null);

        /// <summary>
        /// Процедура создания документа и инициализации исходной структуры данных
        /// </summary>
        /// <param name="linkedEntityId"></param>
        /// <returns></returns>
        VersionedDocument CreateDocument(string linkedEntityId, string standard, int? year = null, int? planNumber = null, int? planVersionNumber = null);

        VersionedDocument CreateDocumentBasedOn(VersionedDocument document, int? year = null);        
    }

    public interface IVersionedDocumentImplementationService
    {
        /// <summary>
        /// Процедура получения актуального дескриптора документа
        /// </summary>
        /// <returns></returns>
        VersionedDocumentDescriptor GetDocumentDescriptor();

        VersionedDocumentTemplate GetDocumentTemplate();

        /// <summary>
        /// Процедура регистрации зависимостей для загрузчиков данных блоков <see cref="IBlockContentLoader"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="document"></param>
        void RegisterDocumentDependencies(ContainerBuilder builder, VersionedDocument document);

        DocumentPartViewModel GetNavigationViewModel(VersionedDocument document);

        Type GetActualSchemaType();
        bool IsInEditableState(VersionedDocument document);

        /// <summary>
        /// Проверка на наличие доступа на редактирование документа конкретным пользователем. Не включает в себя логику проверки состояния документа.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        bool UserHasAccess(IPrincipal user, VersionedDocument document);        
    }

    public class DisciplineWorkingProgramVersionInfo : DocumentVersionInfo
    {
        public DisciplineWorkingProgramVersionInfo(int versionNumber, int moduleWorkingProgramVersionId, int? basedOnVersion) : base(versionNumber)
        {
            ModuleWorkingProgramVersionId = moduleWorkingProgramVersionId;
            BasedOnVersion = basedOnVersion;
        }

        public int ModuleWorkingProgramVersionId { get; }

        public int? BasedOnVersion { get; }

        public override string VersionDisplayName => $"{VersionNumber} ({(BasedOnVersion == null ? "" : $"на основе РПД версии {BasedOnVersion}, ")}РПМ версии {ModuleWorkingProgramVersionId})";
    }

    public class DocumentVersionInfo
    {
        public DocumentVersionInfo(int versionNumber)
        {
            VersionNumber = versionNumber;            
        }

        public int VersionNumber { get; set; }

        public virtual string VersionDisplayName => VersionNumber.ToString();
    }
}