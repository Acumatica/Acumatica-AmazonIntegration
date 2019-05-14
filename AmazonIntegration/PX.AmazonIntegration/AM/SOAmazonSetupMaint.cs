using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Api;
using MarketplaceWebServiceOrders.Model;
using System.Reflection;
using System.Linq;
using PX.Objects.SO;

namespace AmazonIntegration
{
    public class SOAmazonSetupMaint : PXGraph<SOAmazonSetupMaint, SOAmazonSetup>, PXImportAttribute.IPXPrepareItems
    {
        #region Select

        [PXCopyPasteHiddenFields(typeof(SOAmazonSetup.sellerId))]
        public PXSelect<SOAmazonSetup> setupview;

        [PXImport(typeof(SOAmazonSetup))]
        public PXSelect<SOFieldMapping, Where<SOFieldMapping.integrationID, Equal<Current<SOAmazonSetup.integrationID>>>> FieldMapping;

        #endregion
        
        #region Variables
        private static readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders clientOrder = null;

        PXSiteMap.ScreenInfo m_screenInfo = null;
        #endregion

        #region Constructor

        public SOAmazonSetupMaint()
        {
            SOOrderEntry soGraph = PXGraph.CreateInstance<SOOrderEntry>();
            m_screenInfo = ScreenUtils.GetScreenInfo(SOConstants.ScreenID);
        }

        #endregion

        #region Actions

        #region TestConnection

        public PXAction<SOAmazonSetup> TestConnection;
        [PXButton]
        [PXUIField(DisplayName = SOMessages.testConnection, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        protected virtual void testConnection()
        {
            if (setupview.Current == null) return;
            this.Actions.PressSave();
            SOAmazonSetup currentrecord = this.setupview.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                SOServiceCalls.TestSellerAccount(currentrecord);
            });
        }
        #endregion

        #endregion

        #region Event Handlers

        protected virtual void SOAmazonSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SOAmazonSetup row = e.Row as SOAmazonSetup;
            if (row != null)
            {
                PXRSACryptStringAttribute.SetDecrypted<SOAmazonSetup.authToken>(sender, row, true);
                PXRSACryptStringAttribute.SetDecrypted<SOAmazonSetup.accessKey>(sender, row, true);
                PXRSACryptStringAttribute.SetDecrypted<SOAmazonSetup.secretKey>(sender, row, true);
                this.FieldMapping.AllowInsert = !string.IsNullOrEmpty(row.IntegrationID);
                this.FieldMapping.AllowUpdate = !string.IsNullOrEmpty(row.IntegrationID);
                this.FieldMapping.AllowDelete = !string.IsNullOrEmpty(row.IntegrationID);
                TestConnection.SetEnabled(!string.IsNullOrEmpty(row.IntegrationID));
                if (!string.IsNullOrEmpty(row.IntegrationType))
                {
                    PXUIFieldAttribute.SetVisible<SOAmazonSetup.fBMNote>(setupview.Cache, null, row.IntegrationType.Trim().ToLower() == SOConstants.AMIntegrationType.FBM.Trim().ToLower());                    
                }
            }
        }

        protected virtual void SOAmazonSetup_IntegrationType_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            SOAmazonSetup row = e.Row as SOAmazonSetup;
            if (row != null)
            {
                if (e.NewValue == null || (e.NewValue != null && row.IntegrationType != null && row.IntegrationType != Convert.ToString(e.NewValue)))
                {
                    if (Convert.ToString(e.NewValue).Contains(SOConstants.FBA) == row.IntegrationType.Contains(SOConstants.FBM)
                     || Convert.ToString(e.NewValue).Contains(SOConstants.FBM) == row.IntegrationType.Contains(SOConstants.FBA))
                        row.OrderType = null;
                }
            }
        }

        protected virtual void SOFieldMapping_SourceObject_FieldSelecting(PXCache cache, PXFieldSelectingEventArgs e)
        {
            SOFieldMapping row = e.Row as SOFieldMapping;
            if (row != null)
            {
                List<string> sourceObjects = GetSourceObjects();

                PXStringListAttribute.SetList<SOFieldMapping.sourceObject>(cache, row, sourceObjects.ToArray(), sourceObjects.ToArray());
            }
        }

        protected virtual void SOFieldMapping_SourceField_FieldSelecting(PXCache cache, PXFieldSelectingEventArgs e)
        {
            SOFieldMapping row = e.Row as SOFieldMapping;
            if (row == null || String.IsNullOrEmpty(row.SourceObject)) return;

            List<string> sourceFields = GetSourceFields(row.SourceObject);
            PXStringListAttribute.SetList<SOFieldMapping.sourceField>(cache, row, sourceFields.ToArray(), sourceFields.ToArray());            
        }

        public List<string> GetSourceObjects()
        {
            List<string> listSourceObjects = new List<string>();

            string rootObject = typeof(Order).Name;
            listSourceObjects.Add(rootObject);
            foreach (var prop in typeof(Order).GetProperties())
            {
                if (!(prop.PropertyType == typeof(object) || Type.GetTypeCode(prop.PropertyType) != TypeCode.Object))
                {
                    listSourceObjects.Add(rootObject + "->" + prop.Name);
                }
            }

            string rootObjectItem = typeof(OrderItem).Name;
            listSourceObjects.Add(rootObjectItem);
            foreach (var prop in typeof(OrderItem).GetProperties())
            {
                if (!(prop.PropertyType == typeof(object) || Type.GetTypeCode(prop.PropertyType) != TypeCode.Object))
                {
                    listSourceObjects.Add(rootObjectItem + "->" + prop.Name);
                }
            }
            return listSourceObjects;
        }

        public List<string> GetSourceFields(string sourceObject)
        {
            List<string> listSourceFields = new List<string>();

            string sourceObjectField = sourceObject.Substring(sourceObject.LastIndexOf(">") + 1);
            string rootObject = sourceObject.Contains("->") ?
                                    sourceObject.Substring(0, sourceObject.IndexOf("-")) : sourceObject;
            PropertyInfo[] arrProperties = null;
            if (sourceObjectField == typeof(Order).Name)
            {
                arrProperties = typeof(Order).GetProperties();
            }
            else if (sourceObjectField == typeof(OrderItem).Name)
            {
                arrProperties = typeof(OrderItem).GetProperties();
            }
            else if (rootObject == typeof(Order).Name)
            {
                PropertyInfo subProperty = typeof(Order).GetProperties().Where(x => x.Name == sourceObjectField).FirstOrDefault();
                if (subProperty != null)
                    arrProperties = subProperty.PropertyType.GetProperties();
            }
            else if (rootObject == typeof(OrderItem).Name)
            {
                PropertyInfo subProperty = typeof(OrderItem).GetProperties().Where(x => x.Name == sourceObjectField).FirstOrDefault();
                if (subProperty != null)
                    arrProperties = subProperty.PropertyType.GetProperties();
            }

            if (arrProperties != null)
            {
                foreach (var prop in arrProperties)
                {
                    if ((prop.PropertyType == typeof(object) || Type.GetTypeCode(prop.PropertyType) != TypeCode.Object))
                    {
                        listSourceFields.Add(prop.Name);
                    }
                }
            }
            return listSourceFields;
        }

        protected virtual void SOFieldMapping_TargetObject_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (e.Row == null) return;

            List<string> values = new List<string>();
            List<string> labels = new List<string>();

            GetTargetObjects(values, labels);

            PXStringListAttribute.SetList<SOFieldMapping.targetObject>(sender, e.Row, values.ToArray(), 
                                                                                      labels.ToArray());
        }
        protected virtual void SOFieldMapping_TargetField_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            SOFieldMapping row = e.Row as SOFieldMapping;

            if (row == null || String.IsNullOrEmpty(row.TargetObject)) return;

            List<string> values = new List<string>();
            List<string> labels = new List<string>();

            GetTargetFields(values, labels, row.TargetObject);

            PXStringListAttribute.SetList<SOFieldMapping.targetField>(sender, row, values.ToArray(), 
                                                                                    labels.ToArray());
        }

        #endregion

        #region IPXPrepareItems Members
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (viewName.ToUpper() == SOConstants.FieldMapping.ToUpper())
            {
                List<string> allowedValuesObjects = new List<string>();
                List<string> allowedDisplaysObjects = new List<string>();

                GetTargetObjects(allowedValuesObjects, allowedDisplaysObjects);

                string extValueObject = Convert.ToString(values["TargetObject"]);

                if (!String.IsNullOrEmpty(extValueObject) && allowedValuesObjects.Count > 0 &&
                                                             allowedDisplaysObjects.Count > 0)
                {
                    string intValueObject = allowedValuesObjects[allowedDisplaysObjects.IndexOf(extValueObject)];
                    values["TargetObject"] = intValueObject;

                    List<string> allowedValuesFields = new List<string>();
                    List<string> allowedDisplaysFields = new List<string>();

                    GetTargetFields(allowedValuesFields, allowedDisplaysFields, intValueObject);

                    string extValue = Convert.ToString(values["TargetField"]);

                    if (!string.IsNullOrEmpty(extValue) && allowedDisplaysFields.Count > 0 &&
                                                           allowedValuesFields.Count > 0)
                    {
                        string intValue = allowedValuesFields[allowedDisplaysFields.IndexOf(extValue)];
                        values["TargetField"] = intValue;
                    }
                }
            }
            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return row == null;
        }
        public bool RowImported(string viewName, object row, object oldRow)
        {
            return oldRow == null;
        }
        public void PrepareItems(string viewName, IEnumerable items) { }

        #endregion

        #region Target Object & Fields

        private void GetTargetObjects(List<string> values, List<string> labels)
        {
            if (m_screenInfo != null && m_screenInfo.Containers != null)
            {
                foreach (string key in m_screenInfo.Containers.Keys)
                {
                    PX.Data.Description.FieldInfo[] fields = m_screenInfo.Containers[key].Fields;
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (!values.Contains(key))
                        {
                            values.Add(string.Concat(key));
                            labels.Add(string.Concat(m_screenInfo.Containers[key].DisplayName));
                        }
                    }
                }
            }
        }

        private void GetTargetFields(List<string> values, List<string> labels, string sTargetObject)
        {
            if (m_screenInfo != null && m_screenInfo.Containers != null)
            {
                foreach (string key in m_screenInfo.Containers.Keys)
                {
                    PX.Data.Description.FieldInfo[] fields = m_screenInfo.Containers[key].Fields;
                    foreach (PX.Data.Description.FieldInfo fieldInfo in fields)
                    {
                        if (key == sTargetObject)
                        {
                            values.Add(fieldInfo.FieldName);
                            labels.Add(fieldInfo.DisplayName);
                        }
                    }
                }
            }
        }

        #endregion
    }
}