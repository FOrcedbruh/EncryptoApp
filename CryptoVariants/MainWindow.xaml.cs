using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.IO;




namespace CryptoVariants
{
    public partial class MainWindow : Window
    {
        private RSACryptoServiceProvider rsa;
        
        public MainWindow()
        {
            InitializeComponent();
            EncryptionMethodComboBox.SelectedIndex = 0;
            rsa = new RSACryptoServiceProvider();
            RSAParameters privateKey = rsa.ExportParameters(true);
            RSAParameters publicKey = rsa.ExportParameters(false);
        }
        

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            string result = string.Empty;
            if (inputText == "")
            {
                MessageBox.Show("Заполните обязательное поле.");
            }

            switch (EncryptionMethodComboBox.SelectedIndex)
            {
                case 0:
                    result = CaesarCipher(inputText, 3);
                    break;
                case 1:
                    result = SimpleSubstitutionCipher(inputText);
                    break;
                case 2:
                    result = AtbashEncrypt(inputText); 
                    break;
                case 3:
                    result = RsaEncrypt(inputText, rsa.ExportParameters(false));
                    break;
                default:
                    MessageBox.Show("Выберите метод шифрования.");
                    return;
            }

            OutputTextBox.Text = result;
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string outputText = OutputTextBox.Text;
            string result = string.Empty;

            switch (EncryptionMethodComboBox.SelectedIndex)
            {
                case 0:
                    result = CaesarDecrypt(outputText, 3);
                    break;
                case 1:
                    result = SimpleSubstitutionDecrypt(outputText);
                    break;
                case 2:
                    result = AtbashDecrypt(outputText);
                    break;
                case 3:
                    result = RsaDecrypt(outputText, rsa.ExportParameters(true));
                    break;
                default:
                    MessageBox.Show("Выберите метод шифрования.");
                    return;
            }
            MessageBox.Show(string.Format("Дешифрованное слово: {0}", result));
            OutputTextBox.Text = result;
        }

        private string CaesarCipher(string input, int shift)
        {
            StringBuilder result = new StringBuilder();
            const int russianAlphabetLength = 33; 
            const int englishAlphabetLength = 26; 

            foreach (char c in input)
            {
                if (c >= 'А' && c <= 'Я') 
                {
                    char encryptedChar = (char)((((c - 'А' + shift) % russianAlphabetLength) + russianAlphabetLength) % russianAlphabetLength + 'А');
                    result.Append(encryptedChar);
                }
                else if (c >= 'а' && c <= 'я') 
                {
                    char encryptedChar = (char)((((c - 'а' + shift) % russianAlphabetLength) + russianAlphabetLength) % russianAlphabetLength + 'а');
                    result.Append(encryptedChar);
                }
                else if (c >= 'A' && c <= 'Z') 
                {
                    char encryptedChar = (char)((((c - 'A' + shift) % englishAlphabetLength) + englishAlphabetLength) % englishAlphabetLength + 'A');
                    result.Append(encryptedChar);
                }
                else if (c >= 'a' && c <= 'z') 
                {
                    char encryptedChar = (char)((((c - 'a' + shift) % englishAlphabetLength) + englishAlphabetLength) % englishAlphabetLength + 'a');
                    result.Append(encryptedChar);
                }
                else
                {
                    result.Append(c); 
                }
            }

            return result.ToString();
        }

        private string SimpleSubstitutionCipher(string input)
        {
           
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    char encryptedChar = (char)(offset + ('z' - char.ToLower(c)));
                    result.Append(encryptedChar);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        private string SimpleSubstitutionDecrypt(string input)
        {
            
            string originalRussian = "БГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯА";
            string substituteRussian = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            string originalEnglish = "BCDEFGHIJKLMNOPQRSTUVWXYZA";
            string substituteEnglish = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder result = new StringBuilder();

            foreach (char c in input)
            {
                int index;
                if ((index = originalRussian.IndexOf(c)) != -1)
                {
                    result.Append(substituteRussian[index]);
                }
                else if ((index = originalEnglish.IndexOf(c)) != -1)
                {
                    result.Append(substituteEnglish[index]);
                }
                else
                {
                    result.Append(c); 
                }
            }

            return result.ToString();
        }

        private string AtbashEncrypt(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in input)
            {
                if (c >= 'A' && c <= 'Z') 
                {
                    char encryptedChar = (char)('Z' - (c - 'A'));
                    result.Append(encryptedChar);
                }
                else if (c >= 'a' && c <= 'z') 
                {
                    char encryptedChar = (char)('z' - (c - 'a'));
                    result.Append(encryptedChar);
                }
                else if (c >= 'А' && c <= 'Я')
                {
                    char encryptedChar = (char)('Я' - (c - 'А'));
                    result.Append(encryptedChar);
                }
                else if (c >= 'а' && c <= 'я') 
                {
                    char encryptedChar = (char)('я' - (c - 'а'));
                    result.Append(encryptedChar);
                }
                else
                {
                    result.Append(c); 
                }
            }

            return result.ToString();
        }
        

        private static string RsaEncrypt(string plainText, RSAParameters publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes = rsa.Encrypt(plainBytes, false);
                return Convert.ToBase64String(cipherBytes);
            }
        }

        private static string RsaDecrypt(string cipherText, RSAParameters privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = rsa.Decrypt(cipherBytes, false); 
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private void ReadFile_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            
            if (openFileDialog.ShowDialog() == true)
            {
                LoadTextFromFile(openFileDialog.FileName);
            }
        }

        private void LoadTextFromFile(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string content = sr.ReadToEnd();
                    InputTextBox.Text = content;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при чтении файла: " + e.Message);
            }
        }
        private string CaesarDecrypt(string input, int shift)
        {
            return CaesarCipher(input, -shift);
        }



        private string AtbashDecrypt(string input)
        {
            return AtbashEncrypt(input); 
        }
    }
}
