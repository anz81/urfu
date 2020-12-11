using System.Collections.Generic;
using System.IO;
using System.Linq;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Loaders;
using Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs.Processors;
using Urfu.Its.VersionedDocs.Documents.Shared;
using Urfu.Its.VersionedDocs.Documents.Shared.Loaders;
using Urfu.Its.Web.DataContext;
using Urfu.Its.Web.Model.Models;
using Urfu.Its.Web.Model.Models.OHOPModels;
using Urfu.Its.Web.Models;

namespace Urfu.Its.VersionedDocs.Documents.BasicCharacteristicOPs
{
    public class BasicCharacteristicOPSchemaModel
    {
        /// <summary>
        /// Название документа для формирования печатных форм. Без расширения.
        /// </summary>
        [Block(LoaderType = typeof(FileNameLoader))]
        public string FileName { get; set; }

        #region FrontPage

        public RatifyingInfo RatifyingInfo { get; set; } = new RatifyingInfo();
        
        public EduProgramInfo EduProgramInfo { get; set; } = new EduProgramInfo();

      
        public InstituteInfo Institute { get; set; } = new InstituteInfo();

        
        public DirectionInfo2 Direction { get; set; } = new DirectionInfo2();

       
        public ProfileTrajectoriesInfo Profile { get; set; } = new ProfileTrajectoriesInfo();
 
        public ICollection<AuthorInfo> Authors { get; set; } = new List<AuthorInfo>();
        
        public AuthorInfo EduProgramHead { get; set; } = new AuthorInfo();
         
        [Block(LoaderType = typeof(EducationalMethodicalCouncilLoader))]
        public EducationalMethodicalCouncilInfo Council { get; set; } = new EducationalMethodicalCouncilInfo();

        /// <summary>
        /// Дирекция образовательных программ
        /// </summary>
        [Block(LoaderType = typeof(WorkingProgramPersonLoader))]
        public WorkingProgramPersonInfo DirectionHead { get; set; } = new WorkingProgramPersonInfo();

        /// <summary>
        /// Приказы. Из базы подтягиваются данные по кнопке Обновить
        /// </summary>
        public ICollection<RequisitesOrderFgosInfo> RequisitesOrders { get; set; } = new List<RequisitesOrderFgosInfo>();

        #endregion
        
        #region CommonCharacteristics  1. ОБЩИЕ ПОЛОЖЕНИЯ

        /// <summary>
        /// 1.1. Общая характеристика
        /// </summary>
        [Block(LoaderType = typeof(CommonCharacteristicLoader))]
        public string CommonCharacteristic { get; set; }

        /// <summary>
        /// 1.2. Назначение и особенность образовательной программы
        /// </summary>
        [Block(LoaderType = typeof(PurposeAndFeatureLoader))]
        public string PurposeAndFeature { get; set; }

        /// <summary>
        /// 1.3. Форма обучения и срок освоения образовательной программы. Из базы подтягиваются данные по кнопке Обновить
        /// </summary>
        public FormAndDuration FormAndDuration { get; set; }

        /// <summary>
        /// 1.4. Электронное обучение. В шаблоне не используется, есть текст по умолчанию
        /// </summary>
        public string Elearning { get; set; }

        /// <summary>
        /// 1.5. Объем программы. В шаблоне не используется, есть текст по умолчанию с данными из блока 4.
        /// </summary>
        public string ProgramSize { get; set; }

        /// <summary>
        /// 1.6. Язык программы. Из базы подтягиваются данные по кнопке Обновить
        /// </summary>
        public string Language { get; set; }

        #endregion

        #region ProfActivityCharacteristics 2. Характеристика профессиональной деятельности выпускников и описание траекторий образовательной программы

        /// <summary>
        /// 2.1. Текст
        /// </summary>
        public string ProfStandards { get; set; }
        
        /// <summary>
        /// 2.2. Текст
        /// </summary>
        public string VariantText { get; set; }

        /// <summary>
        /// 2.2. Таблица 1.
        /// 3. Таблица 4. Профессиональные компетенции.
        /// </summary>
        public Variants Variants { get; set; } = new Variants();

        #endregion

        #region PlannedResults 3. Планируемые результаты освоения ОП

        /// <summary>
        /// 3. Таблица 2. Универсальные компетенции (УК). Из базы подтягиваются данные по кнопке Обновить
        /// </summary>
        public ICollection<CompetenceInfoVM> UniversalCompetences { get; set; }

        /// <summary>
        /// 3. Таблица 3. Общепрофессиональные компетенции (ОПК). Из базы подтягиваются данные по кнопке Обновить
        /// </summary>
        public ICollection<CompetenceInfoVM> GeneralCompetences { get; set; }

        #endregion

        #region ModuleStructure 4. Структура образовательной программы

        /// <summary>
        /// 4. Таблица 5. Модульная структура образовательной программы
        /// </summary>
        [Block(LoaderType = typeof(ModuleStructureLoader), ProcessorType = typeof(ModuleStructureProcessor))]
        public ModuleStructure ModuleStructure { get; set; }

        #endregion

        #region ProfStandartsList ПРИЛОЖЕНИЕ 1. Профессиональные стандарты

        /// <summary>
        /// ПРИЛОЖЕНИЕ 1. Профессиональные стандарты. Из базы подтягиваются данные по кнопке Обновить.
        /// Использует блок Variants
        /// </summary>
        public ICollection<ProfStandardInfo> ProfStandardsList { get; set; }

        #endregion

        #region ApprovalActs ПРИЛОЖЕНИЕ 2. Акты согласования

        /// <summary>
        /// Используется для Актов согласования (Приложение 2)
        /// </summary>
        public InstituteInfo Chair { get; set; } = new InstituteInfo();

        /// <summary>
        /// ПРИЛОЖЕНИЕ 2. Акты согласования
        /// </summary>
        [Block(ProcessorType = typeof(ApprovalActProcessor))]
        public ICollection<ApprovalAct> ApprovalActs { get; set; } = new List<ApprovalAct>();

        #endregion

        #region Files ПРИЛОЖЕНИЕ 3. Приложенные файлы

        /// <summary>
        /// ПРИЛОЖЕНИЕ 3. Приложенные файлы
        /// </summary>
        [Block(ProcessorType = typeof(FilesProcessor))]
        public ICollection<FileStorageInfo> Files { get; set; } = new List<FileStorageInfo>();

        #endregion
    }
}