using System;
using System.Windows.Forms;

namespace BeelineCipher
{
    /// <summary>
    /// Главное окно приложения. Предоставляет графический интерфейс для модулей
    /// шифрования и дешифрования по алгоритму Билайна (Rail Fence)
    /// и визуализации заполнения зигзагом.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Создаёт и инициализирует форму, настраивает всплывающие подсказки.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            ConfigureToolTips();
        }

        /// <summary>
        /// Привязывает всплывающие подсказки ко всем интерактивным элементам управления.
        /// </summary>
        private void ConfigureToolTips()
        {
            toolTip.SetToolTip(txtInput, "Введите исходный текст для шифрования или дешифрования.");
            toolTip.SetToolTip(numRows, $"Количество строк зигзага (от {BeelineCipherCore.MinRows} до {BeelineCipherCore.MaxRows}).");
            toolTip.SetToolTip(btnEncrypt, "Зашифровать текст методом записи в N строк зигзагом.");
            toolTip.SetToolTip(btnDecrypt, "Расшифровать текст, зашифрованный с тем же количеством строк.");
            toolTip.SetToolTip(btnVisualize, "Показать визуальное заполнение зигзагообразной матрицы.");
            toolTip.SetToolTip(btnClear, "Очистить все поля ввода и вывода.");
            toolTip.SetToolTip(txtOutput, "Здесь будет выведен результат шифрования или дешифрования.");
            toolTip.SetToolTip(txtVisualization, "Графическое представление заполнения матрицы шифра.");
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Зашифровать».
        /// </summary>
        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            ExecuteCipherOperation(BeelineCipherCore.Encrypt, "Зашифровано");
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Расшифровать».
        /// </summary>
        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            ExecuteCipherOperation(BeelineCipherCore.Decrypt, "Расшифровано");
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Показать зигзаг».
        /// Строит визуализацию заполнения матрицы шифра.
        /// </summary>
        private void BtnVisualize_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryGetValidatedInput(out string input, out int rows))
                {
                    return;
                }

                txtVisualization.Text = BeelineCipherCore.Visualize(input, rows);
                statusLabel.Text = $"Визуализация построена для {rows} строк.";
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Очистить».
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            txtOutput.Clear();
            txtVisualization.Clear();
            numRows.Value = 3;
            statusLabel.Text = "Поля очищены.";
            txtInput.Focus();
        }

        /// <summary>
        /// Универсальный метод запуска операции шифрования или дешифрования
        /// с валидацией входных данных и обработкой исключений.
        /// </summary>
        /// <param name="operation">Делегат операции (Encrypt или Decrypt).</param>
        /// <param name="successMessage">Сообщение об успехе для строки состояния.</param>
        private void ExecuteCipherOperation(Func<string, int, string> operation, string successMessage)
        {
            try
            {
                if (!TryGetValidatedInput(out string input, out int rows))
                {
                    return;
                }

                txtOutput.Text = operation(input, rows);
                txtVisualization.Text = BeelineCipherCore.Visualize(input, rows);
                statusLabel.Text = $"{successMessage}. Длина: {input.Length} символов, строк: {rows}.";
            }
            catch (ArgumentNullException ex)
            {
                ShowError($"Ошибка: переданное значение null. {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ShowError($"Ошибка: некорректное число строк. {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Считывает и валидирует исходный текст и количество строк.
        /// </summary>
        /// <param name="input">Получаемый исходный текст.</param>
        /// <param name="rows">Получаемое количество строк.</param>
        /// <returns>true, если данные корректны; false при ошибке валидации.</returns>
        private bool TryGetValidatedInput(out string input, out int rows)
        {
            input = txtInput.Text;
            rows = (int)numRows.Value;

            if (string.IsNullOrEmpty(input))
            {
                ShowError("Поле «Исходный текст» не должно быть пустым.");
                txtInput.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError("Поле «Исходный текст» не должно содержать только пробельные символы.");
                txtInput.Focus();
                return false;
            }

            if (rows < BeelineCipherCore.MinRows || rows > BeelineCipherCore.MaxRows)
            {
                ShowError($"Количество строк должно быть от {BeelineCipherCore.MinRows} до {BeelineCipherCore.MaxRows}.");
                numRows.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Отображает диалог с сообщением об ошибке и обновляет строку состояния.
        /// </summary>
        /// <param name="message">Текст сообщения об ошибке.</param>
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            statusLabel.Text = "Операция прервана из-за ошибки.";
        }
    }
}
