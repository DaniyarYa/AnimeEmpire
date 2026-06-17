# Anime Empire — задачи разработки
#
# Использование:
#   make help        — список команд
#   make test        — запуск unit-тестов
#   make lint        — линт GDScript
#   make format      — авто-форматирование
#   make import      — Godot --headless --import (для CI)
#   make bootstrap   — установка инструментов (tools/bootstrap.sh)

GODOT ?= godot
GODOT_PROJECT := godot

.PHONY: help test lint format import bootstrap clean

help:
	@echo "Anime Empire — make targets:"
	@echo "  test         Запустить unit-тесты"
	@echo "  lint         Линт GDScript через gdlint"
	@echo "  format       Авто-форматирование gdformat"
	@echo "  format-check Проверка форматирования без правок"
	@echo "  import       Headless импорт проекта (для CI)"
	@echo "  bootstrap    Установка инструментов разработки"
	@echo "  clean        Удалить временные файлы Godot"
	@echo ""
	@echo "Переменные:"
	@echo "  GODOT=path/to/godot   (default: godot)"

test:
	@echo "Running tests..."
	$(GODOT) --headless --path $(GODOT_PROJECT) --script res://tests/test_runner.gd

lint:
	@echo "Running gdlint..."
	gdlint $(GODOT_PROJECT) --exclude=$(GODOT_PROJECT)/addons

format:
	@echo "Running gdformat..."
	gdformat $(GODOT_PROJECT)

format-check:
	@echo "Checking gdformat..."
	gdformat --check $(GODOT_PROJECT)

import:
	@echo "Importing Godot project..."
	$(GODOT) --headless --path $(GODOT_PROJECT) --import

bootstrap:
	@bash tools/bootstrap.sh

clean:
	@echo "Cleaning Godot temporary files..."
	rm -rf $(GODOT_PROJECT)/.godot/imported
	rm -rf $(GODOT_PROJECT)/.godot/editor
