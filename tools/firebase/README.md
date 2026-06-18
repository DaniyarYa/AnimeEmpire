# Firebase Setup — Anime Empire

Гайд для развёртывания Firebase backend для MVP. Источник правды: `docs/architecture/10_backend.md` (ADR-002).

Эта папка — артефакты для Phase 0 E5:
- `config.json` — template удалённого конфига, который ты загружаешь на Firebase Hosting
- Этот гайд

## Цель Phase 0

Минимальная backend-интеграция:
- Удалённый JSON-конфиг, который клиент достаёт по HTTP при старте
- При отсутствии сети — fallback на локальный кеш или встроенные defaults
- Не нужны: Auth, save sync, IAP validation, analytics (всё это — Phase 2)

## Стек Phase 0

| Сервис Firebase | Используем | Назначение |
|-----------------|------------|------------|
| Hosting | ✅ | Доставка `config.json` через CDN |
| Realtime Database | ❌ | Phase 2 (save sync) |
| Firestore | ❌ | Phase 2 (player data) |
| Authentication | ❌ | Phase 2 (P2-070) |
| Cloud Functions | ❌ | Phase 2 (IAP validation) |
| Analytics | ❌ | Phase 2 (P2-080) |
| Remote Config (FCM) | ❌ | Используем Hosting вместо официального RC — проще и контролируется (FCM требует client SDK) |
| Crashlytics | ❌ | Phase 2 (P2-082) |

## Шаг 1 · Создать Firebase проект

1. Открыть https://console.firebase.google.com/
2. **Add project** → название (например `anime-empire-dev`)
3. **Disable Google Analytics** на этом этапе (включим позже, Phase 2)
4. **Create project** → подождать минуту

## Шаг 2 · Включить Hosting

1. В консоли проекта → слева **Hosting** (в разделе Build)
2. **Get started**
3. Принять условия. Без необходимости устанавливать CLI можно загружать вручную, но **рекомендуется CLI**.

## Шаг 3 · Установить Firebase CLI (если нет)

```bash
# macOS / Linux
curl -sL https://firebase.tools | bash

# Или через npm
npm install -g firebase-tools

# Проверка
firebase --version
```

## Шаг 4 · Инициализировать Hosting

```bash
cd tools/firebase

# Логин в Firebase
firebase login

# Инициализация (выбираем Hosting)
firebase init hosting
```

На вопросы:
- **Use an existing project:** выбрать созданный проект
- **public directory:** `.` (текущая папка)
- **Configure as single-page app:** No
- **Set up automatic builds:** No
- **Overwrite index.html?** No

После init появятся:
- `.firebaserc` — привязка к проекту
- `firebase.json` — конфигурация Hosting

> ⚠️ Эти файлы **уже в `.gitignore`** (см. ниже). Не коммить их публично, если в проекте есть приватный hosting/auth.

## Шаг 5 · Развернуть config.json

```bash
cd tools/firebase
firebase deploy --only hosting
```

После успешного деплоя получишь URL вида:
- `https://<your-project>.web.app/config.json`
- `https://<your-project>.firebaseapp.com/config.json`

Проверь в браузере — должна показаться JSON-структура.

## Шаг 6 · Подключить URL в Godot

В `godot/project.godot` найти секцию `[anime_empire]` и установить URL:

```ini
[anime_empire]

backend/config_url="https://your-project.web.app/config.json"
```

Или через Godot UI:
- **Project → Project Settings → General → Anime Empire → Backend → Config Url**

Перезапусти Godot project (F5). В Output console увидишь:
```
[RemoteConfig] fetching https://your-project.web.app/config.json
[RemoteConfig] fetched ok, version=1
```

## Шаг 7 · Обновление конфига

Балансировка без билда:

1. Редактируй `tools/firebase/config.json` (увеличь `version` на 1)
2. `firebase deploy --only hosting`
3. Клиенты при следующем запуске подтянут новый

## Локальная разработка

### Без Firebase

Оставь `backend/config_url=""` — клиент использует встроенные defaults из `remote_config.gd`. Игра работает оффлайн.

### С локальным dev-сервером

Запусти простой HTTP-сервер на `tools/firebase/`:

```bash
cd tools/firebase
python3 -m http.server 8080
```

В `project.godot`:
```ini
backend/config_url="http://localhost:8080/config.json"
```

## Безопасность

Hosting URL **публичный** — любой может прочитать `config.json`. Это OK:
- В конфиге **никогда** не хранить секреты (API ключи, JWT secrets, password salt)
- Балансные значения, фича-флаги, A/B тесты — публичная информация (всё равно decompilable на клиенте)
- Чувствительные операции (IAP validation, prestige commit) — через Cloud Functions с auth (Phase 2)

## CORS

Firebase Hosting по умолчанию **не** разрешает cross-origin. Для Godot HTTPRequest это не проблема (нет origin restrictions). Для web-export (если будем делать) — настроить `firebase.json`:

```json
{
  "hosting": {
    "public": ".",
    "headers": [
      {
        "source": "**/*.json",
        "headers": [{ "key": "Access-Control-Allow-Origin", "value": "*" }]
      }
    ]
  }
}
```

## .gitignore секреты

Firebase init создаёт файлы, которые не нужно коммитить в публичный репо:

```gitignore
# В корневом .gitignore уже есть:
tools/firebase/.firebaserc
tools/firebase/firebase.json
tools/firebase/.firebase/
```

Сам `config.json` — **публичный**, коммитим.

## Миграция на AWS (Phase 4 prep)

Когда выручка оправдает SRE-нагрузку (см. ADR-002 в `architecture/10_backend.md`):

1. Поднять CloudFront + S3 bucket
2. Подложить `config.json` в S3
3. Update `backend/config_url` на CloudFront URL
4. Клиентский код **не меняется** — REST универсален

Подробности миграции — в `architecture/10_backend.md`.

## Troubleshooting

### `[RemoteConfig] no URL configured`
В Godot ProjectSettings → `anime_empire/backend/config_url` пустой. Установи URL или оставь пустым для defaults.

### `[RemoteConfig] HTTP 404`
URL неверный или `config.json` не задеплоен. Проверь в браузере.

### `[RemoteConfig] fetch failed (result=2, http=0)`
Нет сети или timeout. Клиент использует кеш / defaults. Проверь подключение.

### Конфиг не обновляется на клиенте
Hosting кешируется CDN. Подожди 1-5 минут или сделай `firebase deploy` с `--force` для invalidation.

## Что дальше

- Phase 2 (P2-070): Firebase Auth
- Phase 2 (P2-080): Firebase Analytics + BigQuery
- Phase 2: Firestore для save sync
- Phase 4 (готовиться): миграция на AWS
