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
    public partial class frmPrincipal : Form
    {
        TabPage tabDestino;
        bool connectionTarget = false;
        private ProjectBase _baseProject;
        public ProjectBase baseProject
        {
            get
            {
                if (_baseProject == null)
                    _baseProject = new ProjectBase();

                return _baseProject;
            }
        }

        public frmPrincipal()
        {
            InitializeComponent();

            tabDestino = tbPrincipal.TabPages[1];
            //tbPrincipal.TabPages.RemoveAt(2);
            tbPrincipal.TabPages.RemoveAt(1);
            
        }

        private void btnAvancar_Click(object sender, EventArgs e)
        {
            if (tbPrincipal.TabPages.Count == 1)
            {
                //if (this.ckOpcoes.SelectedItems.Count == 0)
                //{
                //    MessageBox.Show("Favor selecionar as opções desejadas", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    this.ckOpcoes.Focus();
                //    return;
                //}

                tbPrincipal.TabPages.Insert(1, tabDestino);
                tbPrincipal.SelectTab(1);
                btnAnterior.Enabled = true;

                if (btnTarget.Text == "Logon")
                    btnAvancar.Enabled = false;
            }
            else
            {
                //pnObjects.Visible = true;
                //if (cklObjects.CheckedItems.Count == 0)
                //{
                //    MessageBox.Show("Favor selecione um tipo de objeto que deseja migrar!", "Migração", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                frmSincronizar frm = new frmSincronizar();
                //frm.Ept = cklObjects.GetItemChecked(0);
                //frm.CustomFields = cklObjects.GetItemChecked(1);
                //frm.Projects = cklObjects.GetItemChecked(2);
                //frm.Views = cklObjects.GetItemChecked(3);
                
                frm.pwaOrigem = this.txtSource.Text.Trim();
                frm.pwaDestino = this.txtTarget.Text.Trim();
                frm.baseProject = this.baseProject;
                this.Hide();
                frm.ShowDialog();
                this.Close();
            }
            
        }

        private bool Validar(bool target)
        {
            if (!target)
            {
                if (txtSource.Text.Length == 0)
                {
                    MessageBox.Show("Favor informar o endereço do Pwa Origem.", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSource.Focus();
                    return false;
                }
                if (txtSourceUser.Text.Length == 0)
                {
                    MessageBox.Show("Favor informar o user da farm do Pwa Origem.", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSourceUser.Focus();
                    return false;
                }
                if (txtPassSource.Text.Length == 0)
                {
                    MessageBox.Show("Favor informar a senha do user da farm do Pwa Origem.", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPassSource.Focus();
                    return false;
                }
            }
            else
            {
                if (txtTarget.Text.Length == 0)
                {
                    MessageBox.Show("Favor informar o endereço do Pwa Destino.", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTarget.Focus();
                    return false;
                }
                if (txtTargetUser.Text.Length == 0)
                {
                    MessageBox.Show("Favor informar o user da farm do Pwa Destino.", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTargetUser.Focus();
                    return false;
                }
                if (txtTargetPwd.Text.Length == 0)
                {
                    MessageBox.Show("Favor informar a senha do user da farm do Pwa Destino.", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTargetPwd.Focus();
                    return false;
                }
            }

            return true;
        }

        private void Autenticar(bool target)
        {
            if (Validar(target))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (!target)
                    {
                        var userDomain = txtSourceUser.Text.Trim().Split('\\');

                        if (userDomain.Count() == 0)
                        {
                            MessageBox.Show("Favor informar dominio!", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtSourceUser.Focus();
                            return;
                        }

                        this.baseProject.Domain = userDomain[0];
                        this.baseProject.User = userDomain[1];
                        this.baseProject.PassWord = txtPassSource.Text;

                        string pwaUrl = target ? txtTarget.Text.Trim() : txtSource.Text.Trim();

                        if (btnConectSource.Text == "LogOff")
                        {
                            if (this.baseProject.LogoffPS(pwaUrl))
                            {
                                txtSource.Enabled = true;
                                //txtPassSource.Enabled = true;
                                //txtSourceUser.Enabled = true;
                                btnConectSource.Text = "Logon";
                                pictureBox1.Visible = false;
                            }
                        }
                        else
                        {
                            if (this.baseProject.Logon(pwaUrl))
                            {
                                txtSource.Enabled = false;
                                //txtPassSource.Enabled = false;
                                //txtSourceUser.Enabled = false;
                                btnConectSource.Text = "LogOff";
                                pictureBox1.Visible = true;
                                btnAvancar.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        var userDomain = txtTargetUser.Text.Trim().Split('\\');

                        if (userDomain.Count() == 0)
                        {
                            MessageBox.Show("Favor informar dominio!", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtTargetUser.Focus();
                            return;
                        }

                        this.baseProject.Domain = userDomain[0];
                        this.baseProject.User = userDomain[1];
                        this.baseProject.PassWord = txtTargetPwd.Text.Trim();

                        string pwaUrl = target ? txtTarget.Text.Trim() : txtSource.Text.Trim();

                        if (btnTarget.Text == "LogOff")
                        {
                            if (this.baseProject.LogoffPS(pwaUrl))
                            {
                                txtTarget.Enabled = true;
                                //txtTargetUser.Enabled = true;
                                btnTarget.Text = "Logon";
                                pictureBox2.Visible = false;
                            }
                        }
                        else
                        {
                            if (this.baseProject.Logon(pwaUrl))
                            {
                                txtTarget.Enabled = true;
                                //txtTargetUser.Enabled = true;
                                btnTarget.Text = "LogOff";
                                pictureBox2.Visible = true;
                                btnAvancar.Enabled = true;
                            }
                        }
                    }
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    throw ex;
                }
            }
        }

        private void btnConectSource_Click(object sender, EventArgs e)
        {
            Autenticar(false);  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Autenticar(true);  
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            tbPrincipal.SelectTab(0);
            tbPrincipal.TabPages.RemoveAt(1);
            btnAnterior.Enabled = false;
            btnAvancar.Enabled = true;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Autenticar(false);
        }


    }
}
