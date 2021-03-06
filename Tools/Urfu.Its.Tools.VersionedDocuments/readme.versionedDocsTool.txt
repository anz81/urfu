Тулза vdocs.

Доступные команды:
1. UpdateTemplate - для указанного типа документа обновляется шаблон в БД и связанные с ним документы.
Команда работает в двух режимах: 
1) TryUpdate - если не смог обновиться хотя бы один документ, то обновление не происходит вообще. Режим по умолчанию.
2) UpdateAllThatPossible - обновление выполняется в любом случае. Если некоторые или все документы не обновились, то они остануться на старом шаблон и создастся новый шаблон для успешно обновленных документов. Если в схеме были изменения при которых невозможно выполнить обновление данных документов, то все документы остаются на старом шаблоне и создается новый актуальный шаблон. Режим включается флагом -f/--force.

Примеры:
1) Команда [режим UpdateAllThatPossible, с флагом -f/--force] обновляет шаблон и связанные с ним документы. Если при обновлении некоторых или всех документов возникли ошибки, либо в схеме были изменения, обновление данных для которых невозможно, то они остаются на старом шаблоне. При этом в БД создается новый шаблон с новой схемой и вордовским шаблоном без привязки документов.
vdocs --command UpdateTemplate --documentType ModuleWorkingProgram -t "..\..\..\..\Urfu.Its.VersionedDocs\Templates\MWP.docx" -f
2) Команда [режим TryUpdate] обновляет шаблон и связанные с ним документы. Если при обновлении некоторых или всех документов возникли ошибки, либо в схеме были изменения, обновление данных для которых невозможно, то обновление не происходит (транзакция откатывается) и программа падает с ошибкой.
vdocs --command UpdateTemplate --documentType ModuleWorkingProgram -t "..\..\..\..\Urfu.Its.VersionedDocs\Templates\MWP.docx"

2. DeleteData (!!!В продакшне пользоваться нельзя!!!) - для указанного типа документа удаляет все шаблоны и данные в БД. Удобно при разработке.
Примечание:
Сейчас реализовано удаление для РПМ и РПД. Когда будут таблицы для других РП, можно добавить их удаление в тулзу.

Примеры:
1) Команда удаляет шаблоны и данные для указанного типа документа с диалогом подтверждения действия.
vdocs --command deleteData --documentType ModuleWorkingProgram
2) Команда удаляет шаблоны и данные для указанного типа документа без диалога подтверждения действия.
vdocs -c deleteData -d mwp -f


Доступные типы документов:
Краткое/длинное название
mwp moduleworkingprogram
dwp disciplineworkingprogram
gwp giaworkingprogram    
pwp practicesworkingprogram
mcl modulechangelist    
dcl disciplinechangelist


Тулза регистронезависима.
Для удобства вызова команд сделаны .bat скрипты в папке Tools/Urfu.Its.Tools.VersionedDocuments/Scripts. Скрипты копируются при билде проекта в output-директорию.