using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using DataEntityService.Entities;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System;
using System.ComponentModel;

namespace DataEntityService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private static string connectionString;
        static PurchaseOrderController()
        {
            //connectionString = "Data Source=localhost;Integrated Security=SSPI;Initial Catalog=DemoPaaS;Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
            connectionString = "Data Source=database-1.coumj725wuis.ap-south-1.rds.amazonaws.com,1433;User ID=admin;Password=Ja!GuruJi;Initial Catalog=DemoPaaS;Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
        }
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(ILogger<PurchaseOrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{purchaseOrderId}")]
        public PurchaseOrder GetPurchaseOrder(int purchaseOrderId)
        {
            PurchaseOrder po = null;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("GetPurchaseOrder", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@PurchaseOrderId", purchaseOrderId));
                sqlConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.HasRows && reader.Read())
                    {
                        po = new PurchaseOrder()
                        {
                            PurchaseOrderId = (int)reader["PurchaseOrderId"],
                            PurchaseOrderNumber = (string)reader["PurchaseOrderNumber"],
                            SourceId = (int)reader["SourceId"],
                            UserId = (int)reader["UserId"],
                            Void = (bool)reader["Void"],
                            CreatedBy = (int)reader["CreatedBy"],
                            CreatedOn = (DateTime)reader["CreatedOn"]
                        };
                        reader.NextResult();
                        while (reader.Read())
                        {
                            po.PurchaseOrderDetails.Add(new PurchaseOrderDetails()
                            {
                                ItemId = (int)reader["ItemId"],
                                POLineItemId = (int)reader["POLineItemId"],
                                PurchaseOrderId = (int)reader["PurchaseOrderId"],
                                Void = (bool)reader["Void"],
                                Price = (Decimal)reader["Price"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { throw; }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
            return po;
        }

        [HttpPost]
        [Route("Create")]
        public bool CreatePurchaseOrder([FromBody] PurchaseOrder purchaseOrder)
        {
            bool result = false;
            DataTable dtPO = ConvertToDataTable(new List<PurchaseOrder>() { purchaseOrder });
            dtPO.Columns.Remove("PurchaseOrderDetails");
            dtPO.Columns.Remove("PurchaseOrderId");
            DataTable dtPODetails = ConvertToDataTable(purchaseOrder.PurchaseOrderDetails);
            dtPODetails.Columns.Remove("POLineItemId");
            dtPODetails.Columns.Remove("PurchaseOrderId");
            return result;
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;

        }
    }
}
