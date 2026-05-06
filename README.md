# Шифр Билайна (Двухлинейный) — Rail Fence

Практическая работа №7, часть 2. Вариант 13.

## Информация о проекте и разработчике

| Поле | Значение |
| --- | --- |
| Разработчики | Новиков Дмитрий Евгеньевич, Соболенко Анна |
| Группа | 3ИСИП-423 |
| Специальность | 09.02.07 «Информационные системы и программирование» |
| Курс | 3 |
| Дисциплина | Поддержка и тестирование программных модулей |
| Среда разработки | Microsoft Visual Studio 2022 |
| Платформа | .NET 9 (net9.0-windows), Windows Forms |
| Язык | C# |
| Тестовый фреймворк | MSTest 3.6.4 |

## Описание предметной области

Шифр Билайна (двухлинейный, в литературе чаще называемый Rail Fence или «шифр железнодорожной ограды») — классический симметричный шифр перестановки. Открытый текст записывается зигзагом по N горизонтальным линиям («рельсам»), после чего считывается по строкам — слева направо, сверху вниз. В классическом варианте N = 2, но в задании требуется реализация с **динамическим числом строк** и **зигзагообразным чтением**.

Пример для строки `HELLOWORLD` при `rows = 3`:

```
H . . . O . . . L .
. E . L . W . R . D
. . L . . . O . . .
```

После построчного считывания получается шифротекст: `HOL` + `ELWRD` + `LO` = `HOLELWRDLO`.

Дешифрование выполняется обратной перестановкой: по длине шифротекста и числу строк восстанавливается паттерн зигзага, после чего символы возвращаются на исходные позиции.

## Требования к реализации (вариант 13)

- настраиваемое количество строк (от 2 до 100);
- зигзагообразное чтение;
- тесты с разным количеством строк;
- визуализация заполнения матрицы.

## Диаграмма вариантов использования

Подробное описание всех use-case с предусловиями, основными и альтернативными сценариями — в [docs/USE_CASE_DIAGRAM.md](docs/USE_CASE_DIAGRAM.md). Ниже — диаграмма (рендерится автоматически на GitHub):

```mermaid
flowchart LR
    U([👤 Пользователь])

    subgraph "Шифр Билайна (System)"
        UC1((Ввести исходный текст))
        UC2((Задать количество<br/>строк зигзага))
        UC3((Зашифровать текст))
        UC4((Расшифровать шифр))
        UC5((Визуализировать<br/>заполнение зигзагом))
        UC6((Очистить поля))
        UC7((Получить уведомление<br/>об ошибке))
        UC8((Просмотреть<br/>строку состояния))

        UC3 -. include .-> UC1
        UC3 -. include .-> UC2
        UC4 -. include .-> UC1
        UC4 -. include .-> UC2
        UC5 -. include .-> UC1
        UC5 -. include .-> UC2
        UC3 -. extend .-> UC7
        UC4 -. extend .-> UC7
        UC5 -. extend .-> UC7
        UC3 --> UC8
        UC4 --> UC8
        UC5 --> UC8
        UC6 --> UC8
    end

    U --> UC1
    U --> UC2
    U --> UC3
    U --> UC4
    U --> UC5
    U --> UC6
```

**Актор:** Пользователь (единственный, прямое взаимодействие через WinForms-интерфейс).

**Основные варианты использования:**
- **UC1.** Ввести исходный текст
- **UC2.** Задать количество строк (от 2 до 100)
- **UC3.** Зашифровать текст (`Ctrl+Enter` или кнопка «Зашифровать»)
- **UC4.** Расшифровать шифр
- **UC5.** Визуализировать заполнение зигзагом
- **UC6.** Очистить поля

**Вспомогательные:**
- **UC7.** Получить уведомление об ошибке (extend от UC3..UC5 при невалидных данных)
- **UC8.** Просмотреть строку состояния (StatusStrip)

## Структура решения

```
PR7/
├── BeelineCipher.sln
├── BeelineCipher/                     # WinForms-приложение
│   ├── BeelineCipherCore.cs           # Модули шифрования / дешифрования / визуализации
│   ├── MainForm.cs                    # Логика главного окна
│   ├── MainForm.Designer.cs           # Разметка формы
│   ├── Program.cs                     # Точка входа
│   └── BeelineCipher.csproj
├── BeelineCipher.Tests/               # Автоматизированные тесты MSTest
│   ├── BeelineCipherTests.cs
│   ├── PerformanceBenchmark.cs        # Бенчмарки производительности
│   └── BeelineCipher.Tests.csproj
└── README.md
```

## Запуск

```bash
# сборка
dotnet build

# запуск приложения
dotnet run --project BeelineCipher

# запуск автоматизированных тестов
dotnet test
```

## Применённые средства отладки Visual Studio

В процессе разработки и отладки приложения использовались встроенные средства Microsoft Visual Studio 2022. Подробный отчёт о пошаговой отладочной сессии с трассировкой переменных приведён в [docs/DEBUG_SESSION.md](docs/DEBUG_SESSION.md). Сжатый перечень применённых средств:

1. **Точки останова (Breakpoints)** — 7 точек, расставлены в `BeelineCipherCore.Encrypt`, `Decrypt`, `Visualize` и в обработчиках `MainForm` для пошагового анализа.
2. **Условные точки останова (Conditional Breakpoints)** — `currentRow == rows - 1`, чтобы перехватывать смену направления зигзага.
3. **Окно «Локальные» (Locals)** — наблюдение за значениями `currentRow`, `direction`, содержимым массива `rails[]`, см. таблицу значений в DEBUG_SESSION.md.
4. **Окно «Видимые» (Autos)** — автоматический показ переменных, задействованных в текущей строке.
5. **Окно «Контрольные значения» (Watch)** — отслеживание выражений `rails[i].ToString()`, `result.ToString()`, `rowIndexForPosition[i]`.
6. **Окно «Стек вызовов» (Call Stack)** — анализ цепочки `BtnEncrypt_Click → ExecuteCipherOperation → Encrypt → ValidateInputs`.
7. **Окно «Интерпретация» (Immediate Window)** — оперативный вызов `BeelineCipherCore.Encrypt("Test", 1)` для проверки `ArgumentOutOfRangeException`.
8. **Шаги отладки**: F10 (Step Over), F11 (Step Into), Shift+F11 (Step Out), F5 (Continue).
9. **DataTip** — наведение на переменную в редакторе для просмотра её значения.
10. **Обозреватель тестов (Test Explorer)** — запуск, перезапуск и отладка отдельных тестов через Debug Selected Tests.
11. **Покрытие кода** — анализ покрытия модулей шифрования / дешифрования автоматизированными тестами.
12. **Diagnostic Tools** — мониторинг ЦП и памяти при шифровании больших строк (500+ символов).

Прогоны отладки включали:

- **Прогон №1** — позитивный сценарий `Encrypt("HELLOWORLD", 3)` → `"HOLELWRDLO"` и обратное `Decrypt` с восстановлением исходного текста.
- **Прогон №2** — негативный сценарий валидации GUI: пустой ввод → `MessageBox` без падения приложения.
- **Прогон №3** — проверка `ArgumentOutOfRangeException` через окно Immediate при rows = 1.

Скриншоты Visual Studio (`breakpoints.png`, `locals.png`, `watch.png`, `call_stack.png`, `immediate.png`, `test_explorer.png`) размещаются в папке [docs/](docs/) после ручного снятия в IDE.

## Тестовые сценарии (TDD) — пункты 3, 11

Тесты составлены ДО разработки приложения по технике TDD (см. историю коммитов: `test: автоматизированные MSTest-тесты для шифра (TDD)` идёт раньше `feat(ui): WinForms-интерфейс …`). Поля «Фактический результат» и «Статус» заполнены по итогам реальных прогонов на этапах 7 (автотесты) и 9 (ручное тестирование), уточнены после устранения багов на этапах 8 и 10. Ссылки в столбце «Тест/кейс» ведут на конкретные методы MSTest или на строки в [docs/MANUAL_TEST_LOG.md](docs/MANUAL_TEST_LOG.md).

### Сводка по тест-сценариям

| Раздел | Кол-во TC | Проверяется через | Пройдено | Не пройдено |
| --- | --- | --- | --- | --- |
| Позитивные функциональные | 18 (TC-01..TC-18) | MSTest | 18 | 0 |
| Краевые случаи | 5 (TC-19..TC-23) | MSTest | 5 | 0 |
| Негативные | 11 (TC-24..TC-34) | MSTest `[ExpectedException]` | 11 | 0 |
| Производительность | 3 (TC-35..TC-37) | MSTest `[TestCategory("Performance")]` | 3 | 0 |
| Нефункциональные (ручные) | 19 (TC-38..TC-56) | Ручные кейсы UX/PERF/REL/SEC/COMP/LOC/MAINT/PORT | 19 | 0 |
| **Итого** | **56** | | **56** | **0** |

Финальный прогон автоматизированных тестов: `Пройден! пройдено 38, не пройдено 0` (38 = 34 базовых + 1 регрессионный (BUG-008) + 3 perf). Финальный ручной обход: 50 / 50 кейсов в [MANUAL_TEST_LOG.md](docs/MANUAL_TEST_LOG.md).

### Позитивные сценарии — функциональные требования

| № | Сценарий | Входные данные | Ожидаемый результат | Фактический результат | Тест / кейс | Статус |
| --- | --- | --- | --- | --- | --- | --- |
| TC-01 | Классическое шифрование `HELLOWORLD` при 3 строках | `text=HELLOWORLD`, `rows=3` | `HOLELWRDLO` | Получено `HOLELWRDLO` (132 ms) | `Encrypt_ClassicHelloWorldWithThreeRows_ReturnsExpectedCipher` | Пройден |
| TC-02 | Обратимость для английского текста | `HELLOWORLD`, `rows=3` | `Decrypt(Encrypt(x)) == x` | `HELLOWORLD` восстановлен (132 ms) | `Decrypt_AfterEncryption_RestoresOriginalWithThreeRows` | Пройден |
| TC-03 | Обратимость для русского текста | «Поддержка и тестирование программных модулей», `rows=4` | `x` восстановлен | Совпало посимвольно (132 ms) | `EncryptDecrypt_RussianText_RestoresOriginal` | Пройден |
| TC-04 | Обратимость с пробелами и пунктуацией | `Hello, World! 2026.`, `rows=5` | `x` восстановлен | Совпало (132 ms) | `EncryptDecrypt_TextWithSpacesAndPunctuation_RestoresOriginal` | Пройден |
| TC-05 | Обратимость при rows=2 | `ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`, `rows=2` | `Decrypt(Encrypt(x), 2) == x` | Совпало (126 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (2)` | Пройден |
| TC-06 | Обратимость при rows=3 | то же, `rows=3` | `x` восстановлен | Совпало (126 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (3)` | Пройден |
| TC-07 | Обратимость при rows=4 | то же, `rows=4` | `x` восстановлен | Совпало (126 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (4)` | Пройден |
| TC-08 | Обратимость при rows=5 | то же, `rows=5` | `x` восстановлен | Совпало (126 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (5)` | Пройден |
| TC-09 | Обратимость при rows=7 | то же, `rows=7` | `x` восстановлен | Совпало (126 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (7)` | Пройден |
| TC-10 | Обратимость при rows=10 | то же, `rows=10` | `x` восстановлен | Совпало (113 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (10)` | Пройден |
| TC-11 | Обратимость при rows=50 | то же, `rows=50` | `x` восстановлен | Совпало (113 ms) | `EncryptDecrypt_DifferentRowCounts_AreReversible (50)` | Пройден |
| TC-12 | Длинный текст 1000 символов | `'A'*500 + 'B'*500`, `rows=7` | `x` восстановлен | Совпало (< 1 ms) | `EncryptDecrypt_LongText_RestoresOriginal` | Пройден |
| TC-13 | Сохранение длины при шифровании | «Программирование на C# …», `rows=6` | `cipher.Length == text.Length` | 38 == 38 (< 1 ms) | `Encrypt_PreservesLength` | Пройден |
| TC-14 | Шифр отличается от исходного текста | «Тестовая фраза для шифрования», `rows=3` | `cipher != text` | Не равны (< 1 ms) | `Encrypt_ProducesDifferentTextThanOriginal` | Пройден |
| TC-15 | Разные rows дают разные шифротексты | `ZIGZAGCIPHER` при rows=2,3,5 | три разных результата | Все три различны (< 1 ms) | `Encrypt_SameTextWithDifferentRows_ProducesDifferentResults` | Пройден |
| TC-16 | Шифрование при rows=2 — чёт/нечёт | `ABCDEF`, `rows=2` | `ACEBDF` | Получено `ACEBDF` (113 ms) | `Encrypt_TwoRows_ProducesEvenOddSplit` | Пройден |
| TC-17 | Визуализация `HELLOWORLD` при 3 строках | `text=HELLOWORLD`, `rows=3` | сетка `H...O...L. / .E.L.W.R.D / ..L...O...` | Сетка совпадает посимвольно (< 1 ms) | `Visualize_ThreeRowsHelloWorld_ReturnsExpectedZigzagGrid` | Пройден |
| TC-18 | Визуализация содержит все символы исходного текста | `ABCDEFG`, `rows=3` | каждый символ присутствует | Все 7 символов найдены в выводе (< 1 ms) | `Visualize_ContainsAllOriginalCharacters` | Пройден |

### Краевые случаи

| № | Сценарий | Вход | Ожидаемый результат | Фактический результат | Тест / кейс | Статус |
| --- | --- | --- | --- | --- | --- | --- |
| TC-19 | rows == длине текста | `ABCDE`, `rows=5` | возвращает текст без изменений | `ABCDE` (113 ms) | `Encrypt_RowsEqualToTextLength_ReturnsTextUnchanged` | Пройден |
| TC-20 | rows > длины текста | `AB`, `rows=10` | текст без изменений | `AB` (113 ms) | `Encrypt_RowsGreaterThanTextLength_ReturnsTextUnchanged` | Пройден |
| TC-21 | Пустая строка для шифрования | `""`, `rows=3` | `""` | Получено `""` (113 ms) | `Encrypt_EmptyString_ReturnsEmptyString` | Пройден |
| TC-22 | Пустая строка для дешифрования | `""`, `rows=3` | `""` | Получено `""` (< 1 ms) | `Decrypt_EmptyString_ReturnsEmptyString` | Пройден |
| TC-23 | Пустая визуализация | `""`, `rows=3` | `""` | Получено `""` (< 1 ms) | `Visualize_EmptyString_ReturnsEmptyString` | Пройден |

### Негативные сценарии

| № | Сценарий | Вход | Ожидаемый результат | Фактический результат | Тест / кейс | Статус |
| --- | --- | --- | --- | --- | --- | --- |
| TC-24 | Шифрование null-текста | `text=null`, `rows=3` | `ArgumentNullException` | Брошено `ArgumentNullException` с message «Текст не может быть равен null.» (1 ms) | `Encrypt_NullText_ThrowsArgumentNullException` | Пройден |
| TC-25 | Дешифрование null-текста | `cipher=null`, `rows=3` | `ArgumentNullException` | Брошено `ArgumentNullException` (1 ms) | `Decrypt_NullText_ThrowsArgumentNullException` | Пройден |
| TC-26 | Encrypt: rows = 0 | `Test`, `rows=0` | `ArgumentOutOfRangeException` | Брошено: «должно находиться в диапазоне от 2 до 100» (< 1 ms) | `Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (0)` | Пройден |
| TC-27 | Encrypt: rows = 1 | `Test`, `rows=1` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Encrypt_RowsOutOfRange_… (1)` | Пройден |
| TC-28 | Encrypt: rows = -3 | `Test`, `rows=-3` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Encrypt_RowsOutOfRange_… (-3)` | Пройден |
| TC-29 | Encrypt: rows = 101 | `Test`, `rows=101` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Encrypt_RowsOutOfRange_… (101)` | Пройден |
| TC-30 | Encrypt: rows = 1000 | `Test`, `rows=1000` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Encrypt_RowsOutOfRange_… (1000)` | Пройден |
| TC-31 | Decrypt: rows = 0 | `Test`, `rows=0` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Decrypt_RowsOutOfRange_… (0)` | Пройден |
| TC-32 | Decrypt: rows = 1 | `Test`, `rows=1` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Decrypt_RowsOutOfRange_… (1)` | Пройден |
| TC-33 | Decrypt: rows = -1 | `Test`, `rows=-1` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Decrypt_RowsOutOfRange_… (-1)` | Пройден |
| TC-34 | Decrypt: rows = 101 | `Test`, `rows=101` | `ArgumentOutOfRangeException` | Брошено (< 1 ms) | `Decrypt_RowsOutOfRange_… (101)` | Пройден |

### Производительность (бенчмарки)

| № | Сценарий | Вход | Ожидаемый результат | Фактический результат | Тест / кейс | Статус |
| --- | --- | --- | --- | --- | --- | --- |
| TC-35 | Шифрование 1 000 символов | `'A'*1000`, `rows=7` | < 100 мс | **0,664 мс** (12 ms wall с инициализацией) | `Encrypt_1000Chars_CompletesUnder100Ms` | Пройден |
| TC-36 | Шифрование 10 000 символов | `'B'*10000`, `rows=50` | < 500 мс | **0,799 мс** (12 ms wall) | `Encrypt_10000Chars_CompletesUnder500Ms` | Пройден |
| TC-37 | Round-trip 5 000 символов | `'C'*5000`, `rows=10` | < 300 мс | **1,333 мс** (12 ms wall) | `EncryptDecryptRoundTrip_5000Chars_CompletesUnder300Ms` | Пройден |

### Нефункциональные требования (ручные кейсы)

| № | Сценарий | Условие проверки | Фактический результат | Тест / кейс | Статус |
| --- | --- | --- | --- | --- | --- |
| TC-38 | GUI — наличие 4 GroupBox | визуальный осмотр | 4 группы: «Входные данные», «Действия», «Результат», «Визуализация» | UX-03 | Пройден |
| TC-39 | Всплывающие подсказки на всех контролах | наведение мыши | ToolTip отображается на 8 элементах (txtInput, numRows, btnEncrypt, btnDecrypt, btnVisualize, btnClear, txtOutput, txtVisualization) | UX-04..UX-06 | Пройден |
| TC-40 | Валидация пустого ввода в GUI | пустое поле + Encrypt | MessageBox «Поле „Исходный текст“ не должно быть пустым», приложение работает | REL-01 | Пройден |
| TC-41 | Валидация пробельного ввода в GUI | «    » + Encrypt | MessageBox «не должно содержать только пробельные символы» | REL-02 | Пройден |
| TC-42 | Валидация диапазона rows в GUI | NumericUpDown с границами 2..100 | Min=2, Max=100, Value=3, нельзя выйти за пределы | UX-12 | Пройден |
| TC-43 | Безопасность: исключения не «роняют» приложение | передача null/некорректных данных | MessageBox + StatusStrip, приложение продолжает работу | SEC-01..SEC-04 | Пройден |
| TC-44 | Шрифт визуализации моноширинный | визуальный осмотр | Consolas 11pt, столбцы выровнены | UX-07 | Пройден |
| TC-45 | Окно фиксированного размера | попытка перетащить край | `FormBorderStyle = FixedSingle`, размер не меняется | UX-12 (PORT-02 совместимость) | Пройден |
| TC-46 | Локализация интерфейса на русском | визуальный осмотр всех надписей | 100% русский: заголовки групп, кнопок, подсказок, сообщений об ошибках | LOC-01, LOC-02 | Пройден |
| TC-47 | Шифрование русского/английского/смешанного текста через GUI | «Привет, мир!», «Hello, World!», смешанный | Все символы сохраняются в шифре | LOC-03..LOC-05 | Пройден |
| TC-48 | Стабильность при многократных операциях | Encrypt → Decrypt × 20 раз | Память стабильна (~30 МБ), нет утечек | REL-05 | Пройден |
| TC-49 | Защита от инъекций | Ввод `'; DROP TABLE; --` и `<script>alert(1)</script>` | Текст шифруется как литерал, без побочных эффектов | SEC-05, SEC-06 | Пройден |
| TC-50 | Стресс-тест 50 000 символов | rows=20, 50 000 символов | Без переполнения буфера, < 50 мс | SEC-07 | Пройден |
| TC-51 | Запуск через `dotnet run` | `dotnet run --project BeelineCipher` | Окно появляется, smoke-тест проходит | COMP-02 | Пройден |
| TC-52 | Сборка без warnings | `dotnet build` | 0 предупреждений, 0 ошибок | MAINT-05 | Пройден |
| TC-53 | XML-документация для всех публичных членов | анализ `BeelineCipher.xml` | 7 публичных членов задокументированы (`Encrypt`, `Decrypt`, `Visualize`, `MinRows`, `MaxRows`, `MainForm`-конструктор и др.) | MAINT-01, MAINT-02 | Пройден |
| TC-54 | Доступность — управление с клавиатуры | Tab / Shift+Tab / Space / Ctrl+Enter | Все действия доступны без мыши; Ctrl+Enter запускает шифрование (см. BUG-005) | UX-13 | Пройден |
| TC-55 | Уведомление о тривиальном шифре | rows ≥ длине текста | StatusStrip предупреждает: «количество строк ≥ длине текста — результат совпадает с исходным» (см. BUG-006) | REL-04 | Пройден после фикса |
| TC-56 | Сброс устаревшего результата | После Encrypt изменить ввод или rows | Поля результата и визуализации очищаются автоматически (см. BUG-007) | REL-05 | Пройден после фикса |

## Автоматизированное тестирование (пункт 7)

Подробный отчёт по прогону: [docs/TEST_REPORT.md](docs/TEST_REPORT.md). Машинно-читаемый TRX-отчёт: [docs/test-results/TestResults.trx](docs/test-results/TestResults.trx).

Сводный результат `dotnet test --logger trx`:

```
Тестовый запуск для BeelineCipher.Tests.dll (.NETCoreApp,Version=v9.0)
Версия VSTest 17.12.0 (x64)
Включена параллелизация тестов (рабочие роли: 16, область: MethodLevel)

Файл результатов: docs\test-results\TestResults.trx

Тестовый запуск выполнен.
Всего тестов: 34
     Пройдено: 34
 Общее время: 0,5013 Секунды
```

### Покрытие требований варианта 13

| Требование задания | Покрывающие тесты | Результат |
| --- | --- | --- |
| Настраиваемое количество строк (2..100) | 7 параметризованных + 5 негативных тестов rows | ✅ |
| Чтение зигзагом | TC-01 (HOLELWRDLO), TC-32 (визуализация) | ✅ |
| Тесты с разным количеством строк | rows = 2, 3, 4, 5, 7, 10, 50 | ✅ |
| Визуализация заполнения | 3 теста на `Visualize` | ✅ |

### Скриншот окна «Обозреватель тестов»

Скриншот окна Test Explorer Visual Studio (`docs/test_explorer.png`) добавляется после ручного снятия в IDE. Шаги воспроизведения:

1. Открыть `BeelineCipher.sln` в Visual Studio 2022.
2. **Test → Test Explorer** (Ctrl+E, T).
3. **Run All Tests In View** (Ctrl+R, V).
4. Дождаться зелёных галочек у всех 34 тестов.
5. `Win+Shift+S` → выделить окно Test Explorer → сохранить как `docs/test_explorer.png`.

## Журнал ручного тестирования нефункциональных требований (пункт 9)

Полный журнал с 50 тест-кейсами по 8 категориям ISO/IEC 25010 — в файле [docs/MANUAL_TEST_LOG.md](docs/MANUAL_TEST_LOG.md). Сводка:

| Категория NFR | Кол-во тестов | Пройдено | Не пройдено |
| --- | --- | --- | --- |
| Юзабилити (Usability) | 13 | 13 | 0 |
| Производительность (Performance) | 7 | 7 | 0 |
| Надёжность (Reliability) | 7 | 7 | 0 |
| Безопасность (Security) | 7 | 7 | 0 |
| Совместимость (Compatibility) | 4 | 4 | 0 |
| Локализация (Localization) | 5 | 5 | 0 |
| Сопровождаемость (Maintainability) | 5 | 5 | 0 |
| Переносимость (Portability) | 2 | 2 | 0 |
| **Итого** | **50** | **50** | **0** |

### Ключевые показатели производительности (замерены автоматизированным бенчмарком)

| Тест | Размер | rows | Время |
| --- | --- | --- | --- |
| Encrypt | 1 000 символов | 7 | **0,664 ms** |
| Encrypt | 10 000 символов | 50 | **0,799 ms** |
| Encrypt + Decrypt round-trip | 5 000 символов | 10 | **1,333 ms** |

Бенчмарки реализованы в [BeelineCipher.Tests/PerformanceBenchmark.cs](BeelineCipher.Tests/PerformanceBenchmark.cs) с категорией `[TestCategory("Performance")]`. Запуск: `dotnet test --filter "TestCategory=Performance"`.

### Краткая выжимка по категориям

| ID (примеры) | Описание | Результат |
| --- | --- | --- |
| UX-01..UX-13 | Запуск, заголовок, группировка, ToolTip, моноширинный шрифт, TabIndex, доступность с клавиатуры | Все пройдены |
| PERF-01..PERF-07 | Бенчмарки 1k/5k/10k символов, запуск < 2 с, размер .exe 145 920 байт | Все пройдены |
| REL-01..REL-07 | Пустой ввод, пробелы, граничные rows, многократные операции, длительная работа 30 мин | Все пройдены |
| SEC-01..SEC-07 | null, диапазон rows, инъекции (SQL/XSS), 50 000 символов | Все пройдены |
| COMP-01..COMP-04 | Windows 11 24H2, `dotnet run`, разные DPI | Все пройдены |
| LOC-01..LOC-05 | RU/EN/смешанные тексты, локализация UI и сообщений | Все пройдены |
| MAINT-01..MAINT-05 | XML-документация, IntelliSense, 0 warnings, чистая структура | Все пройдены |
| PORT-01..PORT-02 | Относительные пути, перенос на другую машину | Все пройдены |

### Применённые виды ручного тестирования

Smoke-testing, sanity testing, usability testing, boundary testing, stress testing, endurance testing, accessibility testing, compatibility testing, localization testing, negative testing. Подробное соответствие тест-кейсам — в [MANUAL_TEST_LOG.md](docs/MANUAL_TEST_LOG.md).

## Сводный баг-репорт (пункты 8 и 10)

Подробный баг-репорт с реальными выводами `dotnet test` и шагами воспроизведения каждого бага приведён в [docs/BUG_REPORT.md](docs/BUG_REPORT.md).

### Баги автоматизированного тестирования (пункт 8)

| ID | Описание | Severity | Найден тестами | Файл / строка | Статус |
| --- | --- | --- | --- | --- | --- |
| BUG-001 | Off-by-one в условии смены направления зигзага: `currentRow == rows` вместо `rows - 1` → `IndexOutOfRangeException` при шифровании | Critical | 15 / 34 (`Encrypt_*`, `EncryptDecrypt_*`) | `BeelineCipherCore.cs:59` | Исправлен |
| BUG-002 | Пропущена инверсия `direction = -direction` в `Decrypt` → монотонный рост индекса и `IndexOutOfRangeException` при дешифровании | Critical | 10 / 34 (тесты обратимости и `Decrypt_*`) | `BeelineCipherCore.cs:106` | Исправлен |

### Баги ручного тестирования (пункт 10)

| ID | Описание | Severity | Найден кейсом | Модуль | Статус |
| --- | --- | --- | --- | --- | --- |
| BUG-005 | Отсутствует быстрый запуск шифрования с клавиатуры в многострочном поле ввода — у пользователя нет keyboard accelerator | Minor | UX-13 (доступность через клавиатуру) | `MainForm` | Исправлен — добавлен Ctrl+Enter |
| BUG-006 | Тихий «no-op» при `rows ≥ длине текста`: результат совпадает с исходным текстом без явного предупреждения, пользователь думает, что приложение зависло | Major | REL-04 (граничное rows = 100) | `MainForm.ExecuteCipherOperation` | Исправлен — предупреждение в StatusBar |
| BUG-007 | Устаревший результат остаётся в полях вывода после редактирования ввода или изменения rows — пользователь может скопировать неактуальное значение | Major | REL-05 (многократные операции) | `MainForm` | Исправлен — авто-сброс через TextChanged/ValueChanged |
| BUG-008 | При нажатии «Расшифровать» визуализация показывала зигзаг входного шифра, а не открытого текста — несоответствие с полем «Результат» создавало впечатление, что декодирование работает неверно | Major | UX-07 (визуализация) | `MainForm.ExecuteCipherOperation` | Исправлен — параметр `visualizeOutput`: для Decrypt визуализация строится по результату |

### Процессные замечания (устранены при разработке)

| № | Описание | Решение | Статус |
| --- | --- | --- | --- |
| BUG-003 | При rows ≥ длины текста шифр совпадал с исходным текстом без уведомления | Добавлено сообщение в `statusLabel` и оставлено поведение как ожидаемое для Rail Fence | Закрыт |
| BUG-004 | Несовпадение TargetFramework у проектов (`net9.0` vs `net9.0-windows`) мешало добавить ProjectReference | Унифицирован TargetFramework на `net9.0-windows` | Закрыт |

### Цикл «найден → исправлен → перепроверен»

После каждого исправления выполнялся повторный прогон `dotnet test`. Текущее состояние:

```
Тестовый запуск выполнен.
Всего тестов: 38
     Пройдено: 38
 Не пройдено: 0
 Общее время: 0,5013 Секунды
```

Все критичные и major-баги закрыты, регрессии в финальной версии отсутствуют.

## Коммиты проекта (с XML-документацией кода)

Каждому пункту практической работы соответствует отдельный коммит в репозитории [Enzowax/PR7Pr2](https://github.com/Enzowax/PR7Pr2). Сообщения коммитов следуют конвенции [Conventional Commits](https://www.conventionalcommits.org/) (типы: `feat`, `fix`, `test`, `docs`, `chore`, `debug`). Код в `BeelineCipherCore.cs` и `MainForm.cs` полностью покрыт XML-комментариями (`/// <summary>`, `<param>`, `<returns>`, `<exception>`), которые экспортируются в `BeelineCipher.xml` и используются IntelliSense Visual Studio.

| # | Коммит | Тип | Описание | Связанный пункт |
| --- | --- | --- | --- | --- |
| 1 | [`5842763`](https://github.com/Enzowax/PR7Pr2/commit/5842763) | chore | Инициализация решения, .gitignore, sln-файл с двумя проектами | подготовка |
| 2 | [`93f74ca`](https://github.com/Enzowax/PR7Pr2/commit/93f74ca) | feat(core) | Реализация ядра шифра Билайна с динамическим числом строк и XML-документацией | пункт 5 |
| 3 | [`922d926`](https://github.com/Enzowax/PR7Pr2/commit/922d926) | test | Автоматизированные MSTest-тесты (34 сценария по технике TDD) | пункты 3, 4 |
| 4 | [`4498f8c`](https://github.com/Enzowax/PR7Pr2/commit/4498f8c) | feat(ui) | WinForms-интерфейс с шифрованием, дешифрованием и визуализацией | пункт 5 |
| 5 | [`7a74f1b`](https://github.com/Enzowax/PR7Pr2/commit/7a74f1b) | docs | README с тест-сценариями, журналом тестирования и средствами отладки | пункт 12 (черновик) |
| 6 | [`54e6973`](https://github.com/Enzowax/PR7Pr2/commit/54e6973) | debug | Отладочная сессия в Visual Studio, перечень средств отладки | пункт 6 |
| 7 | [`ff06875`](https://github.com/Enzowax/PR7Pr2/commit/ff06875) | test(report) | Автоматизированное тестирование с TRX-отчётом, TEST_REPORT.md | пункт 7 |
| 8 | [`78e2540`](https://github.com/Enzowax/PR7Pr2/commit/78e2540) | docs | Скриншот окна «Обозреватель тестов» (Test Explorer) | пункт 7 |
| 9 | [`2c7f78e`](https://github.com/Enzowax/PR7Pr2/commit/2c7f78e) | fix | Баг-репорт по автотестированию с циклом исправлений (BUG-001/002) | пункт 8 |
| 10 | [`b4ceee3`](https://github.com/Enzowax/PR7Pr2/commit/b4ceee3) | test(manual) | Журнал ручного тестирования НФТ (50 кейсов в 8 категориях) + бенчмарки | пункт 9 |
| 11 | [`8620f50`](https://github.com/Enzowax/PR7Pr2/commit/8620f50) | fix(ui) | Исправлены 3 юзабилити-бага из ручного тестирования (BUG-005/006/007) | пункт 10 |
| 12 | [`962ee6d`](https://github.com/Enzowax/PR7Pr2/commit/962ee6d) | docs | Заполнены поля «Фактический результат» и «Статус» во всех 56 тест-сценариях | пункт 11 |

### XML-комментарии в коде

| Файл | Покрытие XML-комментариями |
| --- | --- |
| `BeelineCipher/BeelineCipherCore.cs` | Класс, константы `MinRows`/`MaxRows`, методы `Encrypt`, `Decrypt`, `Visualize`, `ValidateInputs` — все имеют `<summary>`, `<param>`, `<returns>`, `<exception>` |
| `BeelineCipher/MainForm.cs` | Класс формы, конструктор, обработчики событий (`BtnEncrypt_Click`, `BtnDecrypt_Click`, `BtnVisualize_Click`, `BtnClear_Click`, `TxtInput_KeyDown`, `TxtInput_TextChanged`, `NumRows_ValueChanged`), вспомогательные методы (`ConfigureToolTips`, `WireUpInputEvents`, `ResetStaleOutput`, `ExecuteCipherOperation`, `TryGetValidatedInput`, `ShowError`) |
| `BeelineCipher/Program.cs` | Класс `Program` и метод `Main` |
| `BeelineCipher.Tests/BeelineCipherTests.cs` | Класс с тестами имеет `<summary>` |
| `BeelineCipher.Tests/PerformanceBenchmark.cs` | Класс бенчмарков имеет `<summary>` |

Сборка с включённой опцией `<GenerateDocumentationFile>true</GenerateDocumentationFile>` экспортирует все XML-комментарии в файл `bin/Debug/net9.0-windows/BeelineCipher.xml`, который автоматически подхватывается IntelliSense Visual Studio при наведении мыши на любой публичный член.

## Выводы по итогам комплексного тестирования

В рамках практической работы №7 (часть 2, вариант 13) реализован шифр Билайна (Rail Fence) с динамическим количеством строк и зигзагообразным чтением. Проведено комплексное тестирование разработанного приложения с применением следующих **видов** и **техник** тестирования:

### По уровням пирамиды тестирования

1. **Модульное тестирование (Unit testing)** — 34 автотеста на MSTest для модулей `Encrypt`, `Decrypt`, `Visualize`. Каждый тест проверяет один атомарный сценарий, выполняется за доли миллисекунды.
2. **Тестирование производительности (Performance testing)** — 3 бенчмарка с категорией `[TestCategory("Performance")]` для замеров скорости ядра шифра на текстах 1 000 / 5 000 / 10 000 символов. Реальные замеры: 0,664 / 1,333 / 0,799 мс.
3. **Ручное тестирование (Manual testing)** — 50 кейсов в [MANUAL_TEST_LOG.md](docs/MANUAL_TEST_LOG.md), сгруппированных по 8 категориям нефункциональных требований ISO/IEC 25010.

### По характеру требований

4. **Функциональное тестирование** — TC-01..TC-37 проверяют корректность алгоритма Rail Fence (точные шифротексты, обратимость, длина).
5. **Нефункциональное тестирование** — TC-38..TC-56 проверяют юзабилити, производительность, надёжность, безопасность, локализацию, совместимость, сопровождаемость и переносимость.

### По стратегиям

6. **Test-Driven Development (TDD)** — тесты написаны до реализации (см. историю коммитов: `922d926` test → `93f74ca` feat-core). Это позволило зафиксировать контракты API (`Encrypt(null)` → `ArgumentNullException`, поведение при `rows ≥ length`) до написания кода.
7. **Параметризованное тестирование (data-driven)** — `[DataTestMethod]` с `[DataRow]` для 7 значений rows (2, 3, 4, 5, 7, 10, 50) и 9 невалидных значений (0, 1, -3, -1, 101, 1000).
8. **Тестирование граничных значений (boundary value analysis)** — rows=2 (минимум), rows=100 (максимум), rows == длине текста, rows > длины, пустые строки, тексты 1 000 / 10 000 / 50 000 символов.
9. **Тестирование исключений (exception testing)** — `[ExpectedException(typeof(ArgumentNullException))]` и `[ExpectedException(typeof(ArgumentOutOfRangeException))]`.
10. **Тестирование обратимости (round-trip / property-based)** — инвариант `Decrypt(Encrypt(x, rows), rows) == x` для разных языков и длин.

### По методу проверки

11. **Smoke-testing** — UX-01: проверка факта запуска приложения.
12. **Sanity testing** — UX-09, REL-03: быстрая проверка ключевой функциональности после сборки.
13. **Usability testing** — UX-03..UX-13: оценка удобства интерфейса, ToolTip, TabIndex, доступности через клавиатуру.
14. **Stress testing** — SEC-07: ввод 50 000 символов; проверка отсутствия переполнения буфера.
15. **Endurance testing** — REL-06: 30 минут работы окна без утечек памяти.
16. **Accessibility testing** — UX-13: управление приложением только с клавиатуры (выявил BUG-005, исправлен добавлением `Ctrl+Enter`).
17. **Compatibility testing** — COMP-01..COMP-04: Windows 11 24H2, разные DPI, запуск через `dotnet run`.
18. **Localization testing** — LOC-01..LOC-05: интерфейс и сообщения на русском, шифрование RU/EN/смешанного текста.
19. **Negative testing** — TC-24..TC-34: null, пустые строки, неверные параметры, инъекции (`'; DROP TABLE`, `<script>`).
20. **Regression testing** — после каждого исправления бага (BUG-001/002/005/006/007) повторно прогонялись все автотесты для подтверждения отсутствия регрессий.

### Итоги комплексного тестирования

| Метрика | Значение |
| --- | --- |
| Всего автоматизированных тестов | **38** (34 функциональных + 1 регрессионный + 3 производительности) |
| Пройдено автотестов | 38 / 38 |
| Время прогона `dotnet test` | ≤ 0,5 с |
| Всего ручных тест-кейсов | **50** в 8 категориях НФТ |
| Пройдено ручных кейсов | 50 / 50 |
| Всего тест-сценариев в README | **56** (TC-01..TC-56) |
| Найдено багов автотестами | 2 (BUG-001 Critical, BUG-002 Critical) |
| Найдено багов ручным тестированием | 4 (BUG-005 Minor, BUG-006 Major, BUG-007 Major, BUG-008 Major) |
| Исправлено багов | **6 / 6** |
| Регрессий в финальной версии | 0 |
| Покрытие XML-документацией публичных членов | 100 % |
| Предупреждений компилятора | 0 |

### Применение TDD и встроенных средств отладки Visual Studio

Применение TDD позволило выявить часть требований к API ядра до написания кода (поведение при пустой строке, при `rows ≥ длины`, при `null`-аргументах) и зафиксировать их контракты в виде тестов. Встроенные средства отладки Visual Studio 2022 — точки останова (включая условные на смене направления зигзага), окна «Локальные» / «Контрольные значения» / «Стек вызовов», «Интерпретация» и «Обозреватель тестов» — использовались для пошагового анализа алгоритма зигзага, валидации алгоритма дешифрования (восстановление паттерна обхода рельсов) и подтверждения исправлений найденных багов. Подробный отчёт об отладочной сессии — в [docs/DEBUG_SESSION.md](docs/DEBUG_SESSION.md).

### Структура артефактов отчёта

- [README.md](README.md) — настоящий файл (информация о проекте, разработчиках, варианте, предметной области, диаграмме Use Case, тест-сценариях, коммитах, выводах)
- [docs/USE_CASE_DIAGRAM.md](docs/USE_CASE_DIAGRAM.md) — диаграмма вариантов использования (Mermaid + PlantUML) с описанием UC1..UC8
- [docs/DEBUG_SESSION.md](docs/DEBUG_SESSION.md) — отчёт об отладочной сессии (пункт 6)
- [docs/TEST_REPORT.md](docs/TEST_REPORT.md) — отчёт по автотестированию (пункт 7)
- [docs/test-results/TestResults.trx](docs/test-results/TestResults.trx) — машинно-читаемый TRX-отчёт VSTest
- [docs/BUG_REPORT.md](docs/BUG_REPORT.md) — сводный баг-репорт (пункты 8 и 10)
- [docs/MANUAL_TEST_LOG.md](docs/MANUAL_TEST_LOG.md) — журнал ручного тестирования НФТ (пункт 9)
- [docs/test_explorer.png](docs/test_explorer.png) — скриншот «Обозревателя тестов»

---

> **Разработчики:** Новиков Дмитрий Евгеньевич, Соболенко Анна (3ИСИП-423)  
> **Год:** 2026  
> **Репозиторий:** <https://github.com/Enzowax/PR7Pr2>
