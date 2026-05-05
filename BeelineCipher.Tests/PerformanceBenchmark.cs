using System;
using System.Diagnostics;
using BeelineCipher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeelineCipher.Tests
{
    /// <summary>
    /// Замеры нефункциональных характеристик (производительность ядра шифра).
    /// </summary>
    [TestClass]
    public class PerformanceBenchmark
    {
        [TestMethod]
        [TestCategory("Performance")]
        public void Encrypt_1000Chars_CompletesUnder100Ms()
        {
            string text = new string('A', 1000);
            int rows = 7;

            var sw = Stopwatch.StartNew();
            string result = BeelineCipherCore.Encrypt(text, rows);
            sw.Stop();

            TestContext?.WriteLine($"Encrypt(1000 chars, rows=7): {sw.Elapsed.TotalMilliseconds:F3} ms");
            Assert.IsTrue(sw.ElapsedMilliseconds < 100, $"Долго: {sw.ElapsedMilliseconds} ms");
            Assert.AreEqual(text.Length, result.Length);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void Encrypt_10000Chars_CompletesUnder500Ms()
        {
            string text = new string('B', 10_000);
            int rows = 50;

            var sw = Stopwatch.StartNew();
            string result = BeelineCipherCore.Encrypt(text, rows);
            sw.Stop();

            TestContext?.WriteLine($"Encrypt(10 000 chars, rows=50): {sw.Elapsed.TotalMilliseconds:F3} ms");
            Assert.IsTrue(sw.ElapsedMilliseconds < 500, $"Долго: {sw.ElapsedMilliseconds} ms");
            Assert.AreEqual(text.Length, result.Length);
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void EncryptDecryptRoundTrip_5000Chars_CompletesUnder300Ms()
        {
            string text = new string('C', 5000);
            int rows = 10;

            var sw = Stopwatch.StartNew();
            string encrypted = BeelineCipherCore.Encrypt(text, rows);
            string decrypted = BeelineCipherCore.Decrypt(encrypted, rows);
            sw.Stop();

            TestContext?.WriteLine($"Encrypt+Decrypt(5000 chars, rows=10): {sw.Elapsed.TotalMilliseconds:F3} ms");
            Assert.IsTrue(sw.ElapsedMilliseconds < 300, $"Долго: {sw.ElapsedMilliseconds} ms");
            Assert.AreEqual(text, decrypted);
        }

        public TestContext? TestContext { get; set; }
    }
}
