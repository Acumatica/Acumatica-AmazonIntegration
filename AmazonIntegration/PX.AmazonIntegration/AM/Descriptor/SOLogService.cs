using System;
using PX.Data;
using PX.Objects.SO;

namespace AmazonIntegration
{
    public class SOLogService
    {
        #region Logfilestatus
        public static void LogImportCount(SOImportProcess.SOImportFilter currentimportcount, string integrationId, SOPartialMaint logGraph, int? processId, string screenAction, bool isNoOrdersPrepared)
        {
            SOOrderProcessLog processcount = null;
            switch (screenAction)
            {
                case SOConstants.importorders:
                    processcount = new SOOrderProcessLog();
                    processcount.ProcessDate = PX.Common.PXTimeZoneInfo.Now;
                    processcount.TotalRecordstoImport = isNoOrdersPrepared == true ? 0 : currentimportcount.TotalRecordsToImport;
                    processcount.ImportedRecordsCount = currentimportcount.TotalImportedRecords;
                    processcount.FailedRecordsCount = currentimportcount.TotalFailedRecords;
                    processcount.IntegrationID = integrationId;
                    processcount.Operation = SOConstants.btnPrepare;
                    logGraph.OrderProcessLog.Insert(processcount);
                    logGraph.Actions.PressSave();
                    logGraph.OrderProcessLog.Current.ParentProcessID = logGraph.OrderProcessLog.Current.ProcessID;
                    logGraph.OrderProcessLog.Update(logGraph.OrderProcessLog.Current);
                    logGraph.Actions.PressSave();
                    break;
                case SOConstants.scheduleimportorders:
                    processcount = PXSelectReadonly<SOOrderProcessLog, Where<SOOrderProcessLog.processID, Equal<Required<SOOrderProcessLog.processID>>>>.Select(logGraph, Convert.ToInt32(processId));
                    if (processcount == null)
                    {
                        processcount = new SOOrderProcessLog();
                        processcount.ProcessDate = PX.Common.PXTimeZoneInfo.Now;
                        processcount.Operation = SOConstants.btnPrepareAndImport;
                        processcount.TotalRecordstoImport = isNoOrdersPrepared == true ? 0 : 1;
                        processcount.ImportedRecordsCount = 0;
                        processcount.FailedRecordsCount = 0;
                        processcount.IntegrationID = integrationId;
                        logGraph.OrderProcessLog.Insert(processcount);
                        logGraph.Actions.PressSave();
                        logGraph.OrderProcessLog.Current.ParentProcessID = logGraph.OrderProcessLog.Current.ProcessID;
                        logGraph.OrderProcessLog.Update(logGraph.OrderProcessLog.Current);
                        logGraph.Actions.PressSave();
                    }
                    else
                    {
                        processcount.ProcessDate = PX.Common.PXTimeZoneInfo.Now;
                        processcount.TotalRecordstoImport = processcount.TotalRecordstoImport + 1;
                        logGraph.OrderProcessLog.Update(processcount);
                        logGraph.Actions.PressSave();
                    }
                    break;
            }

        }

        public static void LogImportStatus(PrepareAndImportOrdersParams orderParams, bool importOrderStatus, string errorlog)
        {
            if (orderParams != null && orderParams.objSOProcessOrderRecord != null)
            {
                UpdateImportStatus(orderParams.objSOPartialMaint, orderParams.objSOProcessOrderRecord, importOrderStatus);
                orderParams.objSOPartialMaint.Clear();
                SOOrderLevelProcessLog logRecord = new SOOrderLevelProcessLog();
                logRecord.IntegrationID = orderParams.objSOProcessOrderRecord.IntegrationID;
                logRecord.AmazonOrderID = orderParams.objSOProcessOrderRecord.AmazonOrderID;
                logRecord.ProcessID = orderParams.objSOProcessOrderRecord.ProcessID;
                logRecord.AcumaticaOrderType = orderParams.objSOOrderEntry.Document.Current != null ? orderParams.objSOOrderEntry.Document.Current.OrderType : null;
                logRecord.AcumaticaOrderID = orderParams.objSOOrderEntry.Document.Current != null ? orderParams.objSOOrderEntry.Document.Current.OrderNbr : null;
                logRecord.ImportStatus = importOrderStatus;
                logRecord.ErrorDesc = errorlog.Truncate(100);
                orderParams.objSOPartialMaint.OrderLevelProcessLog.Current = logRecord;
                orderParams.objSOPartialMaint.OrderLevelProcessLog.Insert(orderParams.objSOPartialMaint.OrderLevelProcessLog.Current);
                orderParams.objSOPartialMaint.Actions.PressSave();
                if (importOrderStatus)
                    RecordImported(orderParams.objSOPartialMaint, orderParams.objSOProcessOrderRecord, orderParams.objSOOrderEntry.Document.Current.OrderNbr, orderParams.objSOOrderEntry.Document.Current.OrderType);
            }
        }

        #endregion

        #region Update Import count

        private static void UpdateImportStatus(SOPartialMaint logGraph, SOProcessOrder currentRecord, bool importOrderStatus)
        {
            logGraph.ProcessOrder.Current = currentRecord;
            logGraph.ProcessOrder.Current.ImportStatus = importOrderStatus;
            logGraph.ProcessOrder.Update(logGraph.ProcessOrder.Current);
            logGraph.Actions.PressSave();
            SOOrderProcessLog processupdatecount = logGraph.UpdateImportProcessLog.Select();
            if (processupdatecount == null) return;
            int? importCount = processupdatecount.TotalRecordstoImport;
            processupdatecount.TotalRecordstoImport = importCount;
            if (importOrderStatus)
                processupdatecount.ImportedRecordsCount = processupdatecount.ImportedRecordsCount + 1;
            else
                processupdatecount.FailedRecordsCount = processupdatecount.FailedRecordsCount + 1;
            processupdatecount.ParentProcessID = currentRecord.ProcessID;
            logGraph.UpdateImportProcessLog.Update(processupdatecount);
            logGraph.Actions.PressSave();
        }
        #endregion

        #region Imported records
        private static void RecordImported(SOPartialMaint logGraph, SOProcessOrder currentRecord, string acmOrderId, string acmOrderType)
        {
            SOImportedRecords importedRecords = new SOImportedRecords();
            importedRecords.ProcessID = currentRecord.ProcessID;
            importedRecords.AmazonOrderID = currentRecord.AmazonOrderID;
            importedRecords.AcumaticaOrderID = acmOrderId;
            importedRecords.AcumaticaOrderType = acmOrderType;
            importedRecords.ImportedDate = logGraph.Accessinfo.BusinessDate;
            importedRecords.IntegrationID = currentRecord.IntegrationID;
            logGraph.ImportedRecords.Insert(importedRecords);
            logGraph.Actions.PressSave();
        }
        #endregion

        #region Submit Feed Log

        public static int? LogSubmitCount(SOPartialMaint logGraph, string integrationID, int? TotalRecordsToSubmit)
        {
            SOSubmitProcessLog processcount = new SOSubmitProcessLog();
            processcount.ProcessDate = PX.Common.PXTimeZoneInfo.Now;
            processcount.TotalRecordstoProcess = TotalRecordsToSubmit;
            processcount.SubmitRecordsCount = 0;
            processcount.SubmitFailedRecordsCount = 0;
            processcount.IntegrationID = integrationID;
            logGraph.SubmitProcesLog.Insert(processcount);
            logGraph.Actions.PressSave();
            SOSubmitProcessLog getProcessId = PXSelectGroupBy<SOSubmitProcessLog, Aggregate<Max<SOSubmitProcessLog.processID>>>.Select(logGraph);
            return getProcessId != null && getProcessId.ProcessID != null ? getProcessId.ProcessID : 1;
        }

        public static void LogSubmitStatus(SubmitFeedParamaters objParams)
        {
            if (objParams != null && objParams.objSOAmazonSetup != null && objParams.objPartialMaint != null)
            {
                objParams.objPartialMaint.Clear();
                UpdateFeedStatus(objParams);
                SOSubmitDetailedProcessLog logRecord = new SOSubmitDetailedProcessLog();
                logRecord.ProcessID = objParams.processID;
                logRecord.IntegrationID = objParams.objSOAmazonSetup.IntegrationID;
                logRecord.AmazonOrderID = objParams.amazonOrderID;
                logRecord.AcumaticaOrderType = objParams.soType;
                logRecord.AcumaticaOrderNbr = objParams.acmOrderNbr;
                logRecord.AcumaticaShipmentType = objParams.shipmentType;
                logRecord.AcumaticaShipmentNbr = objParams.shipmentNbr;
                logRecord.ImportStatus = objParams.importOrderStatus;
                logRecord.ErrorDesc = objParams.feedMessage;
                logRecord.XMLResponse = objParams.xmlMessage;
                objParams.objPartialMaint.SubmitDetailedProcesLog.Insert(logRecord);
                objParams.objPartialMaint.Actions.PressSave();
            }
        }

        private static void UpdateFeedStatus(SubmitFeedParamaters objParams)
        {
            if (objParams != null && objParams.importOrderStatus)
            {
                SOShipment currentshipment = objParams.objPartialMaint.Shipment.Select(objParams.shipmentType, objParams.shipmentNbr);
                if (currentshipment != null)
                {
                    SOShipmentAmazonExt currentshipmentext = currentshipment.GetExtension<SOShipmentAmazonExt>();
                    if (currentshipmentext != null)
                    {
                        objParams.objPartialMaint.Shipment.SetValueExt<SOShipmentAmazonExt.usrSubmitFeedupdate>(currentshipment, true);
                        objParams.objPartialMaint.Shipment.Cache.Update(currentshipment);
                    }
                }
            }
            SOSubmitProcessLog processUpdateCount = null;
            if (objParams != null && objParams.objSOAmazonSetup != null && objParams.processID != null)
                processUpdateCount = objParams.objPartialMaint.UpdateFeedProcessLog.Select(objParams.objSOAmazonSetup.IntegrationID, objParams.processID);
            if (processUpdateCount == null) return;
            processUpdateCount.SubmitRecordsCount = objParams.importOrderStatus ? processUpdateCount.SubmitRecordsCount + 1 : processUpdateCount.SubmitRecordsCount;
            processUpdateCount.SubmitFailedRecordsCount = objParams.importOrderStatus == false ? processUpdateCount.SubmitFailedRecordsCount + 1 : processUpdateCount.SubmitFailedRecordsCount;
            objParams.objPartialMaint.UpdateFeedProcessLog.Update(processUpdateCount);
        }
        #endregion
    }

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}