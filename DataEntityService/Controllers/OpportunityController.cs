
namespace DataEntityService.Controllers
{
    using DataEntityService.Entities;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class OpportunityController : ControllerBase
    {
        private static string connectionString;
        static OpportunityController()
        {
            //connectionString = "Data Source=localhost;Integrated Security=SSPI;Initial Catalog=DemoPaaS;Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
            connectionString = "Data Source=database-1.coumj725wuis.ap-south-1.rds.amazonaws.com,1433;User ID=admin;Password=Ja!GuruJi;Initial Catalog=DemoPaaS;Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
        }
        private readonly ILogger<PurchaseOrderController> _logger;

        public OpportunityController(ILogger<PurchaseOrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{opportunityId}")]
        public async Task<OpportunityData> GetOpportunityDetail(Guid opportunityId)
        {
            OpportunityData opportunityData = null;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("GetOpportunityDetail", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@opportunitySourceId", opportunityId));
                sqlConnection.Open();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    if (reader.HasRows && await reader.ReadAsync())
                    {
                        opportunityData = new OpportunityData()
                        {
                            Active = (bool)reader["Active"],
                            OpportunitySourceCode = (string)reader["OpportunitySourceCode"],
                            OpportunitySourceDescription = (string)reader["OpportunitySourceDescription"],
                            OpportunitySourceId = (Guid)reader["OpportunitySourceId"],
                            OpportunitySourceName = (string)reader["OpportunitySourceName"],
                            OrganizationId = (Guid)reader["OrganizationId"],
                            UserId = (Guid)reader["UserId"],
                            Void = (bool)reader["Void"]
                        };
                    }
                }
            }
            catch (SqlException sqlEx) { throw sqlEx; }
            catch (Exception ex) { throw; }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
            return opportunityData;
        }

        [HttpGet]
        [Route("{organizationId}/opportunity")]
        public async Task<List<OpportunityData>> GetOpportunitiesOfOrganization(Guid organizationId)
        {
            List<OpportunityData> opportunities = new List<OpportunityData>();
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("GetOpportunitiesOfOrganization", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@organizationId", organizationId));
                sqlConnection.Open();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.HasRows && await reader.ReadAsync())
                    {
                        opportunities.Add(new OpportunityData()
                        {
                            Active = (bool)reader["Active"],
                            OpportunitySourceCode = (string)reader["OpportunitySourceCode"],
                            OpportunitySourceDescription = (string)reader["OpportunitySourceDescription"],
                            OpportunitySourceId = (Guid)reader["OpportunitySourceId"],
                            OpportunitySourceName = (string)reader["OpportunitySourceName"],
                            OrganizationId = (Guid)reader["OrganizationId"],
                            UserId = (Guid)reader["UserId"],
                            Void = (bool)reader["Void"]
                        });
                    }
                }
            }
            catch (SqlException sqlEx) { throw sqlEx; }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
            return opportunities;
        }

        //[HttpPost]
        //[Route("Create")]
        //public bool CreateOpportunity([FromBody] OpportunityData opportunityData)
        //{
        //    bool result = false;
        //    DataTable dtPO = ConvertToDataTable(new List<PurchaseOrder>() { purchaseOrder });
        //    dtPO.Columns.Remove("PurchaseOrderDetails");
        //    dtPO.Columns.Remove("PurchaseOrderId");
        //    DataTable dtPODetails = ConvertToDataTable(purchaseOrder.PurchaseOrderDetails);
        //    dtPODetails.Columns.Remove("POLineItemId");
        //    dtPODetails.Columns.Remove("PurchaseOrderId");
        //    return result;
        //}

        //public DataTable ConvertToDataTable<T>(IList<T> data)
        //{
        //    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        //    DataTable table = new DataTable();
        //    foreach (PropertyDescriptor prop in properties)
        //    {
        //        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    }
        //    foreach (T item in data)
        //    {
        //        DataRow row = table.NewRow();
        //        foreach (PropertyDescriptor prop in properties)
        //        {
        //            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        //        }
        //        table.Rows.Add(row);
        //    }
        //    return table;

        //}
    }
}
