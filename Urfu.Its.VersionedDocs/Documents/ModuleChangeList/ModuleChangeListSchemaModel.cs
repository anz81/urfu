using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Documents.ModuleChangeList
{
    public class ModuleChangeListSchemaModel
    {
        public string FileName { get; set; }

        public string Name { get; set; }

        #region FrontPage

        public bool Module { get; set; }
        
        public bool Institute { get; set; }

        /// <summary>
        /// Направления.
        /// </summary>
        public bool Directions { get; set; }

        public bool Profiles { get; set; }
        
        public bool Authors { get; set; }
        
        public bool Head { get; set; }

        public bool EduProgramHead { get; set; }

        public bool Council { get; set; }

        /// <summary>
        /// Дирекция образовательных программ
        /// </summary>
        public bool Direction { get; set; }

        public bool RequisitesOrders { get; set; }

        #endregion

        public bool Annotation { get; set; }

        /// <summary>
        /// 2.	СТРУКТУРА МОДУЛЯ И РАСПРЕДЕЛЕНИЕ УЧЕБНОГО ВРЕМЕНИ ПО ДИСЦИПЛИНАМ
        /// </summary>
        public bool ModuleStructures { get; set; }
        
        public bool DisciplineSequence { get; set; }

        /// <summary>
        /// 4.1.	Планируемые результаты освоения модуля и составляющие их компетенции
        /// </summary>
        public bool PlannedResults { get; set; }

        /// <summary>
        /// 4.2.Распределение формирования компетенций по дисциплинам модуля
        /// </summary>
        public bool DisciplineCompetences { get; set; }

        /// <summary>
        /// 5.2. Форма промежуточной аттестации по модулю:
        /// </summary>
        public bool ModuleIntermediateCertificationForms { get; set; }

        /// <summary>
        /// Таблица из приложения 1. 5.3. Фонд оценочных средств...
        /// </summary>
        public bool ControlEventsEstimationCriterias { get; set; }

        /// <summary>
        /// 5.3.2.1. Перечень примерных  вопросов для интегрированного экзамена по модулю список.
        /// </summary>
        public bool IntegratedExamQuestions { get; set; }

        /// <summary>
        /// 5.3.2.2. Перечень примерных  тем итоговых проектов по модулю список.
        /// </summary>
        public bool ModuleProjectThemes { get; set; }

        /// <summary>
        /// 6. ЛИСТ РЕГИСТРАЦИИ ИЗМЕНЕНИЙ В РАБОЧЕЙ ПРОГРАММЕ МОДУЛЯ
        /// </summary>
        public bool ChangesList { get; set; }

        public ICollection<DisciplineChangesInfo> Disciplines { get; set; } = new List<DisciplineChangesInfo>();
    }

    public class DisciplineChangesInfo
    {
        public string Name { get; set; }

        public string DisciplineId { get; set; }

        public bool Annotation { get; set; }

        public bool Language { get; set; }
        
        public bool PlannedLearningOutcomes { get; set; }
        
        public bool Sections { get; set; }

        /// <summary>
        /// Таблицы 1.4
        /// </summary>
        public bool DisciplineScopes { get; set; }

        public bool TimeDistributions { get; set; }

        /// <summary>
        /// 4.1. Лабораторные работы (по направлениям и формам)
        /// </summary>
        public bool Labs { get; set; }

        /// <summary>
        /// 4.2. Практические занятия (по направлениям и формам)
        /// </summary>
        public bool Practices { get; set; }

        /// <summary>
        /// 4.3. Примерная тематика самостоятельной работы (по направлениям и формам)
        /// </summary>
        public bool SelfWorkThemes { get; set; }

        /// <summary>
        /// 5. СООТНОШЕНИЕ РАЗДЕЛОВ, ТЕМ ДИСЦИПЛИНЫ И ПРИМЕНЯЕМЫХ ТЕХНОЛОГИЙ ОБУЧЕНИЯ
        /// </summary>
        public bool LearningMethods { get; set; }

        /// <summary>
        /// Расшаренный блок. Сохранять его отсюда не нужно.
        /// </summary>
        public bool ControlEventsEstimationCriterias { get; set; }
        
        public bool TechCardDisciplineCertification { get; set; }
        
        public bool TechCardCourseWorksCertification { get; set; }

        public bool TechCardSemesterSignificanceCoefficients { get; set; }

        /// <summary>
        /// 7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИ-СИМОГО ТЕСТОВОГО КОНТРОЛЯ
        /// Блок ФЭПО
        /// </summary>
        public bool SmudsTests { get; set; }

        /// <summary>
        /// 7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИ-СИМОГО ТЕСТОВОГО КОНТРОЛЯ
        /// Блок ФЭПО
        /// </summary>
        public bool FepoTests { get; set; }

        /// <summary>
        /// 7. ПРОЦЕДУРЫ ОЦЕНИВАНИЯ РЕЗУЛЬТАТОВ ОБУЧЕНИЯ В РАМКАХ НЕЗАВИ-СИМОГО ТЕСТОВОГО КОНТРОЛЯ
        /// Блок Интернет-тренажеров
        /// </summary>
        public bool InternetTrainerTests { get; set; }

        /// <summary>
        /// 8.3.1. Примерные  задания для проведения мини-контрольных в рамках учебных занятий
        /// </summary>
        public bool MiniControlWorkThemes { get; set; }

        /// <summary>
        /// 8.3.2. Примерные  контрольные задачи в рамках учебных занятий
        /// </summary>
        public bool ControlWorkThemes { get; set; }

        /// <summary>
        /// 8.3.3. Примерные  контрольные кейсы
        /// </summary>
        public bool ControlKeys { get; set; }

        /// <summary>
        /// 8.3.4. Перечень примерных  вопросов для зачета
        /// </summary>
        public bool TestQuestions { get; set; }

        /// <summary>
        /// 8.3.5. Перечень примерных  вопросов для экзамена
        /// </summary>
        public bool ExamQuestions { get; set; }

        /// <summary>
        /// 8.3.6. Ресурсы АПИМ УрФУ, СКУД УрФУ для проведения тестового контроля в рамках текущей и промежуточной аттестации
        /// </summary>
        public bool UrfuResources { get; set; }

        /// <summary>
        /// 8.3.7. Ресурсы ФЭПО для проведения независимого тестового контроля
        /// </summary>
        public bool FepoResources { get; set; }

        /// <summary>
        /// 8.3.8. Интернет-тренажеры
        /// </summary>
        public bool InternetTrainers { get; set; }

        /// <summary>
        /// 8.3.9…..указать иные наименования оценочных средств, не представленных в списке
        /// </summary>
        public bool OtherEvaluationTools { get; set; }

        /// <summary>
        /// 9.1 Другая литература
        /// </summary>
        public bool Literature { get; set; }

        /// <summary>
        /// 9.2.Методические разработки
        /// </summary>
        public bool MethodicalSupport { get; set; }

        /// <summary>
        /// 9.3.Программное обеспечение
        /// </summary>
        public bool Software { get; set; }

        /// <summary>
        /// 9.4. Базы данных, информационно-справочные и поисковые системы
        /// </summary>
        public bool Databases { get; set; }

        /// <summary>
        /// 9.5.Электронные образовательные ресурсы
        /// </summary>
        public bool ElectronicEducationalResources { get; set; }

        /// <summary>
        /// 10. МАТЕРИАЛЬНО-ТЕХНИЧЕСКОЕ  ОБЕСПЕЧЕНИЕ ДИСЦИПЛИНЫ
        /// </summary>
        public bool TechnicalSupport { get; set; }        
    }
}