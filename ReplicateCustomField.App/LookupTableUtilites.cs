using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using PSLibrary = Microsoft.Office.Project.Server.Library;

namespace Segplan.ReplicateCustomField.App
{
    public class LookupTableUtilites
    {
         /// <summary>
        /// Create a hierarchical text lookup table, with code masks and values.
        /// </summary>
        /// <param name="lookupTable">SvcLookupTable.LookupTable object</param>
        /// <param name="ltName">Name of the lookup table</param>
        /// <param name="maskSequence">Array of code mask sequences and separator characters</param>
        /// <param name="maskValues"></param>
        /// <param name="ltValues">Array of lookup table values: Name, description, level, default</param>
        /// <param name="ltRowDefaultUid">GUID of default value (out)</param>
        /// <returns>GUID of lookup table</returns>
        public Guid CreateLookupTable(
            SvcLookupTable.LookupTable lookupTable, 
            string ltName, 
            byte[] maskSequence,
            string[,] maskValues,
            string[,] ltValues,
            out Guid ltRowDefaultUid)
        {
            // Além região variáveis ​​método, há três regiões 
            // Com um bloco try-catch: 
            // 1. Criar linhas de tabela de pesquisa com máscaras de código em um LookupTableDataSet 
            // 2 Adicionar valores herarchical para cada linha na tabela de pesquisa 
            // 3. CreateLookupTables chamada com o LookupTableDataSet
        
            const string ANY = "any";
            const string DEFAULT = "default";

            #region Method variables
            int levelLength;
            string sLevelLength;
            bool error = false;

            // Save the return GUIDS for lookup table and default value (if any)
            Guid[] returnIds = new Guid[2];        
            int numLtRows = ltValues.Length / 4;   // There are 4 strings for each lookup table row
            int maxLevels = maskValues.Length / 2; // There are 2 strings for each mask level

            if (maxLevels != maskSequence.Length || maxLevels > 5)
            {
                // Error: The number of rows in the maskSequence and maskValues arrays must be the same.
                //        The hierarchical levels can't be more than five levels deep.
                ltRowDefaultUid = Guid.Empty;
                return Guid.Empty;
            }
            int[] parentIndex = new int[numLtRows]; // Index of each lookup table row parent
            Guid[] parentUid = new Guid[numLtRows];  // Parent GUIDs of lookup table tree rows

            for (int i = 0; i < numLtRows; i++)
            {
                parentIndex[i] = Convert.ToInt32(ltValues[i, 2]);
            }
            
            SvcLookupTable.LookupTableDataSet lookupTableDataSet =
                new SvcLookupTable.LookupTableDataSet();
            #endregion

            #region Criar uma linha da tabela de pesquisa com máscaras de código
            try
            {
                //Criar uma linha da tabela de pesquisa a partir da instância LookupTableDataSet
                SvcLookupTable.LookupTableDataSet.LookupTablesRow lookupTableRow =
                    lookupTableDataSet.LookupTables.NewLookupTablesRow();
               
                Guid lookupTableGuid = Guid.NewGuid();
                // Set the lookup table ID 
                returnIds[0] = lookupTableGuid;

                lookupTableRow.LT_UID = lookupTableGuid;
                lookupTableRow.LT_NAME = ltName;
                lookupTableRow.LT_SORT_ORDER_ENUM =
                    (byte)PSLibrary.LookupTables.SortOrder.Ascending;

                lookupTableDataSet.LookupTables.Rows.Add(lookupTableRow);

                // Create the code mask and rows
                SvcLookupTable.LookupTableDataSet.LookupTableMasksDataTable masksDataTable = 
                    new SvcLookupTable.LookupTableDataSet.LookupTableMasksDataTable();

                SvcLookupTable.LookupTableDataSet.LookupTableMasksRow ltMasksRow =
                    masksDataTable.NewLookupTableMasksRow();

                for (int level = 0; level < maxLevels; level++)
                {
                    sLevelLength = maskValues[level, 0];
                    if (string.Compare(sLevelLength, ANY, true) == 0)
                        levelLength = PSLibrary.LookupTables.ANY_LENGTH_SEQUENCE;
                    else
                        levelLength = Convert.ToInt32(maskValues[level, 0]);
                    
                    ltMasksRow = CreateLookupTableMasksRow(
                        lookupTableDataSet,
                        level + 1,
                        maskSequence[level],
                        levelLength,
                        maskValues[level, 1]);
                    lookupTableDataSet.LookupTableMasks.Rows.Add(ltMasksRow);
                }
            }
            catch (DataException ex)
            {
                // Add exception handler for ex
                error = true;
            }
            catch (SoapException ex)
            {
                // Add exception handler for ex
                error = true;
            }
            #endregion
            
            #region Add values to each row
            // Add the lookup table values
            try
            {
                SvcLookupTable.LookupTableDataSet.LookupTableTreesRow ltTreeRow =
                    lookupTableDataSet.LookupTableTrees.NewLookupTableTreesRow();
                
                if (!error)
                {
                    int thisNode;
                    int nextNode;
                    int indexDiff;                       // Difference in levels between nodes 
                    int rowLevel;                        // Level of the current row
                    Guid rowUid;                         // GUID of the current level
                    Guid[] previousLevelUid = new Guid[4];  // GUIDs of up to five previous levels
                                                         //     [0]: level 1; ... ; [4]: level 5
                    parentUid[0] = Guid.Empty;           // Initialize the first parentUid
                    
                    for (int row = 0; row < numLtRows; row++)
                    {
                        rowUid = Guid.NewGuid();
                        thisNode = row;
                        nextNode = thisNode + 1;
                        rowLevel = parentIndex[row];
                        previousLevelUid[rowLevel] = rowUid;  // Reset the previous level

                        ltTreeRow = AddLookupTableValues(
                            lookupTableDataSet,
                            parentUid[row],        // Parent GUID
                            rowUid,                // Current row GUID 
                            ltValues[row, 0],      // Value
                            ltValues[row, 1]       // Description
                         );
                        // Set the parentUid of the next node.
                        if (row < numLtRows - 1)
                        {
                            if (parentIndex[nextNode] == 0)
                            {
                                parentUid[nextNode] = Guid.Empty;
                            }
                            else
                            {
                                indexDiff = parentIndex[nextNode] - parentIndex[thisNode];
                                switch (indexDiff)
                                {
                                    case 1:
                                        parentUid[nextNode] = rowUid;
                                        break;

                                    case 0:
                                        parentUid[nextNode] = parentUid[thisNode];
                                        break;

                                    case -1:
                                    case -2:
                                    case -3:
                                        indexDiff -= 1;
                                        parentUid[nextNode] = previousLevelUid[rowLevel + indexDiff];
                                        break;
                                }
                            }
                        }
                        // Check for the default GUID
                        if (ltValues[row, 3] == DEFAULT)
                            returnIds[1] = rowUid;
                        lookupTableDataSet.LookupTableTrees.Rows.Add(ltTreeRow);
                    }
                }
            }
            catch (SoapException ex)
            {
                // Add exception handler for ex
                error = true;
            }
            catch (Exception ex)
            {
                // Add exception handler for ex
                error = true;
            }
            #endregion

            #region Create and return the lookup table
            try
            {
                if (!error)
                {
                    bool validateOnly = false;
                    bool autoCheckIn = true;
                    lookupTable.CreateLookupTables(lookupTableDataSet, 
                        validateOnly, autoCheckIn);
                }
            }
            catch (SoapException ex)
            {
                string errMess = "";
                // Pass the exception to the PSClientError constructor to get 
                // all error information.
                PSLibrary.PSClientError psiError = new PSLibrary.PSClientError(ex);
                PSLibrary.PSErrorInfo[] psiErrors = psiError.GetAllErrors();

                for (int j = 0; j < psiErrors.Length; j++)
                {
                    errMess += psiErrors[j].ErrId.ToString() + "\n";
                }
                errMess += "\n" + ex.Message.ToString();
                // Send error string to console or message box.

                error = true;
            }
            if (error)
            {
                returnIds[0] = Guid.Empty;
                returnIds[1] = Guid.Empty;
            }
            ltRowDefaultUid = returnIds[1];
            return returnIds[0];
            #endregion
        }

        #region Lookup table utilities

        /// <summary>
        /// Create a lookup table code mask row
        /// </summary>
        /// <param name="ltDataSet">SvcLookupTable.LookupTableDataSet object</param>
        /// <param name="structLevel">Level of the code mask</param>
        /// <param name="maskSequence">Mask sequence type</param>
        /// <param name="levelLength">Number of characters in this level</param>
        /// <param name="separator">Code mask separator character for this level</param>
        /// <returns>LookupTableMasksRow</returns>
        private SvcLookupTable.LookupTableDataSet.LookupTableMasksRow CreateLookupTableMasksRow(
            SvcLookupTable.LookupTableDataSet ltDataSet,
            int structLevel,
            byte maskSequence,
            int levelLength,
            string separator)
        {
            SvcLookupTable.LookupTableDataSet.LookupTableMasksRow ltMaskRow =
                ltDataSet.LookupTableMasks.NewLookupTableMasksRow();

            ltMaskRow.LT_UID = ltDataSet.LookupTables[0].LT_UID;
            ltMaskRow.LT_MASK_STRUCT_LEVEL = structLevel;
            ltMaskRow.LT_MASK_STRUCT_TYPE_ENUM = maskSequence;
            ltMaskRow.LT_MASK_STRUCT_LENGTH = levelLength;
            ltMaskRow.LT_MASK_VALUE_SEPARATOR = separator;

            return ltMaskRow;
        }

        /// <summary>
        /// Add a value to the lookup table row
        /// </summary>
        /// <param name="ltDataSet">SvcLookupTable.LookupTableDataSet object</param>
        /// <param name="parentUid">GUID of the parent row</param>
        /// <param name="rowUid">GUID of the current row</param>
        /// <param name="ltValue">Value of the row</param>
        /// <param name="ltDescription">Description of the row</param>
        /// <returns>LookupTableTreesRow</returns>
        private SvcLookupTable.LookupTableDataSet.LookupTableTreesRow AddLookupTableValues(
            SvcLookupTable.LookupTableDataSet ltDataSet,
            Guid parentUid,
            Guid rowUid,
            string ltValue,
            string ltDescription)
        {
            SvcLookupTable.LookupTableDataSet.LookupTableTreesRow ltTreeRow =
                ltDataSet.LookupTableTrees.NewLookupTableTreesRow();

            ltTreeRow.LT_UID = ltDataSet.LookupTables[0].LT_UID;
            ltTreeRow.LT_STRUCT_UID = rowUid;

            if (parentUid == Guid.Empty)
                ltTreeRow.SetLT_PARENT_STRUCT_UIDNull();
            else
                ltTreeRow.LT_PARENT_STRUCT_UID = parentUid;

            ltTreeRow.LT_VALUE_TEXT = ltValue;
            ltTreeRow.LT_VALUE_DESC = ltDescription;

            return ltTreeRow;
        }
        #endregion
    }
}
