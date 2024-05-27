using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ContactManager
{
    public class ContactRepository
    {
        public ContactRepository()
        {
        }

        /// <summary>Create contact</summary>
        public void Create(Contact contact)
        {
            try
            {
                // Use using to create a SqlConnection object that will be automatically closed after use
                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    if (conn == null)
                    {
                        MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }
                    else if (contact == null)
                    {
                        MessageBox.Show("contact is null", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }

                    // Create an SQL query to insert a new contact into the ‘Contacts’ table using the prepared parameters
                    var query = new SqlCommand(@"insert into ""Contacts"" (""FirstName"", ""LastName"", ""PhoneNumber"", ""DateOfBirth"", ""Email"", ""Note"", ""CategoryId"", ""UserId"", ""Photo"")
                    values (@firstName, @lastName, @phoneNumber, @dateOfBirth, @email, @note, @categoryId, @userId, @file);", conn);

                    conn.Open();

                    // Add parameters to the request using values from the contact
                    SqlParameter categoryIdParam = query.Parameters.AddWithValue("@categoryId", contact.CategoryId);
                    if (contact.CategoryId == null)
                    {
                        categoryIdParam.Value = DBNull.Value;
                    }

                    SqlParameter dateOfBirthParam = query.Parameters.AddWithValue("@dateOfBirth", contact.DateOfBirth);
                    if (contact.DateOfBirth == null)
                    {
                        dateOfBirthParam.Value = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(contact.PhotoPath))
                    {
                        byte[] fileBytes = File.ReadAllBytes(contact.PhotoPath);
                        query.Parameters.AddWithValue("@file", fileBytes);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@file", null);
                    }

                    query.Parameters.AddRange(new[]
                    {
                    new SqlParameter("userId", SqlDbType.Int)
                    {
                        Value = contact.UserId
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Delete contact</summary>
        // This method performs the deletion of a contact from the database by its identifier (contactID)
        public void Delete(int contactId)
        {
            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    if (conn == null)
                    {
                        MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Get filtered contacts</summary>
        public ICollection<Contact> GetFilteredList(int userId, string filter)
        {
            try
            {
                var contacts = new List<Contact>();

                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    if (conn == null)
                    {
                        MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return null;
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        /// <summary>Get contacts</summary>
        public ICollection<Contact> GetList(int userId, int categoryId)
        {
            try
            {
                var contacts = new List<Contact>();

                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    if (conn == null)
                    {
                        MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return null;
                    }

                    // If categoryId is 0, all user contacts are selected,
                    // otherwise user contacts of a certain category are selected.
                    var query = categoryId == 0
                        ? new SqlCommand("select * from \"Contacts\" where \"UserId\"=@userId order by \"LastName\"", conn)
                        : new SqlCommand("select * from \"Contacts\" where \"UserId\"=@userId and \"CategoryId\"=@categoryId order by \"LastName\"", conn);

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
                        BitmapImage bitmap = new BitmapImage();

                        if (!string.IsNullOrEmpty(row["Photo"].ToString()))
                        {
                            byte[] photoBytes = (byte[])row["Photo"];
                            // Convert byte array to BitmapImage
                            
                            using (MemoryStream memoryStream = new MemoryStream(photoBytes))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = memoryStream;
                                bitmap.EndInit();
                            }
                        }

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
                            Photo = bitmap,
                            UserId = int.Parse(row["UserId"].ToString()),
                        });
                    }
                }

                return contacts;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        /// <summary>Update contact</summary>
        public void Update(Contact contact)
        {
            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    if (conn == null)
                    {
                        MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
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

                    SqlParameter categoryIdParam = query.Parameters.AddWithValue("@categoryId", contact.CategoryId);
                    if (contact.CategoryId == null)
                    {
                        categoryIdParam.Value = DBNull.Value;
                    }

                    SqlParameter dateOfBirthParam = query.Parameters.AddWithValue("@dateOfBirth", contact.DateOfBirth);
                    if (contact.DateOfBirth == null)
                    {
                        dateOfBirthParam.Value = DBNull.Value;
                    }

                    query.Parameters.AddRange(new[]
                    {
                        new SqlParameter("userId", SqlDbType.Int)
                        {
                            Value = contact.UserId
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}