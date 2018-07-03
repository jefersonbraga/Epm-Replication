using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Segplan.ReplicateCustomField.App
{
    public partial class AlertForm : Form
    {
        public AlertForm()
        {
            InitializeComponent();
        }

        #region PROPERTIES

        public string Message
        {
            set { labelMessage.Text = value; }
        }

        public int ProgressValueMax
        {
            set
            {
                progressBar1.Maximum = value;
            }
        }

        public int ProgressValue
        {
            get { return progressBar1.Value; }
            set { progressBar1.Value = value; }
        }

        #endregion

       
        #region EVENTS

        public event EventHandler<EventArgs> Canceled;

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Create a copy of the event to work with
            EventHandler<EventArgs> ea = Canceled;
            /* If there are no subscribers, eh will be null so we need to check
             * to avoid a NullReferenceException. */
            if (ea != null)
                ea(this, e);
        }
        #endregion
    }
}
