# Syncro — программа резервного копирования  
  
Привет, меня зовут Емельянов Михаил, я ищу работу backend C#-программиста; Syncro — учебный проект, тестовое задание на проверку навыков использования C# и WPF, которое я составил сам для себя. Моё резюме и ссылки на остальные репозитории находятся [здесь](https://github.com/amaargiru/coverletter).  

## Техническое задание  

Необходимо разработать прототип WPF-приложения, обеспечивающего резервное копирование файлов. Для экономии времени на резервный носитель информации должны быть перенесены только новые и отличающиеся файлы и папки; файлы и папки, удаленные на основном носителе, также должны быть удалены и на резервном носителе; таким образом, в результате работы приложения Syncro резервный носитель должен представлять из себя зеркальную, синхронизированную копию основного носителя.  
  
### Описание задачи  
  
Пользовательский интерфейс прототипа WPF-приложения должен иметь:  
• двухпанельный интерфейс:  
левая панель — строка выбора директории на основном носителе плюс список файлов и папок на основном носителе, требующих модификации или отсутствующих на резервном носителе;  
правая панель — строка выбора директории на резервном носителе плюс список файлов и папок на резервном носителе, которые в процессе синхронизации будут изменены или удалены;  
• каждая строка обеих панелей должна быть снабжена наглядным индикатором, показывающим, какая именно операция будет применена к каждому конкретному файлу или папке — копирование, модификация или удаление;  
• кнопку «Compare», при нажатии на которую запускается процесс сравнения информации на основном и резервном носителе;  
• кнопку «Synchronize», при нажатии на которую запускается процесс копирования информации с основного на резервный носитель;  
• в процессе копирования пользователю должен показываться индикатор прогресса, дающий наглядное представление о ходе процесса и приблизительном времени до завершения;  
• во время процессов как сравнения, так и копирования должна быть показана кнопка «Stop», позволяющая экстренно завершить процесс.  

В случае возникновения ошибки в процессе удаления или копирования файлов и папок приложение должно проинформировать об этом пользователя и дождаться от него ответа. Ответ может быть трех типов — «Cancel», «Ignore» и «Ignore all similar warnings».  
  
После окончания процесса копирования пользователю должен быть доступен log-файл, имеющий в названии временную метку и показывающий, какая операция была произведена над каждым конкретным файлом или директорией.  
  
### Функционал  
  
WPF-приложение должно обеспечивать следующий функционал:  
• удобная для пользователя обработка ошибок;  
• возможность брать настройки, относящиеся как к внешнему виду приложения, так и к логике работы, из файла настроек;  
• отделение логики приложения от внешнего вида с тем, чтобы замена способа взаимодействия с пользователем (например, замена WPF на MAUI) не требовала переработки всего приложения, а только его части;  
• логгирование, пользователь должен иметь возможность проследить операции над каждым файлом или директорией;  
• все публичные методы должны быть покрыты юнит-тестами.  
