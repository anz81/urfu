namespace Urfu.Its.Integration.MqModel
{
    // Данные для публикации изменения принадлежности студента к миноргруппе
    public class MinorgroupMemberMqDto
    {
        // id подгруппы
        public int id { get; set; }
        // название подгруппы
        public string name { get; set; }
        // ключ подгруппы
        public string combinedKey { get; set; }
        // ключ подгруппы равен combinedKey. ITS-1430
        public string combinedKey2 { get; set; }
        // id учебного модуля
        public string moduleId { get; set; }
        // название учебного модуля
        public string moduleName { get; set; }
        public string disciplineUuid { get; set; }
        // id справочной дисциплины
        public string discipline { get; set; }
        // название справочной дисциплины
        public string disciplineName { get; set; }
        // id вида нагрузки
        public string loadType { get; set; }
        // номер семестра
        public int term { get; set; }
        // учебный год
        public int eduyear { get; set; }
        // id студента
        public string student { get; set; }
        // ФИО студента
        public string studentName { get; set; }
    }
}