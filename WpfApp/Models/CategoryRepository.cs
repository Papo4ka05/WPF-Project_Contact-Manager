using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ContactManager
{
    public interface ICategoryRepository
    {
        void Create(int userId, string name);

        void Delete(int categoryId);

        ICollection<Category> GetList(int userId);

        void Update(int categoryId, string name);
    }

    public class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository()
        {
        }

        /// <summary>Create category</summary>
        public void Create(int userId, string name)
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
                    else if (string.IsNullOrEmpty(name))
                    {
                        MessageBox.Show("name is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }

                    var query = new SqlCommand(@"insert into ""Categories"" (""Name"", ""UserId"") values (@name, @userId);", conn);

                    conn.Open();

                    query.Parameters.AddRange(new[]
                    {
                        new SqlParameter("name", SqlDbType.VarChar) { Value = name },
                        new SqlParameter("userId", SqlDbType.Int) { Value = userId },
                    });

                    query.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Delete category</summary>
        public void Delete(int categoryId/*, int userId*/)
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
                    else if (categoryId == 0)
                    {
                        MessageBox.Show("categoryId is 0", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }

                    var query = new SqlCommand("delete \"Categories\" where \"Id\"=@id", conn);

                    conn.Open();

                    query.Parameters.Add(new SqlParameter("id", SqlDbType.Int) { Value = categoryId });

                    query.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Get categories</summary>
        public ICollection<Category> GetList(int userId)
        {
            try
            {
                var categories = new List<Category>();

                using (var conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    if (conn == null)
                    {
                        MessageBox.Show("connection is null", "Database connection", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "SQL error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

        /// <summary>Update category</summary>
        public void Update(int categoryId, string name)
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
                    else if (string.IsNullOrEmpty(name))
                    {
                        MessageBox.Show("name is empty", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }
                    else if (categoryId == 0)
                    {
                        MessageBox.Show("categoryId is 0", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }

                    var query = new SqlCommand(@"update ""Categories"" set ""Name"" = @name where ""Id"" = @id", conn);

                    conn.Open();

                    query.Parameters.AddRange(new[]
                    {
                        new SqlParameter("name", SqlDbType.VarChar) { Value = name },
                        new SqlParameter("id", SqlDbType.Int) { Value = categoryId },
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