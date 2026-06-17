#!/usr/bin/env bash
# Anime Empire — bootstrap script
#
# Проверяет наличие нужных инструментов и помогает их установить.
# Запуск: bash tools/bootstrap.sh  или  make bootstrap
#
# Цели:
#   - Godot 4.x  (godot или godot4 в PATH)
#   - git
#   - git-lfs (v2.13+)
#   - python 3.11+ + gdtoolkit
#   - (опц.) blender 4.x
#
# Скрипт не устанавливает агрессивно — предлагает команды для каждой ОС.

set -euo pipefail

# ─── Цвета ──────────────────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

ok()   { echo -e "${GREEN}✓${NC} $1"; }
warn() { echo -e "${YELLOW}⚠${NC} $1"; }
err()  { echo -e "${RED}✗${NC} $1"; }
info() { echo -e "${BLUE}ℹ${NC} $1"; }

# ─── Определение OS ─────────────────────────────────────────────────────
OS="$(uname -s)"
case "$OS" in
    Darwin*) PLATFORM="macos" ;;
    Linux*)  PLATFORM="linux" ;;
    MINGW*|MSYS*|CYGWIN*) PLATFORM="windows" ;;
    *) PLATFORM="unknown" ;;
esac

echo
echo "=== Anime Empire bootstrap ==="
echo "Platform: ${PLATFORM}"
echo

# ─── Состояние ─────────────────────────────────────────────────────────
HAS_ALL=true
MISSING=()

# ─── Проверки ─────────────────────────────────────────────────────────
check_cmd() {
    local cmd="$1"
    local label="$2"
    if command -v "$cmd" >/dev/null 2>&1; then
        local version
        version="$("$cmd" --version 2>&1 | head -1 || echo "?")"
        ok "${label}: ${version}"
        return 0
    else
        err "${label}: НЕ найден"
        MISSING+=("$label")
        HAS_ALL=false
        return 1
    fi
}

check_godot() {
    if command -v godot >/dev/null 2>&1; then
        local v
        v="$(godot --version 2>&1 | head -1 || echo "?")"
        if [[ "$v" == 4.* ]] || [[ "$v" == *"4."* ]]; then
            ok "Godot: ${v}"
            return 0
        else
            warn "Godot: найден, но возможно не 4.x: ${v}"
            return 0
        fi
    elif command -v godot4 >/dev/null 2>&1; then
        local v
        v="$(godot4 --version 2>&1 | head -1 || echo "?")"
        ok "Godot (как godot4): ${v}"
        info "  Псевдоним: export GODOT=godot4  (или добавь в Makefile)"
        return 0
    else
        err "Godot: НЕ найден"
        MISSING+=("Godot 4.x")
        HAS_ALL=false
        return 1
    fi
}

check_git_lfs() {
    if command -v git-lfs >/dev/null 2>&1; then
        local v
        v="$(git-lfs version 2>&1 | head -1 || echo "?")"
        ok "git-lfs: ${v}"
        # Проверим, что инициализирован для репо
        if git lfs status >/dev/null 2>&1; then
            ok "  git-lfs инициализирован в репо"
        else
            warn "  git-lfs установлен, но не инициализирован — запусти: git lfs install"
        fi
        return 0
    else
        err "git-lfs: НЕ найден"
        MISSING+=("git-lfs")
        HAS_ALL=false
        return 1
    fi
}

check_gdtoolkit() {
    if command -v gdlint >/dev/null 2>&1 && command -v gdformat >/dev/null 2>&1; then
        ok "gdtoolkit: установлен (gdlint, gdformat)"
        return 0
    else
        err "gdtoolkit: НЕ найден"
        MISSING+=("gdtoolkit")
        HAS_ALL=false
        return 1
    fi
}

echo "── Проверка инструментов ──"
check_cmd git "git"
check_git_lfs
check_godot
check_cmd python3 "Python 3"
check_gdtoolkit

echo
echo "── Опциональные ──"
if command -v blender >/dev/null 2>&1; then
    ok "Blender: $(blender --version 2>&1 | head -1)"
else
    warn "Blender: не найден (нужен для очистки 3D-ассетов; опционально)"
fi

if command -v make >/dev/null 2>&1; then
    ok "make: $(make --version 2>&1 | head -1)"
else
    warn "make: не найден (нужен для daily commands; опционально)"
fi

echo

# ─── Инструкции по установке отсутствующих ─────────────────────────────
if [[ "$HAS_ALL" == "true" ]]; then
    ok "Все обязательные инструменты установлены."
    echo
    info "Следующие шаги:"
    info "  1. git lfs pull           — подтянуть бинарные ассеты"
    info "  2. make import            — импорт Godot проекта"
    info "  3. make test              — запустить тесты"
    info "  4. godot godot/project.godot — открыть редактор"
    exit 0
fi

echo
err "Отсутствуют: ${MISSING[*]}"
echo
echo "── Установка ──"
case "$PLATFORM" in
    macos)
        echo "macOS (через Homebrew):"
        echo "  brew install git-lfs godot blender"
        echo "  pip3 install gdtoolkit==4.*"
        ;;
    linux)
        echo "Linux:"
        echo "  # git-lfs:"
        echo "  sudo apt install git-lfs  # Debian/Ubuntu"
        echo "  # Godot — скачать с https://godotengine.org/download/linux"
        echo "  pip3 install gdtoolkit==4.*"
        ;;
    windows)
        echo "Windows:"
        echo "  # git-lfs:"
        echo "  winget install GitHub.GitLFS  # или https://git-lfs.com"
        echo "  # Godot — скачать с https://godotengine.org/download/windows"
        echo "  pip install gdtoolkit==4.*"
        ;;
    *)
        echo "Установи вручную:"
        echo "  - Godot 4.x: https://godotengine.org/download"
        echo "  - Git LFS:   https://git-lfs.com"
        echo "  - gdtoolkit: pip install gdtoolkit==4.*"
        ;;
esac

echo
warn "После установки запусти bootstrap снова: make bootstrap"
exit 1
