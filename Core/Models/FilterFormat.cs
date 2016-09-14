using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace StatTag.Core.Models
{
    public class FilterFormat : IEquatable<FilterFormat>
    {
        private const string InvalidFilterExceptionMessage =
            "The filter value is invalid.  Please use a comma-separated list of values and/or ranges (e.g. 1, 2-5)";

        /// <summary>
        /// The prefix is used when generating the filter out to a tag, so we can have multiple
        /// filters in a single tag that are uniquely identified.
        /// </summary>
        public string Prefix { get; set; }

        public bool Enabled { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public FilterFormat(string prefix)
        {
            Prefix = prefix;
            Type = string.Empty;
            Value = string.Empty;
        }

        public bool Equals(FilterFormat other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Prefix.Equals(this.Prefix)
                   && other.Enabled == this.Enabled
                   && other.Type.Equals(this.Type)
                   && other.Value.Equals(this.Value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FilterFormat);
        }

        private int GetValueFromString(string value)
        {
            int numericValue = 0;
            if (!int.TryParse(value, out numericValue))
            {
                throw new InvalidDataException(InvalidFilterExceptionMessage);
            }

            if (numericValue < 1)
            {
                throw new InvalidDataException(InvalidFilterExceptionMessage);
            }

            // Convert to 0-based index
            return (numericValue - 1);
        }

        /// <summary>
        /// Expand the value string into an array of index values.
        /// The value string will be expressed as 1-based indices, and this will
        /// convert them to a unique, sorted list of 0-based indices.
        /// </summary>
        /// <returns></returns>
        public int[] ExpandValue()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return null;
            }

            var components = Value.Split(Constants.ReservedCharacters.ListDelimiter);
            if (components.Length == 0)
            {
                return null;
            }

            var valueList = new List<int>();
            foreach (var component in components)
            {
                int value = 0;
                var values = component.Split(Constants.ReservedCharacters.RangeDelimiter);
                switch (values.Length)
                {
                    case 1:
                        valueList.Add(GetValueFromString(values[0]));
                        break;
                    case 2:
                        int rangeStartValue = GetValueFromString(values[0]);
                        int rangeEndValue = GetValueFromString(values[1]);

                        // We'll assume at some point somebody will put stuff in the wrong order, so we'll make sure
                        // to flip it if that's the case instead of throwing an exception.
                        int rangeStart = Math.Min(rangeStartValue, rangeEndValue);
                        int rangeEnd = Math.Max(rangeStartValue, rangeEndValue);
                        for (int index = rangeStart; index <= rangeEnd; index++)
                        {
                            valueList.Add(index);
                        }
                        break;
                    default:
                        throw new InvalidDataException(InvalidFilterExceptionMessage);
                }
            }

            valueList.Sort();
            return valueList.Distinct().ToArray();
        }
    }
}
