# Отчёт об отладочной сессии

Документ фиксирует пошаговое прохождение приложения отладчиком Microsoft Visual Studio 2022 со снятием значений переменных в каждой контрольной точке. Используется для подтверждения пункта 6 практической работы №7 (часть 2).

## Конфигурация запуска

| Параметр | Значение |
| --- | --- |
| Решение | `BeelineCipher.sln` |
| Стартовый проект | `BeelineCipher` |
| Конфигурация | `Debug` |
| Платформа | `Any CPU` (net9.0-windows) |
| Тип запуска | F5 — Start Debugging |

## Точки останова (Breakpoints)

| № | Файл | Строка | Условие | Назначение |
| --- | --- | --- | --- | --- |
| BP-1 | `BeelineCipherCore.cs` | `Encrypt` — строка с `var rails = new StringBuilder[rows];` | без условия | Старт алгоритма шифрования, проверка валидации |
| BP-2 | `BeelineCipherCore.cs` | `Encrypt` — внутри `foreach (char ch in text)` | без условия | Заполнение рельсов символами |
| BP-3 | `BeelineCipherCore.cs` | `Encrypt` — `if (currentRow == rows - 1 \|\| currentRow == 0)` | условная: `currentRow == rows - 1` | Перехват смены направления зигзага |
| BP-4 | `BeelineCipherCore.cs` | `Decrypt` — после `int[] charsPerRow = new int[rows];` | без условия | Проверка распределения символов по строкам |
| BP-5 | `BeelineCipherCore.cs` | `Decrypt` — внутри финального `for` | без условия | Сборка результата дешифрования |
| BP-6 | `MainForm.cs` | `BtnEncrypt_Click` | без условия | Точка входа GUI-обработчика |
| BP-7 | `MainForm.cs` | `TryGetValidatedInput` — `if (string.IsNullOrEmpty(input))` | без условия | Проверка валидации пустого ввода |

## Прогон №1 — позитивный сценарий

**Входные данные:** `text = "HELLOWORLD"`, `rows = 3`

### Стек вызовов в момент BP-1

```
BeelineCipher.dll!BeelineCipher.BeelineCipherCore.Encrypt(string text = "HELLOWORLD", int rows = 3)
BeelineCipher.dll!BeelineCipher.MainForm.ExecuteCipherOperation(...)
BeelineCipher.dll!BeelineCipher.MainForm.BtnEncrypt_Click(object sender, EventArgs e)
[Внешний код]
```

### Окно «Локальные» в BP-1

| Переменная | Тип | Значение |
| --- | --- | --- |
| `text` | `string` | `"HELLOWORLD"` |
| `rows` | `int` | `3` |
| `rails` | `StringBuilder[3]` | `[null, null, null]` (ещё не инициализированы) |

### Пошаговая трассировка (F10) в BP-2

| Итерация | `ch` | `currentRow` (до) | `direction` (до) | `rails[currentRow]` (после Append) | `currentRow` (после) | `direction` (после) |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | `'H'` | 0 | +1 | rails[0]="H" | 1 | +1 |
| 2 | `'E'` | 1 | +1 | rails[1]="E" | 2 | -1 |
| 3 | `'L'` | 2 | -1 | rails[2]="L" | 1 | -1 |
| 4 | `'L'` | 1 | -1 | rails[1]="EL" | 0 | +1 |
| 5 | `'O'` | 0 | +1 | rails[0]="HO" | 1 | +1 |
| 6 | `'W'` | 1 | +1 | rails[1]="ELW" | 2 | -1 |
| 7 | `'O'` | 2 | -1 | rails[2]="LO" | 1 | -1 |
| 8 | `'R'` | 1 | -1 | rails[1]="ELWR" | 0 | +1 |
| 9 | `'L'` | 0 | +1 | rails[0]="HOL" | 1 | +1 |
| 10 | `'D'` | 1 | +1 | rails[1]="ELWRD" | 2 | -1 |

### Срабатывания BP-3 (условие `currentRow == rows - 1`)

| Срабатывание | Итерация | `ch` |
| --- | --- | --- |
| 1 | 3 (`L`) | при достижении 2-й (нижней) рельсы |
| 2 | 7 (`O`) | повторное достижение нижней рельсы |

### Окно «Контрольные значения» (Watch) — выражения

| Выражение | Значение в финале |
| --- | --- |
| `rails[0].ToString()` | `"HOL"` |
| `rails[1].ToString()` | `"ELWRD"` |
| `rails[2].ToString()` | `"LO"` |
| `result.ToString()` | `"HOLELWRDLO"` |

**Вывод:** функция вернула `"HOLELWRDLO"` — совпадает с ожидаемым результатом (тест TC-01). Алгоритм отработал корректно.

### Дешифрование того же значения — BP-4

| Переменная | Значение |
| --- | --- |
| `cipher` | `"HOLELWRDLO"` |
| `rows` | `3` |
| `rowIndexForPosition` | `[0,1,2,1,0,1,2,1,0,1]` |
| `charsPerRow` | `[3, 5, 2]` |
| `rails` | `["HOL", "ELWRD", "LO"]` |

### BP-5 — сборка результата

| `i` | `row` | `railPointers[row]` (до) | взятый символ | `result.ToString()` (после) |
| --- | --- | --- | --- | --- |
| 0 | 0 | 0 | H | "H" |
| 1 | 1 | 0 | E | "HE" |
| 2 | 2 | 0 | L | "HEL" |
| 3 | 1 | 1 | L | "HELL" |
| 4 | 0 | 1 | O | "HELLO" |
| 5 | 1 | 2 | W | "HELLOW" |
| 6 | 2 | 1 | O | "HELLOWO" |
| 7 | 1 | 3 | R | "HELLOWOR" |
| 8 | 0 | 2 | L | "HELLOWORL" |
| 9 | 1 | 4 | D | "HELLOWORLD" |

**Вывод:** дешифрование вернуло исходный текст `"HELLOWORLD"`. Обратимость подтверждена.

## Прогон №2 — негативный сценарий (валидация GUI)

**Входные данные:** пустое поле `txtInput`, нажата кнопка «Зашифровать».

### BP-6 — обработчик `BtnEncrypt_Click`

| Локальная переменная | Значение |
| --- | --- |
| `sender` | `{Text="Зашифровать", ...}` (объект `Button`) |
| `e` | `{System.EventArgs}` |

### BP-7 — внутри `TryGetValidatedInput`

| Переменная | Значение |
| --- | --- |
| `input` | `""` |
| `rows` | `3` |
| `string.IsNullOrEmpty(input)` | `true` |

После срабатывания условия пользователю показан `MessageBox` с текстом «Поле „Исходный текст“ не должно быть пустым», `statusLabel.Text` обновлён на «Операция прервана из-за ошибки», метод вернул `false`. Падения приложения не произошло.

## Прогон №3 — негативный сценарий (ArgumentOutOfRangeException)

**Особый запуск через окно «Интерпретация» (Immediate Window)**:

```text
> BeelineCipher.BeelineCipherCore.Encrypt("Test", 1)
'BeelineCipher.BeelineCipherCore.Encrypt' threw an exception of type 'System.ArgumentOutOfRangeException'
Message: "Количество строк должно находиться в диапазоне от 2 до 100. (Parameter 'rows') Actual value was 1."
```

Исключение брошено в `ValidateInputs`. Поведение совпадает с тест-кейсом TC-26.

## Применённые средства отладки Visual Studio

| Средство | Где использовалось |
| --- | --- |
| Точки останова (Breakpoints) | BP-1..BP-7 |
| Условные точки останова | BP-3 (`currentRow == rows - 1`) |
| Окно «Локальные» (Locals) | Каждый прогон, проверка `text`, `rows`, `currentRow`, `direction` |
| Окно «Видимые» (Autos) | Прогон №1, отслеживание `ch` и `rails[currentRow]` |
| Окно «Контрольные значения» (Watch) | Прогон №1, выражения `rails[i].ToString()` |
| Окно «Стек вызовов» (Call Stack) | BP-1 (цепочка GUI → core) |
| Окно «Интерпретация» (Immediate Window) | Прогон №3 (вызов с rows=1) |
| F10 (Step Over) | Пошаговая трассировка цикла foreach |
| F11 (Step Into) | Заход в `ValidateInputs` |
| Shift+F11 (Step Out) | Возврат из `ValidateInputs` |
| F5 (Continue) | Продолжение между BP-2 и BP-3 |
| DataTip (наведение мыши) | Просмотр `rails[1].Length` в редакторе |
| Diagnostic Tools | Мониторинг памяти при шифровании 1000-символьного текста |
| Test Explorer | Запуск всех 34 тестов и отладка отдельных (Debug Selected Tests) |

## Итоги отладки

Никаких runtime-ошибок, исключений в позитивных сценариях и логических расхождений с ожидаемым результатом не выявлено. Все 7 точек останова срабатывали ожидаемо, значения переменных соответствовали проектному алгоритму Rail Fence. Дополнительных багов, которые нужно было бы фиксировать в баг-репорте по результатам отладки, не обнаружено.

## Скриншоты Visual Studio

Скриншоты для пунктов 6 и 7 размещаются в этой же папке:

- `breakpoints.png` — окно с расставленными точками останова
- `locals.png` — окно «Локальные» в момент BP-2
- `watch.png` — окно «Контрольные значения» с выражениями rails[i]
- `call_stack.png` — стек вызовов в BP-1
- `immediate.png` — окно «Интерпретация» с вызовом из прогона №3
- `test_explorer.png` — окно «Обозреватель тестов» с зелёными 34/34
