// - [ mainForm.cs ] -
// Created by Uzis: 3/29/2026

using System;
using System.IO;
using System.Windows.Forms;


namespace SR2JP_Mod_Manager
{
    public partial class mainForm : Form
    {
        bool bHasReadFirstMessage = false; // Just a hack if necessary to make it so the message doesn't pop twice at once.
        public mainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the Form Name w/ Previous Git Hash.
            this.Text = $"Saints Row 2: Juiced Patch Mod Manager {{prc:{GitInfo.Hash}}}";
            if (Directory.Exists(Global.appDataPath) && File.Exists($"{Global.appDataPath}\\settings.txt"))
            {
                Global.SR2Location = File.ReadAllText($"{Global.appDataPath}\\settings.txt");
                if (!(Global.SR2Location == null))
                {
                    Startup.CheckForJuicedPatch();
                    bHasReadFirstMessage = true;
                }
            }
            else
            {
                if (bHasReadFirstMessage == false)
                {
                    if (Global.SR2Location != null || Global.SR2Location == "")
                    {
                        if ((MessageBox.Show("To get started, first off we need to know where your Saints Row 2 Game Directory is located!\n\n" +
                            "If you're using the Steam version its typically in\n\n" +
                            "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Saints Row 2",
                            "Welcome to SR2JP Mod Manager",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)) == DialogResult.OK) ;
                        {
                            if (findSR2dialog.ShowDialog() == DialogResult.OK)
                            {
                                string filePath = findSR2dialog.SelectedPath;
                                //MessageBox.Show(filePath);
                                if (!Directory.Exists(Global.appDataPath)) // Check if our settings path exists, if not, create it.
                                {
                                    Directory.CreateDirectory(Global.appDataPath);
                                }
                                File.WriteAllText($"{Global.appDataPath}\\settings.txt", filePath); // Create a simple settings.txt with our SR2 directory path that we can also edit later.
                                Global.SR2Location = filePath;
                            }
                        }
                        Startup.CheckForJuicedPatch();
                        bHasReadFirstMessage = true;
                    }
                }
            }
        }
    }
}
