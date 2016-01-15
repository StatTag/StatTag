﻿using System;
using System.Text;

namespace AnalysisManager.Core.Models
{
    public class ValueFormat
    {
        public string FormatType { get; set; }
        public int DecimalPlaces { get; set; }
        public bool UseThousands { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }

        /// <summary>
        /// Formats a result given the current configuration
        /// </summary>
        /// <param name="value">The string value to be formatted</param>
        /// <returns></returns>
        public string Format(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            switch (FormatType)
            {
                case Constants.ValueFormatType.Numeric:
                    return FormatNumeric(value);
                case Constants.ValueFormatType.Percentage:
                    return FormatPercentage(value);
                case Constants.ValueFormatType.DateTime:
                    return FormatDateTime(value);
            }

            return value;
        }

        /// <summary>
        /// Format a numeric result
        /// </summary>
        /// <param name="value">The string value to be formatted</param>
        /// <returns></returns>
        protected string FormatNumeric(string value)
        {
            double numericValue = 0;
            if (!double.TryParse(value, out numericValue))
            {
                return string.Empty;
            }

            string formatPrefix = UseThousands ? "#,#" : "#";
            double roundedValue = Math.Round(numericValue, DecimalPlaces);
            if (DecimalPlaces <= 0)
            {
                return ((int)Math.Round(numericValue, 0)).ToString(formatPrefix);
            }

            return roundedValue.ToString(string.Format("{0}.{1}", formatPrefix, Repeat("0", DecimalPlaces)));
        }

        /// <summary>
        /// Format a result as a percentage
        /// </summary>
        /// <param name="value">The string value to be formatted</param>
        /// <returns></returns>
        protected string FormatPercentage(string value)
        {
            double numericValue = 0;
            if (!double.TryParse(value, out numericValue))
            {
                return string.Empty;
            }


            return numericValue.ToString(string.Format("#.{0}%", Repeat("0", DecimalPlaces)));
        }

        /// <summary>
        /// Format a result as a date and/or time
        /// </summary>
        /// <param name="value">The string value to be formatted</param>
        /// <returns></returns>
        protected string FormatDateTime(string value)
        {
            var dateTime = new DateTime();
            if (!DateTime.TryParse(value, out dateTime))
            {
                return string.Empty;
            }

            string format = string.Empty;
            if (!string.IsNullOrWhiteSpace(DateFormat))
            {
                if (DateFormat.Equals(Constants.DateFormats.MMDDYYYY)
                    || DateFormat.Equals(Constants.DateFormats.MonthDDYYYY))
                {
                    format = DateFormat;
                }
            }
            if (!string.IsNullOrWhiteSpace(TimeFormat))
            {
                if (TimeFormat.Equals(Constants.TimeFormats.HHMM)
                    || TimeFormat.Equals(Constants.TimeFormats.HHMMSS))
                {
                    format += " " + TimeFormat;
                }
            }

            if (string.IsNullOrWhiteSpace(format))
            {
                return string.Empty;
            }

            return dateTime.ToString(format.Trim());
        }

        /// <summary>
        /// Repeat a string value a number of times
        /// </summary>
        /// <param name="value">The string to be repeated</param>
        /// <param name="count">The number of times to repeat the value</param>
        /// <returns></returns>
        public static string Repeat(string value, int count)
        {
            return new StringBuilder().Insert(0, value, count).ToString();
        }
    }
}
