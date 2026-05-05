# Отчёт по автоматизированному тестированию

Документ фиксирует результаты прогона автоматизированных тестов для пункта 7 практической работы.

## Сводный результат

| Метрика | Значение |
| --- | --- |
| Тестовый фреймворк | MSTest 3.6.4 |
| Версия VSTest | 17.12.0 (x64) |
| Целевая платформа | net9.0-windows |
| Сборка | `BeelineCipher.Tests.dll` (Debug) |
| Параллелизация | 16 рабочих ролей, область MethodLevel |
| Всего тестов | **34** |
| Пройдено | **34** |
| Не пройдено | **0** |
| Пропущено | **0** |
| Общее время | **0,5013 с** |
| Файл результатов | [test-results/TestResults.trx](test-results/TestResults.trx) |

## Дата и среда запуска

| Параметр | Значение |
| --- | --- |
| Дата прогона | 2026-05-05 |
| Команда | `dotnet test --logger "trx;LogFileName=TestResults.trx"` |
| ОС | Windows 11 Pro 10.0.26200 |
| .NET SDK | 9.0.102 |

## Детальные результаты по тестам

| # | Имя теста | Время |
| --- | --- | --- |
| 1 | Encrypt_ClassicHelloWorldWithThreeRows_ReturnsExpectedCipher | 50 ms |
| 2 | Decrypt_AfterEncryption_RestoresOriginalWithThreeRows | 62 ms |
| 3 | EncryptDecrypt_RussianText_RestoresOriginal | 62 ms |
| 4 | EncryptDecrypt_TextWithSpacesAndPunctuation_RestoresOriginal | 62 ms |
| 5 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=2) | 59 ms |
| 6 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=3) | 59 ms |
| 7 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=4) | 59 ms |
| 8 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=5) | 59 ms |
| 9 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=7) | 59 ms |
| 10 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=10) | 50 ms |
| 11 | EncryptDecrypt_DifferentRowCounts_AreReversible (rows=50) | 50 ms |
| 12 | Encrypt_TwoRows_ProducesEvenOddSplit | 50 ms |
| 13 | Encrypt_RowsEqualToTextLength_ReturnsTextUnchanged | 62 ms |
| 14 | Encrypt_RowsGreaterThanTextLength_ReturnsTextUnchanged | 50 ms |
| 15 | Encrypt_EmptyString_ReturnsEmptyString | 50 ms |
| 16 | Decrypt_EmptyString_ReturnsEmptyString | < 1 ms |
| 17 | Encrypt_NullText_ThrowsArgumentNullException | < 1 ms |
| 18 | Decrypt_NullText_ThrowsArgumentNullException | < 1 ms |
| 19 | Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=0) | < 1 ms |
| 20 | Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=1) | < 1 ms |
| 21 | Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=-3) | < 1 ms |
| 22 | Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=101) | < 1 ms |
| 23 | Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=1000) | < 1 ms |
| 24 | Decrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=0) | < 1 ms |
| 25 | Decrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=1) | < 1 ms |
| 26 | Decrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=-1) | < 1 ms |
| 27 | Decrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException (rows=101) | < 1 ms |
| 28 | Encrypt_PreservesLength | < 1 ms |
| 29 | Encrypt_ProducesDifferentTextThanOriginal | < 1 ms |
| 30 | Encrypt_SameTextWithDifferentRows_ProducesDifferentResults | < 1 ms |
| 31 | EncryptDecrypt_LongText_RestoresOriginal | < 1 ms |
| 32 | Visualize_ThreeRowsHelloWorld_ReturnsExpectedZigzagGrid | < 1 ms |
| 33 | Visualize_EmptyString_ReturnsEmptyString | < 1 ms |
| 34 | Visualize_ContainsAllOriginalCharacters | < 1 ms |

## Распределение тестов по категориям

| Категория | Количество | % покрытия задач варианта |
| --- | --- | --- |
| Позитивные функциональные | 12 | основной алгоритм Rail Fence |
| Параметризованные (data-driven) | 7 | проверка разных rows (2,3,4,5,7,10,50) — требование «настраиваемое количество строк» |
| Краевые случаи | 5 | пустые строки, rows == длине, rows > длины |
| Негативные (исключения) | 7 | null-аргументы и rows вне допустимого диапазона |
| Визуализация | 3 | требование «визуализация заполнения» |
| **Итого** | **34** | |

## Покрытие требований варианта 13

| Требование | Покрывающие тесты | Статус |
| --- | --- | --- |
| Настраиваемое количество строк (2..100) | TC-05..TC-11, TC-25..TC-29 | ✅ |
| Чтение зигзагом | TC-01, TC-32 | ✅ |
| Тесты с разным количеством строк | TC-05..TC-11 | ✅ |
| Визуализация заполнения | TC-32, TC-33, TC-34 | ✅ |

## Команда для повторного запуска

```bash
# из корня репозитория
dotnet test --logger "trx;LogFileName=TestResults.trx" --results-directory ./docs/test-results
```

или через Visual Studio: **Test → Run All Tests** (Ctrl+R, A).

## Скриншот «Обозревателя тестов»

Скриншот окна Test Explorer Visual Studio с результатом «34 пройдено / 0 не пройдено» сохраняется в файл [test_explorer.png](test_explorer.png).

Шаги для снятия скриншота:

1. Открыть `BeelineCipher.sln` в Visual Studio 2022.
2. Меню **Test → Test Explorer** (Ctrl+E, T).
3. Кнопка **Run All Tests In View** (или Ctrl+R, V).
4. Дождаться появления зелёных галочек у всех 34 тестов.
5. Win+Shift+S — снять область с окном Test Explorer.
6. Сохранить как `docs/test_explorer.png` и закоммитить.

## Сырой TRX-отчёт

Машинно-читаемый XML-отчёт о прогоне доступен в [test-results/TestResults.trx](test-results/TestResults.trx). TRX поддерживается Visual Studio (открывается двойным щелчком), Azure DevOps, Jenkins.
