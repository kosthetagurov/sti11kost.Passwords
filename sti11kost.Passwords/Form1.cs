﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using sti11kost.Passwords.PasswordGenerator;
using System.Threading;
using System.IO;
using System.Reflection;

namespace sti11kost.Passwords
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly().GetName();
            version.Text = $"{assembly.Name} v{assembly.Version}";
            passwordsCount.Text = Properties.Settings.Default.PasswordsCount.ToString();
            size.Text = Properties.Settings.Default.PasswordLength.ToString();
            dontUse.Text = Properties.Settings.Default.ExcludeCharacters;
            includeStr.MaxLength = Properties.Settings.Default.PasswordLength;
        }
        
        private void Button1_Click(object sender, EventArgs e)
        {
            ResetErrorMessages(new List<Label>() {
                sizeError, includeStrWarning, countErrorMsg, fileExtError
            });

            var passwordGenerator = new CustomPasswordGenerator();

            passwordGenerator.size = int.TryParse(size.Text, out int _size) ? _size : -1;

            if (passwordGenerator.size == -1)
            {
                AddErrorMessage(sizeError, "Введите число!");
                return;
            }

            passwordGenerator.includeString = !string.IsNullOrEmpty(includeStr.Text) ? includeStr.Text : "";
            passwordGenerator.difirentRegister = difirentRegister.Checked ? true : false;
            passwordGenerator.useNumerics = useNums.Checked ? true : false;
            passwordGenerator.useSpecDigits = useSpecDigits.Checked ? true : false;
            passwordGenerator.unusebleDigits = dontUse.Text.Trim().Split(',');


            if (includeStr.Text.Length > _size)
            {
                AddErrorMessage(includeStrWarning, "Входная строка больше указанного размера.");
            }

            for (int i = 0; i < passwordsCount.Value; i++)
            {
                PrintPassword(totalPasswords, passwordGenerator.GeneratePassword());
                // In order to would be no identical passwords.
                Thread.Sleep(10);
            }

            if (!string.IsNullOrEmpty(totalPasswords.Text))
            {
                exportTxt.Enabled = true;
            }

        }


        #region Private Actions
        private void AddErrorMessage(Label label, string text)
        {
            label.Text = text;
        }

        private void ResetErrorMessages(List<Label> labels)
        {
            totalPasswords.Text = "";
            foreach (Label l in labels)
            {
                l.Text = " ";
            }
        }

        private void PrintPassword(TextBox textBox, List<string> list)
        {
            foreach (string str in list)
            {
                textBox.Text += str + "\r\n";
            }
        }

        private void PrintPassword(TextBox textBox, string str)
        {
            textBox.Text += str + "\r\n";
        }
        #endregion

        private void exportTxt_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.Title = "Выберите файл для записи";
                dialog.DefaultExt = "txt";
                dialog.ShowDialog();
                var path = dialog.FileName;

                if (!string.IsNullOrEmpty(totalPasswords.Text))
                {
                    if (Path.GetExtension(path) == ".txt")
                    {
                        using (var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            StreamWriter streamWriter = new StreamWriter(file);
                            streamWriter.Write(totalPasswords.Text);
                            streamWriter.Flush();
                        }
                        MessageBox.Show($"Успешно сохранено в файл \"{path}\"", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        fileExtError.Text = "Файл имеет неверный формат!";
                    }
                }
                else
                {
                    fileExtError.Text = "Сгенерируйте пароль (и)!";
                }
            }
        }

        private void clear_Click(object sender, EventArgs e)
        {
            exportTxt.Enabled = false;
            totalPasswords.Text = "";
        }

        private void settings_Click(object sender, EventArgs e)
        {
            new Settings().ShowDialog();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            passwordsCount.Text = Properties.Settings.Default.PasswordsCount.ToString();
            size.Text = Properties.Settings.Default.PasswordLength.ToString();
            dontUse.Text = Properties.Settings.Default.ExcludeCharacters;
        }

        private void size_ValueChanged(object sender, EventArgs e)
        {
            includeStr.MaxLength = int.Parse(size.Value.ToString());
        }
    }
}
