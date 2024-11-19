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

namespace CryptoVariants
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EncryptionMethodComboBox.SelectedIndex = 0;
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
                    result = VigenereCipher(inputText, "KEY"); 
                    break;
                case 3:
                    result = RsaEncrypt(inputText);
                    break;
                default:
                    MessageBox.Show("Выберите метод шифрования.");
                    return;
            }

            OutputTextBox.Text = result;
        }

        private string CaesarCipher(string input, int shift)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    char encryptedChar = (char)((((c + shift) - offset + 26) % 26) + offset);
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

        private string VigenereCipher(string input, string key)
        {
            StringBuilder result = new StringBuilder();
            int keyIndex = 0;

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    char keyChar = char.ToUpper(key[keyIndex % key.Length]);
                    int shift = keyChar - 'A';
                    char encryptedChar = (char)((((c + shift) - offset + 26) % 26) + offset);
                    result.Append(encryptedChar);
                    keyIndex++;
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private string RsaEncrypt(string plainText)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
               
                rsa.KeySize = 2048;
                RSAParameters rsaParams = rsa.ExportParameters(true);

               
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = rsa.Encrypt(plainBytes, false);

                
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }
}
