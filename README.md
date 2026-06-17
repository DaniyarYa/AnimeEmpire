# Anime Empire

Idle-tycoon на Godot 4.x в аниме-стиле. Mobile-first (iOS / Android).

## Содержание репозитория

```
AnimeEmpire/
├── GDD/             Game Design Document (источник правды по геймдизайну)
├── docs/            Архитектура, дизайн-спеки, TODO по фазам
│   ├── README.md         ← начни отсюда
│   ├── architecture/
│   ├── design/
│   └── todo/
├── godot/           Godot 4.x проект
├── tools/           Скрипты сборки, миграций, балансировки
└── .github/         CI / workflow конфиги
```

## Начало работы

1. Установить:
   - Godot 4.x (Mobile / Compatibility renderer): https://godotengine.org/download
   - Git LFS: `brew install git-lfs` (macOS) или https://git-lfs.com/
   - Python 3.11+ (для `tools/`)
2. Клонировать:
   ```bash
   git clone <repo-url>
   cd AnimeEmpire
   git lfs install
   git lfs pull
   ```
3. Открыть `godot/project.godot` в Godot 4.x

## Документация

- `docs/README.md` — навигация
- `docs/architecture/00_overview.md` — высокоуровневая архитектура
- `docs/todo/00_milestones.md` — карта фаз и сроки
- `CODE_STYLE.md` — конвенции кода

## Стек

- **Движок:** Godot 4.x (Compatibility renderer, GLES3)
- **Язык:** GDScript 2.0
- **Бэкенд:** Firebase (MVP) → AWS (prod)
- **3D-ассеты:** [3daistudio.com](https://www.3daistudio.com/) → Blender → Godot
- **Платформы:** iOS, Android

## Лицензия

MIT — см. `LICENSE`.
