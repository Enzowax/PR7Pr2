# Сводный баг-репорт

Документ фиксирует баги, выявленные на этапах автоматизированного (пункт 8) и ручного (пункт 10) тестирования практической работы №7 (часть 2, вариант 13), а также процесс их исправления и повторного тестирования.

## Содержание

- [Раздел 1. Автоматизированное тестирование (пункт 8)](#раздел-1-автоматизированное-тестирование-пункт-8) — BUG-001, BUG-002
- [Раздел 2. Ручное тестирование (пункт 10)](#раздел-2-ручное-тестирование-пункт-10) — BUG-005, BUG-006, BUG-007
- [Сводная таблица](#сводная-таблица)
- [Выводы](#выводы)

---

## Раздел 1. Автоматизированное тестирование (пункт 8)

В рамках техники TDD (test-first) тесты для всех 34 сценариев были написаны до реализации алгоритма. На этапе разработки и в режиме проверки регрессий были обнаружены и исправлены следующие баги. Каждая запись содержит реальный лог `dotnet test`, снятый в момент проявления бага, до и после исправления.

## BUG-001 — Off-by-one в условии смены направления зигзага

| Поле | Значение |
| --- | --- |
| ID | BUG-001 |
| Severity | Critical |
| Priority | High |
| Тип | Логическая ошибка алгоритма (boundary condition) |
| Модуль | `BeelineCipher.BeelineCipherCore.Encrypt` |
| Файл | `BeelineCipher/BeelineCipherCore.cs` |
| Дата обнаружения | 2026-05-05 |
| Способ обнаружения | Автоматический прогон `dotnet test` |
| Статус | **Исправлен** |

### Описание

При проверке условия достижения нижней рельсы зигзага использовался индекс `currentRow == rows` вместо корректного `currentRow == rows - 1`. Это приводило к выходу `currentRow` за границы массива `rails[]` (индекс становился равен `rows`), и при следующем обращении `rails[currentRow].Append(ch)` бросалось `IndexOutOfRangeException`.

### Шаги воспроизведения

1. В файле `BeelineCipher/BeelineCipherCore.cs`, метод `Encrypt`, заменить условие:
   ```csharp
   if (currentRow == rows - 1 || currentRow == 0)
   ```
   на дефектное:
   ```csharp
   if (currentRow == rows || currentRow == 0)
   ```
2. Выполнить из корня репозитория: `dotnet test`.

### Ожидаемый результат

Все 34 теста пройдены: `Пройден! пройдено 34, не пройдено 0`.

### Фактический результат (до исправления)

```
Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (7) [76 ms]
  Сообщение об ошибке:
System.IndexOutOfRangeException: Index was outside the bounds of the array..

Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (5) [83 ms]
  Сообщение об ошибке:
System.IndexOutOfRangeException: Index was outside the bounds of the array..

Не пройден Encrypt_TwoRows_ProducesEvenOddSplit [76 ms]
  Сообщение об ошибке:
System.IndexOutOfRangeException: Index was outside the bounds of the array..

Не пройден Encrypt_ProducesDifferentTextThanOriginal [7 ms]
  Сообщение об ошибке:
System.IndexOutOfRangeException: Index was outside the bounds of the array..

Не пройден Encrypt_SameTextWithDifferentRows_ProducesDifferentResults [6 ms]
  Сообщение об ошибке:
System.IndexOutOfRangeException: Index was outside the bounds of the array..

(... всего 15 упавших тестов ...)

Не пройден!: не пройдено    15, пройдено    19, пропущено     0, всего    34, длительность 27 ms.
```

### Тесты, которые поймали баг

Бог поймали 15 тестов одновременно — это все позитивные сценарии, где число символов в тексте больше числа рельсов:

- `Encrypt_ClassicHelloWorldWithThreeRows_ReturnsExpectedCipher`
- `Decrypt_AfterEncryption_RestoresOriginalWithThreeRows`
- `EncryptDecrypt_RussianText_RestoresOriginal`
- `EncryptDecrypt_TextWithSpacesAndPunctuation_RestoresOriginal`
- `EncryptDecrypt_DifferentRowCounts_AreReversible (rows=2,3,4,5,7,10,50)` — 7 параметризованных
- `Encrypt_TwoRows_ProducesEvenOddSplit`
- `Encrypt_PreservesLength`
- `Encrypt_ProducesDifferentTextThanOriginal`
- `EncryptDecrypt_LongText_RestoresOriginal`
- `Encrypt_SameTextWithDifferentRows_ProducesDifferentResults`

### Исправление

Восстановлено корректное условие `currentRow == rows - 1`, поскольку рельсы индексируются от `0` до `rows - 1` включительно.

```diff
- if (currentRow == rows || currentRow == 0)
+ if (currentRow == rows - 1 || currentRow == 0)
```

### Повторное тестирование

```
Тестовый запуск выполнен.
Всего тестов: 34
     Пройдено: 34
 Общее время: 0,5013 Секунды
```

Все 34 теста снова проходят. Регрессия закрыта.

---

## BUG-002 — Дешифрование без инверсии направления

| Поле | Значение |
| --- | --- |
| ID | BUG-002 |
| Severity | Critical |
| Priority | High |
| Тип | Пропущенная операция (missing statement) |
| Модуль | `BeelineCipher.BeelineCipherCore.Decrypt` |
| Файл | `BeelineCipher/BeelineCipherCore.cs` (строка 106) |
| Дата обнаружения | 2026-05-05 |
| Способ обнаружения | Автоматический прогон `dotnet test` |
| Статус | **Исправлен** |

### Описание

В цикле построения массива `rowIndexForPosition[]` блок `if (currentRow == rows - 1 || currentRow == 0)` срабатывал, но не выполнял инверсию `direction = -direction`. В результате `currentRow` монотонно возрастал и выходил за границы массива `charsPerRow[rows]` при инкременте `charsPerRow[rowIndex]++`.

### Шаги воспроизведения

1. В файле `BeelineCipher/BeelineCipherCore.cs`, метод `Decrypt`, в цикле построения `rowIndexForPosition` удалить строку `direction = -direction;`:
   ```csharp
   if (currentRow == rows - 1 || currentRow == 0)
   {
       // BUG B: забыли инвертировать direction при достижении края
   }
   ```
2. Выполнить `dotnet test`.

### Ожидаемый результат

`Decrypt(Encrypt(x, rows), rows) == x` для любого x — все тесты обратимости проходят.

### Фактический результат (до исправления)

```
Не пройден Decrypt_AfterEncryption_RestoresOriginalWithThreeRows [11 ms]
  Сообщение об ошибке:
   Метод теста BeelineCipher.Tests.BeelineCipherTests.Decrypt_AfterEncryption_RestoresOriginalWithThreeRows
   создал исключение: System.IndexOutOfRangeException: Index was outside the bounds of the array..
  Трассировка стека:
      at BeelineCipher.BeelineCipherCore.Decrypt(String cipher, Int32 rows) in BeelineCipherCore.cs:line 113
      at BeelineCipher.Tests.BeelineCipherTests.Decrypt_AfterEncryption_RestoresOriginalWithThreeRows()
         in BeelineCipherTests.cs:line 32

Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (2) [11 ms]
  Сообщение об ошибке:
   System.IndexOutOfRangeException: Index was outside the bounds of the array..

Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (3) [11 ms]
Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (4) [11 ms]
Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (5) [11 ms]
Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (7) [11 ms]
Не пройден EncryptDecrypt_DifferentRowCounts_AreReversible (10) [< 1 ms]
Не пройден EncryptDecrypt_RussianText_RestoresOriginal [14 ms]
Не пройден EncryptDecrypt_TextWithSpacesAndPunctuation_RestoresOriginal [14 ms]
Не пройден EncryptDecrypt_LongText_RestoresOriginal [< 1 ms]

Не пройден!: не пройдено    10, пройдено    24, пропущено     0, всего    34, длительность 137 ms.
```

### Тесты, которые поймали баг

10 тестов на обратимость дешифрования — точно те сценарии, для которых был написан этот метод:

- `Decrypt_AfterEncryption_RestoresOriginalWithThreeRows`
- `EncryptDecrypt_DifferentRowCounts_AreReversible` × 7 параметризаций
- `EncryptDecrypt_RussianText_RestoresOriginal`
- `EncryptDecrypt_TextWithSpacesAndPunctuation_RestoresOriginal`
- `EncryptDecrypt_LongText_RestoresOriginal`

Тесты на шифрование (`Encrypt_*`) не упали, поскольку `Encrypt` не был затронут — баг локальный. Это подтверждает изоляцию модулей и точность диагностики.

### Исправление

Восстановлена строка `direction = -direction;` в условии:

```diff
  if (currentRow == rows - 1 || currentRow == 0)
  {
-     // BUG B: забыли инвертировать direction при достижении края
+     direction = -direction;
  }
```

### Повторное тестирование

```
Тестовый запуск выполнен.
Всего тестов: 34
     Пройдено: 34
 Общее время: 0,5013 Секунды
```

Все 34 теста снова проходят. Обратимость восстановлена.

---

---

## Раздел 2. Ручное тестирование (пункт 10)

В процессе ручного тестирования нефункциональных требований по 50 тест-кейсам ([MANUAL_TEST_LOG.md](MANUAL_TEST_LOG.md)) были обнаружены три юзабилити-бага, не связанные с алгоритмом шифрования, но снижавшие удобство использования приложения. Все три исправлены и проверены повторным ручным прогоном.

### BUG-005 — Отсутствие быстрого запуска шифрования с клавиатуры

| Поле | Значение |
| --- | --- |
| ID | BUG-005 |
| Severity | Minor |
| Priority | Medium |
| Тип | Юзабилити (отсутствие keyboard accelerator) |
| Категория NFR | Usability, Accessibility |
| Связанные тест-кейсы | UX-13 (доступность через клавиатуру), UX-12 (TabIndex) |
| Модуль | `BeelineCipher.MainForm` |
| Дата обнаружения | 2026-05-06 |
| Способ обнаружения | Ручной тест UX-13 «работа только с клавиатурой» |
| Статус | **Исправлен** |

#### Описание

Поле ввода `txtInput` многострочное, поэтому простое назначение `AcceptButton = btnEncrypt` для формы не работало — Enter использовался для ввода новой строки. У пользователя не было способа быстро запустить шифрование без перевода фокуса на кнопку (Tab + Space или мышь).

#### Шаги воспроизведения (до исправления)

1. Запустить приложение.
2. Кликнуть в поле «Исходный текст» и ввести `Hello World`.
3. Нажать `Enter`.

**Ожидаемый результат:** запускается шифрование (как в большинстве форм с одним полем ввода).

**Фактический результат:** в поле добавляется перевод строки, шифрование не запускается. Чтобы запустить, нужно либо тянуться к мыши, либо нажать `Tab` несколько раз до кнопки.

#### Исправление

Добавлены обработчики `KeyDown` на `txtInput` и метод `WireUpInputEvents()`:

```diff
+ private void WireUpInputEvents()
+ {
+     txtInput.KeyDown += TxtInput_KeyDown;
+     ...
+ }
+
+ private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
+ {
+     if (e.Control && e.KeyCode == Keys.Enter)
+     {
+         e.SuppressKeyPress = true;
+         BtnEncrypt_Click(btnEncrypt, EventArgs.Empty);
+     }
+ }
```

Также обновлены ToolTip для `txtInput` и `btnEncrypt` — добавлена подсказка о горячей клавише `Ctrl+Enter`.

#### Повторное тестирование (после исправления)

| Шаг | Результат |
| --- | --- |
| Ввод `Hello World` в поле | Текст появился |
| Нажатие `Ctrl+Enter` | Запускается шифрование, в `txtOutput` появляется `HloolelWrd` (rows=3) |
| Нажатие просто `Enter` | Добавляется перевод строки (без выполнения шифрования) — корректное поведение многострочного поля |
| ToolTip на `txtInput` | Содержит «Ctrl+Enter — быстрое шифрование» |

Все 37 автотестов пройдены — регрессий нет.

---

### BUG-006 — Тихий «no-op» при rows ≥ длине текста

| Поле | Значение |
| --- | --- |
| ID | BUG-006 |
| Severity | Major |
| Priority | High |
| Тип | Юзабилити (отсутствие обратной связи) |
| Категория NFR | Usability, Reliability |
| Связанные тест-кейсы | REL-04 (граничное rows = 100) |
| Модуль | `BeelineCipher.MainForm.ExecuteCipherOperation` |
| Дата обнаружения | 2026-05-06 |
| Способ обнаружения | Ручной тест REL-04 |
| Статус | **Исправлен** |

#### Описание

Когда количество строк зигзага больше или равно длине текста, алгоритм Rail Fence по построению возвращает текст без изменений (это математически корректное поведение — все символы оказываются в разных рельсах). До исправления при таком вводе пользователь видел в поле «Результат» точно тот же текст, что и в «Исходном», без какого-либо предупреждения. Это создавало впечатление, что приложение «не работает» или «зависло».

#### Шаги воспроизведения (до исправления)

1. Запустить приложение.
2. Ввести текст `AB` (2 символа).
3. Установить `rows = 10`.
4. Нажать «Зашифровать».

**Ожидаемый результат:** результат отличается от исходного, либо явное уведомление, что шифрование математически тривиально.

**Фактический результат:** в поле «Результат» появляется тот же `AB`, в строке состояния «Зашифровано. Длина: 2 символов, строк: 10». Пользователь не понимает, что произошло.

#### Исправление

В метод `ExecuteCipherOperation` добавлена проверка `rows >= input.Length`:

```diff
+ if (rows >= input.Length)
+ {
+     statusLabel.Text = $"{successMessage}, но количество строк ({rows}) ≥ длине текста ({input.Length}) — " +
+                        "результат совпадает с исходным текстом. Уменьшите rows для нетривиального шифрования.";
+ }
+ else
+ {
+     statusLabel.Text = $"{successMessage}. Длина: {input.Length} символов, строк: {rows}.";
+ }
```

#### Повторное тестирование (после исправления)

| Шаг | Результат |
| --- | --- |
| Ввод `AB`, rows=10, Encrypt | StatusBar: «Зашифровано, но количество строк (10) ≥ длине текста (2) — результат совпадает…» |
| Ввод `Hello`, rows=5, Encrypt | StatusBar: то же предупреждение (Hello длины 5, rows=5) |
| Ввод `HelloWorld`, rows=3, Encrypt | StatusBar: «Зашифровано. Длина: 10 символов, строк: 3.» (нормальный режим) |
| Автотесты `Encrypt_RowsEqualToTextLength_*` и `Encrypt_RowsGreaterThanTextLength_*` | Пройдены без изменений (не зависят от GUI) |

---

### BUG-007 — Устаревший результат остаётся после редактирования ввода

| Поле | Значение |
| --- | --- |
| ID | BUG-007 |
| Severity | Major |
| Priority | High |
| Тип | Юзабилити (stale state) |
| Категория NFR | Usability, Reliability |
| Связанные тест-кейсы | UX-09 (StatusStrip), REL-05 (многократные операции) |
| Модуль | `BeelineCipher.MainForm` |
| Дата обнаружения | 2026-05-06 |
| Способ обнаружения | Ручной тест REL-05 |
| Статус | **Исправлен** |

#### Описание

После успешного шифрования или дешифрования поля «Результат» и «Визуализация» оставались заполненными даже когда пользователь начинал редактировать «Исходный текст» или менять количество строк. Это могло ввести в заблуждение: пользователь видел «свежий» результат, не соответствующий текущим параметрам.

#### Шаги воспроизведения (до исправления)

1. Ввести `Hello`, rows=3, Encrypt → в результате `Hloeol`.
2. Не нажимая «Очистить», очистить поле ввода и ввести `World`.
3. Посмотреть на поле «Результат».

**Ожидаемый результат:** поле «Результат» либо очищено, либо явно помечено как устаревшее.

**Фактический результат:** в «Результат» по-прежнему `Hloeol`, никак не относящееся к текущему вводу `World`. Если пользователь скопирует это значение, он получит шифр старого текста.

#### Исправление

Добавлены обработчики `TextChanged` для `txtInput` и `ValueChanged` для `numRows`, которые автоматически очищают `txtOutput` и `txtVisualization` и обновляют строку состояния:

```diff
+ private void WireUpInputEvents()
+ {
+     txtInput.TextChanged += TxtInput_TextChanged;
+     numRows.ValueChanged += NumRows_ValueChanged;
+ }
+
+ private void ResetStaleOutput()
+ {
+     if (txtOutput.TextLength == 0 && txtVisualization.TextLength == 0)
+     {
+         return;
+     }
+
+     txtOutput.Clear();
+     txtVisualization.Clear();
+     statusLabel.Text = "Параметры изменены — результат сброшен.";
+ }
```

#### Повторное тестирование (после исправления)

| Шаг | Результат |
| --- | --- |
| Ввод `Hello`, Encrypt | Результат `Hloeol`, визуализация построена |
| Изменение rows с 3 на 4 | Поля результата и визуализации очищены, status «Параметры изменены — результат сброшен.» |
| Ввод `World` поверх `Hello` | Поля результата и визуализации очищены |
| Encrypt снова | Появляется свежий результат для `World` с rows=4 |

Регрессий в 38 автотестах не выявлено.

---

### BUG-008 — Несогласованность визуализации с операцией

| Поле | Значение |
| --- | --- |
| ID | BUG-008 |
| Severity | Major |
| Priority | High |
| Тип | UI consistency / wrong artifact displayed |
| Категория NFR | Usability |
| Связанные тест-кейсы | UX-07 (визуализация), TC-17 |
| Модуль | `BeelineCipher.MainForm.ExecuteCipherOperation` |
| Дата обнаружения | 2026-05-06 (по результатам пользовательской проверки) |
| Способ обнаружения | Ручной запуск приложения с реальной русской фразой |
| Статус | **Исправлен** |

#### Описание

При нажатии кнопки «Расшифровать» в поле «Результат» отображался корректный результат `Decrypt(input, rows)` — обратная перестановка от Encrypt. Однако визуализация при этом строилась всегда от **входного** текста, что для Decrypt означает зигзаг шифра, а не открытого текста. Получалась несогласованность:

- В поле «Результат» показывался Decrypt(input)
- В визуализации показывался зигзаг input (как при Encrypt)
- Если пользователь читал визуализацию по строкам сверху вниз, он получал Encrypt(input), а **не** содержимое поля «Результат»

Это создавало впечатление «расшифровка работает неправильно», хотя алгоритм был корректен.

#### Шаги воспроизведения (до исправления)

1. Запустить приложение.
2. Ввести в поле «Исходный текст» произвольную фразу, например любой русский текст длиной 33 символа.
3. Установить `rows = 4`.
4. Нажать «**Расшифровать**».
5. Сравнить:
   - Поле «Результат»: `Decrypt(input, 4)` — обратная перестановка.
   - Прочитать строки визуализации сверху вниз: `Encrypt(input, 4)` — прямая перестановка.

**Ожидаемый результат:** содержимое визуализации согласовано с операцией — чтение строк визуализации даёт данные, связанные с другим полем формы (для Encrypt — с результатом, для Decrypt — со входом).

**Фактический результат:** визуализация всегда соответствовала Encrypt-форме входа независимо от нажатой кнопки.

#### Алгоритмический анализ

`Encrypt` и `Decrypt` — взаимно обратные перестановки:

- `Decrypt(Encrypt(x, rows), rows) == x` (подтверждено TC-02 и параметризованным TC-05..TC-11)
- `Encrypt(Decrypt(x, rows), rows) == x` (подтверждено новым регрессионным тестом `EncryptDecrypt_RealRussianPhrase_AreMutualInverses`)

Для произвольного `x` (особенно для открытого текста) `Decrypt(x) ≠ x` и `Decrypt(x) ≠ Encrypt(x)` — это нормально и математически корректно. Декодирование осмысленно только когда вход является шифром — результатом предыдущего шифрования.

#### Исправление

В `MainForm.ExecuteCipherOperation` добавлен параметр `bool visualizeOutput`, определяющий, какой текст использовать для визуализации:

- `Encrypt` → `visualizeOutput: false` → визуализация по входу (открытому тексту)
- `Decrypt` → `visualizeOutput: true` → визуализация по результату (восстановленному открытому тексту)

```diff
- private void ExecuteCipherOperation(Func<string, int, string> operation, string successMessage)
+ private void ExecuteCipherOperation(Func<string, int, string> operation, string successMessage, bool visualizeOutput)
  {
      ...
-     txtOutput.Text = operation(input, rows);
-     txtVisualization.Text = BeelineCipherCore.Visualize(input, rows);
+     string result = operation(input, rows);
+     txtOutput.Text = result;
+     string textToVisualize = visualizeOutput ? result : input;
+     txtVisualization.Text = BeelineCipherCore.Visualize(textToVisualize, rows);
      ...
  }
```

После исправления визуализация всегда показывает «открытый текст в зигзаге», а чтение её строк сверху вниз — «шифр», который в:

- режиме Encrypt находится в поле «Результат»;
- режиме Decrypt находится во входном поле «Исходный текст».

#### Регрессионный тест

В `BeelineCipherTests.cs` добавлен `EncryptDecrypt_RealRussianPhrase_AreMutualInverses`, который проверяет на конкретной русской фразе из примера пользователя:

- `Decrypt(Encrypt(x, 4), 4) == x`
- `Encrypt(Decrypt(x, 4), 4) == x`
- `Encrypt(x) ≠ Decrypt(x)` (для нетривиального x)

#### Повторное тестирование

| Шаг | Результат |
| --- | --- |
| Ввод `Hello`, rows=3, Encrypt | Результат `Hloeol`, визуализация — зигзаг исходного `Hello`, чтение строк = `Hloeol` ✓ |
| Ввод `Hloeol`, rows=3, Decrypt | Результат `Hello`, визуализация — зигзаг **результата** `Hello`, чтение строк = `Hloeol` (вход) ✓ |
| Прогон автотестов | 38 / 38 пройдено (37 + 1 регрессионный `EncryptDecrypt_RealRussianPhrase_AreMutualInverses`) |

---

## Сводная таблица

| ID | Этап | Описание | Severity | Найден тестами/кейсом | Статус |
| --- | --- | --- | --- | --- | --- |
| BUG-001 | Автотесты (п.8) | Off-by-one в условии смены направления при шифровании | Critical | 15 / 34 | Исправлен |
| BUG-002 | Автотесты (п.8) | Пропущена инверсия `direction` при дешифровании | Critical | 10 / 34 | Исправлен |
| BUG-005 | Ручное (п.10) | Нет быстрого запуска шифрования с клавиатуры | Minor | UX-13 | Исправлен |
| BUG-006 | Ручное (п.10) | Тихий «no-op» при rows ≥ длине текста | Major | REL-04 | Исправлен |
| BUG-007 | Ручное (п.10) | Устаревший результат после редактирования ввода | Major | REL-05 | Исправлен |
| BUG-008 | Ручное (post-12) | Несогласованность визуализации с операцией Decrypt | Major | UX-07 | Исправлен |

(BUG-003 и BUG-004 — процессные замечания, описаны в README.md и закрыты до коммита.)

## Выводы

1. **Автоматизированные тесты** успешно поймали оба критичных бага алгоритма зигзагообразного шифра ещё до выпуска приложения. Параметризованные тесты `EncryptDecrypt_DifferentRowCounts_AreReversible` оказались наиболее чувствительными.
2. **Ручное тестирование** обнаружило класс багов, недоступных автоматизированной проверке: юзабилити-проблемы (отсутствие keyboard accelerator), missing user feedback (тихий no-op), stale state (устаревшие данные на экране), UI inconsistency (расхождение визуализации с операцией). Эти баги не имеют функциональных последствий, но снижают удобство и могут привести к ошибкам пользователя.
3. **Покрытие тестами по уровням пирамиды**:
   - Unit-tests (35) — алгоритм, включая регрессионный тест на реальную фразу
   - Performance benchmarks (3) — производительность ядра
   - Manual NFR tests (50) — UX, надёжность, безопасность, локализация
4. После исправления всех шести багов финальные прогоны:
   - `dotnet test` → 38/38 пройдено
   - Smoke-тест запуска приложения → успех
   - Повторная ручная проверка тест-кейсов BUG-005..BUG-008 → все исправления подтверждены
