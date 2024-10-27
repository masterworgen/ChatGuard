# ChatGuard Telegram Bot
### Описание
ChatGuard — это телеграмм-бот, который может удалять пользователя из всех чатов, к которым он имеет доступ. Бот упрощает процесс удаления пользователей, требуя только указания профиля без необходимости добавления символа @ перед никнеймом.
Требования
Регистрация API в Telegram: Для работы бота требуется API ID и API Hash, которые можно получить, зарегистрировавшись в Telegram по ссылке my.telegram.org/auth.

Запуск через Docker Compose:
``` yaml
services:
  chatguard:
    image: masterworgen/chatguard:latest
    environment:
      - UserConfiguration__ApiId=
      - UserConfiguration__ApiHash=
      - UserConfiguration__BotToken=
    ports:
      - "25410:8080"
      - "25411:8081"
```

Пример использования бота:
1. На портале компании сотрудники вводят свои ники;
2. Бот добавляется в каждый рабочий чат;
3. При увольнении сотрудника отправляется запрос, который инициирует команду бота на удаление данного пользователя из всех чатов, где он присутствует.
