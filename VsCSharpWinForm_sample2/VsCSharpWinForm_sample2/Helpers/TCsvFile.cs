using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class TCsvFile
    {
        /// Updated date: 2020-09-21
        /// https://en.wikipedia.org/wiki/Comma-separated_values
        /// https://dotnetcoretutorials.com/2018/08/04/csv-parsing-in-net-core/
        /// The field must be quoted if any below conditions meet.
        /// 1. The field contains commas (delimiter).
        /// 2. The field contains double quotes (text qualifier).
        /// 3. The field contains new line.
        public static TLog Logger { get; set; }
        public static char Delimiter { get; set; } = ',';
        public static char TextQualifier { get; set; } = '"';
        private static string DoubleTextQualifier { get { return string.Format("{0}{0}", TextQualifier); } }

        #region ReadRegion
        /// Return value = is the result field finished. True if finished, otherwise false.
        /// input = input field
        /// continuedField = the field content of the last field. If it is empty or null, it means that it is not continued field. If it is a continued field, this parameter must not be empty.
        /// output = output
        private static bool GetFieldForRead(string input, string continuedField, out string output)
        {
            output = continuedField ?? string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                if (string.IsNullOrEmpty(continuedField))
                {
                    /// If it is not a continued field, return empty string and return true.
                    return true;
                }
                else
                {
                    /// If it is a continued field, return false.
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(continuedField))
                {
                    /// Check the leading quote.
                    string s1 = input.TrimStart(' ', (char)9);
                    if (s1.StartsWith(TextQualifier.ToString()))
                    {
                        /// Remove the leading quote.
                        string s2 = s1.Substring(1);
                        string s3 = s2.TrimEnd(' ', (char)9);
                        /// Replace double quotes before checking the trailing quote.
                        if (s3.Replace(DoubleTextQualifier, "").EndsWith(TextQualifier.ToString()))
                        {
                            output = s3.Substring(0, s3.Length - 1).Replace(DoubleTextQualifier, TextQualifier.ToString());
                            return true;
                        }
                        else
                        {
                            /// The field is not finished. Not trim the trailing spaces.
                            output = s2.Replace(DoubleTextQualifier, TextQualifier.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        if (input.Contains(TextQualifier))
                        {
                            throw new Exception(string.Format("Cannot contain text qualifier (quote) in the field without leading quote and trailing quote. Field = {0}", input));
                        }
                        /// Field without quotes.
                        output = input;
                        return true;
                    }
                }
                else
                {
                    /// Replace double quotes before checking the trailing quote.
                    string s4 = input.TrimEnd(' ', (char)9);
                    if (s4.Replace(DoubleTextQualifier, "").EndsWith(TextQualifier.ToString()))
                    {
                        output += s4.Substring(0, s4.Length - 1).Replace(DoubleTextQualifier, TextQualifier.ToString());
                        return true;
                    }
                    else
                    {
                        /// The field is not finished. Not trim the trailing spaces.
                        output += input.Replace(DoubleTextQualifier, TextQualifier.ToString());
                        return false;
                    }
                }
            }
        }

        /// Read file with imported action.
        /// Return value = Number of records
        /// https://www.tutorialsteacher.com/csharp/constraints-in-generic-csharp
        public static int ReadFileWithImportedAction(string path, Action<string[]> handlingMethod)
        {
            List<string> list = null;
            try
            {
                int recordCount = 0;
                using (System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    using (System.IO.BufferedStream bs = new System.IO.BufferedStream(fs))
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(bs))
                        {
                            int lineIndex = 0;
                            string continuedField = string.Empty;
                            /// Loop each line.
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                try
                                {
                                    string[] array = line.Split(Delimiter);
                                    /// Loop each field.
                                    int lastArrayIndex = array.Length - 1;
                                    int i = 0;
                                    while (i < array.Length)
                                    {
                                        if (GetFieldForRead(array[i], continuedField, out string output))
                                        {
                                            if (list == null) { list = new List<string>(); }
                                            list.Add(output);
                                            continuedField = string.Empty;
                                            /// Handle the last field.
                                            if (i == lastArrayIndex)
                                            {
                                                recordCount++;
                                                handlingMethod(list?.ToArray());
                                                /// Clear the list for next record.
                                                list?.Clear();
                                                list = null;
                                            }
                                        }
                                        else
                                        {
                                            /// Handle the last field.
                                            if (i == lastArrayIndex)
                                            {
                                                continuedField = output + Environment.NewLine;
                                            }
                                            else
                                            {
                                                continuedField = string.Format("{0}{1}", output, Delimiter);
                                            }
                                        }
                                        i++;
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    Logger?.Error("Error occurs in line index {0}: {1}", lineIndex, line);
                                    Logger?.Error(ex2);
                                }
                                lineIndex++;
                            }
                        }
                    }
                }
                return recordCount;
            }
            catch (Exception ex)
            {
                Logger?.Error("Path = {0}", path);
                Logger?.Error(ex);
                return -1;
            }
            finally
            {
                list?.Clear();
                list = null;
            }
        }

        /// Read file and get an array of string arraies.
        /// Return value = Number of records
        public static string[][] ReadFileAndGetArrayOfStringArray(string path)
        {
            List<string[]> list = new List<string[]>();
            try
            {
                int i = ReadFileWithImportedAction(path, array =>
                {
                    list.Add(array);
                });
                return list.ToArray();
            }
            catch (Exception ex)
            {
                Logger?.Error("Path = {0}", path);
                Logger?.Error(ex);
                return null;
            }
        }
        #endregion

        #region WriteRegion
        private static string GetQuotedFieldForWrite(string input)
        {
            return string.Format("{0}{1}{0}", TextQualifier, input?.Replace(TextQualifier.ToString(), DoubleTextQualifier));
        }

        /// Check field before writing to CSV.
        private static string CheckAndGetFieldForWrite(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return input; }
            if (input.Contains(Delimiter)
                || input.Contains(TextQualifier)
                || input.Contains(Environment.NewLine))
            {
                return GetQuotedFieldForWrite(input);
            }
            else { return input; }
        }

        /// Write a string array to a string.
        /// Return Value = output string. It is null if errors occur.
        public static string WriteToString(string[] array, bool mustQuote)
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                //int arrayLength = array?.Length ?? 0;
                //if (arrayLength > 0)
                //{
                //    if (mustQuote)
                //    {
                //        /// The case if arrayLength >= 1.
                //        sb.Append(GetQuotedFieldForWrite(array[0]));
                //        if (arrayLength > 1)
                //        {
                //            /// Length must be larger than 1.
                //            int i = 1;
                //            while (i < arrayLength)
                //            {
                //                sb.Append(Delimiter).Append(GetQuotedFieldForWrite(array[i]));
                //                i++;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        /// The case if arrayLength >= 1.
                //        sb.Append(CheckAndGetFieldForWrite(array[0]));
                //        if (arrayLength > 1)
                //        {
                //            /// Length must be larger than 1.
                //            int i = 1;
                //            while (i < arrayLength)
                //            {
                //                sb.Append(Delimiter).Append(CheckAndGetFieldForWrite(array[i]));
                //                i++;
                //            }
                //        }
                //    }
                //}
                //sb.AppendLine();
                //return sb.ToString();
                if (array == null) { return null; }
                string[] array2 = mustQuote ? array.Select(x => GetQuotedFieldForWrite(x)).ToArray() : array.Select(x => CheckAndGetFieldForWrite(x)).ToArray();
                return string.Join(Delimiter.ToString(), array2) + Environment.NewLine;
            }
            catch (Exception ex)
            {
                Logger?.Error("Array = {0}", array);
                Logger?.Error(ex);
                return null;
            }
            //finally { sb = null; }
        }

        /// Write a string array to stream writer.
        /// Return Value = True if success, otherwise false.
        public static bool WriteToStreamWriter(System.IO.StreamWriter sw, string[] array, bool mustQuote)
        {
            try
            {
                sw.Write(WriteToString(array, mustQuote));
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error("Array = {0}", array);
                Logger?.Error(ex);
                return false;
            }
        }

        /// Write a string array to CSV file.
        /// Return Value = True if success, otherwise false.
        /// isApend = True if append, otherwise false.
        public static bool WriteToFile(string path, string[] array, bool mustQuote)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
                {
                    return WriteToStreamWriter(sw, array, mustQuote);
                }
            }
            catch (Exception ex)
            {
                Logger?.Error("Path = {0}", path);
                Logger?.Error("Array = {0}", array);
                Logger?.Error(ex);
                return false;
            }
        }
        #endregion
    }
}
