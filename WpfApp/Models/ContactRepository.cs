using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ContactManager
{
    public class ContactRepository
    {
        public ContactRepository(int userId, int categoryId)
        {
            Categories = GetCategories(userId);
            Contacts = GetContacts(userId, categoryId);
        }

        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Contact> Contacts { get; set; } = new List<Contact>();

        /// <summary>Create contact</summary>
        public void Create(Contact contact)
        {
            // Use using to create a SqlConnection object that will be automatically closed after use
            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                // // If the object conn null, throw an exception
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }
                else if (contact == null)
                {
                    // If contact null, throw an exception
                    throw new Exception("contact is null");
                }

                // Create an SQL query to insert a new contact into the ‘Contacts’ table using the prepared parameters
                var query = new SqlCommand(@"insert into ""Contacts"" (""FirstName"", ""LastName"", ""PhoneNumber"", ""DateOfBirth"", ""Email"", ""Note"", ""CategoryId"")
                    values (@firstName, @lastName, @phoneNumber, @dateOfBirth, @email, @note, @categoryId);", conn);

                conn.Open();

                // Add parameters to the request using values from the contact
                query.Parameters.AddRange(new[]
                {
                    new SqlParameter("categoryId", SqlDbType.Int)
                    {
                        Value = contact.CategoryId
                    },
                    new SqlParameter("dateOfBirth", SqlDbType.Date)
                    {
                        Value = contact.DateOfBirth
                    },
                    new SqlParameter("email", SqlDbType.VarChar)
                    {
                        Value = contact.Email
                    },
                    new SqlParameter("firstName", SqlDbType.VarChar)
                    {
                        Value = contact.FirstName
                    },
                    new SqlParameter("lastName", SqlDbType.VarChar)
                    {
                        Value = contact.LastName
                    },
                    new SqlParameter("note", SqlDbType.VarChar)
                    {
                        Value = contact.Note
                    },
                    new SqlParameter("phoneNumber", SqlDbType.VarChar)
                    {
                        Value = contact.PhoneNumber
                    },
                });

                query.ExecuteNonQuery();
            }
        }

        /// <summary>Delete contact</summary>
        // This method performs the deletion of a contact from the database by its identifier (contactID)
        public void Delete(int contactId)
        {
            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }

                var query = new SqlCommand("delete \"Contacts\" where \"Id\"=@contactId", conn);

                conn.Open();

                // Adds a parameter to the SQL query that specifies the ID of the contact to be deleted
                // This is done to prevent SQL injection attacks and ensure safe execution of the query.
                query.Parameters.Add(new SqlParameter("contactId", SqlDbType.Int)
                {
                    Value = contactId
                });

                query.ExecuteNonQuery();
            }
        }

        /// <summary>Get categories</summary>
        public List<Category> GetCategories(int userId)
        {
            var categories = new List<Category>();

            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }

                var query = new SqlCommand("select * from \"Categories\" order by \"Name\"", conn);
                //var query = new SqlCommand("select * from \"Categories\" where \"UserId\"=@userId", conn);
                //query.Parameters.Add(new SqlParameter("userId", SqlDbType.Int)
                //{
                //    Value = userId
                //});

                conn.Open();

                // SqlDataAdapter используется для заполнения DataTable данными из источника данных,
                // в данном случае - из результата SQL-запроса к базе данных.Он предоставляет интерфейс для выполнения операций
                // с базой данных, таких как выборка, вставка, обновление и удаление данных.

                // It is used to execute a query request to the database
                // A SqlDataAdapter which is used to fill the DataTable with data obtained from the result of the query execution.
                var sqlDataAdapter = new SqlDataAdapter(query);
                var dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                categories.Add(new Category
                {
                    Id = 0,
                    Name = "All",
                });

                foreach (DataRow row in dataTable.Rows)
                {
                    categories.Add(new Category
                    {
                        Id = int.Parse(row["Id"].ToString()),
                        Name = row["Name"].ToString(),
                    });
                }
            }

            return categories;
        }

        /// <summary>Get contacts</summary>
        public List<Contact> GetContacts(int userId, int categoryId)
        {
            var contacts = new List<Contact>();

            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }

                // If categoryId is 0, all user contacts are selected,
                // otherwise user contacts of a certain category are selected.
                var query = categoryId == 0
                    ? new SqlCommand("select * from \"Contacts\" where \"UserId\"=@userId", conn)
                    : new SqlCommand("select * from \"Contacts\" where \"UserId\"=@userId and \"CategoryId\"=@categoryId", conn);
                
                query.Parameters.Add(new SqlParameter("userId", SqlDbType.Int)
                {
                    Value = userId
                });

                if (categoryId != 0)
                {
                    query.Parameters.Add(new SqlParameter("categoryId", SqlDbType.Int)
                    {
                        Value = categoryId
                    });
                }

                conn.Open();

                var sqlDataAdapter = new SqlDataAdapter(query);
                var dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    contacts.Add(new Contact
                    {
                        CategoryId = int.TryParse(row["CategoryId"].ToString(), out var cId) ? (int?)cId : null,
                        DateOfBirth = DateTime.TryParse(row["DateOfBirth"].ToString(), out var dateOfBirth) ? (DateTime?)dateOfBirth : null,
                        Email = row["Email"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        Id = int.Parse(row["Id"].ToString()),
                        LastName = row["LastName"].ToString(),
                        Note = row["Note"].ToString(),
                        PhoneNumber = row["PhoneNumber"].ToString(),
                    });
                }
            }

            return contacts;
        }

        /// <summary>Get filtered contacts</summary>
        public List<Contact> GetContactsBySearch(int userId, string filter)
        {
            var contacts = new List<Contact>();

            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }

                var query = new SqlCommand(@"
                    select * from ""Contacts""
                    where ""UserId"" = @userId
                        and (charindex(lower(""Email""), @filter) > 0)
                            or (charindex(lower(""FirstName""), @filter) > 0)
                            or (charindex(lower(""LastName""), @filter) > 0)
                            or (charindex(lower(""Note""), @filter) > 0)
                            or (charindex(lower(""PhoneNumber""), @filter) > 0)", conn);

                query.Parameters.AddRange(new[] {
                    new SqlParameter("filter", SqlDbType.VarChar)
                    {
                        Value = filter.ToLower(),
                    },
                    new SqlParameter("userId", SqlDbType.Int)
                    {
                        Value = userId
                    }
                });

                conn.Open();

                var sqlDataAdapter = new SqlDataAdapter(query);
                var dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    contacts.Add(new Contact
                    {
                        CategoryId = int.TryParse(row["CategoryId"].ToString(), out var categoryId) ? (int?)categoryId : null,
                        DateOfBirth = DateTime.TryParse(row["DateOfBirth"].ToString(), out var dateOfBirth) ? (DateTime?)dateOfBirth : null,
                        Email = row["Email"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        Id = int.Parse(row["Id"].ToString()),
                        LastName = row["LastName"].ToString(),
                        Note = row["Note"].ToString(),
                        PhoneNumber = row["PhoneNumber"].ToString(),
                    });
                }

                return contacts;
            }
        }

        /// <summary>Update contact</summary>
        public void Update(Contact contact)
        {
            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }

                var query = new SqlCommand(@"
                    update ""Contacts""
                    set
                        ""CategoryId"" = @categoryId,
                        ""DateOfBirth"" = @dateOfBirth,
                        ""Email"" = @email,
                        ""FirstName"" = @firstName,
                        ""LastName"" = @lastName,
                        ""Note"" = @note,
                        ""PhoneNumber"" = @phoneNumber
                    where ""Id"" = @id", conn);

                conn.Open();

                query.Parameters.AddRange(new[] {
                    new SqlParameter("categoryId", SqlDbType.Int)
                    {
                        Value = contact.CategoryId
                    },
                    new SqlParameter("dateOfBirth", SqlDbType.Date)
                    {
                        Value = contact.DateOfBirth
                    },
                    new SqlParameter("email", SqlDbType.VarChar)
                    {
                        Value = contact.Email
                    },
                    new SqlParameter("firstName", SqlDbType.VarChar)
                    {
                        Value = contact.FirstName
                    },
                    new SqlParameter("id", SqlDbType.Int)
                    {
                        Value = contact.Id
                    },
                    new SqlParameter("lastName", SqlDbType.VarChar)
                    {
                        Value = contact.LastName
                    },
                    new SqlParameter("note", SqlDbType.VarChar)
                    {
                        Value = contact.Note
                    },
                    new SqlParameter("phoneNumber", SqlDbType.VarChar)
                    {
                        Value = contact.PhoneNumber
                    },
                });

                query.ExecuteNonQuery();
            }
        }
    }
}