using Microsoft.ProjectServer.Client;
using SvcLookupTable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Segplan.ReplicateCustomField.App
{
    public enum EtapaProcessamento
    {
        EPT = 0,
        CustomFields = 1,
        Projects = 2,
        Views = 3
    }

    public partial class frmSincronizar : Form
    {
        #region Fields

        public bool Ept { get; set; }
        public bool CustomFields { get; set; }
        public bool Projects { get; set; }
        public bool Views { get; set; }

        DataSet dsEnterpriseProjectTypesDestino;
        DataSet dsEnterpriseProjectTypesOrigem;
        DataSet dsProjectListOrigem;
        DataSet dsProjectListDestino;

        public EtapaProcessamento etapaProcessamento { get; set; }
        public ProjectBase baseProject { get; set; }

        public string pwaDestino { get; set; }
        public string pwaOrigem { get; set; }
        AlertForm alert;
        int indexPageSelect = -1;
        #endregion

        int count = 0;
        int countCustom;
        #region Ctor
        public frmSincronizar()
        {
            InitializeComponent();
            dgCustomFieldsOrigem.AutoGenerateColumns = false;
            dgCustomFieldsDestino.AutoGenerateColumns = false;
            label4.BackColor = Color.Red;
            
            tbPrincipal.TabPages.RemoveAt(4);
            tbPrincipal.TabPages.RemoveAt(3);
            tbPrincipal.TabPages.RemoveAt(2);
            tbPrincipal.TabPages.RemoveAt(1);
            //tbPrincipal.TabPages.RemoveAt(0);
            
        }
        #endregion

        #region Metodos Project.
        /// <summary>
        /// Marcando custom fields já existentes nos dois pwa destino e origem
        /// </summary>
        private void ObjectsExistsTarget()
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (DataGridViewRow item in dgCustomFieldsOrigem.Rows)
            {
                foreach (DataGridViewRow row in this.dgCustomFieldsDestino.Rows)
                {
                    if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                    {
                        //item.DefaultCellStyle.ForeColor = Color.White;
                        //row.DefaultCellStyle.ForeColor = Color.White;
                        
                        item.DefaultCellStyle.BackColor = Color.Red;
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }

            foreach (DataGridViewRow item in dgEnterpriseProjectTypeOrigem.Rows)
            {
                foreach (DataGridViewRow row in this.dgEnterpriseProjectTypeDestino.Rows)
                {
                    if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                    {
                        //item.DefaultCellStyle.ForeColor = Color.White;
                        //row.DefaultCellStyle.ForeColor = Color.White;

                        item.DefaultCellStyle.BackColor = Color.Red;
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }

            foreach (DataGridViewRow item in dgProjectsOrigem.Rows)
            {
                foreach (DataGridViewRow row in this.dgProjectsDestino.Rows)
                {
                    if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                    {
                        //item.DefaultCellStyle.ForeColor = Color.White;
                        //row.DefaultCellStyle.ForeColor = Color.White;

                        item.DefaultCellStyle.BackColor = Color.Red;
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            foreach (DataGridViewRow item in dgViewsResumo.Rows)
            {
                foreach (DataGridViewRow row in this.dgViewsDestino.Rows)
                {
                    if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                    {
                        //item.DefaultCellStyle.ForeColor = Color.White;
                        //row.DefaultCellStyle.ForeColor = Color.White;

                        item.DefaultCellStyle.BackColor = Color.Red;
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Carregando os CustomFields e LookupTables pwa destino e origem
        /// </summary>
        private void loadCustomFieldsLookupTables()
        {
            
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataSet dsDestino = this.baseProject.GetCustomfields(pwaDestino, true);
                DataSet dsOrigem = this.baseProject.GetCustomfields(pwaOrigem, true);

                DataTable dtDestino = this.baseProject.DtGridView(1, null);
                foreach (DataRow item in dsDestino.Tables[0].Rows)
                {
                    
                    DataRow oRow = dtDestino.NewRow();
                    oRow["TIPO_PROP"] = item["MD_ENT_TYPE_UID"];
                    oRow["MD_PROP_NAME"] = item["MD_PROP_NAME"];
                    foreach (DataRow itemLookup in dsDestino.Tables[1].Rows)
                    {
                        if (itemLookup["LT_UID"].ToString() == item["MD_LOOKUP_TABLE_UID"].ToString())
                        {
                            oRow["LT_NAME"] = itemLookup["LT_NAME"];
                            break;
                        }
                    }
                    
                    oRow["MD_PROP_FORMULA"] = item["MD_PROP_FORMULA"];

                    dtDestino.Rows.Add(oRow);
                }

                DataTable dtOrgiem = this.baseProject.DtGridView(1,null);
                foreach (DataRow item in dsOrigem.Tables[0].Rows)
                {

                    DataRow oRow = dtOrgiem.NewRow();
                    oRow["TIPO_PROP"] = item["MD_ENT_TYPE_UID"];
                    oRow["MD_PROP_NAME"] = item["MD_PROP_NAME"];
                    foreach (DataRow itemLookup in dsOrigem.Tables[1].Rows)
                    {
                        if (itemLookup["LT_UID"].ToString() == item["MD_LOOKUP_TABLE_UID"].ToString())
                        {
                            oRow["LT_NAME"] = itemLookup["LT_NAME"];
                            break;
                        }
                    }

                    oRow["MD_PROP_FORMULA"] = item["MD_PROP_FORMULA"];

                    dtOrgiem.Rows.Add(oRow);
                }

                dgCustomFieldsDestino.DataSource = dtDestino; //dsDestino.Tables[0];
                dgCustomFieldsOrigem.DataSource = dtOrgiem;

                this.lblttotalTarget.Text = string.Format("Total CustomFields: {0}", dsDestino.Tables[0].Rows.Count);
                this.lblttotalSource.Text = string.Format("Total CustomFields: {0}", dsOrigem.Tables[0].Rows.Count);

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                throw ex;
            }
        }

        /// <summary>
        /// Carregando enterprise project types
        /// </summary>
        private void loadEnterpriseProjectTypes()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                dsEnterpriseProjectTypesDestino = this.baseProject.GetEnterpriseProjectTypeList(pwaDestino);
                dsEnterpriseProjectTypesOrigem = this.baseProject.GetEnterpriseProjectTypeList(pwaOrigem);

                foreach (var item in dsEnterpriseProjectTypesOrigem.Tables)
                {
                    this.cbWorflowTables.Items.Add(item.ToString());
                }

                if (this.cbWorflowTables.Items.Count > 0)
                    this.cbWorflowTables.SelectedIndex = 0;

                dgEnterpriseProjectTypeOrigem.DataSource = dsEnterpriseProjectTypesOrigem.Tables[cbWorflowTables.SelectedIndex];
                dgEnterpriseProjectTypeDestino.DataSource = dsEnterpriseProjectTypesDestino.Tables[cbWorflowTables.SelectedIndex];

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Carregando lista de projetos
        /// </summary>
        private void loadProjectsList()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                dsProjectListDestino = this.baseProject.GetProjectsList(pwaDestino);
                dsProjectListOrigem = this.baseProject.GetProjectsList(pwaOrigem);

                dgProjectsOrigem.DataSource = dsProjectListOrigem.Tables[0];
                dgProjectsDestino.DataSource = dsProjectListDestino.Tables[0];

                lblTotalProjectsOrigem.Text = string.Format("Total: {0}", dsProjectListOrigem.Tables[0].Rows.Count);
                lblTotalProjectsDestino.Text = string.Format("Total: {0}", dsProjectListDestino.Tables[0].Rows.Count);

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Contagem de total de registros de objetos selecionados.
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        private int CountObjectsExistsTarget(int tabIndex)
        {
            int countExite = 0;
            if (tabIndex == 0)
            {
                foreach (DataGridViewRow item in dgEnterpriseProjectTypeOrigem.SelectedRows)
                    foreach (DataGridViewRow row in this.dgEnterpriseProjectTypeDestino.Rows)
                        if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                            countExite++;
            }
            else if (tabIndex == 1)
            {
                foreach (DataGridViewRow item in dgCustomFieldsOrigem.SelectedRows)
                    foreach (DataGridViewRow row in this.dgCustomFieldsDestino.Rows)
                        if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                            countExite++;
            }
            else if (tabIndex == 2)
            {
                foreach (DataGridViewRow item in dgProjectsOrigem.SelectedRows)
                    foreach (DataGridViewRow row in this.dgProjectsDestino.Rows)
                        if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                            countExite++;
            }
            else if (tabIndex == 3)
            {
                foreach (DataGridViewRow item in dgViewsOrigem.SelectedRows)
                    foreach (DataGridViewRow row in this.dgViewsOrigem.Rows)
                        if (item.Cells[1].Value.ToString() == row.Cells[1].Value.ToString())
                            countExite++;
            }


            return countExite;
        }

        #endregion

        #region Metodos e Eventos do Formulario

        private void loadSelectedObjects()
        {
            this.lblTotalCustomFieldsSelect.Text = string.Empty;
            this.lblTotalEnterpriseProjectSelect.Text = string.Empty;
            this.lblTotalProjectSelected.Text = string.Empty;
            this.lblTotalViewsSelect.Text = string.Empty;

            #region Enterprise Project Types
            if (dgEnterpriseProjectTypeOrigem.SelectedRows.Count > 0)
            {
                DataTable dtResumo = this.baseProject.DtGridView(0, dsEnterpriseProjectTypesOrigem.Tables[cbWorflowTables.SelectedIndex]);
               
                int i = 0;
                foreach (DataGridViewRow row in dgEnterpriseProjectTypeOrigem.SelectedRows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.Red)
                        continue;

                    dtResumo.Rows.Add(dtResumo.NewRow());
                    for (int j = 0; j < dgCustomFieldsOrigem.ColumnCount; ++j)
                    {
                        dtResumo.Rows[i][j] = row.Cells[j].Value;
                    }
                    ++i;
                }

                dgEnterpriseProjectTypeResumo.DataSource = dtResumo;
                this.lblTotalEnterpriseProjectSelect.Text = string.Format("Selecionados:{0}", dgEnterpriseProjectTypeResumo.Rows.Count);
            }
            #endregion

            #region CustomFields
            if (dgCustomFieldsOrigem.SelectedRows.Count > 0)
            {
                DataTable dtResumo = this.baseProject.DtGridView(1,null);
                int i = 0;
                foreach (DataGridViewRow row in dgCustomFieldsOrigem.SelectedRows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.Red)
                        continue;

                    dtResumo.Rows.Add(dtResumo.NewRow());
                    for (int j = 0; j < dgCustomFieldsOrigem.ColumnCount; ++j)
                    {
                        dtResumo.Rows[i][j] = row.Cells[j].Value;
                    }
                    ++i;
                }

                dgCustomFieldsResumo.DataSource = dtResumo;

                this.lblTotalCustomFieldsSelect.Text = string.Format("Selecionados:{0}", dgCustomFieldsResumo.Rows.Count);
            }
            #endregion

            #region Projects
            if (dgProjectsOrigem.SelectedRows.Count > 0)
            {
                DataTable dtResumo = this.baseProject.DtGridView(0, dsProjectListOrigem.Tables[0]); 
                int i = 0;
                foreach (DataGridViewRow row in dgProjectsOrigem.SelectedRows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.Red)
                        continue;

                    dtResumo.Rows.Add(dtResumo.NewRow());
                    for (int j = 0; j < dgProjectsOrigem.ColumnCount; ++j)
                    {
                        dtResumo.Rows[i][j] = row.Cells[j].Value;
                    }
                    ++i;
                }

                dgProjectsResumo.DataSource = dtResumo;
                this.lblTotalProjectSelected.Text = string.Format("Selecionados:{0}", dgProjectsResumo.Rows.Count);
            }
            #endregion

            #region Views
            //this.lblTotalViewsSelect.Text = string.Format("Selecionados:{0}", dgViewsResumo.Rows.Count);
            #endregion
        }

        /// <summary>
        /// Load do Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmSincronizar_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            loadEnterpriseProjectTypes();
            
            this.Text = string.Format("Segplan | Migração Pwa Origem: {0} Pwa Destino: {1}", pwaOrigem, pwaDestino);
            this.lblTotalCustomFieldsSelect.Text = string.Empty;
            this.lblTotalEnterpriseProjectSelect.Text = string.Empty;
            this.lblTotalProjectSelected.Text = string.Empty;
            this.lblTotalViewsSelect.Text = string.Empty;
            this.cbWorflowTables.SelectedIndex = 8;
            this.cbWorflowTables.Enabled = false;

            //if (this.Ept)
            //    tbPrincipal.TabPages.Add(tbProjectTypes);
            //if (this.CustomFields)
            //    tbPrincipal.TabPages.Add(tbCustomFields);
            //if (this.Projects)
            //    tbPrincipal.TabPages.Add(tbProjects);
            //if (this.Views)
            //    tbPrincipal.TabPages.Add(tbViews);

            
            Cursor.Current = Cursors.Default;
        }

      
        private void dgSource_Click(object sender, EventArgs e)
        {
            this.ckPularCustomFields.Enabled = true;
            if (dgCustomFieldsOrigem.SelectedRows.Count > 0)
            {
                int existe = CountObjectsExistsTarget(1);

                this.lblCustomfildSelects.Text = string.Format("Selecionado(s): {0}", dgCustomFieldsOrigem.SelectedRows.Count - existe);
                this.btnAvancar.Enabled = true;
                this.ckPularCustomFields.Enabled = false;
            }
            else
            {
                this.lblCustomfildSelects.Text = string.Empty;
                this.btnAvancar.Enabled = false;
            }
        }

        private void dgSource_Sorted(object sender, EventArgs e)
        {
            ObjectsExistsTarget();
        }

        private void dgTarget_Sorted(object sender, EventArgs e)
        {
            ObjectsExistsTarget();
        }

        private void Anterior(int tabIndex)
        {
            btnAvancar.Text = "Avançar";

            if (ckPularEnterpriseProjectTypes.Checked || ckPularCustomFields.Checked 
                || ckPularViews.Checked || ckPularProjects.Checked)
                btnAvancar.Enabled = true;
            
            if (tabIndex - 1 == 0)
                btnAnterior.Enabled = false;

            if (tabIndex != 0) {
                tbPrincipal.SelectedIndex = tabIndex - 1;
                tbPrincipal.TabPages.RemoveAt(tabIndex);
            }
        }

        private void Avancar(int tabIndex)
        {
            if (tabIndex > 0)
                btnAnterior.Enabled = true;

            btnAvancar.Text = "Avançar";
            
            btnAvancar.Enabled = false;

            switch (tabIndex)
            {
                case 0: // enterprise project types
                    tbPrincipal.SelectedIndex = tabIndex;
                    btnAvancar.Enabled = ckPularEnterpriseProjectTypes.Checked;
                    
                    break;
                case 1: // customfields
                    tbPrincipal.TabPages.Add(tbCustomFields);
                    tbPrincipal.SelectedIndex = tabIndex;
                    btnAvancar.Enabled = ckPularCustomFields.Checked;
                    
                    loadCustomFieldsLookupTables();
                    ObjectsExistsTarget();

                    break;
                case 2: // projects
                    tbPrincipal.TabPages.Add(tbProjects);
                    tbPrincipal.SelectedIndex = tabIndex;
                    btnAvancar.Enabled = ckPularProjects.Checked;
                    loadProjectsList();
                    break;
                case 3: // views
                    tbPrincipal.TabPages.Add(tbViews);
                    tbPrincipal.SelectedIndex = tabIndex;
                    btnAvancar.Enabled = ckPularViews.Checked;
                    break;
                case 4: // resumo

                    if (dgEnterpriseProjectTypeOrigem.SelectedRows.Count == 0 && dgProjectsOrigem.SelectedRows.Count == 0 
                        && dgCustomFieldsOrigem.SelectedRows.Count == 0 && dgViewsOrigem.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Não foi selecionado nenhum objeto para migração!", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }

                    tbPrincipal.TabPages.Add(tbResumo);
                    tbPrincipal.SelectedIndex = tabIndex;

                    loadSelectedObjects();

                    if (dgViewsResumo.Rows.Count == 0 && tbResumoObjects.TabPages.Contains(tabPage4))
                        tbResumoObjects.TabPages.RemoveAt(3);

                    if (dgProjectsResumo.Rows.Count == 0 && tbResumoObjects.TabPages.Contains(tabPage3))
                        tbResumoObjects.TabPages.RemoveAt(2);

                    if (dgCustomFieldsResumo.Rows.Count == 0 && tbResumoObjects.TabPages.Contains(tabPage2))
                        tbResumoObjects.TabPages.RemoveAt(1);

                    if (dgEnterpriseProjectTypeResumo.Rows.Count == 0 && tbResumoObjects.TabPages.Contains(tabPage1))
                        tbResumoObjects.TabPages.RemoveAt(0);

                    btnAvancar.Enabled = true;
                    btnAvancar.Text = "Processar";

                    break;
                default:
                    var option = MessageBox.Show("Deseja sincronizar os objetos selecionados?", "Migrate", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    if (option == System.Windows.Forms.DialogResult.OK)
                    {
                        if (bkWorker.IsBusy != true)
                        {
                            // create a new instance of the alert form
                            alert = new AlertForm();
                            // event handler for the Cancel button in AlertForm
                            alert.Canceled += new EventHandler<EventArgs>(btnCancelar_Click);
                            alert.ProgressValueMax = 100;
                            alert.Show();
                            // Start the asynchronous operation.
                            int totalRegistros = this.dgCustomFieldsResumo.Rows.Count + this.dgEnterpriseProjectTypeResumo.Rows.Count + this.dgProjectsResumo.Rows.Count + this.dgViewsResumo.Rows.Count;
                            countCustom = 100 / totalRegistros;
                            bkWorker.RunWorkerAsync();
                        }
                    }
                    break;
            }
        }

        private void btnAvancar_Click(object sender, EventArgs e)
        {

            if (tbPrincipal.SelectedIndex == 0 && tbPrincipal.TabPages.Contains(tbProjectTypes))
            {

            }
            if (tbPrincipal.SelectedIndex == 0 && tbPrincipal.TabPages.Contains(tbProjectTypes))
            {

            }        

            int index = tbPrincipal.SelectedIndex + 1;

            Avancar(index);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (bkWorker.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                bkWorker.CancelAsync();
                // Close the AlertForm
                alert.Close();
            }
        }

        private void dgSource_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void tbPrincipal_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            Anterior(tbPrincipal.SelectedIndex);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbWorflowTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgEnterpriseProjectTypeOrigem.DataSource = dsEnterpriseProjectTypesOrigem.Tables[cbWorflowTables.SelectedIndex];
            dgEnterpriseProjectTypeDestino.DataSource = dsEnterpriseProjectTypesDestino.Tables[cbWorflowTables.SelectedIndex];

            this.lblTotalEnterpriseProjectTypeOrigem.Text = string.Format("Total {0}: {1}", cbWorflowTables.SelectedText, dsEnterpriseProjectTypesOrigem.Tables[cbWorflowTables.SelectedIndex].Rows.Count);
            this.lblTotalEnterpriseProjectTypesDestino.Text = string.Format("Total {0}: {1}", cbWorflowTables.SelectedText, dsEnterpriseProjectTypesDestino.Tables[cbWorflowTables.SelectedIndex].Rows.Count);
            ObjectsExistsTarget();
        }

        private void dgEnterpriseProjectTypeOrigem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            ckPularEnterpriseProjectTypes.Enabled = true;
            string PDPs = string.Empty;
            
            foreach (DataGridViewRow item in dgEnterpriseProjectTypeOrigem.SelectedRows)
            {
                txtNameProjectType.Visible = true;
                ckNameProjectType.Visible = true;
                txtNameProjectType.Text = item.Cells[1].Value.ToString();
                ckPularEnterpriseProjectTypes.Enabled = false;
                PDPs = this.baseProject.VerifyPDPExist(pwaOrigem, pwaDestino, Guid.Parse(item.Cells[0].Value.ToString()));
            }

            if (dgEnterpriseProjectTypeOrigem.SelectedRows.Count > 0)
            {
                int existe = CountObjectsExistsTarget(0);

                if (PDPs.Length > 0)
                    MessageBox.Show(string.Format("O objeto selecionado: {0}, não possui as Paginas de Detalhe de Projeto (PDPs): {1} no Destino! Caso selecionado sera considerada uma nova PDPs que seja existente no destino.", this.cbWorflowTables.SelectedItem.ToString(), PDPs), "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.lblEnterpriseProjectTypesSelect.Text = string.Format("Selecionado(s): {0}", this.dgEnterpriseProjectTypeOrigem.SelectedRows.Count - existe);
                this.btnAvancar.Enabled = true;
            }
            else
            {
                this.lblEnterpriseProjectTypesSelect.Text = string.Empty;
                this.btnAvancar.Enabled = false;
            }
            Cursor.Current = Cursors.Default;
        }
        private void dgViewsOrigem_Click(object sender, EventArgs e)
        {
            this.ckPularViews.Enabled = true;
            if (dgViewsOrigem.SelectedRows.Count > 0)
            {
                int existe = CountObjectsExistsTarget(3);
                this.lblTotalViewsSelect.Text = string.Format("Selecionado(s): {0}", this.dgViewsOrigem.SelectedRows.Count - existe);
                this.ckPularViews.Enabled = false;
            }
            else
            {
                this.lblTotalViewsSelect.Text = string.Empty;
            }
        }
        private void ckNameProjectType_CheckedChanged(object sender, EventArgs e)
        {
            txtNameProjectType.Enabled = ckNameProjectType.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            btnAvancar.Enabled = ckPularEnterpriseProjectTypes.Checked;
        }

        private void ckCustomFields_CheckedChanged(object sender, EventArgs e)
        {
            btnAvancar.Enabled = ckPularCustomFields.Checked;
        }

        private void ckPularViews_CheckedChanged(object sender, EventArgs e)
        {
            btnAvancar.Enabled = ckPularViews.Checked;
        }

        private void ckPularProjects_CheckedChanged(object sender, EventArgs e)
        {
            btnAvancar.Enabled = ckPularProjects.Checked;
        }

        private void dgProjectsOrigem_Click(object sender, EventArgs e)
        {
            this.ckPularProjects.Enabled = true;
            if (dgProjectsOrigem.SelectedRows.Count > 0)
            {
                int existe = CountObjectsExistsTarget(2);

                this.lblTotalProjectsSelect.Text = string.Format("Selecionado(s): {0}", this.dgProjectsOrigem.SelectedRows.Count - existe);
                this.btnAvancar.Enabled = true;
                this.ckPularProjects.Enabled = false;
            }
            else
            {
                this.lblTotalProjectsSelect.Text = string.Empty;
                this.btnAvancar.Enabled = false;
            }
        }
        #endregion

        #region Metodos do Processamento em Background
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
                // Perform a time consuming operation and report progress.
            foreach (DataGridViewRow row in this.dgEnterpriseProjectTypeResumo.Rows)
            {
                etapaProcessamento = EtapaProcessamento.EPT;
                count++;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                string nameEPT = ckNameProjectType.Checked ? txtNameProjectType.Text.Trim() : string.Empty;
                
                this.baseProject.CreateEnterpriseProjectType(pwaOrigem, pwaDestino,nameEPT, Guid.Parse(row.Cells[0].Value.ToString()));

                worker.ReportProgress(countCustom * count);

                System.Threading.Thread.Sleep(1000);
            }

            foreach (DataGridViewRow row in this.dgCustomFieldsResumo.Rows)
            {
                etapaProcessamento = EtapaProcessamento.CustomFields;
                count++;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                if (row.Cells[2].Value.ToString() != string.Empty)
                {
                    this.baseProject.CreateLooukupTable(pwaOrigem, pwaDestino, row.Cells[2].Value.ToString());
                }

                this.baseProject.CreateCustomField(pwaOrigem, pwaDestino, row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());

                worker.ReportProgress(countCustom * count);

                System.Threading.Thread.Sleep(1000);
            }

            foreach (DataGridViewRow row in this.dgProjectsResumo.Rows)
            {
                etapaProcessamento = EtapaProcessamento.Projects;
                count++;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                this.baseProject.CreateProject(pwaOrigem, pwaDestino, Guid.Parse(row.Cells[0].Value.ToString()));

                worker.ReportProgress(countCustom * count);

                System.Threading.Thread.Sleep(1000);

            }


            foreach (DataGridViewRow row in this.dgViewsResumo.Rows)
            {
                etapaProcessamento = EtapaProcessamento.Views;
                count++;
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                ///Write code Migrate

                worker.ReportProgress(countCustom * count);

                System.Threading.Thread.Sleep(1000);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (etapaProcessamento)
            {
                case EtapaProcessamento.EPT:
                    alert.Message = "Processando EPT, por favor aguarde... " + e.ProgressPercentage.ToString() + "%";
                    break;
                case EtapaProcessamento.CustomFields:
                    alert.Message = "Processando CustomFields, por favor aguarde... " + e.ProgressPercentage.ToString() + "%";
                    break;
                case EtapaProcessamento.Projects:
                    alert.Message = "Processando Projects, por favor aguarde... " + e.ProgressPercentage.ToString() + "%";
                    break;
                case EtapaProcessamento.Views:
                    alert.Message = "Processando Views, por favor aguarde... " + e.ProgressPercentage.ToString() + "%";
                    break;
                default:
                    break;
            }

            alert.ProgressValue = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("O Processamento foi cancelado!","Migrate",  MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {
                MessageBox.Show(string.Format("Ocorreu erro noprocessamento: {0}",e.Error.Message), "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            else
            {
                MessageBox.Show("O processamento foi concluido com sucesso!", "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            alert.Close();
            this.Close();
        }
        #endregion

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgCustomFieldsDestino.SelectedRows)
            {
                this.baseProject.DeleteCustomField(pwaDestino, row.Cells[1].Value.ToString());
                break;
            }
        }
    }
}
