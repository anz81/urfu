using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity.Migrations;
using Autofac;
using Autofac.Features.Indexed;
using Urfu.Its.Common;
using Urfu.Its.Web.DataContext;
//using Urfu.Its.Web.Migrations;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IVersionedDocumentUpdater
    {
        void UpdateLinkedDocumentsForTemplate(VersionedDocumentTemplate template,
            VersionedDocumentDescriptor newDescriptor,
            out IReadOnlyDictionary<VersionedDocument, string[]> documentErrors, DateTime updateTime);
    }

    public class VersionedDocumentUpdaterNotSupportedTheseChangesException : Exception
    {
        public VersionedDocumentUpdaterNotSupportedTheseChangesException(string message) : base(message)
        {
            
        }
    }

    public enum UpdateMode
    {
        TryUpdate,
        UpdateAllThatPossible
    }
}