using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace room_reservation
{
    public partial class Form1 : Form
    {
        private readonly Button[] otaqlar;
        private readonly Label[] vaxtLabelleri;
        private readonly Color[] ilkinRengler;
        private readonly double[] bitmeVaxtlari;

        private readonly Stopwatch saat = Stopwatch.StartNew();
        private readonly System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();

            otaqlar = new[]
            {
                button1, button2, button3,
                button6, button5, button4
            };

            vaxtLabelleri = new[]
            {
                label3, label4, label5,
                label6, label7, label8
            };

            ilkinRengler = new Color[otaqlar.Length];
            bitmeVaxtlari = new double[otaqlar.Length];

            label2.Text = "Müddət (san.):";
            maskedTextBox1.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            for (int i = 0; i < otaqlar.Length; i++)
            {
                ilkinRengler[i] = otaqlar[i].BackColor;
                otaqlar[i].Click += Otaq_Click;

                vaxtLabelleri[i].AutoSize = false;
                vaxtLabelleri[i].Left = otaqlar[i].Left;
                vaxtLabelleri[i].Width = otaqlar[i].Width;
                vaxtLabelleri[i].TextAlign = ContentAlignment.MiddleCenter;
                vaxtLabelleri[i].Text = "Boş";
            }

            components ??= new Container();
            timer = new System.Windows.Forms.Timer(components);
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
        }

        private void Otaq_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            int indeks = Array.IndexOf(otaqlar, button);

            if (indeks < 0 || bitmeVaxtlari[indeks] > 0)
                return;

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Ad və soyad daxil edin.");
                textBox1.Focus();
                return;
            }

            if (!int.TryParse(maskedTextBox1.Text.Trim(), out int saniye)
                || saniye <= 0)
            {
                MessageBox.Show("Müddəti 1–999 saniyə arasında daxil edin.");
                maskedTextBox1.Focus();
                return;
            }

            bitmeVaxtlari[indeks] = saat.Elapsed.TotalSeconds + saniye;

            button.BackColor = Color.Red;
            button.Enabled = false;
            vaxtLabelleri[indeks].Text = $"{saniye} san.";

            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            bool rezervVar = false;
            double indi = saat.Elapsed.TotalSeconds;

            for (int i = 0; i < otaqlar.Length; i++)
            {
                if (bitmeVaxtlari[i] == 0)
                    continue;

                int qalanSaniye =
                    (int)Math.Ceiling(bitmeVaxtlari[i] - indi);

                if (qalanSaniye <= 0)
                {
                    bitmeVaxtlari[i] = 0;
                    otaqlar[i].BackColor = ilkinRengler[i];
                    otaqlar[i].Enabled = true;
                    vaxtLabelleri[i].Text = "Boş";
                }
                else
                {
                    vaxtLabelleri[i].Text = $"{qalanSaniye} san.";
                    rezervVar = true;
                }
            }

            if (!rezervVar)
                timer.Stop();
        }
    }
}