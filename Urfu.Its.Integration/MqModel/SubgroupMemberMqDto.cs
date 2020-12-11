namespace Urfu.Its.Integration.MqModel
{
    // Данные для публикации изменения принадлежности студента к подгруппе
    public class SubgroupMemberMqDto
    {
        // id подгруппы
        public int id { get; set; }
        // название подгруппы
        public string name { get; set; }
        // ключ подгруппы
        public string combinedKey { get; set; }
        // ключ подгруппы равен combinedKey. ITS-1430
        public string combinedKey2 { get; set; }
        // id учебной группы
        public string groupId { get; set; }
        // название учебной группы
        public string groupName { get; set; }
        // id учебного модуля
        public string moduleId { get; set; }
        // название учебного модуля
        public string moduleName { get; set; }
        // id справочной дисциплины
        public string catalogDisciplineUuid { get; set; }
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