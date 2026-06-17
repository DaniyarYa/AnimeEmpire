# 10 · Локализация

## Источник

`GDD/2_auditory.md` (целевые регионы), `GDD/9_uiux.md` (Tier-1/Tier-2 языки).

## Языки

### Tier-1 (на запуск)
- 🇺🇸 English (en)
- 🇪🇸 Spanish (es)
- 🇫🇷 French (fr)
- 🇩🇪 German (de)
- 🇯🇵 Japanese (ja)

### Tier-2 (post-launch)
- 🇵🇹 Portuguese (pt-BR)
- 🇷🇺 Russian (ru)
- 🇰🇷 Korean (ko)
- 🇨🇳 Simplified Chinese (zh-CN)

### Tier-3 (по запросу)
- Italian (it), Turkish (tr), Arabic (ar — нужен RTL), Indonesian (id)

## Технология

- Godot `TranslationServer` + `.po` (gettext) формат
- Один `.po` файл на язык
- Источник правды — `.csv` (Google Sheets) → конвертация в `.po` через `tools/loc/csv_to_po.py`

## Структура ключей

```
ui.<screen>.<element>           ui.shop.title
ui.<screen>.<button>            ui.shop.buy_button
modal.<name>.<field>            modal.upgrade.cost_label
event.<event_id>.<field>        event.farm_festival.title
notification.<id>               notification.daily_bonus
building.<id>.name              building.wheat_farm.name
building.<id>.description       building.wheat_farm.description
npc.<id>.name                   npc.gatherer_farmer.name
resource.<id>.name              resource.wheat.name
achievement.<id>.title          achievement.first_million.title
tutorial.step.<n>               tutorial.step.1
common.<word>                   common.gold
```

Dot-нотация → удобный поиск в IDE.

## Плюрализация

Godot `TranslationServer.translate_plural()`:
```
inventory.wheat.count {n} ↔ "{n} ear of wheat" / "{n} ears of wheat" (en)
                            "{n} колосок" / "{n} колоска" / "{n} колосков" (ru)
                            "小麦 {n}本" (ja)
```

## Форматирование чисел

Локаль-зависимо:
- en: `1,234,567.89`
- de: `1.234.567,89`
- ru: `1 234 567,89`
- ja: `1,234,567.89`

Свой wrapper `Locale.format_number(n)` использует `TranslationServer.get_locale()` + lookup в таблице.

Большие числа (idle-стиль) с суффиксами:
- en: `1K, 1M, 1B, 1T, 1aa, 1ab, ...`
- de: то же
- ru: `1К, 1М, 1Г, ...` или общепринятые сокращения `1K, 1M, 1B` (выбор по A/B)
- ja: `1万, 1億, 1兆, ...` (специфика)

## Валюта

Только локализованное отображение цен IAP:
- Store автоматически даёт price formatted (через native API)
- Используем это значение, не пересчитываем сами

## Шрифты

| Локали | Font | Размер источника |
|--------|------|-------------------|
| en, es, fr, de, pt, ru, it, tr | Nunito | 800 KB |
| ja | Noto Sans JP | 3 MB |
| ko | Noto Sans KR | 3 MB |
| zh-CN | Noto Sans SC | 4 MB |
| ar | Noto Sans Arabic | 1 MB |

Шрифты грузим по выбору локали, не все сразу.

## Длина текста

- Немецкий часто длиннее английского на 30-50%
- Русский — длиннее на 15-30%
- Японский — короче по символам, но широкие глифы
- Контейнеры — flexible, не фиксированные ширины

## RTL (Arabic)

- Mirror всего UI horizontal
- Tween-направления инвертируются
- Цифры остаются LTR (latin numerals)
- Поддержка — Phase post-launch (не MVP)

## Tutorial и lore

- Локализуется в первую очередь (от первого впечатления зависит retention)
- Native review для Tier-1 (минимум)
- Tier-2 — машинный перевод + native proof-reading

## Источники переводов

| Тип | Кто переводит |
|-----|---------------|
| MVP UI strings (~200) | Native фрилансеры (ProZ.com, Smartling) |
| Tutorial | Native, обязательно proof |
| Lore (post-launch) | Native + редактура |
| Hotfix UI | DeepL / GPT-4 машинно + review |

## Workflow

```
1. Разработчик добавляет ключ + en string в csv
2. CI auto-detect → создаёт issue для перевода
3. Translator получает CSV → возвращает все локали
4. CI генерирует .po → commit
5. Тестировщик проверяет на устройстве
```

## QA

- Каждая локаль — отдельный test pass (визуальный)
- Sanity tests: нет clip text, нет пустых ключей, нет «??» fallback
- Особо: длинные сейв-нотификации, попапы с числами

## Auto-detect локали

- При первом запуске — `OS.get_locale()` → выбираем
- Fallback на en, если язык не поддерживается
- Игрок может изменить в Settings

## Перевод имён собственных

- NPC имена — не переводим (Anya, Hiroshi и т.д.)
- Здания — переводим (Wheat Farm → Weizenfarm, 小麦農場)
- Бренды / меню (Anime Empire) — обычно остаётся в латинице

## Открытые вопросы

- [ ] Native voice для tutorial — приоритет?
- [ ] Translation memory tool (Crowdin, Lokalise) — когда переходим с CSV?
- [ ] RTL поддержка — целевая дата?
