using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatTag.Core.Models
{
    public class CommandResult
    {
        public string ValueResult { get; set; }
        public string FigureResult { get; set; }
        public Table TableResult { get; set; }

        /// <summary>
        /// Indicates that a promise is made to deliver table data, but that the data may
        /// not be ready to be pulled yet.  The information stored in this property will
        /// be sufficient for each statistical package processor to create a populated
        /// instance of TableResult.
        /// <remarks>
        /// Although not strictly enforced, the TableResult or TableResultPromise should be
        /// set at one time, but not both.  The presernce of the TableResultPromise member
        /// will be used as a flag to indicate that the TableResult needs to be set. Once
        /// the table is set, we clear the promise since it has been fulfilled.
        /// </remarks>
        /// </summary>
        public string TableResultPromise { get; set; }

        public bool IsEmpty()
        {
            return (string.IsNullOrWhiteSpace(ValueResult)
                    && string.IsNullOrWhiteSpace(FigureResult)
                    && (TableResult == null || TableResult.IsEmpty())
                    && string.IsNullOrWhiteSpace(TableResultPromise));
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(ValueResult))
            {
                return ValueResult;
            }

            if (!string.IsNullOrWhiteSpace(FigureResult))
            {
                return FigureResult;
            }

            if (TableResult != null)
            {
                return TableResult.ToString();
            }

            if (!string.IsNullOrWhiteSpace(TableResultPromise))
            {
                return string.Format("Table promise: {0}", TableResultPromise);
            }

            return string.Empty;
        }
    }
}
