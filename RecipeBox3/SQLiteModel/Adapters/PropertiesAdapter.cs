using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3.SQLiteModel.Adapters
{
    /// <summary>Adapter for the Properties table</summary>
    public sealed class PropertiesAdapter : SQLiteAdapter<PropertyRow>
    {
        /// <inheritdoc/>
        public override string TableName => "Properties";

        /// <inheritdoc/>
        public override IEnumerable<TableColumn> DataColumns => new TableColumn[]
        {
            new TableColumn("Name", System.Data.DbType.String, options: ColumnOptions.NotNull | ColumnOptions.Unique),
            new TableColumn("Value", System.Data.DbType.String)
        };

        private SQLiteCommand SelectByNameCommand;

        /// <inheritdoc/>
        protected override SQLiteConnection Connection
        {
            get => base.Connection;
            set
            {
                if (value == _connection) return;
                else
                {
                    if (SelectByNameCommand != null) SelectByNameCommand.Connection = value;
                    base.Connection = value;
                }
            }
        }

        /// <summary>Create a new instance of the class</summary>
        public PropertiesAdapter() : base() { }

        /// <summary>Create a new instance of the class</summary>
        /// <param name="connectionString"></param>
        public PropertiesAdapter(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        protected override void Initialize(string connectionString)
        {
            base.Initialize(connectionString);

            var nameParam = TableColumnExtensions.GetParameterName("Name");
            SelectByNameCommand = new SQLiteCommand($"SELECT Name, Value FROM `{TableName}` WHERE `Name`={nameParam}", Connection);

            if (DataParameters.TryGetValue("Name", out SQLiteParameter nameParameter))
                SelectByNameCommand.Parameters.Add(nameParameter);
        }

        /// <summary>Get a property by name</summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyRow SelectByName(string propertyName)
        {
            if (SelectByNameCommand?.Connection == null) return null;
            else
            {
                TrySetParameterValue("Name", propertyName);
                PropertyRow result = null;

                using (var reader = ExecuteCommandReader(SelectByNameCommand))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        result = GetRowFromReader(reader);
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        protected override PropertyRow GetRowFromReader(SQLiteDataReader reader)
        {
            try
            {
                return new PropertyRow()
                {
                    Name = reader.GetString(0),
                    Value = reader.GetString(1),
                    Status = RowStatus.Unchanged
                };
            }
            catch (InvalidCastException e)
            {
                App.LogException(e);
                return null;
            }
        }

        /// <inheritdoc/>
        protected override void SetDataParametersFromRow(PropertyRow row)
        {
            TrySetParameterValue("Name", row.Name);
            TrySetParameterValue("Value", row.Value);
        }
    }
}
