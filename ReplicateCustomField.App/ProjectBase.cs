using Microsoft.ProjectServer.Client;
using Microsoft.SharePoint.Client;
using SvcCustomFields;
using SvcLookupTable;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using PSLibrary = Microsoft.Office.Project.Server.Library;
namespace Segplan.ReplicateCustomField.App
{
    public class ProjectBase
    {
        #region Fields
        public ClientContext clientContext;

        public string User { get; set; }
        public string PassWord { get; set; }
        public string Domain { get; set; }

        private BasicHttpBinding binding;
        private BasicHttpBinding Binding
        {
            get
            {
                const int MAXSIZE = 500000000;
                binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Name = "basicHttpConf";
                binding.SendTimeout = TimeSpan.MaxValue;
                binding.MaxReceivedMessageSize = MAXSIZE;
                binding.ReaderQuotas.MaxNameTableCharCount = MAXSIZE;
                binding.MessageEncoding = WSMessageEncoding.Text;
                binding.OpenTimeout = new TimeSpan(0, 1, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);

                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                binding.Security.Transport.Realm = "";

                return binding;
            }
        }

        private DataSet dsGenerico;
        SvcLookupTable.LookupTableDataSet dsLookupTableOrigem;
        SvcLookupTable.LookupTableDataSet dsLookupTableDestino;
  
        #endregion

        #region Construtor
        public ProjectBase()
        {
           
        }

        #endregion

        #region Metodos Project
        
        public bool LogoffPS(string baseUrl)
        {
            bool loggedOff = true;
            try
            {
                const string LOGINWINDOWS = "/_vti_bin/PSI/LoginWindows.asmx";

                WebLoginReference.LoginWindowsSoapClient clientLogin = new WebLoginReference.LoginWindowsSoapClient(Binding, endPointAdress(baseUrl, LOGINWINDOWS));
                clientLogin.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
                clientLogin.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                
                clientLogin.Logoff();
                loggedOff = true;
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Logon Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                loggedOff = false;
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Logoff Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                loggedOff = false;
            }
            
            return loggedOff;
        }

        public bool Logon(string baseUrl)
        {
            const string LOGINWINDOWS = "/_vti_bin/PSI/LoginWindows.asmx";
            bool logonSucceeded = false;

            ClientContext clientContext = new ClientContext(baseUrl);
            //var credentials = new NetworkCredential(this.User, this.PassWord, this.Domain);
            //clientContext.Credentials = credentials; //CredentialCache.DefaultCredentials;
            ////clientContext.RequestTimeout = timeOutConnection;
            //clientContext.Load(clientContext.Web);
            //clientContext.ExecuteQuery();
            //Logged = true;

            try
            {
               
                var endpoint = endPointAdress(baseUrl, LOGINWINDOWS);
                WebLoginReference.LoginWindowsSoapClient clientLogin = new WebLoginReference.LoginWindowsSoapClient(Binding, endpoint);
                clientLogin.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
                clientLogin.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                //SvcLoginWindows.LoginWindows login = new SvcLoginWindows.LoginWindows();
                //login.Url = endpoint.ToString();

                if (clientLogin.Login()) logonSucceeded = true;

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Logon Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Logon Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return logonSucceeded;
        }

        internal CookieContainer GetLogonCookie(string baseUrl)
        {
            // Create an instance of the loginWindows object.
            SvcLoginWindows.LoginWindows loginWindows = new SvcLoginWindows.LoginWindows();
            //http://epmprod03/escritoriodeprojetos/_vti_bin/PSI/LoginWindows.asmx?wsdl
            loginWindows.Url = baseUrl + "/_vti_bin/PSI/LoginWindows.asmx";
            loginWindows.Credentials = CredentialCache.DefaultCredentials;

            loginWindows.CookieContainer = new CookieContainer();

            if (!loginWindows.Login())
            {
                // Login failed; throw an exception.
                throw new UnauthorizedAccessException("Login failed.");
            }
            return loginWindows.CookieContainer;
        }

        internal EndpointAddress endPointAdress(string pwaUrl, string svcRouter)
        {
            // The endpoint address is the ProjectServer.svc router for all public PSI calls.
            return new EndpointAddress(pwaUrl + svcRouter);
        }

        

        private void CreateDepartament(string pwaOrigem, string pwaDestino)
        {
            SvcResource.ResourceClient client = new SvcResource.ResourceClient(Binding, endPointAdress(pwaOrigem, "/_vti_bin/psi/Resource.asmx"));
            SvcResourcePlan.ResourcePlanClient clientPlan = new SvcResourcePlan.ResourcePlanClient(Binding, endPointAdress(pwaOrigem, "/_vti_bin/psi/Resource.asmx"));
            
            //adminClient.re
        }

        public void DeleteCustomField(string target, string field)
        {
            CustomFieldDataSet cfDS = new CustomFieldDataSet();
            var RowField = this.dsGenerico.Tables[0].Select("MD_PROP_NAME ='" + field + "'").CopyToDataTable();
            
            SvcCustomFields.CustomFieldsClient fieldTarget = new CustomFieldsClient(Binding, endPointAdress(target, "/_vti_bin/psi/CustomFields.asmx"));
            fieldTarget.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
            Guid[] fieldsArray = new Guid[1];
            //fieldsArray[0] =
            fieldTarget.DeleteCustomFields(fieldsArray);
        }

        /// <summary>
        /// Metodo para criar o Custom Field no Detino
        /// </summary>
        /// <param name="source">Origem</param>
        /// <param name="target">Destino</param>
        /// <param name="field">Nome CustomField</param>
        public void CreateCustomField(string source, string target, string field, string lookupTable)
        {
            CustomFieldDataSet cfDS = new CustomFieldDataSet();
            var RowField = this.dsGenerico.Tables[0].Select("MD_PROP_NAME ='" + field + "'").CopyToDataTable();
            
            SvcCustomFields.CustomFieldsClient fieldTarget = new CustomFieldsClient(Binding, endPointAdress(target, "/_vti_bin/psi/CustomFields.asmx"));
            fieldTarget.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
            
            Guid cfUid = Guid.NewGuid();

            foreach (DataRow item in RowField.Rows)
            {
                
                SvcCustomFields.CustomFieldDataSet.CustomFieldsRow cfRow = cfDS.CustomFields.NewCustomFieldsRow();

                foreach (DataColumn itemColumn in RowField.Columns)
                {
                    if (!cfDS.CustomFields.Columns.Contains(itemColumn.ToString()))
                        continue;

                    if (itemColumn.ToString() == "MD_LOOKUP_TABLE_UID" && lookupTable != string.Empty) 
                    {
                        var lt = GetLookupTable(target).Tables[0].Select("LT_NAME ='" + lookupTable + "'").CopyToDataTable();
                        Guid lt_Uid = Guid.Parse(lt.Rows[0]["LT_UID"].ToString());
                        cfRow[itemColumn.ToString()] = lt_Uid;
                        continue;
                    }

                    if (itemColumn.ToString().Equals("MD_PROP_UID"))
                    {
                        cfRow[itemColumn.ToString()] = cfUid;
                        continue;
                    }

                    if (itemColumn.ToString().Equals("MD_PROP_DEFAULT_VALUE"))
                    {
                        continue;
                    }

                    if (item[itemColumn.ToString()] != null && !itemColumn.ReadOnly)
                        cfRow[itemColumn.ToString()] = item[itemColumn.ToString()];
                }
                cfDS.CustomFields.Rows.Add(cfRow);
            }

            try
            {
                bool validateOnly = false;
                bool autoCheckIn = true;
                fieldTarget.CreateCustomFields(cfDS, validateOnly, autoCheckIn);
            }
            catch (SoapException ex)
            {
                string errMess = "";
                PSLibrary.PSClientError psiError = new PSLibrary.PSClientError(ex);
                PSLibrary.PSErrorInfo[] psiErrors = psiError.GetAllErrors();

                for (int j = 0; j < psiErrors.Length; j++)
                {
                    errMess += psiErrors[j].ErrId.ToString() + "\n";
                }
                errMess += "\n" + ex.Message.ToString();
                throw ex;
            }
        }

        /// <summary>
        /// Metodo para Carregar LookupTables
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal LookupTableDataSet GetLookupTable(string source)
        {
            SvcLookupTable.LookupTableDataSet dsLookup = new SvcLookupTable.LookupTableDataSet();
            try
            {
                SvcLookupTable.LookupTableClient clientLookup = new SvcLookupTable.LookupTableClient(Binding, endPointAdress(source, "/_vti_bin/psi/LookupTable.asmx"));
                clientLookup.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
                return clientLookup.ReadLookupTables(null, false, 1033);
            }
            catch (SoapException ex)
            {
                throw ex;
            }
            
            //PSLibrary.Filter cfFilter = new Microsoft.Office.Project.Server.Library.Filter();
            //cfFilter.FilterTableName = dsLookup.LookupTables.TableName;


            //foreach (var item in dsLookup.LookupTables.Columns)
            //{
            //    cfFilter.Fields.Add(new PSLibrary.Filter.Field(dsLookup.LookupTables.TableName, item.ToString()));
            //}

            //var fields = cfFilter.GetXml();

            
            //return clientLookup.ReadLookupTables(cfFilter.GetXml(), false, 0);

        }

        /// <summary>
        /// Metodo para criar as Lookuptable e suas dependencias
        /// </summary>
        /// <param name="pwaDestino">Destino</param>
        /// <param name="itemRow">Item CustomField</param>
        /// <param name="ds">Data Set LookupTable</param>
        public void CreateLooukupTable(string pwaOrigem, string pwaDestino, string pLT_NAME)
        {
            try
            {
                //verificando se já existe a lookuptable no destino
                dsLookupTableDestino = GetLookupTable(pwaDestino);

                DataRow[] dataRowDestino = dsLookupTableDestino.Tables[0].Select("LT_NAME = '" + pLT_NAME + "'");

                if (dataRowDestino.Count() > 0)
                    return;

                if (dsLookupTableOrigem == null)
                    dsLookupTableOrigem = GetLookupTable(pwaOrigem);

                DataRow[] dataRow = dsLookupTableOrigem.Tables[0].Select("LT_NAME = '" + pLT_NAME + "'");

                SvcLookupTable.LookupTableDataSet ltDS = new LookupTableDataSet();

                SvcLookupTable.LookupTableClient clientLookupTable = new LookupTableClient(Binding, endPointAdress(pwaDestino, "/_vti_bin/psi/LookupTable.asmx"));
                clientLookupTable.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);

                foreach (SvcLookupTable.LookupTableDataSet.LookupTablesRow item in dataRow)
                {
                    SvcLookupTable.LookupTableDataSet.LookupTablesRow ltRow = ltDS.LookupTables.NewLookupTablesRow();
                    Guid LT_UID = Guid.NewGuid();

                    ltRow.LT_UID = LT_UID;
                    
                    ltRow.LT_FILL_ALL_LEVELS = item.LT_FILL_ALL_LEVELS;
                    ltRow.LT_NAME = item.LT_NAME;

                    if (!item.IsAPP_ENTITY_UIDNull())
                        ltRow.APP_ENTITY_UID = item.APP_ENTITY_UID;

                    if (!item.IsLT_PRIMARY_LCIDNull())
                        ltRow.LT_PRIMARY_LCID = item.LT_PRIMARY_LCID;

                    if (!item.IsLT_SORT_ORDER_ENUMNull())
                        ltRow.LT_SORT_ORDER_ENUM = item.LT_SORT_ORDER_ENUM;

                    ltDS.LookupTables.Rows.Add(ltRow);


                    foreach (SvcLookupTable.LookupTableDataSet.LookupTableMasksRow itemMaskRow in dsLookupTableOrigem.LookupTableMasks.Select("LT_UID = '" + item.LT_UID + "'"))
                    {
                        SvcLookupTable.LookupTableDataSet.LookupTableMasksRow ltMasksRow = ltDS.LookupTableMasks.NewLookupTableMasksRow();
                        foreach (DataColumn itemColumn in dsLookupTableOrigem.LookupTableMasks.Columns)
                        {
                            if (!ltDS.LookupTableMasks.Columns.Contains(itemColumn.ToString()))
                                continue;

                            if (itemColumn.ToString() == "LT_UID")
                            {
                                ltMasksRow[itemColumn.ToString()] = LT_UID;
                                continue;
                            }

                            if (itemMaskRow[itemColumn.ToString()] != null && !itemColumn.ReadOnly)
                                ltMasksRow[itemColumn.ToString()] = itemMaskRow[itemColumn.ToString()];
                        }

                        //ltMasksRow.LT_MASK_STRUCT_LEVEL = itemMaskRow.LT_MASK_STRUCT_LEVEL;
                        //ltMasksRow.LT_MASK_STRUCT_TYPE_ENUM = itemMaskRow.LT_MASK_STRUCT_TYPE_ENUM;
                        //ltMasksRow.LT_MASK_STRUCT_LENGTH = itemMaskRow.LT_MASK_STRUCT_LENGTH;
                        //ltMasksRow.LT_MASK_VALUE_SEPARATOR = itemMaskRow.LT_MASK_VALUE_SEPARATOR;

                        ltDS.LookupTableMasks.Rows.Add(ltMasksRow);
                    }

                    

                    foreach (SvcLookupTable.LookupTableDataSet.LookupTableTreesRow itemTreesRow in dsLookupTableOrigem.LookupTableTrees.Select("LT_UID = '" + item.LT_UID + "'"))
                    {
                        //ltTreesRow.LT_UID = ltDS.LookupTables[0].LT_UID;
                        SvcLookupTable.LookupTableDataSet.LookupTableTreesRow ltTreesRow = ltDS.LookupTableTrees.NewLookupTableTreesRow();
                        foreach (DataColumn itemColumn in dsLookupTableOrigem.LookupTableTrees.Columns)
                        {
                            if (!ltDS.LookupTableTrees.Columns.Contains(itemColumn.ToString()))
                                continue;

                            //if (itemColumn.ToString() == "LT_STRUCT_UID")
                            //{
                            //    ltTreesRow[itemColumn.ToString()] = Guid.NewGuid();
                            //    continue;
                            //}

                            if (itemColumn.ToString() == "LT_UID")
                            {
                                ltTreesRow[itemColumn.ToString()] = LT_UID;
                                continue;
                            }


                            if (itemTreesRow[itemColumn.ToString()] != null && !itemColumn.ReadOnly)
                                ltTreesRow[itemColumn.ToString()] = itemTreesRow[itemColumn.ToString()];
                        }

                        ltDS.LookupTableTrees.Rows.Add(ltTreesRow);
                    }

                    try
                    {
                        bool validateOnly = false;
                        bool autoCheckIn = true;

                        clientLookupTable.CreateLookupTables(ltDS, validateOnly, autoCheckIn);
                    }
                    catch (SoapException ex)
                    {
                        string errMess = "";
                        // Pass the exception to the PSClientError constructor to 
                        // get all error information.
                        PSLibrary.PSClientError psiError = new PSLibrary.PSClientError(ex);
                        PSLibrary.PSErrorInfo[] psiErrors = psiError.GetAllErrors();

                        for (int j = 0; j < psiErrors.Length; j++)
                        {
                            errMess += psiErrors[j].ErrId.ToString() + "\n";
                        }
                        errMess += "\n" + ex.Message.ToString();

                        MessageBox.Show(errMess);
                        // Send error string to console or message box.
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Metodo para Buscar os CustomFields e LookupTable pelo metodo internal (GetLookupTable)
        /// </summary>
        /// <param name="pwaUrl"></param>
        /// <returns></returns>
        public DataSet GetCustomfields(string pwaUrl, bool getLookupTable)
        {
            //CustomFieldDataSet cfDS = new CustomFieldDataSet();
            //PSLibrary.Filter cfFilter = new Microsoft.Office.Project.Server.Library.Filter();
            //cfFilter.FilterTableName = cfDS.CustomFields.TableName;

            //foreach (var item in cfDS.CustomFields.Columns)
            //{
            //    cfFilter.Fields.Add(new PSLibrary.Filter.Field(cfDS.CustomFields.TableName, item.ToString()));
            //}

            
            //cfFilter.Fields.Add(new PSLibrary.Filter.Field(cfDS.CustomFields.TableName, cfDS.CustomFields.MD_PROP_IS_REQUIREDColumn.ColumnName));
            //cfFilter.Fields.Add(new PSLibrary.Filter.Field(cfDS.CustomFields.TableName, cfDS.CustomFields.MD_PROP_IS_REQUIREDColumn.ColumnName));
            
            //var fields = cfFilter.GetXml();
            try
            {
                SvcCustomFields.CustomFieldsClient customFieldClient = new CustomFieldsClient(Binding, endPointAdress(pwaUrl, "/_vti_bin/psi/CustomFields.asmx"));
                customFieldClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);

                dsGenerico = new DataSet();
                dsGenerico.Tables.Add(customFieldClient.ReadCustomFields(null, false).Tables[0].Copy());


                if (getLookupTable) {
                    DataSet dsLookupTableOrigem = this.GetLookupTable(pwaUrl);
                    if (dsLookupTableOrigem != null)
                    {
                        var table = dsLookupTableOrigem.Tables[0].Copy();
                        dsGenerico.Tables.Add(table);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsGenerico;
        }


        /// <summary>
        /// Busca lista WorkFlowDataSet
        /// </summary>
        /// <param name="pwaUrl">url do pwa</param>
        /// <returns></returns>
        public SvcWorkflow.WorkflowDataSet GetEnterpriseProjectTypeList(string pwaUrl)
        {
            //SvcProject.ProjectClient clientProject = new SvcProject.ProjectClient(Binding, endPointAdress(pwaUrl, "/_vti_bin/psi/Project.asmx"));
            SvcWorkflow.WorkflowClient clientWorkflow = new SvcWorkflow.WorkflowClient(Binding, endPointAdress(pwaUrl, "/_vti_bin/psi/Workflow.asmx"));
            clientWorkflow.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
            
            //clientLogin.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
            return clientWorkflow.ReadEnterpriseProjectTypeList();
        }

        /// <summary>
        /// Buca os Projetos
        /// </summary>
        /// <param name="pwaUrl">url do pwa</param>
        /// <returns></returns>
        public SvcProject.ProjectDataSet GetProjectsList(string pwaUrl)
        {
            SvcProject.ProjectClient clientProject = new SvcProject.ProjectClient(Binding, endPointAdress(pwaUrl, "/_vti_bin/psi/Workflow.asmx"));
            clientProject.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
            return clientProject.ReadProjectList();
        }

        public void CreateProject(string pwaOrigem, string pwaDestino, Guid projectUid)
        {
            try
            {
                SvcProject.ProjectClient clientProjectOrigem = new SvcProject.ProjectClient(Binding, endPointAdress(pwaOrigem, "/_vti_bin/psi/Workflow.asmx"));
                clientProjectOrigem.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
                SvcProject.ProjectClient clientProjectDestino = new SvcProject.ProjectClient(Binding, endPointAdress(pwaDestino, "/_vti_bin/psi/Workflow.asmx"));
                clientProjectDestino.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);

                SvcProject.ProjectDataSet dsProject = clientProjectOrigem.ReadProject(projectUid, SvcProject.DataStoreEnum.PublishedStore);
                SvcProject.ProjectDataSet dsProjectDestino = new SvcProject.ProjectDataSet();

                //var projectType = GetEnterpriseProjectTypeList(pwaDestino);
                var PROJ_UID = Guid.NewGuid();
                //informações do projeto
                foreach (SvcProject.ProjectDataSet.ProjectRow oRow in dsProject.Project.Rows)
                {
                    

                    SvcProject.ProjectDataSet.ProjectRow oRowDestino = dsProjectDestino.Project.NewProjectRow();
                    oRowDestino.PROJ_TYPE = (int)PSLibrary.Project.ProjectType.Project;
                    oRowDestino.PROJ_UID = PROJ_UID;
                    oRowDestino.PROJ_NAME = oRow.PROJ_NAME;

                    oRowDestino.WPROJ_DESCRIPTION = oRow.WPROJ_DESCRIPTION;

                    //if (oRow.IsPROJ_PROP_MANAGERNull() || oRow.PROJ_PROP_MANAGER.Length == 0)
                    //    oRowDestino.SetPROJ_PROP_MANAGERNull();
                    //else
                    //    oRowDestino.PROJ_PROP_MANAGER = oRow.PROJ_PROP_MANAGER;

                    if (oRow.IsPROJ_PROP_TITLENull() || oRow.PROJ_PROP_TITLE.Length == 0)
                        oRowDestino.SetPROJ_PROP_TITLENull();
                    else
                    
                        oRowDestino.PROJ_PROP_TITLE = oRow.PROJ_PROP_TITLE;

                    if (oRow.IsPROJ_PROP_SUBJECTNull() || oRow.PROJ_PROP_SUBJECT.Length == 0)
                        oRowDestino.SetPROJ_PROP_SUBJECTNull();
                    else
                        oRowDestino.PROJ_PROP_SUBJECT = oRow.PROJ_PROP_SUBJECT;

                    dsProjectDestino.Project.AddProjectRow(oRowDestino);
                }

                DataSet dsCustomField = this.GetCustomfields(pwaDestino, false);
                DataSet dsLookupTableDestino = this.GetLookupTable(pwaDestino);
                DataSet dsLookupTableOrigem = this.GetLookupTable(pwaOrigem);

                /// informações custom fields do projeto
                foreach (SvcProject.ProjectDataSet.ProjectCustomFieldsRow oRow in dsProject.ProjectCustomFields.Rows)
                {
                    SvcProject.ProjectDataSet.ProjectCustomFieldsRow oRowDestino = dsProjectDestino.ProjectCustomFields.NewProjectCustomFieldsRow();
                    CustomFieldDataSet.CustomFieldsRow oRowCustomFieldDestino = (CustomFieldDataSet.CustomFieldsRow)dsCustomField.Tables[0].Select(string.Format("MD_PROP_ID = '{0}'", oRow.MD_PROP_ID)).SingleOrDefault();

                    if (!oRowCustomFieldDestino.IsMD_PROP_FORMULANull())
                        continue;

                    

                    foreach (DataColumn itemColumn in dsProject.ProjectCustomFields.Columns)
                    {
                        if (!dsProjectDestino.ProjectCustomFields.Columns.Contains(itemColumn.ToString()))
                            continue;

                        //if (itemColumn.ColumnName.Equals("CUSTOM_FIELD_UID "))
                        //{
                        //    oRowDestino[itemColumn.ToString()] = Guid.NewGuid();
                        //    continue;
                        //}

                        if (itemColumn.ColumnName.Equals("PROJ_UID"))
                        {
                            oRowDestino[itemColumn.ToString()] = PROJ_UID;
                            continue;
                        }

                        if (itemColumn.ColumnName.Equals("MD_PROP_ID"))
                        {
                            oRowDestino[itemColumn.ToString()] = oRowCustomFieldDestino.MD_PROP_ID;
                            continue;
                        }

                        if (itemColumn.ColumnName.Equals("MD_PROP_UID"))
                        {
                            oRowDestino[itemColumn.ToString()] = oRowCustomFieldDestino.MD_PROP_UID;
                            continue;
                        }

                        if (itemColumn.ColumnName.Equals("CODE_VALUE") && oRow[itemColumn.ToString()] != null && oRow[itemColumn.ToString()].ToString().Length > 0)
                        {

                            LookupTableDataSet.LookupTableTreesRow ltTreesRowOrigem = (LookupTableDataSet.LookupTableTreesRow)dsLookupTableOrigem.Tables[2].Select(string.Format("LT_STRUCT_UID = '{0}'",oRow[itemColumn.ToString()])).SingleOrDefault();
                            LookupTableDataSet.LookupTableTreesRow ltTreesRowDestino = (LookupTableDataSet.LookupTableTreesRow)dsLookupTableDestino.Tables[2].Select(string.Format("LT_VALUE_FULL = '{0}'", ltTreesRowOrigem.LT_VALUE_FULL)).FirstOrDefault();

                            oRowDestino[itemColumn.ToString()] = ltTreesRowDestino.LT_STRUCT_UID;
                            continue;

                        }


                        if (oRow[itemColumn.ToString()] != null && !itemColumn.ReadOnly && oRow[itemColumn.ToString()].ToString().Length > 0)
                            oRowDestino[itemColumn.ToString()] = oRow[itemColumn.ToString()];
                    }
                    dsProjectDestino.ProjectCustomFields.AddProjectCustomFieldsRow(oRowDestino);
                }

                /////informações das task do projeto
                foreach (SvcProject.ProjectDataSet.TaskRow oRow in dsProject.Task.Rows)
                {
                    SvcProject.ProjectDataSet.TaskRow oRowDestino = dsProjectDestino.Task.NewTaskRow();

                    foreach (DataColumn itemColumn in dsProject.Task.Columns)
                    {
                        if (!dsProjectDestino.Task.Columns.Contains(itemColumn.ToString()))
                            continue;

                        if (itemColumn.ColumnName.Equals("PROJ_UID"))
                        {
                            oRowDestino[itemColumn.ToString()] = PROJ_UID;
                            continue;
                        }

                        if (itemColumn.ToString().Equals("TASK_UID"))
                        {
                            oRowDestino[itemColumn.ToString()] = Guid.NewGuid();
                            continue;
                        }
                        
                        if (itemColumn.ToString().Equals("TASK_OUTLINE_LEVEL"))
                            continue;

                        if (oRow[itemColumn.ToString()] != null) // && !itemColumn.ReadOnly)
                            oRowDestino[itemColumn.ToString()] = oRow[itemColumn.ToString()];
                    }
                    dsProjectDestino.Task.AddTaskRow(oRowDestino);
                }
                /////informações customfields da task
                //foreach (SvcProject.ProjectDataSet.TaskCustomFieldsRow oRow in dsProject.TaskCustomFields.Rows)
                //{
                //    SvcProject.ProjectDataSet.TaskCustomFieldsRow oRowDestino = dsProjectDestino.TaskCustomFields.NewTaskCustomFieldsRow();

                //    foreach (DataColumn itemColumn in dsProject.TaskCustomFields.Columns)
                //    {
                //        if (!dsProjectDestino.TaskCustomFields.Columns.Contains(itemColumn.ToString()))
                //            continue;

                //        if (oRow[itemColumn.ToString()] != null && !itemColumn.ReadOnly)
                //            oRowDestino[itemColumn.ToString()] = oRow[itemColumn.ToString()];
                //    }
                //    dsProjectDestino.TaskCustomFields.AddTaskCustomFieldsRow(oRowDestino);
                //}


                var createJobUid = Guid.NewGuid();
                clientProjectDestino.QueueCreateProject(createJobUid, dsProjectDestino, false);

                var publishJobUid = Guid.NewGuid();
                clientProjectDestino.QueuePublish(publishJobUid, PROJ_UID, true, string.Empty);

                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetPwaName(string pwaUrl)
        {
            string pwa = string.Empty;
            for (int i = pwaUrl.Length - 1; i > 0; i--)
            {
                pwa += pwaUrl[i].ToString();
                if (pwa.Contains("/"))
                    break;
            }

            return pwaUrl.Substring(pwaUrl.Length - pwa.Length, pwa.Length);
        }
        private string GetDnsName(string pwaUrl)
        {
            string pwa = string.Empty;
            for (int i = pwaUrl.Length - 1; i > 0; i--)
            {
                pwa += pwaUrl[i].ToString();
                if (pwa.Contains("/"))
                    break;
            }

            return pwaUrl.Substring(0,pwaUrl.Length - pwa.Length);
        }

        public string VerifyPDPExist(string pwaOrigem, string pwaDestino, Guid EPT_UID)
        {
            SvcWorkflow.WorkflowClient clientWorkflowOrigem = new SvcWorkflow.WorkflowClient(Binding, endPointAdress(pwaOrigem, "/_vti_bin/psi/Workflow.asmx"));
            clientWorkflowOrigem.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);
            
            SvcWorkflow.WorkflowClient clientWorkflowDestino = new SvcWorkflow.WorkflowClient(Binding, endPointAdress(pwaDestino, "/_vti_bin/psi/Workflow.asmx"));
            clientWorkflowDestino.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);

            SvcWorkflow.WorkflowDataSet dsEnterpriseOrigem = clientWorkflowOrigem.ReadEnterpriseProjectType(EPT_UID);
            SvcWorkflow.WorkflowDataSet dsEnterpriseDestino = clientWorkflowDestino.ReadEnterpriseProjectTypeList();

            string PDPs = string.Empty;
            foreach (DataRow item in dsEnterpriseOrigem.EnterpriseProjectTypePDPs.Rows)
            {
                var oRowPDP = dsEnterpriseDestino.EnterpriseProjectTypePDPs.Select(string.Format("PDP_NAME = '{0}'", item["PDP_NAME"].ToString())).FirstOrDefault();
                if (oRowPDP == null)
                {
                    PDPs += item["PDP_NAME"].ToString() + ",";
                }
            }

            if (PDPs.Length == 0)
                return string.Empty;
            
            return PDPs.Substring(0,PDPs.Length-1);
        }

        public void CreateEnterpriseProjectType(string pwaOrigem, string pwaDestino, string nameProjecType, Guid Uid)
        {
            //SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypeDataTable dt = new SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypeDataTable();
            try
            {
                SvcWorkflow.WorkflowClient clientWorkflowOrigem = new SvcWorkflow.WorkflowClient(Binding, endPointAdress(pwaOrigem, "/_vti_bin/psi/Workflow.asmx"));
                clientWorkflowOrigem.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);

                SvcWorkflow.WorkflowClient clientWorkflowDestino = new SvcWorkflow.WorkflowClient(Binding, endPointAdress(pwaDestino, "/_vti_bin/psi/Workflow.asmx"));
                clientWorkflowDestino.ClientCredentials.Windows.ClientCredential = new NetworkCredential(this.User, this.PassWord, this.Domain);

                SvcWorkflow.WorkflowDataSet dsEnterpriseOrigem = clientWorkflowOrigem.ReadEnterpriseProjectType(Uid);
                SvcWorkflow.WorkflowDataSet dsEnterpriseDestino = new SvcWorkflow.WorkflowDataSet();
                SvcWorkflow.WorkflowDataSet dsEnterpriseDestino2 = clientWorkflowDestino.ReadEnterpriseProjectTypeList();

                //SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypePDPsDataTable dtPDP = clientWorkflowOrigem.read

                Guid ENTERPRISE_PROJECT_TYPE_UID = Guid.Empty;

                foreach (SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypeRow itemEnterprise in dsEnterpriseOrigem.EnterpriseProjectType.Rows)
                {
                    SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypeRow oRow = dsEnterpriseDestino.EnterpriseProjectType.NewEnterpriseProjectTypeRow();
                    foreach (DataColumn itemColumn in dsEnterpriseOrigem.EnterpriseProjectType.Columns)
                    {
                        if (!dsEnterpriseDestino.EnterpriseProjectType.Columns.Contains(itemColumn.ToString()))
                            continue;

                        if (itemColumn.ToString().Equals("WORKFLOW_ASSOCIATION_UID") || itemColumn.ToString().Equals("WORKFLOW_ASSOCIATION_NAME"))
                            continue;

                        var dnsEquals = GetDnsName(pwaOrigem) == GetDnsName(pwaDestino);

                        if (nameProjecType.Length > 0 && itemColumn.ToString().Equals("ENTERPRISE_PROJECT_TYPE_NAME"))
                        {
                            oRow[itemColumn.ToString()] = nameProjecType;
                            continue;
                        }

                        if (itemColumn.ToString().Equals("ENTERPRISE_PROJECT_TYPE_UID")) {
                            ENTERPRISE_PROJECT_TYPE_UID = Guid.Parse(itemEnterprise[itemColumn.ToString()].ToString());
                        }

                        if (itemColumn.ToString().Equals("ENTERPRISE_PROJECT_TYPE_UID") && dnsEquals)
                        {
                            ENTERPRISE_PROJECT_TYPE_UID = Guid.NewGuid();
                            oRow[itemColumn.ToString()] = ENTERPRISE_PROJECT_TYPE_UID;
                            continue;
                        }

                        if (itemColumn.ToString().Equals("ENTERPRISE_PROJECT_WORKSPACE_TEMPLATE_NAME"))
                        {
                            oRow[itemColumn.ToString()] = "ProjectSite#0";
                            continue;
                        }

                        if (itemColumn.ToString().Equals("ENTERPRISE_PROJECT_TYPE_IMAGE_URL"))
                        {
                            oRow[itemColumn.ToString()] = string.Format("/_layouts/15/inc/{0}/images/CenterNormalProject.png", GetPwaName(pwaDestino).Replace("/",string.Empty));
                            continue;
                        }
                        
                        //EnterpriseProjectTypeInvalidWorkspaceTemplateName

                        if (itemEnterprise[itemColumn.ToString()] != null && !itemColumn.ReadOnly)
                            oRow[itemColumn.ToString()] = itemEnterprise[itemColumn.ToString()];
                    }
                    dsEnterpriseDestino.EnterpriseProjectType.AddEnterpriseProjectTypeRow(oRow);
                    //dsWorkflow.EnterpriseProjectType.AddEnterpriseProjectTypeRow(oRow);
                }

                foreach (SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypePDPsRow itemPDPs in dsEnterpriseOrigem.EnterpriseProjectTypePDPs.Rows)
                {
                    SvcWorkflow.WorkflowDataSet.EnterpriseProjectTypePDPsRow oRowPDP = dsEnterpriseDestino.EnterpriseProjectTypePDPs.NewEnterpriseProjectTypePDPsRow();
                    foreach (DataColumn itemColumn in dsEnterpriseOrigem.EnterpriseProjectTypePDPs.Columns)
                    {
                        if (itemColumn.ToString().Equals("PDP_UID"))
                            continue;

                        if (itemColumn.ToString().Equals("PDP_NAME"))
                        {
                            var oDataRow = dsEnterpriseDestino2.EnterpriseProjectTypePDPs.Select(string.Format("PDP_NAME = '{0}'", itemPDPs[itemColumn.ToString()])).FirstOrDefault();
                            if (oDataRow != null)
                                oRowPDP["PDP_UID"] = oDataRow["PDP_UID"];
                            else
                            {
                                var oDataRow2 = dsEnterpriseDestino2.EnterpriseProjectTypePDPs.Select().FirstOrDefault();
                                oRowPDP["PDP_UID"] = oDataRow2["PDP_UID"];
                            }
                        }

                        if (itemColumn.ToString().Equals("ENTERPRISE_PROJECT_TYPE_UID"))
                        {
                            oRowPDP[itemColumn.ToString()] = ENTERPRISE_PROJECT_TYPE_UID;
                            continue;
                        }

                        if (itemPDPs[itemColumn.ToString()] != null && !itemColumn.ReadOnly)
                            oRowPDP[itemColumn.ToString()] = itemPDPs[itemColumn.ToString()];
                    }
                    dsEnterpriseDestino.EnterpriseProjectTypePDPs.AddEnterpriseProjectTypePDPsRow(oRowPDP);    
                    
                }
                //dsWorkflow.EnterpriseProjectType.AcceptChanges();
                //dsWorkflow.EnterpriseProjectTypePDPs.AcceptChanges();
                clientWorkflowDestino.CreateEnterpriseProjectType(dsEnterpriseDestino);
                
            }
            catch (SoapException ex)
            {
                string errMess = "";
                // Pass the exception to the PSClientError constructor to 
                // get all error information.
                PSLibrary.PSClientError psiError = new PSLibrary.PSClientError(ex);
                PSLibrary.PSErrorInfo[] psiErrors = psiError.GetAllErrors();

                for (int j = 0; j < psiErrors.Length; j++)
                {
                    errMess += psiErrors[j].ErrId.ToString() + "\n";
                }
                errMess += "\n" + ex.Message.ToString();

                MessageBox.Show(errMess);
                // Send error string to console or message box.
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Metodos 
        public DataTable DtGridView(int tabIndex, DataTable dt)
        {
            DataTable retorno = new DataTable();
            if (tabIndex == 1)
            {
                retorno.Columns.Add(new DataColumn("TIPO_PROP", typeof(String)));
                retorno.Columns.Add(new DataColumn("MD_PROP_NAME", typeof(String)));
                retorno.Columns.Add(new DataColumn("LT_NAME", typeof(String)));
                retorno.Columns.Add(new DataColumn("MD_PROP_FORMULA", typeof(String)));
            }
            else 
            {
                foreach (DataColumn item in dt.Columns)
                {
                    DataColumn oColumn = new DataColumn();
                    oColumn.ColumnName = item.ColumnName;
                    oColumn.DataType = item.DataType;

                    retorno.Columns.Add(oColumn);
                }
            }

            return retorno;
        }
        #endregion
    }
}
