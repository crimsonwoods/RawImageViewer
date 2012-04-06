using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RawImageViewer
{
    public partial class SizeDialog : Form
    {
        private Size size;

        public SizeDialog()
        {
            InitializeComponent();

            this.Shown += delegate
            {
                numericUpDownWidth.Value = size.Width;
                numericUpDownHeight.Value = size.Height;
            };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                size.Width = (int)numericUpDownWidth.Value;
                size.Height = (int)numericUpDownHeight.Value;
            }
        }

        public Size Value
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
    }
}
