using System;
using System.Linq;
using System.Text;

namespace BeelineCipher
{
    /// <summary>
    /// Реализация шифра Билайна (Rail Fence / двухлинейный шифр)
    /// с динамическим числом строк и зигзагообразным чтением.
    /// </summary>
    public static class BeelineCipherCore
    {
        /// <summary>
        /// Минимально допустимое число строк для шифрования.
        /// </summary>
        public const int MinRows = 2;

        /// <summary>
        /// Максимально допустимое число строк для шифрования.
        /// </summary>
        public const int MaxRows = 100;

        /// <summary>
        /// Шифрует текст методом записи зигзагом по заданному количеству строк.
        /// </summary>
        /// <param name="text">Исходный текст для шифрования.</param>
        /// <param name="rows">Количество строк (rails) для зигзагообразной записи.</param>
        /// <returns>Зашифрованный текст, считанный построчно.</returns>
        /// <exception cref="ArgumentNullException">Текст равен null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Число строк вне допустимого диапазона.</exception>
        public static string Encrypt(string text, int rows)
        {
            ValidateInputs(text, rows);

            if (text.Length == 0)
            {
                return string.Empty;
            }

            if (rows == 1 || rows >= text.Length)
            {
                return text;
            }

            var rails = new StringBuilder[rows];
            for (int i = 0; i < rows; i++)
            {
                rails[i] = new StringBuilder();
            }

            int currentRow = 0;
            int direction = 1;

            foreach (char ch in text)
            {
                rails[currentRow].Append(ch);
                currentRow += direction;

                if (currentRow == rows - 1 || currentRow == 0)
                {
                    direction = -direction;
                }
            }

            var result = new StringBuilder(text.Length);
            for (int i = 0; i < rows; i++)
            {
                result.Append(rails[i]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Дешифрует текст, ранее зашифрованный методом Билайна
        /// с тем же количеством строк.
        /// </summary>
        /// <param name="cipher">Зашифрованный текст.</param>
        /// <param name="rows">Количество строк (rails), использованных при шифровании.</param>
        /// <returns>Восстановленный исходный текст.</returns>
        /// <exception cref="ArgumentNullException">Шифр равен null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Число строк вне допустимого диапазона.</exception>
        public static string Decrypt(string cipher, int rows)
        {
            ValidateInputs(cipher, rows);

            if (cipher.Length == 0)
            {
                return string.Empty;
            }

            if (rows == 1 || rows >= cipher.Length)
            {
                return cipher;
            }

            int[] rowIndexForPosition = new int[cipher.Length];
            int currentRow = 0;
            int direction = 1;
            for (int i = 0; i < cipher.Length; i++)
            {
                rowIndexForPosition[i] = currentRow;
                currentRow += direction;
                if (currentRow == rows - 1 || currentRow == 0)
                {
                    direction = -direction;
                }
            }

            int[] charsPerRow = new int[rows];
            foreach (int rowIndex in rowIndexForPosition)
            {
                charsPerRow[rowIndex]++;
            }

            string[] rails = new string[rows];
            int offset = 0;
            for (int i = 0; i < rows; i++)
            {
                rails[i] = cipher.Substring(offset, charsPerRow[i]);
                offset += charsPerRow[i];
            }

            int[] railPointers = new int[rows];
            var result = new StringBuilder(cipher.Length);
            for (int i = 0; i < cipher.Length; i++)
            {
                int row = rowIndexForPosition[i];
                result.Append(rails[row][railPointers[row]++]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Возвращает строковое представление зигзагообразной матрицы заполнения
        /// для визуализации процесса шифрования.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <param name="rows">Количество строк.</param>
        /// <returns>Многострочная строка с символом '.' вместо пустых ячеек.</returns>
        /// <exception cref="ArgumentNullException">Текст равен null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Число строк вне допустимого диапазона.</exception>
        public static string Visualize(string text, int rows)
        {
            ValidateInputs(text, rows);

            if (text.Length == 0)
            {
                return string.Empty;
            }

            int effectiveRows = Math.Min(rows, Math.Max(text.Length, 1));
            char[,] grid = new char[effectiveRows, text.Length];
            for (int r = 0; r < effectiveRows; r++)
            {
                for (int c = 0; c < text.Length; c++)
                {
                    grid[r, c] = '.';
                }
            }

            int currentRow = 0;
            int direction = effectiveRows == 1 ? 0 : 1;

            for (int col = 0; col < text.Length; col++)
            {
                grid[currentRow, col] = text[col];
                currentRow += direction;
                if (effectiveRows > 1 && (currentRow == effectiveRows - 1 || currentRow == 0))
                {
                    direction = -direction;
                }
            }

            var sb = new StringBuilder();
            for (int r = 0; r < effectiveRows; r++)
            {
                for (int c = 0; c < text.Length; c++)
                {
                    sb.Append(grid[r, c]);
                    if (c < text.Length - 1)
                    {
                        sb.Append(' ');
                    }
                }
                if (r < effectiveRows - 1)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Проверяет корректность входных параметров шифрования.
        /// </summary>
        private static void ValidateInputs(string text, int rows)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text), "Текст не может быть равен null.");
            }

            if (rows < MinRows || rows > MaxRows)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rows),
                    rows,
                    $"Количество строк должно находиться в диапазоне от {MinRows} до {MaxRows}.");
            }
        }
    }
}
