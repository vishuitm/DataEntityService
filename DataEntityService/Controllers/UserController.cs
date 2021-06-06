
namespace DataEntityService.Controllers
{
    using DataEntityService.Entities;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static string connectionString;
        static UserController()
        {
            //connectionString = "Data Source=localhost;Integrated Security=SSPI;Initial Catalog=DemoPaaS;Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
            connectionString = "Data Source=database-1.coumj725wuis.ap-south-1.rds.amazonaws.com,1433;User ID=admin;Password=Ja!GuruJi;Initial Catalog=DemoPaaS;Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;";
        }
        private readonly ILogger<PurchaseOrderController> _logger;

        public UserController(ILogger<PurchaseOrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<Guest> GetUserDetail(Guid userId)
        {
            Guest user = null;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("GetUserDetail", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@userId", userId));
                sqlConnection.Open();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    if (reader.HasRows && await reader.ReadAsync())
                    {
                        user = new Guest()
                        {
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
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
            return user;
        }

        [HttpPost]
        [Route("Users")]
        public async Task<List<Guest>> GetUsers(IList<Guid> userIds)
        {
            List<Guest> opportunities = new List<Guest>();
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("GetUsers", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlParameter param = new SqlParameter("@userIds", ListToDataTable<Guid>(userIds));
                param.SqlDbType = SqlDbType.Structured;

                cmd.Parameters.Add(param);
                sqlConnection.Open();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (reader.HasRows && await reader.ReadAsync())
                    {
                        opportunities.Add(new Guest()
                        {
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
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

        public DataTable ListToDataTable<T>(IList<T> items)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn());

            foreach (T item in items)
            {
                dataTable.Rows.Add(item);
            }

            return dataTable;
        }

        public DataTable ToDataTable<T>(IList<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
