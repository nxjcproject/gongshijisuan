using ExpressionHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
    public partial class frmValidator : Form
    {
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public frmValidator()
        {
            InitializeComponent();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();
            stopwatch.Start();

            var errors = Utility.ValidateExpression(txtExpression.Text);
            if (errors.Count == 0)
            {
                rtbResult.AppendText("验证通过" + Environment.NewLine);

                Calculator cal = new Calculator();
                if (txtExpression.Text.Contains('A') || txtExpression.Text.Contains('S') || txtExpression.Text.Contains('P'))
                {
                    rtbResult.AppendText("含有变量，无法计算" + Environment.NewLine);
                }
                else
                {
                    rtbResult.AppendText("结果：" + cal.Calculate(txtExpression.Text).ToString() + Environment.NewLine);
                }
            }
            else
            {
                foreach (var error in errors)
                {
                    rtbResult.AppendText(error.Message + "， 位置：" + error.Location + Environment.NewLine); 
                }
            }

            stopwatch.Stop();
            rtbResult.AppendText("耗时：" + stopwatch.ElapsedMilliseconds + "ms" + Environment.NewLine);
            rtbResult.AppendText("----------------------------------------------" + Environment.NewLine);

            rtbResult.ScrollToCaret();
        }
    }
}
