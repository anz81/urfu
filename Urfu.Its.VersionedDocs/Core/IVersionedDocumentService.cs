using System.IO;
using Newtonsoft.Json.Schema;
using TemplateEngine;
using Urfu.Its.VersionedDocs.Services;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IVersionedDocumentService
    {
        /// <summary>
        /// Процедура создания сериализованного представления модели документа.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="loadBlocks">Названия блоков, которые необходимо загрузить</param>
        /// <returns></returns>
        string CreateSerializedModel(VersionedDocument document, params string[] loadBlocks);

        /// <summary>
        /// Создание прокси-модели из данных документа в БД. Тип создается в рантайме.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="loadBlocks">Названия блоков, которые необходимо загрузить</param>
        /// <returns></returns>
        object CreateProxyModel(VersionedDocument document, params string[] loadBlocks);

        /// <summary>
        /// Процедура создания типизированной модели данных документа. Возможно использовать только для документов с актуальной схемой.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="loadBlocks"></param>
        /// <returns></returns>
        object CreateModel(VersionedDocument document, params string[] loadBlocks);

        /// <summary>
        /// Формирование печатной формы документа
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        Stream Print(VersionedDocument document, FileFormat fileFormat);

        /// <summary>
        /// Формирование архива с документом
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        MemoryStream PrintZip(VersionedDocument document, FileFormat fileFormat);

        /// <summary>
        /// Процедура проверки схемы указанного документа на актуальность текущей схемы в программе.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        bool IsSchemaActual(VersionedDocument document);

        /// <summary>
        /// Процедура сохранения данных блоков с поддержкой версионности.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="serializedDocumentData"></param>
        string ApplyDocumentChanges(VersionedDocument document, string serializedDocumentData, out VersionedDocumentBlockInspectionInfo[] inspections);

        bool ValidateBySchema(VersionedDocument document, out ValidationError[] validationErrors);

        void ResaveDocument(VersionedDocument document);
    }

    public static class VersionedDocumentServiceExtensions
    {
        public static T CreateModel<T>(this IVersionedDocumentService documentService, VersionedDocument document, params string[] loadBlocks)
        {
            return (T) documentService.CreateModel(document, loadBlocks);
        }

        public static string ApplyDocumentChanges(this IVersionedDocumentService documentService, VersionedDocument document, string serializedDocumentData)
        {
            return documentService.ApplyDocumentChanges(document, serializedDocumentData, out var inspections);
        }
    }
}