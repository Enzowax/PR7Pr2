using System;
using BeelineCipher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeelineCipher.Tests
{
    /// <summary>
    /// Автоматизированные тесты шифра Билайна (Rail Fence)
    /// для проверки функциональных требований модулей шифрования и дешифрования.
    /// </summary>
    [TestClass]
    public class BeelineCipherTests
    {
        [TestMethod]
        public void Encrypt_ClassicHelloWorldWithThreeRows_ReturnsExpectedCipher()
        {
            string original = "HELLOWORLD";
            int rows = 3;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);

            Assert.AreEqual("HOLELWRDLO", encrypted);
        }

        [TestMethod]
        public void Decrypt_AfterEncryption_RestoresOriginalWithThreeRows()
        {
            string original = "HELLOWORLD";
            int rows = 3;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);
            string decrypted = BeelineCipherCore.Decrypt(encrypted, rows);

            Assert.AreEqual(original, decrypted);
        }

        [TestMethod]
        public void EncryptDecrypt_RussianText_RestoresOriginal()
        {
            string original = "Поддержка и тестирование программных модулей";
            int rows = 4;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);
            string decrypted = BeelineCipherCore.Decrypt(encrypted, rows);

            Assert.AreEqual(original, decrypted);
            Assert.AreNotEqual(original, encrypted);
        }

        [TestMethod]
        public void EncryptDecrypt_TextWithSpacesAndPunctuation_RestoresOriginal()
        {
            string original = "Hello, World! 2026.";
            int rows = 5;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);
            string decrypted = BeelineCipherCore.Decrypt(encrypted, rows);

            Assert.AreEqual(original, decrypted);
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(10)]
        [DataRow(50)]
        public void EncryptDecrypt_DifferentRowCounts_AreReversible(int rows)
        {
            string original = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string encrypted = BeelineCipherCore.Encrypt(original, rows);
            string decrypted = BeelineCipherCore.Decrypt(encrypted, rows);

            Assert.AreEqual(original, decrypted, $"Reversibility failed for rows={rows}");
        }

        [TestMethod]
        public void Encrypt_TwoRows_ProducesEvenOddSplit()
        {
            string original = "ABCDEF";
            int rows = 2;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);

            Assert.AreEqual("ACEBDF", encrypted);
        }

        [TestMethod]
        public void Encrypt_RowsEqualToTextLength_ReturnsTextUnchanged()
        {
            string original = "ABCDE";
            int rows = 5;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);

            Assert.AreEqual(original, encrypted);
        }

        [TestMethod]
        public void Encrypt_RowsGreaterThanTextLength_ReturnsTextUnchanged()
        {
            string original = "AB";
            int rows = 10;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);

            Assert.AreEqual(original, encrypted);
        }

        [TestMethod]
        public void Encrypt_EmptyString_ReturnsEmptyString()
        {
            string encrypted = BeelineCipherCore.Encrypt(string.Empty, 3);

            Assert.AreEqual(string.Empty, encrypted);
        }

        [TestMethod]
        public void Decrypt_EmptyString_ReturnsEmptyString()
        {
            string decrypted = BeelineCipherCore.Decrypt(string.Empty, 3);

            Assert.AreEqual(string.Empty, decrypted);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encrypt_NullText_ThrowsArgumentNullException()
        {
            BeelineCipherCore.Encrypt(null!, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Decrypt_NullText_ThrowsArgumentNullException()
        {
            BeelineCipherCore.Decrypt(null!, 3);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(-3)]
        [DataRow(101)]
        [DataRow(1000)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Encrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException(int rows)
        {
            BeelineCipherCore.Encrypt("Test", rows);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(-1)]
        [DataRow(101)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Decrypt_RowsOutOfRange_ThrowsArgumentOutOfRangeException(int rows)
        {
            BeelineCipherCore.Decrypt("Test", rows);
        }

        [TestMethod]
        public void Encrypt_PreservesLength()
        {
            string original = "Программирование на C# в Visual Studio";
            int rows = 6;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);

            Assert.AreEqual(original.Length, encrypted.Length);
        }

        [TestMethod]
        public void Encrypt_ProducesDifferentTextThanOriginal()
        {
            string original = "Тестовая фраза для шифрования";
            int rows = 3;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);

            Assert.AreNotEqual(original, encrypted);
        }

        [TestMethod]
        public void Visualize_ThreeRowsHelloWorld_ReturnsExpectedZigzagGrid()
        {
            string original = "HELLOWORLD";
            int rows = 3;
            string expected =
                "H . . . O . . . L ." + Environment.NewLine +
                ". E . L . W . R . D" + Environment.NewLine +
                ". . L . . . O . . .";

            string visualization = BeelineCipherCore.Visualize(original, rows);

            Assert.AreEqual(expected, visualization);
        }

        [TestMethod]
        public void Visualize_EmptyString_ReturnsEmptyString()
        {
            string visualization = BeelineCipherCore.Visualize(string.Empty, 3);

            Assert.AreEqual(string.Empty, visualization);
        }

        [TestMethod]
        public void Visualize_ContainsAllOriginalCharacters()
        {
            string original = "ABCDEFG";
            int rows = 3;

            string visualization = BeelineCipherCore.Visualize(original, rows);

            foreach (char ch in original)
            {
                StringAssert.Contains(visualization, ch.ToString());
            }
        }

        [TestMethod]
        public void EncryptDecrypt_LongText_RestoresOriginal()
        {
            string original = new string('A', 500) + new string('B', 500);
            int rows = 7;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);
            string decrypted = BeelineCipherCore.Decrypt(encrypted, rows);

            Assert.AreEqual(original, decrypted);
        }

        [TestMethod]
        public void Encrypt_SameTextWithDifferentRows_ProducesDifferentResults()
        {
            string original = "ZIGZAGCIPHER";

            string withTwo = BeelineCipherCore.Encrypt(original, 2);
            string withThree = BeelineCipherCore.Encrypt(original, 3);
            string withFive = BeelineCipherCore.Encrypt(original, 5);

            Assert.AreNotEqual(withTwo, withThree);
            Assert.AreNotEqual(withThree, withFive);
            Assert.AreNotEqual(withTwo, withFive);
        }

        [TestMethod]
        public void EncryptDecrypt_RealRussianPhrase_AreMutualInverses()
        {

            string original = "Привет мир!";
            int rows = 4;

            string encrypted = BeelineCipherCore.Encrypt(original, rows);
            string decryptedBack = BeelineCipherCore.Decrypt(encrypted, rows);
            Assert.AreEqual(original, decryptedBack, "Decrypt(Encrypt(x)) ≠ x");

            string decrypted = BeelineCipherCore.Decrypt(original, rows);
            string encryptedBack = BeelineCipherCore.Encrypt(decrypted, rows);
            Assert.AreEqual(original, encryptedBack, "Encrypt(Decrypt(x)) ≠ x");

            Assert.AreNotEqual(encrypted, decrypted,
                "Encrypt и Decrypt должны давать разные результаты для одного входа");
        }
    }
}
