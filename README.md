# BoardGame_Records
Консольное приложение дает возможность вести статистику сыгранных партий в настольные игры, дублируя ее в базу данных SQL, используя Entity Framework. При вводе никнейма с сайта boardgamegeek.com по API скачивается xml-файл, парсится с последующим формированием коллекции игрока, истории игр и новых xml-файлов с новой структурой и необходимыми данными (название игры, количество партии, время сыгранной партии). При уже существующем файле с историей либо коллекцией локальные данные и данный с сайта синхронизируются.
