using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PhonebookApp.Models;

namespace PhonebookApp.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PhonebookDbConnection");
        }

        public async Task<PagedResult<Contact>> GetContactsPagedAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var contacts = new List<Contact>();
            int totalCount;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetContactsPaged", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber });
                cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });
                cmd.Parameters.Add(new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 255)
                {
                    Value = (object)searchTerm ?? DBNull.Value
                });

                var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(totalCountParam);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        contacts.Add(MapReaderToContact(reader));
                    }
                }

                totalCount = totalCountParam.Value != DBNull.Value ? (int)totalCountParam.Value : 0;
            }

            return new PagedResult<Contact>
            {
                Items = contacts,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetContactById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToContact(reader);
                    }
                }
            }

            return null;
        }

        public async Task<int> InsertContactAsync(Contact contact)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_InsertContact", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 255) { Value = contact.Name });
                cmd.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50) { Value = contact.PhoneNumber });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255)
                {
                    Value = (object)contact.Email ?? DBNull.Value
                });
                cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, -1)
                {
                    Value = (object)contact.Address ?? DBNull.Value
                });

                var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(newIdParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : 0;
            }
        }

        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateContact", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = contact.Id });
                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 255) { Value = contact.Name });
                cmd.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 50) { Value = contact.PhoneNumber });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255)
                {
                    Value = (object)contact.Email ?? DBNull.Value
                });
                cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, -1)
                {
                    Value = (object)contact.Address ?? DBNull.Value
                });

                await conn.OpenAsync();
                object result = await cmd.ExecuteScalarAsync();
                int rowsAffected = result != null ? Convert.ToInt32(result) : 0;

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_DeleteContact", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                await conn.OpenAsync();
                object result = await cmd.ExecuteScalarAsync();
                int rowsAffected = result != null ? Convert.ToInt32(result) : 0;

                return rowsAffected > 0;
            }
        }

        private static Contact MapReaderToContact(SqlDataReader reader)
        {
            return new Contact
            {
                Id = (int)reader["Id"],
                Name = reader["Name"] as string,
                PhoneNumber = reader["PhoneNumber"] as string,
                Email = reader["Email"] == DBNull.Value ? null : (string)reader["Email"],
                Address = reader["Address"] as string,
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
