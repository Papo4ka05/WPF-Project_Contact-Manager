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
            using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("connection string is null");
                }
                else if (contact == null)
                {
                    throw new Exception("contact is null");
                }

                var query = new SqlCommand(@"insert into ""Contacts"" (""FirstName"", ""LastName"", ""PhoneNumber"", ""DateOfBirth"", ""Email"", ""Note"", ""CategoryId"")
                    values (@firstName, @lastName, @phoneNumber, @dateOfBirth, @email, @note, @categoryId);", conn);

                conn.Open();

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